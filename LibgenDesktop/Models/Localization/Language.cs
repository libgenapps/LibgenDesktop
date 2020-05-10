using System;
using System.Collections.Generic;
using System.Linq;
using LibgenDesktop.Models.Localization.Localizators.Export;
using LibgenDesktop.Models.Localization.Localizators.Tabs;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.Models.Localization
{
    internal class Language
    {
        private readonly List<Translation> translations;
        private MainWindowLocalizator mainWindow;
        private SetupWizardWindowLocalizator setupWizardWindow;
        private SetupWizardProxySettingsWindowLocalizator setupWizardProxySettingsWindowLocalizator;
        private SearchTabLocalizator searchTab;
        private NonFictionSearchResultsTabLocalizator nonFictionSearchResultsTab;
        private FictionSearchResultsTabLocalizator fictionSearchResultsTab;
        private SciMagSearchResultsTabLocalizator sciMagSearchResultsTab;
        private CommonDetailsTabLocalizator commonDetailsTab;
        private NonFictionDetailsTabLocalizator nonFictionDetailsTab;
        private FictionDetailsTabLocalizator fictionDetailsTab;
        private SciMagDetailsTabLocalizator sciMagDetailsTab;
        private ImportWindowLocalizator import;
        private ExportPanelLocalizator exportPanel;
        private NonFictionExporterLocalizator nonFictionExporter;
        private FictionExporterLocalizator fictionExporter;
        private SciMagExporterLocalizator sciMagExporter;
        private SynchronizationWindowLocalizator synchronization;
        private DatabaseWindowLocalizator database;
        private DatabaseErrorWindowLocalizator databaseError;
        private SqlDebuggerWindowLocalizator sqlDebugger;
        private DownloadManagerTabLocalizator downloadManager;
        private ApplicationUpdateWindowLocalizator applicationUpdate;
        private LibraryTabLocalizator library;
        private SettingsWindowLocalizator settings;
        private AboutWindowLocalizator about;
        private MessageBoxLocalizator messageBox;
        private ErrorWindowLocalizator errorWindow;

        public Language(List<Translation> prioritizedTranslationList, decimal percentTranslated)
        {
            translations = prioritizedTranslationList;
            mainWindow = null;
            setupWizardWindow = null;
            setupWizardProxySettingsWindowLocalizator = null;
            searchTab = null;
            nonFictionSearchResultsTab = null;
            fictionSearchResultsTab = null;
            sciMagSearchResultsTab = null;
            commonDetailsTab = null;
            nonFictionDetailsTab = null;
            fictionDetailsTab = null;
            sciMagDetailsTab = null;
            import = null;
            exportPanel = null;
            nonFictionExporter = null;
            fictionExporter = null;
            sciMagExporter = null;
            synchronization = null;
            database = null;
            databaseError = null;
            sqlDebugger = null;
            downloadManager = null;
            applicationUpdate = null;
            settings = null;
            about = null;
            messageBox = null;
            errorWindow = null;
            Translation mainTranslation = prioritizedTranslationList.First();
            Name = mainTranslation.General?.Name?.Trim() ?? String.Empty;
            LocalizedName = mainTranslation.General?.LocalizedName?.Trim() ?? String.Empty;
            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(LocalizedName))
            {
                DisplayName = $"{Name} ({LocalizedName})";
            }
            else
            {
                DisplayName = "Error";
            }
            CultureCode = mainTranslation.General?.CultureCode?.Trim() ?? String.Empty;
            TranslatorName = mainTranslation.General?.TranslatorName ?? "unknown";
            PercentTranslated = percentTranslated;
            Formatter = new LanguageFormatter(prioritizedTranslationList);
        }

        public string Name { get; }
        public string LocalizedName { get; }
        public string DisplayName { get; }
        public string CultureCode { get; }
        public string TranslatorName { get; }
        public decimal PercentTranslated { get; }
        public LanguageFormatter Formatter { get; }

        public MainWindowLocalizator MainWindow => mainWindow ?? (mainWindow = new MainWindowLocalizator(translations, Formatter));

        public SetupWizardWindowLocalizator SetupWizardWindow =>
            setupWizardWindow ?? (setupWizardWindow = new SetupWizardWindowLocalizator(translations, Formatter));

        public SetupWizardProxySettingsWindowLocalizator SetupWizardProxySettingsWindowLocalizator =>
            setupWizardProxySettingsWindowLocalizator ?? (setupWizardProxySettingsWindowLocalizator = new SetupWizardProxySettingsWindowLocalizator(translations, Formatter));

        public SearchTabLocalizator SearchTab => searchTab ?? (searchTab = new SearchTabLocalizator(translations, Formatter));

        public NonFictionSearchResultsTabLocalizator NonFictionSearchResultsTab =>
            nonFictionSearchResultsTab ?? (nonFictionSearchResultsTab = new NonFictionSearchResultsTabLocalizator(translations, Formatter));

        public FictionSearchResultsTabLocalizator FictionSearchResultsTab =>
            fictionSearchResultsTab ?? (fictionSearchResultsTab = new FictionSearchResultsTabLocalizator(translations, Formatter));

        public SciMagSearchResultsTabLocalizator SciMagSearchResultsTab =>
            sciMagSearchResultsTab ?? (sciMagSearchResultsTab = new SciMagSearchResultsTabLocalizator(translations, Formatter));

        public CommonDetailsTabLocalizator CommonDetailsTab =>
            commonDetailsTab ?? (commonDetailsTab = new CommonDetailsTabLocalizator(translations, Formatter));

        public NonFictionDetailsTabLocalizator NonFictionDetailsTab =>
            nonFictionDetailsTab ?? (nonFictionDetailsTab = new NonFictionDetailsTabLocalizator(translations, Formatter));

        public FictionDetailsTabLocalizator FictionDetailsTab =>
            fictionDetailsTab ?? (fictionDetailsTab = new FictionDetailsTabLocalizator(translations, Formatter));

        public SciMagDetailsTabLocalizator SciMagDetailsTab =>
            sciMagDetailsTab ?? (sciMagDetailsTab = new SciMagDetailsTabLocalizator(translations, Formatter));

        public ImportWindowLocalizator Import => import ?? (import = new ImportWindowLocalizator(translations, Formatter));

        public ExportPanelLocalizator ExportPanel => exportPanel ?? (exportPanel = new ExportPanelLocalizator(translations, Formatter));

        public NonFictionExporterLocalizator NonFictionExporter =>
            nonFictionExporter ?? (nonFictionExporter = new NonFictionExporterLocalizator(translations, Formatter));

        public FictionExporterLocalizator FictionExporter => fictionExporter ?? (fictionExporter = new FictionExporterLocalizator(translations, Formatter));

        public SciMagExporterLocalizator SciMagExporter => sciMagExporter ?? (sciMagExporter = new SciMagExporterLocalizator(translations, Formatter));

        public SynchronizationWindowLocalizator Synchronization =>
            synchronization ?? (synchronization = new SynchronizationWindowLocalizator(translations, Formatter));

        public DatabaseWindowLocalizator Database => database ?? (database = new DatabaseWindowLocalizator(translations, Formatter));

        public DatabaseErrorWindowLocalizator DatabaseError => databaseError ?? (databaseError = new DatabaseErrorWindowLocalizator(translations, Formatter));

        public SqlDebuggerWindowLocalizator SqlDebugger => sqlDebugger ?? (sqlDebugger = new SqlDebuggerWindowLocalizator(translations, Formatter));

        public DownloadManagerTabLocalizator DownloadManager =>
            downloadManager ?? (downloadManager = new DownloadManagerTabLocalizator(translations, Formatter));

        public ApplicationUpdateWindowLocalizator ApplicationUpdate =>
            applicationUpdate ?? (applicationUpdate = new ApplicationUpdateWindowLocalizator(translations, Formatter));

        public LibraryTabLocalizator Library => library ?? (library = new LibraryTabLocalizator(translations, Formatter));

        public SettingsWindowLocalizator Settings => settings ?? (settings = new SettingsWindowLocalizator(translations, Formatter));

        public AboutWindowLocalizator About => about ?? (about = new AboutWindowLocalizator(translations, Formatter));

        public MessageBoxLocalizator MessageBox => messageBox ?? (messageBox = new MessageBoxLocalizator(translations, Formatter));

        public ErrorWindowLocalizator ErrorWindow => errorWindow ?? (errorWindow = new ErrorWindowLocalizator(translations, Formatter));
    }
}
