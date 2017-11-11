using System.IO;
using Newtonsoft.Json;

namespace LibgenDesktop.Settings
{
    internal static class SettingsStorage
    {
        private const string CONFIG_FILE_NAME = "libgen.config";

        private static AppSettings appSettings;

        static SettingsStorage()
        {
            appSettings = AppSettings.Default;
        }

        public static AppSettings AppSettings => appSettings;

        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(CONFIG_FILE_NAME))
                {
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    using (StreamReader streamReader = new StreamReader(CONFIG_FILE_NAME))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        appSettings = jsonSerializer.Deserialize<AppSettings>(jsonTextReader);
                    }
                }
                AppSettings.ValidateAndCorrect(appSettings);
            }
            catch
            {
                appSettings = AppSettings.Default;
            }
        }

        public static void SaveSettings()
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
