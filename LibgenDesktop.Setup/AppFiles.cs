using System;

namespace LibgenDesktop.Setup
{
    internal static class AppFiles
    {
        public static string GetBinariesDirectoryPath(bool is64Bit)
        {
            return String.Format(Constants.BINARIES_DIRECTORY_PATH_FORMAT, (is64Bit ? "64" : "86"));
        }

        public static string[] GetFileList(bool is64Bit)
        {
            return new[]
            {
                Constants.MAIN_EXECUTABLE_NAME,
                "System.ValueTuple.dll",
                "Newtonsoft.Json.dll",
                "SharpCompress.dll",
                "MaterialDesignThemes.Wpf.dll",
                "MaterialDesignColors.dll",
                "Dragablz.dll",
                "System.Data.SQLite.dll",
                (is64Bit ? "x64" : "x86") + @"\SQLite.Interop.dll",
                "mirrors.config"
            };
        }
    }
}
