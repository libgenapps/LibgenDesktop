namespace LibgenDesktop.Models.ProgressArgs
{
    internal class DownloadFileProgress
    {
        public DownloadFileProgress(int downloadedBytes, int fileSize)
        {
            DownloadedBytes = downloadedBytes;
            FileSize = fileSize;
        }

        public int DownloadedBytes { get; }
        public int FileSize { get; }
    }
}
