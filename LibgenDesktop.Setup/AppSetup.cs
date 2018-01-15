using System;
using System.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

namespace LibgenDesktop.Setup
{
    internal static class AppSetup
    {
        public static void Build32BitSetupPackage()
        {
            BuildSetupPackage(false, "418C00BD-8473-40B6-8D83-DC917B3AF457");
        }

        public static void Build64BitSetupPackage()
        {
            BuildSetupPackage(true, "88846ABC-26D2-4038-B51A-94A082CE53D2");
        }

        private static void BuildSetupPackage(bool is64Bit, string projectGuid)
        {
            string productTitle = String.Format(Constants.PRODUCT_TITLE_FORMAT, is64Bit ? 64 : 32);
            string shortcutTitle = String.Format(Constants.SHORTCUT_TITLE_FORMAT, is64Bit ? 64 : 32);
            string normalizedCurrentVersion = Constants.CURRENT_VERSION.Count(c => c == '.') > 1 ? Constants.CURRENT_VERSION : Constants.CURRENT_VERSION + ".0";
            string installerFileName = String.Format(Constants.INSTALLER_FILE_NAME_FORMAT, is64Bit ? 64 : 32);
            Project project = new Project(productTitle, new Dir(@"%ProgramFiles%\Libgen Desktop"));
            Dir targetDirectory = project.Dirs.First().Dirs.First();
            foreach (string fileName in AppFiles.GetFileList(is64Bit))
            {
                string filePath = Utils.GetFullFilePath(AppFiles.GetBinariesDirectoryPath(is64Bit), fileName);
                File file = new File(filePath);
                if (fileName == Constants.MAIN_EXECUTABLE_NAME)
                {
                    file.Shortcuts = new[]
                    {
                        new FileShortcut(shortcutTitle, "%ProgramMenu%")
                    };
                }
                targetDirectory.AddFile(file);
            }
            project.GUID = new Guid(projectGuid);
            project.ControlPanelInfo.Manufacturer = Constants.PRODUCT_COMPANY;
            project.ControlPanelInfo.ProductIcon = Constants.APP_ICON_PATH;
            project.Version = new Version(Constants.CURRENT_VERSION);
            project.MajorUpgradeStrategy = new MajorUpgradeStrategy
            {
                UpgradeVersions = new VersionRange
                {
                    Minimum = "0.0.0",
                    Maximum = normalizedCurrentVersion,
                    IncludeMinimum = true,
                    IncludeMaximum = false
                },
                PreventDowngradingVersions = new VersionRange
                {
                    Minimum = normalizedCurrentVersion,
                    IncludeMinimum = false
                },
                NewerProductInstalledErrorMessage = "Newer version is already installed."
            };
            project.SetNetFxPrerequisite("WIX_IS_NETFRAMEWORK_45_OR_LATER_INSTALLED", ".NET Framework 4.5 or newer must be installed first. You can download it at http://dot.net.");
            project.Platform = is64Bit ? Platform.x64 : Platform.x86;
            project.InstallScope = InstallScope.perMachine;
            project.UI = WUI.WixUI_InstallDir;
            project.CustomUI = new DialogSequence()
                .On(NativeDialogs.WelcomeDlg, Buttons.Next, new ShowDialog(NativeDialogs.InstallDirDlg))
                .On(NativeDialogs.InstallDirDlg, Buttons.Back, new ShowDialog(NativeDialogs.WelcomeDlg));
            project.BuildMsi(installerFileName);
            Utils.MoveFile($"{installerFileName}.msi", @"..\Release");
        }
    }
}
