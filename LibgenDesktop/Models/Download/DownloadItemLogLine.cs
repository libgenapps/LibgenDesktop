using System;

namespace LibgenDesktop.Models.Download
{
    internal class DownloadItemLogLine
    {
        public DownloadItemLogLine(DownloadItemLogLineType type, DateTime timeStamp, string text)
        {
            Type = type;
            TimeStamp = timeStamp;
            Text = text;
        }

        public DownloadItemLogLineType Type { get; }
        public DateTime TimeStamp { get; }
        public string Text { get; }
    }
}
