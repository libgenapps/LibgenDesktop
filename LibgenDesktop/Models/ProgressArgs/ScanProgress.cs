namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ScanProgress
    {
        public ScanProgress(string relativeFilePath, bool error = false)
        {
            RelativeFilePath = relativeFilePath;
            Found = false;
            Error = error;
            Authors = null;
            Title = null;
        }

        public ScanProgress(string relativeFilePath, string authors, string title)
        {
            RelativeFilePath = relativeFilePath;
            Found = true;
            Error = false;
            Authors = authors;
            Title = title;
        }

        public string RelativeFilePath { get; }
        public bool Found { get; }
        public bool Error { get; }
        public string Authors { get; }
        public string Title { get; }
    }
}
