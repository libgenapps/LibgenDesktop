namespace LibgenDesktop.Models.Download
{
    internal enum DownloadItemStatus
    {
        QUEUED = 1,
        DOWNLOADING,
        STOPPED,
        RETRY_DELAY,
        ERROR,
        COMPLETED,
        REMOVED
    }
}
