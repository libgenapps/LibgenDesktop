using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Common
{
    internal static class Environment
    {
        static Environment()
        {
            AppBinariesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDataDirectory = AppBinariesDirectory;
            AppSettingsFilePath = Path.Combine(AppDataDirectory, APP_SETTINGS_FILE_NAME);
            if (File.Exists(AppSettingsFilePath))
            {
                IsInPortableMode = true;
            }
            else
            {
                try
                {
                    using (FileStream fileStream = File.Create(AppSettingsFilePath))
                    {
                        fileStream.Close();
                    }
                    IsInPortableMode = true;
                }
                catch (UnauthorizedAccessException)
                {
                    IsInPortableMode = false;
                }
            }
            if (!IsInPortableMode)
            {
                AppDataDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "LibgenDesktop");
                if (!Directory.Exists(AppDataDirectory))
                {
                    Directory.CreateDirectory(AppDataDirectory);
                }
            }
            string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
            LogFilePath = Path.Combine(AppDataDirectory, logFileName);
            AppSettingsFilePath = Path.Combine(AppDataDirectory, APP_SETTINGS_FILE_NAME);
            MirrorsFilePath = Path.Combine(AppBinariesDirectory, MIRRORS_FILE_NAME);
            OsName = GetOsName();
            IsIn64BitProcess = System.Environment.Is64BitProcess;
        }

        public static string AppBinariesDirectory { get; }
        public static string AppDataDirectory { get; }
        public static string LogFilePath { get; }
        public static string AppSettingsFilePath { get; }
        public static string MirrorsFilePath { get; }
        public static string OsName { get; }
        public static bool IsInPortableMode { get; }
        public static bool IsIn64BitProcess { get; }

        private static string GetOsName()
        {
            ManagementObject osInfo = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>().FirstOrDefault();
            if (osInfo != null)
            {
                return $"{osInfo.Properties["Caption"].Value.ToString()} {osInfo.Properties["Version"].Value.ToString()} {osInfo.Properties["OSArchitecture"].Value.ToString()}";
            }
            else
            {
                return "Unknown";
            }
        }
    }
}
