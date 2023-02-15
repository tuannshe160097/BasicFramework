using BasicFramework.Config.Exception;
using BasicFramework.Kernel;
using Newtonsoft.Json;

namespace BasicFramework.Config
{
    public static class Config
    {
        [ConfigRequired]
        public static readonly string Host;

        [ConfigRequired]
        public static readonly int Port;

        public static readonly List<int> list;

        public static readonly dynamic Env;

        static Config()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            //JObject config = JObject.Parse(File.ReadAllText(exeDir + "application.json"));
            dynamic config = JsonConvert.DeserializeObject<DDCollection>(File.ReadAllText(exeDir + "application.json"));

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
                        field.SetValue(null, Convert.ChangeType(value, field.FieldType));
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
