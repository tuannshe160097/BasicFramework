using System.Reflection;

namespace BasicFramework.Kernel
{
    using BasicFramework.DependencyInjection;
    using BasicFramework.Http;

    public abstract class RouterKernel
    {
        protected class RouterBinding
        {
            public ControllerKernel Controller { get; set; }
            public MethodInfo Method { get; set; }

            public RouterBinding(ControllerKernel controller, string method)
            {
                Controller = controller;

                Method = controller.GetType().GetMethod(method);
                if (Method == null) throw new Exception($"Method {method} does not exist in controller {controller.GetType().Name}");
            }
        }

        protected static Dictionary<(HttpMethod, string), RouterBinding> routes = new();

        protected RouterKernel()
        {
            AddRouter();
        }

        protected abstract void AddRouter();

        protected void Route<Controller>(HttpMethod method, string url, string function) where Controller : ControllerKernel
        {
            routes.Add((method, url), new RouterBinding(CreateController<Controller>(), function));
        }

        protected void Route<Controller>(string url, string function) where Controller : ControllerKernel
        {
            routes.Add((HttpMethod.GET, url), new RouterBinding(CreateController<Controller>(), function));
        }

        protected void Route<Controller>(string function) where Controller : ControllerKernel
        {
            routes.Add((HttpMethod.GET, $"/{typeof(Controller).Name}/{function}"), new RouterBinding(CreateController<Controller>(), function));
        }

        private Controller CreateController<Controller>()
        {
            ConstructorInfo[] constructors = typeof(Controller).GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0) throw new Exception($"Controller {typeof(Controller).Name} does not contain a valid constructor");

            ParameterInfo[] constructorParameters = constructors[0].GetParameters();

            if (constructorParameters.Length == 0) return (Controller)Activator.CreateInstance(typeof(Controller));

            List<object> parameters = new();

            foreach(var param in constructorParameters)
            {
                if (DIManager.DIList.ContainsKey(param.ParameterType))
                {
                    parameters.Add(Activator.CreateInstance(DIManager.DIList[param.ParameterType]));
                }
            }

            try
            {
                return (Controller)Activator.CreateInstance(typeof(Controller), parameters.ToArray());
            }
            catch
            {
                throw new Exception($"Controller {typeof(Controller).Name} does not contain a valid constructor");
            }
        }

        internal static string? MapController(HttpRequest request)
        {
            if (!routes.ContainsKey((request.Method, request.Url))) throw new HttpException(404, new { message = "Page Not Found" });

            RouterBinding binding = routes[(request.Method, request.Url)];

            var controller = binding.Controller;
            var method = binding.Method;

            object? response = method.Invoke(controller, new object[] { request });
            if (response == null || response is not HttpResponse) throw new Exception($"Method {method.Name} must return data of type HttpResponse");

            return response.ToString();
        }

        internal static void UseRouting()
        {
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
