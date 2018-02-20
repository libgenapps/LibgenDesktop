using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LibgenDesktop.Models.Download
{
    internal static class DownloadQueueStorage
    {
        internal class StorageDownloadItem
        {
            public Guid Id { get; set; }
            public string FileName { get; set; }
            public DownloadItemStatus Status { get; set; }
            public string DownloadPageUrl { get; set; }
            public string DirectFileUrl { get; set; }
            public string DownloadDirectory { get; set; }
            public Dictionary<string, string> Cookies { get; set; }
            public string Referer { get; set; }
            public string DownloadTransformations { get; set; }
            public bool FileCreated { get; set; }
            public long? DownloadedFileSize { get; set; }
            public long? TotalFileSize { get; set; }
            public int CurrentAttempt { get; set; }
        }

        public static List<DownloadItem> LoadDownloadQueue(string downloadQueueFilePath)
        {
            List<StorageDownloadItem> storageDownloads;
            try
            {
                if (File.Exists(downloadQueueFilePath))
                {
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    using (StreamReader streamReader = new StreamReader(downloadQueueFilePath))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        storageDownloads = jsonSerializer.Deserialize<List<StorageDownloadItem>>(jsonTextReader);
                    }
                }
                else
                {
                    storageDownloads = null;
                }
            }
            catch
            {
                storageDownloads = null;
            }
            List<DownloadItem> result;
            if (storageDownloads != null)
            {
                result = storageDownloads.Select(FromStorageDownloadItem).ToList();
            }
            else
            {
                result = new List<DownloadItem>();
            }
            return result;
        }

        public static void SaveDownloadQueue(string downloadQueueFilePath, List<DownloadItem> downloadQueue)
        {
            List<StorageDownloadItem> storageDownloads = downloadQueue.Select(ToStorageDownloadItem).ToList();
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamWriter streamWriter = new StreamWriter(downloadQueueFilePath))
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                jsonTextWriter.Formatting = Formatting.Indented;
                jsonTextWriter.Indentation = 4;
                jsonSerializer.Serialize(jsonTextWriter, storageDownloads);
            }
        }

        private static StorageDownloadItem ToStorageDownloadItem(DownloadItem downloadItem)
        {
            return new StorageDownloadItem
            {
                Id = downloadItem.Id,
                FileName = downloadItem.FileName,
                Status = downloadItem.Status,
                DownloadPageUrl = downloadItem.DownloadPageUrl,
                DirectFileUrl = downloadItem.DirectFileUrl,
                DownloadDirectory = downloadItem.DownloadDirectory,
                Cookies = downloadItem.Cookies,
                Referer = downloadItem.Referer,
                DownloadTransformations = downloadItem.DownloadTransformations,
                FileCreated = downloadItem.FileCreated,
                DownloadedFileSize = downloadItem.DownloadedFileSize,
                TotalFileSize = downloadItem.TotalFileSize,
                CurrentAttempt = downloadItem.CurrentAttempt
            };
        }

        private static DownloadItem FromStorageDownloadItem(StorageDownloadItem storageDownloadItem)
        {
            DownloadItem result = new DownloadItem(storageDownloadItem.Id, storageDownloadItem.DownloadPageUrl, storageDownloadItem.DownloadDirectory,
                storageDownloadItem.FileName, storageDownloadItem.DownloadTransformations)
            {
                Status = storageDownloadItem.Status,
                DirectFileUrl = storageDownloadItem.DirectFileUrl,
                Referer = storageDownloadItem.Referer,
                FileCreated = storageDownloadItem.FileCreated,
                DownloadedFileSize = storageDownloadItem.DownloadedFileSize,
                TotalFileSize = storageDownloadItem.TotalFileSize,
                CurrentAttempt = storageDownloadItem.CurrentAttempt
            };
            foreach (KeyValuePair<string, string> cookieValuePair in storageDownloadItem.Cookies)
            {
                result.Cookies.Add(cookieValuePair.Key, cookieValuePair.Value);
            }
            return result;
        }
    }
}
