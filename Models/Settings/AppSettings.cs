using LibgenDesktop.Infrastructure;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models.Settings
{
    internal class AppSettings
    {
        internal class MainWindowSettings
        {
            public bool Maximized { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        internal class BookWindowSettings
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        internal class ColumnSettings
        {
            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int SeriesColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int PublisherColumnWidth { get; set; }
            public int FormatColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
            public int OcrColumnWidth { get; set; }
        }

        public static AppSettings Default
        {
            get
            {
                return new AppSettings
                {
                    DatabaseFileName = DEFAULT_DATABASE_FILE_NAME,
                    OfflineMode = true,
                    // ResultLimit = 0,
                    MainWindow = DefaultMainWindowSettings,
                    BookWindow = DefaultBookWindowSettings,
                    Columns = DefaultColumnSettings
                };
            }
        }

        private static MainWindowSettings DefaultMainWindowSettings
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

        private static BookWindowSettings DefaultBookWindowSettings
        {
            get
            {
                return new BookWindowSettings
                {
                    Width = DEFAULT_BOOK_WINDOW_WIDTH,
                    Height = DEFAULT_BOOK_WINDOW_HEIGHT
                };
            }
        }

        private static ColumnSettings DefaultColumnSettings
        {
            get
            {
                return new ColumnSettings
                {
                    TitleColumnWidth = DEFAULT_TITLE_COLUMN_WIDTH,
                    AuthorsColumnWidth = DEFAULT_AUTHORS_COLUMN_WIDTH,
                    SeriesColumnWidth = DEFAULT_SERIES_COLUMN_WIDTH,
                    YearColumnWidth = DEFAULT_YEAR_COLUMN_WIDTH,
                    PublisherColumnWidth = DEFAULT_PUBLISHER_COLUMN_WIDTH,
                    FormatColumnWidth = DEFAULT_FORMAT_COLUMN_WIDTH,
                    FileSizeColumnWidth = DEFAULT_FILESIZE_COLUMN_WIDTH,
                    OcrColumnWidth = DEFAULT_OCR_COLUMN_WIDTH
                };
            }
        }

        public string DatabaseFileName { get; set; }
        public bool OfflineMode { get; set; }
        // public int ResultLimit { get; set; }
        public MainWindowSettings MainWindow { get; set; }
        public BookWindowSettings BookWindow { get; set; }
        public ColumnSettings Columns { get; set; }

        public static void ValidateAndCorrect(AppSettings appSettings)
        {
            if (string.IsNullOrEmpty(appSettings.DatabaseFileName))
            {
                appSettings.DatabaseFileName = DEFAULT_DATABASE_FILE_NAME;
            }
            if (appSettings.MainWindow == null)
            {
                appSettings.MainWindow = DefaultMainWindowSettings;
            }
            else
            {
                if (appSettings.MainWindow.Width < MAIN_WINDOW_MIN_WIDTH)
                {
                    appSettings.MainWindow.Width = MAIN_WINDOW_MIN_WIDTH;
                }
                if (appSettings.MainWindow.Height < MAIN_WINDOW_MIN_HEIGHT)
                {
                    appSettings.MainWindow.Height = MAIN_WINDOW_MIN_HEIGHT;
                }
                if (appSettings.MainWindow.Left >= WindowManager.ScreenWidth)
                {
                    appSettings.MainWindow.Left = WindowManager.ScreenWidth - appSettings.MainWindow.Width;
                }
                if (appSettings.MainWindow.Top >= WindowManager.ScreenHeight)
                {
                    appSettings.MainWindow.Top = WindowManager.ScreenHeight - appSettings.MainWindow.Height;
                }
                if (appSettings.MainWindow.Left < 0)
                {
                    appSettings.MainWindow.Left = 0;
                }
                if (appSettings.MainWindow.Top < 0)
                {
                    appSettings.MainWindow.Top = 0;
                }
            }
            if (appSettings.BookWindow == null)
            {
                appSettings.BookWindow = DefaultBookWindowSettings;
            }
            else
            {
                if (appSettings.BookWindow.Width < BOOK_WINDOW_MIN_WIDTH)
                {
                    appSettings.BookWindow.Width = BOOK_WINDOW_MIN_WIDTH;
                }
                if (appSettings.BookWindow.Height < BOOK_WINDOW_MIN_HEIGHT)
                {
                    appSettings.BookWindow.Height = BOOK_WINDOW_MIN_HEIGHT;
                }
            }
            if (appSettings.Columns == null)
            {
                appSettings.Columns = DefaultColumnSettings;
            }
            else
            {
                if (appSettings.Columns.TitleColumnWidth < TITLE_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.TitleColumnWidth = TITLE_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.AuthorsColumnWidth < AUTHORS_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.AuthorsColumnWidth = AUTHORS_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.SeriesColumnWidth < SERIES_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.SeriesColumnWidth = SERIES_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.YearColumnWidth < YEAR_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.YearColumnWidth = YEAR_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.PublisherColumnWidth < PUBLISHER_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.PublisherColumnWidth = PUBLISHER_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.FormatColumnWidth < FORMAT_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.FormatColumnWidth = FORMAT_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.FileSizeColumnWidth < FILESIZE_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.FileSizeColumnWidth = FILESIZE_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.OcrColumnWidth < OCR_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.OcrColumnWidth = OCR_COLUMN_MIN_WIDTH;
                }
            }
        }
    }
}
