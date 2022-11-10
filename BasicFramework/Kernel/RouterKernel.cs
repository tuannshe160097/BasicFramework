using System.Reflection;

namespace BasicFramework.Kernel
{
    using BasicFramework.DependencyInjection;
    using BasicFramework.Http;

    public abstract class RouterKernel
    {
        private class RouterBinding
        {
            public Type Controller { get; private set; }
            private ParameterInfo[] ConstructorParams { get; set; }
            private List<object> Dependencies { get; set; }
            public MethodInfo Method { get; private set; }

            public RouterBinding(Type controller, string method)
            {
                Controller = controller;

                ConstructorInfo[] constructors = controller.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                if (constructors.Length == 0) throw new Exception($"Controller {controller.Name} does not contain a valid constructor");

                ConstructorParams = constructors[0].GetParameters();

                Dependencies = new();

                Method = controller.GetMethod(method) ?? throw new Exception($"Method {method} does not exist in controller {controller.GetType().Name}");
            }

            public ControllerKernel CreateController()
            {
                Dependencies = new();

                if (ConstructorParams.Length == 0) return (ControllerKernel)Activator.CreateInstance(Controller);

                foreach (var param in ConstructorParams)
                {
                    if (DIManager.DIList.ContainsKey(param.ParameterType))
                    {
                        Dependencies.Add(Activator.CreateInstance(DIManager.DIList[param.ParameterType]));
                    }
                }

                try
                {
                    return (ControllerKernel)Activator.CreateInstance(Controller, Dependencies.ToArray());
                }
                catch
                {
                    throw new Exception($"Controller {Controller.Name} does not contain a valid constructor");
                }
            }

            public void DisposeDependencies()
            {
                foreach (var dependency in Dependencies)
                {
                    if (dependency is IDisposable)
                    {
                        (dependency as IDisposable).Dispose();
                    }
                }
            }
        }

        private static Dictionary<(HttpMethod, string), RouterBinding> routes = new();

        protected RouterKernel()
        {
            AddRouter();
        }

        protected abstract void AddRouter();

        protected void Route<Controller>(HttpMethod method, string url, string function) where Controller : ControllerKernel
        {
            routes.Add((method, url), new RouterBinding(typeof(Controller), function));
        }

        protected void Route<Controller>(string url, string function) where Controller : ControllerKernel
        {
            routes.Add((HttpMethod.GET, url), new RouterBinding(typeof(Controller), function));
        }

        protected void Route<Controller>(string function) where Controller : ControllerKernel
        {
            routes.Add((HttpMethod.GET, $"/{typeof(Controller).Name}/{function}"), new RouterBinding(typeof(Controller), function));
        }

        internal static string? MapController(HttpRequest request)
        {
            if (!routes.ContainsKey((request.Method, request.Url))) throw new HttpException(404, new { message = "Page Not Found" });

            RouterBinding binding = routes[(request.Method, request.Url)];

            var controller = binding.CreateController();
            var method = binding.Method;

            object? response = null;
            try
            {
                response = method.Invoke(controller, new object[] { request });
            } 
            catch
            {
                throw new Exception($"Endpoint {method.Name} of controller {binding.Controller.Name} is not valid");
            }
            finally
            {
                binding.DisposeDependencies();
            }

            if (response == null || response is not HttpResponse) throw new Exception($"Method {method.Name} must return data of type HttpResponse");

            return response.ToString();
        }

        internal static void UseRouting()
        {
            Console.WriteLine("Mapping controllers\n");
            foreach (Type child in GetInheritedClasses(typeof(RouterKernel)))
            {
                Activator.CreateInstance(child);
            }
        }

        private static List<Type> GetInheritedClasses(Type MyType)
        {
            return Assembly.GetEntryAssembly()?.GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToList() ?? new List<Type>();
        }
    }
}
