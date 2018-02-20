using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using Microsoft.Win32;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Common
{
    internal static class Environment
    {
        private const string NET_FRAMEWORK_REGISTRY_KEY = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

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
            MirrorsDirectoryPath = Path.Combine(AppBinariesDirectory, MIRRORS_DIRECTORY_NAME);
            LanguagesDirectoryPath = Path.Combine(AppBinariesDirectory, LANGUAGES_DIRECTORY_NAME);
            OsVersion = GetOsVersion();
            NetFrameworkVersion = GetNetFrameworkVersion();
            IsIn64BitProcess = System.Environment.Is64BitProcess;
        }

        private static string GetOsVersion()
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

        private static string GetNetFrameworkVersion()
        {
            using (RegistryKey netFrameworkRegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).
                OpenSubKey(NET_FRAMEWORK_REGISTRY_KEY))
            {
                if (netFrameworkRegistryKey != null)
                {
                    object releaseValue = netFrameworkRegistryKey.GetValue("Release");
                    if (releaseValue != null)
                    {
                        if (Int32.TryParse(releaseValue.ToString(), out int releaseNumber))
                        {
                            if (releaseNumber >= 461308)
                            {
                                return "4.7.1 or later";
                            }
                            else if (releaseNumber >= 460798)
                            {
                                return "4.7";
                            }
                            else if (releaseNumber >= 394802)
                            {
                                return "4.6.2";
                            }
                            else if (releaseNumber >= 394254)
                            {
                                return "4.6.1";
                            }
                            else if (releaseNumber >= 393295)
                            {
                                return "4.6";
                            }
                            else if (releaseNumber >= 379893)
                            {
                                return "4.5.2";
                            }
                            else if (releaseNumber >= 378675)
                            {
                                return "4.5.1";
                            }
                            else if (releaseNumber >= 378389)
                            {
                                return "4.5";
                            }
                        }
                    }
                }
                return "unknown";
            }
        }

        public static string AppBinariesDirectory { get; }
        public static string AppDataDirectory { get; }
        public static string LogFilePath { get; }
        public static string AppSettingsFilePath { get; }
        public static string MirrorsDirectoryPath { get; }
        public static string LanguagesDirectoryPath { get; }
        public static string OsVersion { get; }
        public static string NetFrameworkVersion { get; }
        public static bool IsInPortableMode { get; }
        public static bool IsIn64BitProcess { get; }
    }
}
