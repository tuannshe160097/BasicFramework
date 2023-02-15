namespace BasicFramework.Config
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class ConfigRequiredAttribute : Attribute
    {
        public readonly bool AllowNull = false;

        public ConfigRequiredAttribute(bool allowNull)
        {
            AllowNull = allowNull;
        }

        public ConfigRequiredAttribute() { }
    }
}
