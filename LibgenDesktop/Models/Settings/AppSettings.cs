﻿using System;
using System.Collections.Generic;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models.Entities;
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

        internal class ExportPanelSettings
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

            public static ExportPanelSettings Default
            {
                get
                {
                    return new ExportPanelSettings
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
                        IdentifierColumnWidth = DEFAULT_NON_FICTION_GRID_IDENTIFIER_COLUMN_WIDTH,
                        LanguageColumnWidth = DEFAULT_NON_FICTION_GRID_LANGUAGE_COLUMN_WIDTH,
                        PublisherColumnWidth = DEFAULT_NON_FICTION_GRID_PUBLISHER_COLUMN_WIDTH,
                        FormatColumnWidth = DEFAULT_NON_FICTION_GRID_FORMAT_COLUMN_WIDTH,
                        FileSizeColumnWidth = DEFAULT_NON_FICTION_GRID_FILESIZE_COLUMN_WIDTH,
                        OcrColumnWidth = DEFAULT_NON_FICTION_GRID_OCR_COLUMN_WIDTH,
                        ExistsInLibraryColumnWidth = DEFAULT_NON_FICTION_GRID_EXISTS_IN_LIBRARY_COLUMN_WIDTH
                    };
                }
            }

            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int SeriesColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int IdentifierColumnWidth { get; set; }
            public int LanguageColumnWidth { get; set; }
            public int PublisherColumnWidth { get; set; }
            public int FormatColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
            public int OcrColumnWidth { get; set; }
            public int ExistsInLibraryColumnWidth { get; set; }
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
                        ExportPanel = ExportPanelSettings.Default
                    };
                }
            }

            public NonFictionDetailsWindowSettings DetailsWindow { get; set; }
            public NonFictionColumnSettings Columns { get; set; }
            public ExportPanelSettings ExportPanel { get; set; }
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
                        LanguageColumnWidth = DEFAULT_FICTION_GRID_LANGUAGE_COLUMN_WIDTH,
                        PublisherColumnWidth = DEFAULT_FICTION_GRID_PUBLISHER_COLUMN_WIDTH,
                        FormatColumnWidth = DEFAULT_FICTION_GRID_FORMAT_COLUMN_WIDTH,
                        FileSizeColumnWidth = DEFAULT_FICTION_GRID_FILESIZE_COLUMN_WIDTH,
                        ExistsInLibraryColumnWidth = DEFAULT_FICTION_GRID_EXISTS_IN_LIBRARY_COLUMN_WIDTH
                    };
                }
            }

            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int SeriesColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int LanguageColumnWidth { get; set; }
            public int PublisherColumnWidth { get; set; }
            public int FormatColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
            public int ExistsInLibraryColumnWidth { get; set; }
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
                        ExportPanel = ExportPanelSettings.Default
                    };
                }
            }

            public FictionDetailsWindowSettings DetailsWindow { get; set; }
            public FictionColumnSettings Columns { get; set; }
            public ExportPanelSettings ExportPanel { get; set; }
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
                        DoiColumnWidth = DEFAULT_SCI_MAG_GRID_DOI_COLUMN_WIDTH,
                        ExistsInLibraryColumnWidth = DEFAULT_SCI_MAG_GRID_EXISTS_IN_LIBRARY_COLUMN_WIDTH
                    };
                }
            }

            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int JournalColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
            public int DoiColumnWidth { get; set; }
            public int ExistsInLibraryColumnWidth { get; set; }
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
                        ExportPanel = ExportPanelSettings.Default
                    };
                }
            }

            public SciMagDetailsWindowSettings DetailsWindow { get; set; }
            public SciMagColumnSettings Columns { get; set; }
            public ExportPanelSettings ExportPanel { get; set; }
        }

        internal class DownloadManagerTabSettings
        {
            public static DownloadManagerTabSettings Default
            {
                get
                {
                    return new DownloadManagerTabSettings
                    {
                        LogPanelHeight = DOWNLOAD_MANAGER_TAB_LOG_PANEL_DEFAULT_HEIGHT,
                        ShowDebugLogs = false
                    };
                }
            }

            public int LogPanelHeight { get; set; }
            public bool ShowDebugLogs { get; set; }
        }

        internal class LastUpdateSettings
        {
            public static LastUpdateSettings Default
            {
                get
                {
                    return new LastUpdateSettings
                    {
                        LastCheckedAt = null,
                        IgnoreReleaseName = null
                    };
                }
            }

            public DateTime? LastCheckedAt { get; set; }
            public string IgnoreReleaseName { get; set; }
        }

        internal class BookmarkSettings
        {
            internal class Bookmark
            {
                public string Name { get; set; }
                public string Query { get; set; }
                public LibgenObjectType Type { get; set; }
            }

            public static BookmarkSettings Default
            {
                get
                {
                    return new BookmarkSettings
                    {
                        Bookmarks = new List<Bookmark>()
                    };
                }
            }

            public List<Bookmark> Bookmarks { get; set; }
        }

        internal class GeneralSettings
        {
            internal enum UpdateCheckInterval
            {
                NEVER = 1,
                DAILY,
                WEEKLY,
                MONTHLY
            }

            public static GeneralSettings Default
            {
                get
                {
                    return new GeneralSettings
                    {
                        Language = null,
                        UpdateCheck = UpdateCheckInterval.NEVER,
                        UpdateUrl = DEFAULT_GITHUB_RELEASE_API_URL
                    };
                }
            }

            public string Language { get; set; }
            public UpdateCheckInterval UpdateCheck { get; set; }
            public string UpdateUrl { get; set; }
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

        internal class DownloadSettings
        {
            public static DownloadSettings Default
            {
                get
                {
                    return new DownloadSettings
                    {
                        UseDownloadManager = false,
                        DownloadDirectory = null,
                        Timeout = DEFAULT_DOWNLOAD_TIMEOUT,
                        Attempts = DEFAULT_DOWNLOAD_ATTEMPT_COUNT,
                        RetryDelay = DEFAULT_DOWNLOAD_RETRY_DELAY
                    };
                }
            }

            public bool UseDownloadManager { get; set; }
            public string DownloadDirectory { get; set; }
            public int Timeout { get; set; }
            public int Attempts { get; set; }
            public int RetryDelay { get; set; }
        }

        internal class MirrorSettings
        {
            public static MirrorSettings Default
            {
                get
                {
                    return new MirrorSettings
                    {
                        NonFictionBooksMirrorName = DEFAULT_DOWNLOAD_MIRROR_NAME,
                        NonFictionCoversMirrorName = DEFAULT_DOWNLOAD_MIRROR_NAME,
                        NonFictionSynchronizationMirrorName = DEFAULT_SYNCHRONIZATION_MIRROR_NAME,
                        FictionBooksMirrorName = DEFAULT_DOWNLOAD_MIRROR_NAME,
                        FictionCoversMirrorName = DEFAULT_DOWNLOAD_MIRROR_NAME,
                        ArticlesMirrorName = DEFAULT_DOWNLOAD_MIRROR_NAME
                    };
                }
            }

            public string NonFictionBooksMirrorName { get; set; }
            public string NonFictionCoversMirrorName { get; set; }
            public string NonFictionSynchronizationMirrorName { get; set; }
            public string FictionBooksMirrorName { get; set; }
            public string FictionCoversMirrorName { get; set; }
            public string ArticlesMirrorName { get; set; }
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
                        LoggingEnabled = false,
                        SqlDebuggerEnabled = false
                    };
                }
            }

            public bool LoggingEnabled { get; set; }
            public bool SqlDebuggerEnabled { get; set; }
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
                    DownloadManagerTab = DownloadManagerTabSettings.Default,
                    LastUpdate = LastUpdateSettings.Default,
                    Bookmarks = BookmarkSettings.Default,
                    General = GeneralSettings.Default,
                    Network = NetworkSettings.Default,
                    Download = DownloadSettings.Default,
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
        public DownloadManagerTabSettings DownloadManagerTab { get; set; }
        public LastUpdateSettings LastUpdate { get; set; }
        public BookmarkSettings Bookmarks { get; set; }
        public GeneralSettings General { get; set; }
        public NetworkSettings Network { get; set; }
        public DownloadSettings Download { get; set; }
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
                appSettings.ValidateAndCorrectSciMagSettings();
                appSettings.ValidateAndCorrectDownloadManagerTabSettings();
                appSettings.ValidateAndCorrectLastUpdateSettings();
                appSettings.ValidateAndCorrectBookmarkSettings();
                appSettings.ValidateAndCorrectGeneralSettings();
                appSettings.ValidateAndCorrectNetworkSettings();
                appSettings.ValidateAndCorrectDownloadSettings();
                appSettings.ValidateAndCorrectMirrorSettings();
                appSettings.ValidateAndCorrectSearchSettings();
                appSettings.ValidateAndCorrectExportSettings();
                appSettings.ValidateAndCorrectAdvancedSettings();
                return appSettings;
            }
        }

        private static ExportPanelSettings ValidateAndCorrectExportPanelSettings(ExportPanelSettings exportPanelSettings)
        {
            if (exportPanelSettings == null)
            {
                exportPanelSettings = ExportPanelSettings.Default;
            }
            else
            {
                if (!Enum.IsDefined(typeof(ExportPanelSettings.ExportFormat), exportPanelSettings.Format))
                {
                    exportPanelSettings.Format = ExportPanelSettings.ExportFormat.XLSX;
                }
                if (!Enum.IsDefined(typeof(ExportPanelSettings.CsvSeparator), exportPanelSettings.Separator))
                {
                    exportPanelSettings.Separator = ExportPanelSettings.CsvSeparator.COMMA;
                }
            }
            return exportPanelSettings;
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
                    if (nonFictionColumns.LanguageColumnWidth < NON_FICTION_GRID_LANGUAGE_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.LanguageColumnWidth = NON_FICTION_GRID_LANGUAGE_COLUMN_MIN_WIDTH;
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
                    if (nonFictionColumns.ExistsInLibraryColumnWidth < NON_FICTION_GRID_EXISTS_IN_LIBRARY_COLUMN_MIN_WIDTH)
                    {
                        nonFictionColumns.ExistsInLibraryColumnWidth = NON_FICTION_GRID_EXISTS_IN_LIBRARY_COLUMN_MIN_WIDTH;
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
                    if (fictionColumns.LanguageColumnWidth < FICTION_GRID_LANGUAGE_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.LanguageColumnWidth = FICTION_GRID_LANGUAGE_COLUMN_MIN_WIDTH;
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
                    if (fictionColumns.ExistsInLibraryColumnWidth < FICTION_GRID_EXISTS_IN_LIBRARY_COLUMN_MIN_WIDTH)
                    {
                        fictionColumns.ExistsInLibraryColumnWidth = FICTION_GRID_EXISTS_IN_LIBRARY_COLUMN_MIN_WIDTH;
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
                    if (sciMagColumns.ExistsInLibraryColumnWidth < SCI_MAG_GRID_EXISTS_IN_LIBRARY_COLUMN_MIN_WIDTH)
                    {
                        sciMagColumns.ExistsInLibraryColumnWidth = SCI_MAG_GRID_EXISTS_IN_LIBRARY_COLUMN_MIN_WIDTH;
                    }
                }
                SciMag.ExportPanel = ValidateAndCorrectExportPanelSettings(SciMag.ExportPanel);
            }
        }

        private void ValidateAndCorrectDownloadManagerTabSettings()
        {
            if (DownloadManagerTab == null)
            {
                DownloadManagerTab = DownloadManagerTabSettings.Default;
            }
            else
            {
                if (DownloadManagerTab.LogPanelHeight < DOWNLOAD_MANAGER_TAB_LOG_PANEL_MIN_HEIGHT)
                {
                    DownloadManagerTab.LogPanelHeight = DOWNLOAD_MANAGER_TAB_LOG_PANEL_MIN_HEIGHT;
                }
            }
        }

        private void ValidateAndCorrectLastUpdateSettings()
        {
            if (LastUpdate == null)
            {
                LastUpdate = LastUpdateSettings.Default;
            }
        }

        private void ValidateAndCorrectBookmarkSettings()
        {
            if (Bookmarks == null)
            {
                Bookmarks = BookmarkSettings.Default;
            }
            else
            {
                if (Bookmarks.Bookmarks == null)
                {
                    Bookmarks = BookmarkSettings.Default;
                }
                else
                {
                    for (int bookmarkIndex = Bookmarks.Bookmarks.Count - 1; bookmarkIndex >= 0; bookmarkIndex--)
                    {
                        BookmarkSettings.Bookmark bookmark = Bookmarks.Bookmarks[bookmarkIndex];
                        if (String.IsNullOrEmpty(bookmark.Name) || String.IsNullOrEmpty(bookmark.Query) ||
                            !Enum.IsDefined(typeof(LibgenObjectType), bookmark.Type))
                        {
                            Bookmarks.Bookmarks.RemoveAt(bookmarkIndex);
                        }
                    }
                }
            }
        }

        private void ValidateAndCorrectGeneralSettings()
        {
            if (General == null)
            {
                General = GeneralSettings.Default;
            }
            else
            {
                if (!Enum.IsDefined(typeof(GeneralSettings.UpdateCheckInterval), General.UpdateCheck))
                {
                    General.UpdateCheck = GeneralSettings.UpdateCheckInterval.NEVER;
                }
                if (String.IsNullOrWhiteSpace(General.UpdateUrl) || !Uri.IsWellFormedUriString(General.UpdateUrl, UriKind.Absolute) ||
                    (!General.UpdateUrl.ToLower().StartsWith("http://") && !General.UpdateUrl.ToLower().StartsWith("https://")))
                {
                    General.UpdateUrl = DEFAULT_GITHUB_RELEASE_API_URL;
                }
            }
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
                if (Network.ProxyUserName == null)
                {
                    Network.ProxyUserName = String.Empty;
                }
                if (Network.ProxyPassword == null)
                {
                    Network.ProxyPassword = String.Empty;
                }
            }
        }

        private void ValidateAndCorrectDownloadSettings()
        {
            if (Download == null)
            {
                Download = DownloadSettings.Default;
            }
            else
            {
                if (Download.Timeout < MIN_DOWNLOAD_TIMEOUT)
                {
                    Download.Timeout = MIN_DOWNLOAD_TIMEOUT;
                }
                else if (Download.Timeout > MAX_DOWNLOAD_TIMEOUT)
                {
                    Download.Timeout = MAX_DOWNLOAD_TIMEOUT;
                }
                if (Download.Attempts < 1)
                {
                    Download.Attempts = 1;
                }
                else if (Download.Attempts > MAX_DOWNLOAD_ATTEMPT_COUNT)
                {
                    Download.Attempts = MAX_DOWNLOAD_ATTEMPT_COUNT;
                }
                if (Download.RetryDelay < 0)
                {
                    Download.RetryDelay = 0;
                }
                else if (Download.RetryDelay > MAX_DOWNLOAD_RETRY_DELAY)
                {
                    Download.RetryDelay = MAX_DOWNLOAD_RETRY_DELAY;
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
