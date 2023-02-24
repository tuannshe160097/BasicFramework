using BasicFramework.Config.Exception;
using BasicFramework.DataType;
using Newtonsoft.Json;

namespace BasicFramework.Config
{
    public static class Config
    {
        [ConfigRequired]
        public static readonly string Host;

        [ConfigRequired]
        public static readonly int Port;

        public static readonly dynamic Env;

        static Config()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            dynamic? config = JsonConvert.DeserializeObject<TObject>(File.ReadAllText(exeDir + "application.json"));

            if (config == null)
            {
                throw new ConfigNotFoundException();
            }

            foreach (var field in typeof(Config).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                var value = config[field.Name.ToLower()];
                bool nullable = !Attribute.IsDefined(field, typeof(ConfigRequiredAttribute));

                if (string.IsNullOrWhiteSpace(value?.ToString()) && nullable)
                {
                    throw new ConfigNullException(field.Name);
                }
                else
                {
                    try
                    {
                        if (value is IConvertible)
                        {
                            field.SetValue(null, Convert.ChangeType(value, field.FieldType));
                        }
                        else
                        {
                            field.SetValue(null, value);
                        }
                    }
                    catch (TypeInitializationException e)
                    {
                        throw new ConfigException(field.Name);
                    }
                }
            }
        }
    }
}
