using System;

namespace LibgenDesktop.Models.Download
{
    internal class DownloadItemAddedEventArgs : EventArgs
    {
        public DownloadItemAddedEventArgs(DownloadItem addedDownloadItem)
        {
            AddedDownloadItem = addedDownloadItem.Clone();
        }

        public DownloadItem AddedDownloadItem { get; }
    }
}
