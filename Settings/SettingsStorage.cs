using System.IO;
using System.Runtime.Serialization.Json;

namespace LibgenDesktop.Settings
{
    internal static class SettingsStorage
    {
        private const string CONFIG_FILE_NAME = "libgen.config";

        public static AppSettings LoadSettings()
        {
            AppSettings result = null;
            if (File.Exists(CONFIG_FILE_NAME))
            {
                DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(typeof(AppSettings));
                using (FileStream fileStream = new FileStream(CONFIG_FILE_NAME, FileMode.Open))
                {
                    result = dataContractSerializer.ReadObject(fileStream) as AppSettings;
                }
            }
            if (result == null)
            {
                result = AppSettings.Default;
            }
            return result;
        }

        public static void SaveSettings(AppSettings appSettings)
        {
            DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(typeof(AppSettings));
            using (FileStream fileStream = new FileStream(CONFIG_FILE_NAME, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, appSettings);
            }
        }
    }
}
