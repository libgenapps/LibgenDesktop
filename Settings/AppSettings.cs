using System.Windows.Forms;

namespace LibgenDesktop.Settings
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
            public int IdColumnWidth { get; set; }
            public int TitleColumnWidth { get; set; }
            public int AuthorsColumnWidth { get; set; }
            public int SeriesColumnWidth { get; set; }
            public int YearColumnWidth { get; set; }
            public int PublisherColumnWidth { get; set; }
            public int FormatColumnWidth { get; set; }
            public int FileSizeColumnWidth { get; set; }
            public int OcrColumnWidth { get; set; }
        }

        public const int MAIN_WINDOW_MIN_WIDTH = 800;
        public const int MAIN_WINDOW_MIN_HEIGHT = 600;
        public const int BOOK_WINDOW_MIN_WIDTH = 800;
        public const int BOOK_WINDOW_MIN_HEIGHT = 600;
        public const int ID_COLUMN_MIN_WIDTH = 20;
        public const int TITLE_COLUMN_MIN_WIDTH = 100;
        public const int AUTHORS_COLUMN_MIN_WIDTH = 100;
        public const int SERIES_COLUMN_MIN_WIDTH = 100;
        public const int YEAR_COLUMN_MIN_WIDTH = 20;
        public const int PUBLISHER_COLUMN_MIN_WIDTH = 50;
        public const int FORMAT_COLUMN_MIN_WIDTH = 20;
        public const int FILESIZE_COLUMN_MIN_WIDTH = 30;
        public const int OCR_COLUMN_MIN_WIDTH = 40;

        private const string DEFAULT_DATABASE_FILE_NAME = "libgen.db";
        private const int DEFAULT_MAIN_WINDOW_WIDTH = 1200;
        private const int DEFAULT_MAIN_WINDOW_HEIGHT = 650;
        private const int DEFAULT_BOOK_WINDOW_WIDTH = 1200;
        private const int DEFAULT_BOOK_WINDOW_HEIGHT = 618;
        public const int DEFAULT_ID_COLUMN_WIDTH = 60;
        public const int DEFAULT_TITLE_COLUMN_WIDTH = 200;
        public const int DEFAULT_AUTHORS_COLUMN_WIDTH = 200;
        public const int DEFAULT_SERIES_COLUMN_WIDTH = 200;
        public const int DEFAULT_YEAR_COLUMN_WIDTH = 60;
        public const int DEFAULT_PUBLISHER_COLUMN_WIDTH = 200;
        public const int DEFAULT_FORMAT_COLUMN_WIDTH = 60;
        public const int DEFAULT_FILESIZE_COLUMN_WIDTH = 110;
        public const int DEFAULT_OCR_COLUMN_WIDTH = 40;

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
                    Left = (Screen.PrimaryScreen.WorkingArea.Width - DEFAULT_MAIN_WINDOW_WIDTH) / 2,
                    Top = (Screen.PrimaryScreen.WorkingArea.Height - DEFAULT_MAIN_WINDOW_HEIGHT) / 2,
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
                    IdColumnWidth = DEFAULT_ID_COLUMN_WIDTH,
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
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
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
                if (appSettings.MainWindow.Left >= screenWidth)
                {
                    appSettings.MainWindow.Left = screenWidth - appSettings.MainWindow.Width;
                }
                if (appSettings.MainWindow.Top >= screenHeight)
                {
                    appSettings.MainWindow.Top = screenHeight - appSettings.MainWindow.Height;
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
                if (appSettings.Columns.IdColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.IdColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.TitleColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.TitleColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.AuthorsColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.AuthorsColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.SeriesColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.SeriesColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.YearColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.YearColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.PublisherColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.PublisherColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.FormatColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.FormatColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.FileSizeColumnWidth < ID_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.FileSizeColumnWidth = ID_COLUMN_MIN_WIDTH;
                }
                if (appSettings.Columns.OcrColumnWidth < OCR_COLUMN_MIN_WIDTH)
                {
                    appSettings.Columns.OcrColumnWidth = OCR_COLUMN_MIN_WIDTH;
                }
            }
        }
    }
}
