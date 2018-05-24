using LibgenDesktop.Models.Entities;

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
            LibgenObject = null;
        }

        public ScanProgress(string relativeFilePath, string authors, string title, NonFictionBook libgenObject)
        {
            RelativeFilePath = relativeFilePath;
            Found = true;
            Error = false;
            Authors = authors;
            Title = title;
            LibgenObject = libgenObject;
        }

        public string RelativeFilePath { get; }
        public bool Found { get; }
        public bool Error { get; }
        public string Authors { get; }
        public string Title { get; }
        public NonFictionBook LibgenObject { get; }
    }
}
