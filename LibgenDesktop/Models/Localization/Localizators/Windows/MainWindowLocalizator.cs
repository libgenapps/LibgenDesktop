using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class MainWindowLocalizator : Localizator<Translation.MainWindowTranslation>
    {
        public MainWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.MainWindow)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            ToolbarDownloadManagerTooltip = Format(section => section?.DownloadManagerTooltip);
            ToolbarBookmarksTooltip = Format(section => section?.BookmarksTooltip);
            ToolbarNoBookmarks = Format(section => section?.NoBookmarks);
            ToolbarUpdate = Format(section => section?.Update);
            ToolbarImport = Format(section => section?.Import);
            ToolbarSynchronize = Format(section => section?.Synchronize);
            ToolbarLibrary = Format(section => section?.Library);
            ToolbarDatabase = Format(section => section?.Database);
            ToolbarSqlDebugger = Format(section => section?.SqlDebugger);
            ToolbarSettings = Format(section => section?.Settings);
            ToolbarAbout = Format(section => section?.About);
        }

        public string WindowTitle { get; }
        public string ToolbarDownloadManagerTooltip { get; }
        public string ToolbarBookmarksTooltip { get; }
        public string ToolbarNoBookmarks { get; }
        public string ToolbarUpdate { get; }
        public string ToolbarImport { get; }
        public string ToolbarSynchronize { get; }
        public string ToolbarLibrary { get; }
        public string ToolbarDatabase { get; }
        public string ToolbarSqlDebugger { get; }
        public string ToolbarSettings { get; }
        public string ToolbarAbout { get; }

        private string Format(Func<Translation.MainMenuTranslation, string> field) => Format(translation => field(translation?.MainWindow?.MainMenu));
    }
}
