using System;

namespace LibgenDesktop.Models.Download
{
    internal class DownloadItemChangedEventArgs : EventArgs
    {
        public DownloadItemChangedEventArgs(DownloadItem changedDownloadItem)
        {
            ChangedDownloadItem = changedDownloadItem.Clone();
        }

        public DownloadItem ChangedDownloadItem { get; }
    }
}
