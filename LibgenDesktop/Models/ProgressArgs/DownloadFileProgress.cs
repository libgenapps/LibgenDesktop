namespace LibgenDesktop.Models.ProgressArgs
{
    internal class DownloadFileProgress
    {
        public DownloadFileProgress(long downloadedBytes, long fileSize)
        {
            DownloadedBytes = downloadedBytes;
            FileSize = fileSize;
        }

        public long DownloadedBytes { get; }
        public long FileSize { get; }
    }
}
