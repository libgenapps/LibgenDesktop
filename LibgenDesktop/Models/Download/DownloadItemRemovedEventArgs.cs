using System;

namespace LibgenDesktop.Models.Download
{
    internal class DownloadItemRemovedEventArgs : EventArgs
    {
        public DownloadItemRemovedEventArgs(DownloadItem removedDownloadItem)
        {
            RemovedDownloadItem = removedDownloadItem.Clone();
        }

        public DownloadItem RemovedDownloadItem { get; }
    }
}
