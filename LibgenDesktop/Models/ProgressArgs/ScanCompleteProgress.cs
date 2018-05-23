namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ScanCompleteProgress
    {
        public ScanCompleteProgress(int found, int notFound, int errors)
        {
            Found = found;
            NotFound = notFound;
            Errors = errors;
        }

        public int Found { get; }
        public int NotFound { get; }
        public int Errors { get; }
    }
}
