namespace LibgenDesktop.Models.ProgressArgs
{
    internal class SearchProgress
    {
        public SearchProgress(int itemsFound)
        {
            ItemsFound = itemsFound;
        }

        public int ItemsFound { get; }
    }
}
