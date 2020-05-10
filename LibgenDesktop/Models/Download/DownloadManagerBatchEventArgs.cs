using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Download
{
    internal class DownloadManagerBatchEventArgs : EventArgs
    {
        public DownloadManagerBatchEventArgs()
        {
            BatchEvents = new List<DownloadItemEventArgs>();
            AddEventCount = 0;
            RemoveEventCount = 0;
        }

        public List<DownloadItemEventArgs> BatchEvents { get; }
        public int AddEventCount { get; private set; }
        public int RemoveEventCount { get; private set; }

        public void Add(DownloadItemAddedEventArgs downloadItemAddedEventArgs)
        {
            BatchEvents.Add(downloadItemAddedEventArgs);
            AddEventCount++;
        }

        public void Add(DownloadItemChangedEventArgs downloadItemChangedEventArgs)
        {
            BatchEvents.Add(downloadItemChangedEventArgs);
        }

        public void Add(DownloadItemRemovedEventArgs downloadItemRemovedEventArgs)
        {
            BatchEvents.Add(downloadItemRemovedEventArgs);
            RemoveEventCount++;
        }

        public void Add(DownloadItemLogLineEventArgs downloadItemLogLineEventArgs)
        {
            BatchEvents.Add(downloadItemLogLineEventArgs);
        }
    }
}
