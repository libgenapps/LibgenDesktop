using System;

namespace LibgenDesktop.Models.Download
{
    internal class DownloadItemLogLineEventArgs : EventArgs
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
