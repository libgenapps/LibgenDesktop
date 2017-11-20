namespace LibgenDesktop.Common
{
    internal static class Constants
    {
        public const int MAIN_WINDOW_MIN_WIDTH = 1000;
        public const int MAIN_WINDOW_MIN_HEIGHT = 500;
        public const int BOOK_WINDOW_MIN_WIDTH = 1000;
        public const int BOOK_WINDOW_MIN_HEIGHT = 500;
        public const int ERROR_WINDOW_MIN_WIDTH = 400;
        public const int ERROR_WINDOW_MIN_HEIGHT = 300;
        public const int TITLE_COLUMN_MIN_WIDTH = 150;
        public const int AUTHORS_COLUMN_MIN_WIDTH = 150;
        public const int SERIES_COLUMN_MIN_WIDTH = 150;
        public const int YEAR_COLUMN_MIN_WIDTH = 60;
        public const int PUBLISHER_COLUMN_MIN_WIDTH = 150;
        public const int FORMAT_COLUMN_MIN_WIDTH = 80;
        public const int FILESIZE_COLUMN_MIN_WIDTH = 130;
        public const int OCR_COLUMN_MIN_WIDTH = 55;

        public const string DEFAULT_DATABASE_FILE_NAME = "libgen.db";
        public const int DEFAULT_MAIN_WINDOW_WIDTH = 1200;
        public const int DEFAULT_MAIN_WINDOW_HEIGHT = 650;
        public const int DEFAULT_BOOK_WINDOW_WIDTH = 1200;
        public const int DEFAULT_BOOK_WINDOW_HEIGHT = 618;
        public const int DEFAULT_TITLE_COLUMN_WIDTH = 200;
        public const int DEFAULT_AUTHORS_COLUMN_WIDTH = 200;
        public const int DEFAULT_SERIES_COLUMN_WIDTH = 180;
        public const int DEFAULT_YEAR_COLUMN_WIDTH = 60;
        public const int DEFAULT_PUBLISHER_COLUMN_WIDTH = 180;
        public const int DEFAULT_FORMAT_COLUMN_WIDTH = 100;
        public const int DEFAULT_FILESIZE_COLUMN_WIDTH = 150;
        public const int DEFAULT_OCR_COLUMN_WIDTH = 55;

        public const int SEARCH_REPORT_PROGRESS_BATCH_SIZE = 2000;
        public const int INSERT_TRANSACTION_BATCH = 500;

        public const string BOOK_COVER_URL_PREFIX = "http://libgen.io/covers/";
        public const string BOOK_DOWNLOAD_URL_PREFIX = "http://libgen.io/book/index.php?md5=";
    }
}
