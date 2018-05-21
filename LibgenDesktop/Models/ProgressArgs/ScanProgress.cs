namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ScanProgress
    {
        public ScanProgress(string relativeFilePath)
        {
            RelativeFilePath = relativeFilePath;
            Found = false;
            Authors = null;
            Title = null;
        }

        public ScanProgress(string relativeFilePath, string authors, string title)
        {
            RelativeFilePath = relativeFilePath;
            Found = true;
            Authors = authors;
            Title = title;
        }

        public string RelativeFilePath { get; }
        public bool Found { get; }
        public string Authors { get; }
        public string Title { get; }
    }
}
