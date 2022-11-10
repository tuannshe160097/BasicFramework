namespace BasicFramework.DependencyInjection
{
    public static class DIManager
    {
        public static Dictionary<Type, Type> DIList = new();

        public static void ControllerDependency<Parent, Child>()
            where Parent : class
            where Child : Parent, new()
        {
            DIList.Add(typeof(Parent), typeof(Child));
        }

        public static void ControllerDependency<Dependency>()
            where Dependency : class, new()
        {
            DIList.Add(typeof(Dependency), typeof(Dependency));
        }
    }
}
