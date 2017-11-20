namespace LibgenDesktop.Models.ProgressArgs
{
    internal class LoadAllBooksProgress
    {
        public LoadAllBooksProgress(int booksLoaded, int totalBookCount, bool isFinished = false)
        {
            BooksLoaded = booksLoaded;
            TotalBookCount = totalBookCount;
            IsFinished = isFinished;
        }

        public int BooksLoaded { get; }
        public int TotalBookCount { get; }
        public bool IsFinished { get; }
    }
}
