using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LibgenDesktop.Models.Download
{
    internal class DownloadItem
    {
        private CancellationTokenSource cancellationTokenSource;

        public DownloadItem(Guid id, string downloadPageUrl, string downloadDirectory, string fileName, string downloadTransformations, string md5Hash)
        {
            cancellationTokenSource = new CancellationTokenSource();
            Id = id;
            DownloadPageUrl = downloadPageUrl;
            DownloadDirectory = downloadDirectory;
            Logs = new List<DownloadItemLogLine>();
            Cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            DownloadTransformations = downloadTransformations;
            Md5Hash = md5Hash;
            CancellationToken = cancellationTokenSource.Token;
            FileName = fileName;
            DirectFileUrl = null;
            Referer = null;
            Status = DownloadItemStatus.QUEUED;
            FileCreated = false;
            FileHandleOpened = false;
            DownloadedFileSize = null;
            TotalFileSize = null;
            CurrentAttempt = 1;
        }

        private DownloadItem(DownloadItem source)
        {
            Id = source.Id;
            DownloadPageUrl = source.DownloadPageUrl;
            DownloadDirectory = source.DownloadDirectory;
            Logs = source.Logs.ToList();
            Cookies = new Dictionary<string, string>(source.Cookies, StringComparer.OrdinalIgnoreCase);
            DownloadTransformations = source.DownloadTransformations;
            Md5Hash = source.Md5Hash;
            FileName = source.FileName;
            DirectFileUrl = source.DirectFileUrl;
            Referer = source.Referer;
            Status = source.Status;
            FileCreated = source.FileCreated;
            DownloadedFileSize = source.DownloadedFileSize;
            TotalFileSize = source.TotalFileSize;
            CurrentAttempt = source.CurrentAttempt;
        }

        public Guid Id { get; }
        public string DownloadPageUrl { get; }
        public string DownloadDirectory { get; }
        public List<DownloadItemLogLine> Logs { get; }
        public Dictionary<string, string> Cookies { get; }
        public string DownloadTransformations { get; }
        public string Md5Hash { get; }
        public CancellationToken CancellationToken { get; set; }
        public string FileName { get; set; }
        public string DirectFileUrl { get; set; }
        public string Referer { get; set; }
        public DownloadItemStatus Status { get; set; }
        public bool FileCreated { get; set; }
        public bool FileHandleOpened { get; set; }
        public long? DownloadedFileSize { get; set; }
        public long? TotalFileSize { get; set; }
        public int CurrentAttempt { get; set; }

        public void CancelDownload()
        {
            cancellationTokenSource.Cancel();
        }

        public void CreateNewCancellationToken()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = cancellationTokenSource.Token;
        }

        public DownloadItem Clone()
        {
            return new DownloadItem(this);
        }
    }
}
