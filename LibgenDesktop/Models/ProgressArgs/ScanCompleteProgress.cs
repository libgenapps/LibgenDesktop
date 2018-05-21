namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ScanCompleteProgress
    {
        public ScanCompleteProgress(int found, int notFound)
        {
            Found = found;
            NotFound = notFound;
        }

        public int Found { get; }
        public int NotFound { get; }
    }
}
