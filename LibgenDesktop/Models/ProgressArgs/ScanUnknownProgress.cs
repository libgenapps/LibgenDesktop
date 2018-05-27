namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ScanUnknownProgress
    {
        public ScanUnknownProgress(string relativeFilePath, bool error)
        {
            RelativeFilePath = relativeFilePath;
            Error = error;
        }

        public string RelativeFilePath { get; }
        public bool Error { get; }
    }
}
