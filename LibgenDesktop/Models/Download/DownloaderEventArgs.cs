using System;

namespace LibgenDesktop.Models.Download
{
    internal abstract class DownloadItemEventArgs
    {
    }

    internal class DownloadItemAddedEventArgs : DownloadItemEventArgs
    {
        public DownloadItemAddedEventArgs(DownloadItem addedDownloadItem)
        {
            AddedDownloadItem = addedDownloadItem.Clone();
        }

        public DownloadItem AddedDownloadItem { get; }
    }

    internal class DownloadItemChangedEventArgs : DownloadItemEventArgs
    {
        public DownloadItemChangedEventArgs(DownloadItem changedDownloadItem)
        {
            ChangedDownloadItem = changedDownloadItem.Clone();
        }

        public DownloadItem ChangedDownloadItem { get; }
    }

    internal class DownloadItemRemovedEventArgs : DownloadItemEventArgs
    {
        public DownloadItemRemovedEventArgs(DownloadItem removedDownloadItem)
        {
            RemovedDownloadItem = removedDownloadItem.Clone();
        }

        public DownloadItem RemovedDownloadItem { get; }
    }

    internal class DownloadItemLogLineEventArgs : DownloadItemEventArgs
    {
        public DownloadItemLogLineEventArgs(Guid downloadItemId, int lineIndex, DownloadItemLogLine logLine)
        {
            DownloadItemId = downloadItemId;
            LineIndex = lineIndex;
            LogLine = logLine;
        }

        public Guid DownloadItemId { get; }
        public int LineIndex { get; }
        public DownloadItemLogLine LogLine { get; }
    }
}
