using System.IO;
using Newtonsoft.Json;

namespace LibgenDesktop.Models.Settings
{
    internal static class SettingsStorage
    {
        private const string CONFIG_FILE_NAME = "libgen.config";

        public static AppSettings LoadSettings()
        {
            AppSettings result = AppSettings.Default;
            try
            {
                if (File.Exists(CONFIG_FILE_NAME))
                {
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    using (StreamReader streamReader = new StreamReader(CONFIG_FILE_NAME))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        result = jsonSerializer.Deserialize<AppSettings>(jsonTextReader);
                    }
                }
                result = AppSettings.ValidateAndCorrect(result);
            }
            catch
            {
                result = AppSettings.Default;
            }
            return result;
        }

        public static void SaveSettings(AppSettings appSettings)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamWriter streamWriter = new StreamWriter(CONFIG_FILE_NAME))
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                jsonTextWriter.Formatting = Formatting.Indented;
                jsonTextWriter.Indentation = 4;
                jsonSerializer.Serialize(jsonTextWriter, appSettings);
            }
        }
    }
}
