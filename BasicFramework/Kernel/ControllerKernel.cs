using System.Reflection;

namespace BasicFramework.Kernel
{
    public abstract class ControllerKernel
    {
        /// <summary>
        ///  Function is still in testing, DO NOT USE
        /// </summary>
        public T TryCastObject<T>(dynamic source) where T : new()
        {
            T result = new();

            foreach (PropertyInfo property in result.GetType().GetProperties())
            {
                string name = property.Name;
                Type type = source.GetType();
                PropertyInfo sourceProp = source.GetType()
                    .GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (name == sourceProp?.Name)
                {
                    var value = sourceProp.GetValue(source, null);
                    property.SetValue(result, value);
                }
            }

            return result;
        }
    }
}
