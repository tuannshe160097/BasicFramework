namespace BasicFramework.DependencyInjection
{
    internal static class DIManager
    {
        public static Dictionary<Type, Type> DIList = new();

        public static void RegisterDependency<Parent, Child>()
            where Parent : class
            where Child : Parent, new()
        {
            DIList.Add(typeof(Parent), typeof(Child));
        }
    }
}
