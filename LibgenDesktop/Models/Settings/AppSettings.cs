using System;
using LibgenDesktop.Infrastructure;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models.Settings
{
    internal class AppSettings
    {
        internal class MainWindowSettings
        {
            public static MainWindowSettings Default
            {
                get
                {
                    return new MainWindowSettings
                    {
                        Maximized = false,
                        Left = (WindowManager.ScreenWidth - DEFAULT_MAIN_WINDOW_WIDTH) / 2,
                        Top = (WindowManager.ScreenHeight - DEFAULT_MAIN_WINDOW_HEIGHT) / 2,
                        Width = DEFAULT_MAIN_WINDOW_WIDTH,
                        Height = DEFAULT_MAIN_WINDOW_HEIGHT
                    };
                }
            }

            public bool Maximized { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        internal class ExportPanelSettngs
        {
            internal enum ExportFormat
            {
                XLSX = 1,
                CSV
            }

            internal enum CsvSeparator
            {
                COMMA = 1,
                SEMICOLON,
                TAB
            }

            public static ExportPanelSettngs Default
            {
                get
                {
                    return new ExportPanelSettngs
                    {
                        ExportDirectory = null,
                        Format = ExportFormat.XLSX,
                        Separator = CsvSeparator.COMMA,
                        LimitSearchResults = false
                    };
                }
            }

            public string ExportDirectory { get; set; }
            public ExportFormat Format { get; set; }
            public CsvSeparator Separator { get; set; }
            public bool LimitSearchResults { get; set; }
        }

        internal abstract class DetailsWindowSettings
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        internal class NonFictionDetailsWindowSettings : DetailsWindowSettings
        {
            public static NonFictionDetailsWindowSettings Default
            {
                get
                {
                    return new NonFictionDetailsWindowSettings
                    {
                        Width = DEFAULT_NON_FICTION_DETAILS_WINDOW_WIDTH,
                        Height = DEFAULT_NON_FICTION_DETAILS_WINDOW_HEIGHT
                    };
                }
            }
        }

        internal class NonFictionColumnSettings
        {
            public static NonFictionColumnSettings Default
            {
                get
                {
                    return new NonFictionColumnSettings
                    {
                        TitleColumnWidth = DEFAULT_NON_FICTION_GRID_TITLE_COLUMN_WIDTH,
                        AuthorsColumnWidth = DEFAULT_NON_FICTION_GRID_AUTHORS_COLUMN_WIDTH,
                        SeriesColumnWidth = DEFAULT_NON_FICTION_GRID_SERIES_COLUMN_WIDTH,
                        YearColumnWidth = DEFAULT_NON_FICTION_GRID_YEAR_COLUMN_WIDTH,
                        PublisherColumnWidth = DEFAULT_NON_FICTION_GRID_PUBLISHER_COLUMN_WIDTH,
                        FormatColumnWidth = DEFAULT_NON_FICTION_GRID_FORMAT_COLUMN_WIDTH,
                        FileSizeColumnWidth = DEFAULT_NON_FICTION_GRID_FILESIZE_COLUMN_WIDTH,
                        OcrColumnWidth = DEFAULT_NON_FICTION_GRID_OCR_COLUMN_WIDTH
                    };
                }
            }

            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int SeriesColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int PublisherColumnWidth { get; set; }
            public int FormatColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
            public int OcrColumnWidth { get; set; }
        }

        internal class NonFictionSettings
        {
            public static NonFictionSettings Default
            {
                get
                {
                    return new NonFictionSettings
                    {
                        DetailsWindow = NonFictionDetailsWindowSettings.Default,
                        Columns = NonFictionColumnSettings.Default,
                        ExportPanel = ExportPanelSettngs.Default
                    };
                }
            }

            public NonFictionDetailsWindowSettings DetailsWindow { get; set; }
            public NonFictionColumnSettings Columns { get; set; }
            public ExportPanelSettngs ExportPanel { get; set; }
        }

        internal class FictionDetailsWindowSettings : DetailsWindowSettings
        {
            public static FictionDetailsWindowSettings Default
            {
                get
                {
                    return new FictionDetailsWindowSettings
                    {
                        Width = DEFAULT_FICTION_DETAILS_WINDOW_WIDTH,
                        Height = DEFAULT_FICTION_DETAILS_WINDOW_HEIGHT
                    };
                }
            }
        }

        internal class FictionColumnSettings
        {
            public static FictionColumnSettings Default
            {
                get
                {
                    return new FictionColumnSettings
                    {
                        TitleColumnWidth = DEFAULT_FICTION_GRID_TITLE_COLUMN_WIDTH,
                        AuthorsColumnWidth = DEFAULT_FICTION_GRID_AUTHORS_COLUMN_WIDTH,
                        SeriesColumnWidth = DEFAULT_FICTION_GRID_SERIES_COLUMN_WIDTH,
                        YearColumnWidth = DEFAULT_FICTION_GRID_YEAR_COLUMN_WIDTH,
                        PublisherColumnWidth = DEFAULT_FICTION_GRID_PUBLISHER_COLUMN_WIDTH,
                        FormatColumnWidth = DEFAULT_FICTION_GRID_FORMAT_COLUMN_WIDTH,
                        FileSizeColumnWidth = DEFAULT_FICTION_GRID_FILESIZE_COLUMN_WIDTH
                    };
                }
            }

            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int SeriesColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int PublisherColumnWidth { get; set; }
            public int FormatColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
        }

        internal class FictionSettings
        {
            public static FictionSettings Default
            {
                get
                {
                    return new FictionSettings
                    {
                        DetailsWindow = FictionDetailsWindowSettings.Default,
                        Columns = FictionColumnSettings.Default,
                        ExportPanel = ExportPanelSettngs.Default
                    };
                }
            }

            public FictionDetailsWindowSettings DetailsWindow { get; set; }
            public FictionColumnSettings Columns { get; set; }
            public ExportPanelSettngs ExportPanel { get; set; }
        }

        internal class SciMagDetailsWindowSettings : DetailsWindowSettings
        {
            public static SciMagDetailsWindowSettings Default
            {
                get
                {
                    return new SciMagDetailsWindowSettings
                    {
                        Width = DEFAULT_SCI_MAG_DETAILS_WINDOW_WIDTH,
                        Height = DEFAULT_SCI_MAG_DETAILS_WINDOW_HEIGHT
                    };
                }
            }
        }

        internal class SciMagColumnSettings
        {
            public static SciMagColumnSettings Default
            {
                get
                {
                    return new SciMagColumnSettings
                    {
                        TitleColumnWidth = DEFAULT_SCI_MAG_GRID_TITLE_COLUMN_WIDTH,
                        AuthorsColumnWidth = DEFAULT_SCI_MAG_GRID_AUTHORS_COLUMN_WIDTH,
                        JournalColumnWidth = DEFAULT_SCI_MAG_GRID_JOURNAL_COLUMN_WIDTH,
                        YearColumnWidth = DEFAULT_SCI_MAG_GRID_YEAR_COLUMN_WIDTH,
                        FileSizeColumnWidth = DEFAULT_SCI_MAG_GRID_FILESIZE_COLUMN_WIDTH,
                        DoiColumnWidth = DEFAULT_SCI_MAG_GRID_DOI_COLUMN_WIDTH
                    };
                }
            }

            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int JournalColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
            public int DoiColumnWidth { get; set; }
        }

        internal class SciMagSettings
        {
            public static SciMagSettings Default
            {
                get
                {
                    return new SciMagSettings
                    {
                        DetailsWindow = SciMagDetailsWindowSettings.Default,
                        Columns = SciMagColumnSettings.Default,
                        ExportPanel = ExportPanelSettngs.Default
                    };
                }
            }

            public SciMagDetailsWindowSettings DetailsWindow { get; set; }
            public SciMagColumnSettings Columns { get; set; }
            public ExportPanelSettngs ExportPanel { get; set; }
        }

        internal class NetworkSettings
        {
            public static NetworkSettings Default
            {
                get
                {
                    return new NetworkSettings
                    {
                        OfflineMode = true,
                        UseProxy = false,
                        ProxyAddress = null,
                        ProxyPort = null,
                        ProxyUserName = null,
                        ProxyPassword = null
                    };
                }
            }

            public bool OfflineMode { get; set; }
            public bool UseProxy { get; set; }
            public string ProxyAddress { get; set; }
            public int? ProxyPort { get; set; }
            public string ProxyUserName { get; set; }
            public string ProxyPassword { get; set; }
        }

        internal class MirrorSettings
        {
            public static MirrorSettings Default
            {
                get
                {
                    return new MirrorSettings
                    {
                        NonFictionBooksMirrorName = DEFAULT_MIRROR_NAME,
                        NonFictionCoversMirrorName = DEFAULT_MIRROR_NAME,
                        NonFictionSynchronizationMirrorName = DEFAULT_MIRROR_NAME,
                        FictionBooksMirrorName = DEFAULT_MIRROR_NAME,
                        FictionCoversMirrorName = DEFAULT_MIRROR_NAME,
                        ArticlesMirrorMirrorName = DEFAULT_MIRROR_NAME
                    };
                }
            }

            public string NonFictionBooksMirrorName { get; set; }
            public string NonFictionCoversMirrorName { get; set; }
            public string NonFictionSynchronizationMirrorName { get; set; }
            public string FictionBooksMirrorName { get; set; }
            public string FictionCoversMirrorName { get; set; }
            public string ArticlesMirrorMirrorName { get; set; }
        }

        internal class SearchSettings
        {
            internal enum DetailsMode
            {
                NEW_MODAL_WINDOW = 1,
                NEW_NON_MODAL_WINDOW,
                NEW_TAB
            }

            public static SearchSettings Default
            {
                get
                {
                    return new SearchSettings
                    {
                        LimitResults = true,
                        MaximumResultCount = DEFAULT_MAXIMUM_SEARCH_RESULT_COUNT,
                        OpenDetailsMode = DetailsMode.NEW_MODAL_WINDOW
                    };
                }
            }

            public bool LimitResults { get; set; }
            public int MaximumResultCount { get; set; }
            public DetailsMode OpenDetailsMode { get; set; }
        }

        internal class ExportSettings
        {
            public static ExportSettings Default
            {
                get
                {
                    return new ExportSettings
                    {
                        OpenResultsAfterExport = false,
                        SplitIntoMultipleFiles = false,
                        MaximumRowsPerFile = MAX_EXPORT_ROWS_PER_FILE
                    };
                }
            }

            public bool OpenResultsAfterExport { get; set; }
            public bool SplitIntoMultipleFiles { get; set; }
            public int MaximumRowsPerFile { get; set; }
        }

        internal class AdvancedSettings
        {
            public static AdvancedSettings Default
            {
                get
                {
                    return new AdvancedSettings
                    {
                        LoggingEnabled = false
                    };
                }
            }

            public bool LoggingEnabled { get; set; }
        }

        public static AppSettings Default
        {
            get
            {
                return new AppSettings
                {
                    DatabaseFileName = String.Empty,
                    MainWindow = MainWindowSettings.Default,
                    NonFiction = NonFictionSettings.Default,
                    Fiction = FictionSettings.Default,
                    SciMag = SciMagSettings.Default,
                    Network = NetworkSettings.Default,
                    Mirrors = MirrorSettings.Default,
                    Search = SearchSettings.Default,
                    Export = ExportSettings.Default,
                    Advanced = AdvancedSettings.Default
                };
            }
        }

        public string DatabaseFileName { get; set; }
        public MainWindowSettings MainWindow { get; set; }
        public NonFictionSettings NonFiction { get; set; }
        public FictionSettings Fiction { get; set; }
        public SciMagSettings SciMag { get; set; }
        public NetworkSettings Network { get; set; }
        public MirrorSettings Mirrors { get; set; }
        public SearchSettings Search { get; set; }
        public ExportSettings Export { get; set; }
        public AdvancedSettings Advanced { get; set; }

        public static AppSettings ValidateAndCorrect(AppSettings appSettings)
        {
            if (appSettings == null)
            {
                return Default;
            }
            else
            {
                appSettings.ValidateAndCorrectDatabaseFileName();
                appSettings.ValidateAndCorrectMainWindowSettings();
                appSettings.ValidateAndCorrectNonFictionSettings();
                appSettings.ValidateAndCorrectFictionSettings();
                appSettings.ValidateAndCorrectNetworkSettings();
                appSettings.ValidateAndCorrectMirrorSettings();
                appSettings.ValidateAndCorrectSearchSettings();
                appSettings.ValidateAndCorrectSciMagSettings();
                appSettings.ValidateAndCorrectExportSettings();
                appSettings.ValidateAndCorrectAdvancedSettings();
                return appSettings;
            }
        }

        private void ValidateAndCorrectDatabaseFileName()
        {
            if (DatabaseFileName == null)
            {
                DatabaseFileName = String.Empty;
            }
        }

        private void ValidateAndCorrectMainWindowSettings()
        {
            if (MainWindow == null)
            {
                MainWindow = MainWindowSettings.Default;
            }
            else
            {
                if (MainWindow.Width < MAIN_WINDOW_MIN_WIDTH)
                {
                    MainWindow.Width = MAIN_WINDOW_MIN_WIDTH;
                }
                if (MainWindow.Height < MAIN_WINDOW_MIN_HEIGHT)
                {
                    MainWindow.Height = MAIN_WINDOW_MIN_HEIGHT;
                }
                if (MainWindow.Left >= WindowManager.ScreenWidth)
                {
                    MainWindow.Left = WindowManager.ScreenWidth - MainWindow.Width;
                }
                if (MainWindow.Top >= WindowManager.ScreenHeight)
                {
                    MainWindow.Top = WindowManager.ScreenHeight - MainWindow.Height;
                }
                if (MainWindow.Left < 0)
                {
                    MainWindow.Left = 0;
                }
                if (MainWindow.Top < 0)
                {
                    MainWindow.Top = 0;
                }
            }
        }

        private void ValidateAndCorrectNonFictionSettings()
        {
            if (NonFiction == null)
            {
                NonFiction = NonFictionSettings.Default;
            }
            else
            {
                if (NonFiction.DetailsWindow == null)
                {
                    NonFiction.DetailsWindow = NonFictionDetailsWindowSettings.Default;
                }
                else
                {
                    NonFictionDetailsWindowSettings nonFictionDetailsWindow = NonFiction.DetailsWindow;
                    if (nonFictionDetailsWindow.Width < NON_FICTION_DETAILS_WINDOW_MIN_WIDTH)
                    {
                        nonFictionDetailsWindow.Width = NON_FICTION_DETAILS_WINDOW_MIN_WIDTH;
                    }
                    if (nonFictionDetailsWindow.Height < NON_FICTION_DETAILS_WINDOW_MIN_HEIGHT)
                    {
                        nonFictionDetailsWindow.Height = NON_FICTION_DETAILS_WINDOW_MIN_HEIGHT;
                    }
                }
                if (NonFiction.Columns == null)
                {
                    NonFiction.Columns = NonFictionColumnSettings.Default;
                }
                else
                {
                    NonFictionColumnSettings nonFictionColumns = NonFiction.Columns;
                    if (nonFictionColumns.TitleColumnWidth < NON_FICTION_GRID_TITLE_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.TitleColumnWidth = NON_FICTION_GRID_TITLE_COLUMN_MIN_WIDTH;
                    }
                    if (nonFictionColumns.AuthorsColumnWidth < NON_FICTION_GRID_AUTHORS_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.AuthorsColumnWidth = NON_FICTION_GRID_AUTHORS_COLUMN_MIN_WIDTH;
                    }
                    if (nonFictionColumns.SeriesColumnWidth < NON_FICTION_GRID_SERIES_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.SeriesColumnWidth = NON_FICTION_GRID_SERIES_COLUMN_MIN_WIDTH;
                    }
                    if (nonFictionColumns.YearColumnWidth < NON_FICTION_GRID_YEAR_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.YearColumnWidth = NON_FICTION_GRID_YEAR_COLUMN_MIN_WIDTH;
                    }
                    if (nonFictionColumns.PublisherColumnWidth < NON_FICTION_GRID_PUBLISHER_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.PublisherColumnWidth = NON_FICTION_GRID_PUBLISHER_COLUMN_MIN_WIDTH;
                    }
                    if (nonFictionColumns.FormatColumnWidth < NON_FICTION_GRID_FORMAT_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.FormatColumnWidth = NON_FICTION_GRID_FORMAT_COLUMN_MIN_WIDTH;
                    }
                    if (nonFictionColumns.FileSizeColumnWidth < NON_FICTION_GRID_FILESIZE_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.FileSizeColumnWidth = NON_FICTION_GRID_FILESIZE_COLUMN_MIN_WIDTH;
                    }
                    if (nonFictionColumns.OcrColumnWidth < NON_FICTION_GRID_OCR_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.OcrColumnWidth = NON_FICTION_GRID_OCR_COLUMN_MIN_WIDTH;
                    }
                }
                NonFiction.ExportPanel = ValidateAndCorrectExportPanelSettings(NonFiction.ExportPanel);
            }
        }

        private void ValidateAndCorrectFictionSettings()
        {
            if (Fiction == null)
            {
                Fiction = FictionSettings.Default;
            }
            else
            {
                if (Fiction.DetailsWindow == null)
                {
                    Fiction.DetailsWindow = FictionDetailsWindowSettings.Default;
                }
                else
                {
                    FictionDetailsWindowSettings fictionDetailsWindow = Fiction.DetailsWindow;
                    if (fictionDetailsWindow.Width < FICTION_DETAILS_WINDOW_MIN_WIDTH)
                    {
                        fictionDetailsWindow.Width = FICTION_DETAILS_WINDOW_MIN_WIDTH;
                    }
                    if (fictionDetailsWindow.Height < FICTION_DETAILS_WINDOW_MIN_HEIGHT)
                    {
                        fictionDetailsWindow.Height = FICTION_DETAILS_WINDOW_MIN_HEIGHT;
                    }
                }
                if (Fiction.Columns == null)
                {
                    Fiction.Columns = FictionColumnSettings.Default;
                }
                else
                {
                    FictionColumnSettings fictionColumns = Fiction.Columns;
                    if (fictionColumns.TitleColumnWidth < FICTION_GRID_TITLE_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.TitleColumnWidth = FICTION_GRID_TITLE_COLUMN_MIN_WIDTH;
                    }
                    if (fictionColumns.AuthorsColumnWidth < FICTION_GRID_AUTHORS_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.AuthorsColumnWidth = FICTION_GRID_AUTHORS_COLUMN_MIN_WIDTH;
                    }
                    if (fictionColumns.SeriesColumnWidth < FICTION_GRID_SERIES_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.SeriesColumnWidth = FICTION_GRID_SERIES_COLUMN_MIN_WIDTH;
                    }
                    if (fictionColumns.YearColumnWidth < FICTION_GRID_YEAR_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.YearColumnWidth = FICTION_GRID_YEAR_COLUMN_MIN_WIDTH;
                    }
                    if (fictionColumns.PublisherColumnWidth < FICTION_GRID_PUBLISHER_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.PublisherColumnWidth = FICTION_GRID_PUBLISHER_COLUMN_MIN_WIDTH;
                    }
                    if (fictionColumns.FormatColumnWidth < FICTION_GRID_FORMAT_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.FormatColumnWidth = FICTION_GRID_FORMAT_COLUMN_MIN_WIDTH;
                    }
                    if (fictionColumns.FileSizeColumnWidth < FICTION_GRID_FILESIZE_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.FileSizeColumnWidth = FICTION_GRID_FILESIZE_COLUMN_MIN_WIDTH;
                    }
                }
                Fiction.ExportPanel = ValidateAndCorrectExportPanelSettings(Fiction.ExportPanel);
            }
        }

        private void ValidateAndCorrectSciMagSettings()
        {
            if (SciMag == null)
            {
                SciMag = SciMagSettings.Default;
            }
            else
            {
                if (SciMag.DetailsWindow == null)
                {
                    SciMag.DetailsWindow = SciMagDetailsWindowSettings.Default;
                }
                else
                {
                    SciMagDetailsWindowSettings sciMagDetailsWindow = SciMag.DetailsWindow;
                    if (sciMagDetailsWindow.Width < SCI_MAG_DETAILS_WINDOW_MIN_WIDTH)
                    {
                        sciMagDetailsWindow.Width = SCI_MAG_DETAILS_WINDOW_MIN_WIDTH;
                    }
                    if (sciMagDetailsWindow.Height < SCI_MAG_DETAILS_WINDOW_MIN_HEIGHT)
                    {
                        sciMagDetailsWindow.Height = SCI_MAG_DETAILS_WINDOW_MIN_HEIGHT;
                    }
                }
                if (SciMag.Columns == null)
                {
                    SciMag.Columns = SciMagColumnSettings.Default;
                }
                else
                {
                    SciMagColumnSettings sciMagColumns = SciMag.Columns;
                    if (sciMagColumns.TitleColumnWidth < SCI_MAG_GRID_TITLE_COLUMN_MIN_WIDTH)
                    {
                        sciMagColumns.TitleColumnWidth = SCI_MAG_GRID_TITLE_COLUMN_MIN_WIDTH;
                    }
                    if (sciMagColumns.AuthorsColumnWidth < SCI_MAG_GRID_AUTHORS_COLUMN_MIN_WIDTH)
                    {
                        sciMagColumns.AuthorsColumnWidth = SCI_MAG_GRID_AUTHORS_COLUMN_MIN_WIDTH;
                    }
                    if (sciMagColumns.JournalColumnWidth < SCI_MAG_GRID_JOURNAL_COLUMN_MIN_WIDTH)
                    {
                        sciMagColumns.JournalColumnWidth = SCI_MAG_GRID_JOURNAL_COLUMN_MIN_WIDTH;
                    }
                    if (sciMagColumns.YearColumnWidth < SCI_MAG_GRID_YEAR_COLUMN_MIN_WIDTH)
                    {
                        sciMagColumns.YearColumnWidth = SCI_MAG_GRID_YEAR_COLUMN_MIN_WIDTH;
                    }
                    if (sciMagColumns.FileSizeColumnWidth < SCI_MAG_GRID_FILESIZE_COLUMN_MIN_WIDTH)
                    {
                        sciMagColumns.FileSizeColumnWidth = SCI_MAG_GRID_FILESIZE_COLUMN_MIN_WIDTH;
                    }
                    if (sciMagColumns.DoiColumnWidth < SCI_MAG_GRID_DOI_COLUMN_MIN_WIDTH)
                    {
                        sciMagColumns.DoiColumnWidth = SCI_MAG_GRID_DOI_COLUMN_MIN_WIDTH;
                    }
                }
                SciMag.ExportPanel = ValidateAndCorrectExportPanelSettings(SciMag.ExportPanel);
            }
        }

        private ExportPanelSettngs ValidateAndCorrectExportPanelSettings(ExportPanelSettngs exportPanelSettngs)
        {
            if (exportPanelSettngs == null)
            {
                exportPanelSettngs = ExportPanelSettngs.Default;
            }
            else
            {
                if (!Enum.IsDefined(typeof(ExportPanelSettngs.ExportFormat), exportPanelSettngs.Format))
                {
                    exportPanelSettngs.Format = ExportPanelSettngs.ExportFormat.XLSX;
                }
                if (!Enum.IsDefined(typeof(ExportPanelSettngs.CsvSeparator), exportPanelSettngs.Separator))
                {
                    exportPanelSettngs.Separator = ExportPanelSettngs.CsvSeparator.COMMA;
                }
            }
            return exportPanelSettngs;
        }

        private void ValidateAndCorrectNetworkSettings()
        {
            if (Network == null)
            {
                Network = NetworkSettings.Default;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(Network.ProxyAddress))
                {
                    Network.UseProxy = false;
                }
                if (Network.ProxyPort.HasValue && (Network.ProxyPort.Value < MIN_PROXY_PORT || Network.ProxyPort.Value > MAX_PROXY_PORT))
                {
                    Network.ProxyPort = null;
                    Network.UseProxy = false;
                }
            }
        }

        private void ValidateAndCorrectMirrorSettings()
        {
            if (Mirrors == null)
            {
                Mirrors = MirrorSettings.Default;
            }
        }

        private void ValidateAndCorrectSearchSettings()
        {
            if (Search == null)
            {
                Search = SearchSettings.Default;
            }
            else
            {
                if (Search.MaximumResultCount <= 0)
                {
                    Search.MaximumResultCount = DEFAULT_MAXIMUM_SEARCH_RESULT_COUNT;
                }
                if (!Enum.IsDefined(typeof(SearchSettings.DetailsMode), Search.OpenDetailsMode))
                {
                    Search.OpenDetailsMode = SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
                }
            }
        }

        private void ValidateAndCorrectExportSettings()
        {
            if (Export == null)
            {
                Export = ExportSettings.Default;
            }
            else
            {
                if (Export.MaximumRowsPerFile <= 0 || Export.MaximumRowsPerFile > MAX_EXPORT_ROWS_PER_FILE)
                {
                    Export.MaximumRowsPerFile = MAX_EXPORT_ROWS_PER_FILE;
                }
            }
        }

        private void ValidateAndCorrectAdvancedSettings()
        {
            if (Advanced == null)
            {
                Advanced = AdvancedSettings.Default;
            }
        }
    }
}
