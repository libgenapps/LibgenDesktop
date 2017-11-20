namespace LibgenDesktop.Models.ProgressArgs
{
    internal class SearchBooksProgress
    {
        public SearchBooksProgress(int booksFound, bool isFinished = false)
        {
            BooksFound = booksFound;
            IsFinished = isFinished;
        }

        public int BooksFound { get; }
        public bool IsFinished { get; }
    }
}
