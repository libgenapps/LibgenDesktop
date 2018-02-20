using System;

namespace LibgenDesktop.ViewModels.EventArguments
{
    internal class SelectDownloadEventArgs : EventArgs
    {
        public SelectDownloadEventArgs(Guid downloadId)
        {
            DownloadId = downloadId;
        }

        public Guid DownloadId { get; }
    }
}
