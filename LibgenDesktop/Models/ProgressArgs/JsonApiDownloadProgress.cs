namespace LibgenDesktop.Models.ProgressArgs
{
    internal class JsonApiDownloadProgress
    {
        public JsonApiDownloadProgress(int booksDownloaded)
        {
            BooksDownloaded = booksDownloaded;
        }

        public int BooksDownloaded { get; }
    }
}
