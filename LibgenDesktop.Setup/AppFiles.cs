using System;
using System.Collections.Generic;

namespace LibgenDesktop.Setup
{
    internal static class AppFiles
    {
        static AppFiles()
        {
            X86 = new List<AppFile>();
            X64 = new List<AppFile>();
            AddFile(Constants.MAIN_EXECUTABLE_NAME);
            AddFile("System.ValueTuple.dll");
            AddFile("Newtonsoft.Json.dll");
            AddFile("SharpCompress.dll");
            AddFile("MaterialDesignThemes.Wpf.dll");
            AddFile("MaterialDesignColors.dll");
            AddFile("Dragablz.dll");
            AddFile("System.Data.SQLite.dll");
            AddFile("NLog.dll");
            AddFile("EPPlus.dll");
            AddFile("HtmlAgilityPack.dll");
            AddFile("Microsoft.WindowsAPICodePack.dll");
            AddFile("Microsoft.WindowsAPICodePack.Shell.dll");
            AddFile("WpfMouseWheelLib.dll");
            X86.Add(new AppFile(@"x86\SQLite.Interop.dll", "SQLite.Interop.dll"));
            X64.Add(new AppFile(@"x64\SQLite.Interop.dll", "SQLite.Interop.dll"));
            AddFile(@"Languages\English.lng");
            AddFile(@"Languages\Russian.lng");
            AddFile(@"Languages\Romanian.lng");
            AddFile(@"Languages\Ukrainian.lng");
            AddFile(@"Languages\Spanish.lng");
            AddFile(@"Languages\French.lng");
            AddFile(@"Languages\Simplified Chinese.lng");
            AddFile(@"Mirrors\mirrors.config");
            AddFile(@"Mirrors\booksdescr_org_nonfiction.xslt");
            AddFile(@"Mirrors\booksdescr_org_fiction.xslt");
            AddFile(@"Mirrors\booksdescr_org_scimag.xslt");
            AddFile(@"Mirrors\libgen_me_nonfiction_step1.xslt");
            AddFile(@"Mirrors\libgen_me_nonfiction_step2.xslt");
            AddFile(@"Mirrors\libgen_me_fiction_step1.xslt");
            AddFile(@"Mirrors\libgen_me_fiction_step2.xslt");
            AddFile(@"Mirrors\libgen_me_scimag_step1.xslt");
            AddFile(@"Mirrors\libgen_me_scimag_step2.xslt");
            AddFile(@"Mirrors\bookfi_net.xslt");
            AddFile(@"Mirrors\b_ok_xyz_step1.xslt");
            AddFile(@"Mirrors\b_ok_xyz_step2.xslt");
            AddFile(@"Mirrors\booksc_org_step1.xslt");
            AddFile(@"Mirrors\booksc_org_step2.xslt");
        }

        public static string GetBinariesDirectoryPath(bool is64Bit)
        {
            return String.Format(Constants.BINARIES_DIRECTORY_PATH_FORMAT, (is64Bit ? "64" : "86"));
        }

        public static List<AppFile> X86 { get; }
        public static List<AppFile> X64 { get; }

        private static void AddFile(string filePath)
        {
            X86.Add(new AppFile(filePath, filePath));
            X64.Add(new AppFile(filePath, filePath));
        }
    }
}
