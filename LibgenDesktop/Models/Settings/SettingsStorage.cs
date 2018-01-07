using System.IO;
using Newtonsoft.Json;

namespace LibgenDesktop.Models.Settings
{
    internal static class SettingsStorage
    {
        public static AppSettings LoadSettings(string settingsFilePath)
        {
            AppSettings result;
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    using (StreamReader streamReader = new StreamReader(settingsFilePath))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        result = jsonSerializer.Deserialize<AppSettings>(jsonTextReader);
                    }
                    result = AppSettings.ValidateAndCorrect(result);
                }
                else
                {
                    result = AppSettings.Default;
                }
            }
            catch
            {
                result = AppSettings.Default;
            }
            return result;
        }

        public static void SaveSettings(AppSettings appSettings, string settingsFilePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamWriter streamWriter = new StreamWriter(settingsFilePath))
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                jsonTextWriter.Formatting = Formatting.Indented;
                jsonTextWriter.Indentation = 4;
                jsonSerializer.Serialize(jsonTextWriter, appSettings);
            }
        }
    }
}
