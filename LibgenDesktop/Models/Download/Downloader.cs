using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using HtmlAgilityPack;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.Utils;
using static LibgenDesktop.Common.Constants;
using static LibgenDesktop.Models.Settings.AppSettings;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.Models.Download
{
    internal class Downloader : IDisposable
    {
        private readonly object downloadQueueLock;
        private readonly string downloadQueueFilePath;
        private readonly List<DownloadItem> downloadQueue;
        private readonly Dictionary<Guid, DownloadItem> downloadQueueKeyPairs;
        private readonly BlockingCollection<DownloaderBatchEventArgs> eventQueue;
        private readonly Task downloadTask;
        private readonly AutoResetEvent downloadTaskResetEvent;
        private DownloadManagerLocalizator localization;
        private HttpClient httpClient;
        private DownloadSettings downloadSettings;
        private bool isInOfflineMode;
        private bool isShuttingDown;
        private bool disposed;

        public Downloader()
        {
            downloadQueueLock = new object();
            downloadQueueFilePath = Path.Combine(Environment.AppDataDirectory, DOWNLOAD_LIST_FILE_NAME);
            downloadQueue = DownloadQueueStorage.LoadDownloadQueue(downloadQueueFilePath);
            downloadQueueKeyPairs = downloadQueue.ToDictionary(downloadItem => downloadItem.Id);
            eventQueue = new BlockingCollection<DownloaderBatchEventArgs>();
            downloadTaskResetEvent = new AutoResetEvent(false);
            localization = null;
            httpClient = null;
            downloadSettings = null;
            isInOfflineMode = false;
            isShuttingDown = false;
            StartEventPublisherTask();
            downloadTask = StartDownloadTask();
            disposed = false;
        }

        public event EventHandler<DownloaderBatchEventArgs> DownloaderBatchEvent;

        public void Configure(Language currentLanguage, NetworkSettings networkSettings, DownloadSettings downloadSettings)
        {
            localization = currentLanguage.DownloadManager;
            this.downloadSettings = downloadSettings;
            if (networkSettings.OfflineMode)
            {
                if (!isInOfflineMode)
                {
                    isInOfflineMode = true;
                    SwitchToOfflineMode();
                }
            }
            else
            {
                httpClient = CreateNewHttpClient(networkSettings, downloadSettings);
                isInOfflineMode = false;
                ResumeDownloadTask();
            }
            Logger.Debug("Downloader configuration complete.");
        }

        public List<DownloadItem> GetDownloadQueueSnapshot()
        {
            lock (downloadQueueLock)
            {
                return downloadQueue.ToList();
            }
        }

        public DownloadItem GetDownloadItemByDownloadPageUrl(string downloadPageUrl)
        {
            lock (downloadQueueLock)
            {
                return downloadQueue.FirstOrDefault(downloadItem => downloadItem.DownloadPageUrl == downloadPageUrl)?.Clone();
            }
        }

        public void EnqueueDownloadItems(IEnumerable<DownloadItemRequest> downloadItemRequests)
        {
            lock (downloadQueueLock)
            {
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                foreach (DownloadItemRequest downloadItemRequest in downloadItemRequests)
                {
                    string fileName = String.Concat(FileUtils.RemoveInvalidFileNameCharacters(downloadItemRequest.FileNameWithoutExtension,
                        downloadItemRequest.Md5Hash), ".", downloadItemRequest.FileExtension.ToLower());
                    DownloadItem newDownloadItem = new DownloadItem(Guid.NewGuid(), downloadItemRequest.DownloadPageUrl, downloadSettings.DownloadDirectory,
                        fileName, downloadItemRequest.DownloadTransformations, downloadItemRequest.Md5Hash, downloadItemRequest.RestartSessionOnTimeout);
                    downloadQueue.Add(newDownloadItem);
                    downloadQueueKeyPairs.Add(newDownloadItem.Id, newDownloadItem);
                    batchEventArgs.Add(new DownloadItemAddedEventArgs(newDownloadItem));
                    DownloadItemLogLineEventArgs logLineEventArgs =
                        AddLogLine(newDownloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.LogLineQueued);
                    batchEventArgs.Add(logLineEventArgs);
                }
                eventQueue.Add(batchEventArgs);
                SaveDownloadQueue();
                ResumeDownloadTask();
            }
        }

        public void StartDownloads(IEnumerable<Guid> downloadItemIds)
        {
            lock (downloadQueueLock)
            {
                bool resumeDownloadTask = false;
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                foreach (Guid downloadItemId in downloadItemIds)
                {
                    if (!downloadQueueKeyPairs.TryGetValue(downloadItemId, out DownloadItem downloadItem))
                    {
                        Logger.Debug($"Download item with ID = {downloadItemId} not found.");
                        continue;
                    }
                    Logger.Debug($"Start download requested for download ID = {downloadItemId}.");
                    if (downloadItem.Status == DownloadItemStatus.STOPPED || downloadItem.Status == DownloadItemStatus.ERROR)
                    {
                        downloadItem.CreateNewCancellationToken();
                        downloadItem.CurrentAttempt = 1;
                        DownloadItemLogLineEventArgs logLineEventArgs =
                            AddLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.LogLineQueued);
                        batchEventArgs.Add(logLineEventArgs);
                        downloadItem.Status = DownloadItemStatus.QUEUED;
                        batchEventArgs.Add(new DownloadItemChangedEventArgs(downloadItem));
                        resumeDownloadTask = true;
                    }
                    else
                    {
                        Logger.Debug($"Incorrect download item status = {downloadItem.Status} to start it.");
                    }
                }
                eventQueue.Add(batchEventArgs);
                SaveDownloadQueue();
                if (resumeDownloadTask)
                {
                    ResumeDownloadTask();
                }
            }
        }

        public void StopDownloads(IEnumerable<Guid> downloadItemIds)
        {
            lock (downloadQueueLock)
            {
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                foreach (Guid downloadItemId in downloadItemIds)
                {
                    if (!downloadQueueKeyPairs.TryGetValue(downloadItemId, out DownloadItem downloadItem))
                    {
                        Logger.Debug($"Download item with ID = {downloadItemId} not found.");
                        continue;
                    }
                    Logger.Debug($"Stop download requested for download ID = {downloadItemId}.");
                    if (downloadItem.Status == DownloadItemStatus.QUEUED || downloadItem.Status == DownloadItemStatus.DOWNLOADING ||
                        downloadItem.Status == DownloadItemStatus.RETRY_DELAY)
                    {
                        downloadItem.CancelDownload();
                        DownloadItemLogLineEventArgs logLineEventArgs =
                            AddLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.LogLineStopped);
                        batchEventArgs.Add(logLineEventArgs);
                        downloadItem.Status = DownloadItemStatus.STOPPED;
                        batchEventArgs.Add(new DownloadItemChangedEventArgs(downloadItem));
                    }
                    else
                    {
                        Logger.Debug($"Incorrect download item status = {downloadItem.Status} to stop it.");
                    }
                }
                eventQueue.Add(batchEventArgs);
                SaveDownloadQueue();
            }
        }

        public void RemoveDownloads(IEnumerable<Guid> downloadItemIds)
        {
            lock (downloadQueueLock)
            {
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                foreach (Guid downloadItemId in downloadItemIds)
                {
                    if (!downloadQueueKeyPairs.TryGetValue(downloadItemId, out DownloadItem downloadItem))
                    {
                        Logger.Debug($"Download item with ID = {downloadItemId} not found.");
                        return;
                    }
                    Logger.Debug($"Remove download requested for download ID = {downloadItemId}.");
                    downloadItem.CancelDownload();
                    downloadQueue.Remove(downloadItem);
                    downloadQueueKeyPairs.Remove(downloadItemId);
                    downloadItem.Status = DownloadItemStatus.REMOVED;
                    batchEventArgs.Add(new DownloadItemRemovedEventArgs(downloadItem));
                    if (downloadItem.FileCreated && !downloadItem.FileHandleOpened)
                    {
                        string filePath = Path.Combine(downloadItem.DownloadDirectory, downloadItem.FileName);
                        if (downloadItem.Status != DownloadItemStatus.COMPLETED)
                        {
                            filePath += ".part";
                        }
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception exception)
                        {
                            Logger.Exception(exception);
                        }
                    }
                }
                eventQueue.Add(batchEventArgs);
                SaveDownloadQueue();
            }
        }

        public void Shutdown()
        {
            isShuttingDown = true;
            lock (downloadQueueLock)
            {
                SaveDownloadQueue();
                foreach (DownloadItem downloadItem in downloadQueue)
                {
                    downloadItem.Status = DownloadItemStatus.STOPPED;
                    downloadItem.CancelDownload();
                }
            }
            ResumeDownloadTask();
            try
            {
                downloadTask.Wait();
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }
            Logger.Debug("Downloader was shut down successfully.");
        }

        public void Dispose()
        {
            if (!disposed)
            {
                eventQueue?.Dispose();
                downloadTaskResetEvent?.Dispose();
                disposed = true;
            }
        }

        private void ResumeDownloadTask()
        {
            downloadTaskResetEvent.Set();
        }

        private Task StartDownloadTask()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    DownloadItem downloadItem = null;
                    if (isShuttingDown)
                    {
                        return;
                    }
                    if (httpClient == null || isInOfflineMode)
                    {
                        downloadTaskResetEvent.WaitOne();
                    }
                    else
                    {
                        lock (downloadQueueLock)
                        {
                            if (downloadItem == null || downloadItem.Status != DownloadItemStatus.DOWNLOADING)
                            {
                                downloadItem = downloadQueue.FirstOrDefault(item => item.Status == DownloadItemStatus.QUEUED ||
                                    item.Status == DownloadItemStatus.DOWNLOADING || item.Status == DownloadItemStatus.RETRY_DELAY);
                                if (downloadItem != null)
                                {
                                    ReportStatusChange(downloadItem, DownloadItemStatus.DOWNLOADING, DownloadItemLogLineType.INFORMATIONAL,
                                        localization.LogLineStarted);
                                    SaveDownloadQueue();
                                }
                            }
                        }
                        if (downloadItem == null)
                        {
                            downloadTaskResetEvent.WaitOne();
                        }
                        else
                        {
                            if (!downloadItem.FileCreated || downloadItem.RestartSessionOnTimeout)
                            {
                                string url = downloadItem.DownloadPageUrl;
                                string referer = null;
                                if (!String.IsNullOrWhiteSpace(downloadItem.DownloadTransformations))
                                {
                                    foreach (string transformationName in downloadItem.DownloadTransformations.Split(',').
                                        Select(transformation => transformation.Trim()))
                                    {
                                        if (String.IsNullOrEmpty(transformationName))
                                        {
                                            continue;
                                        }
                                        string pageContent;
                                        try
                                        {
                                            pageContent = await DownloadPageAsync(downloadItem, url, referer);
                                        }
                                        catch (TaskCanceledException)
                                        {
                                            Logger.Debug("Page download has been cancelled.");
                                            break;
                                        }
                                        if (pageContent == null)
                                        {
                                            break;
                                        }
                                        try
                                        {
                                            referer = url;
                                            url = ExecuteTransformation(pageContent, transformationName);
                                        }
                                        catch (Exception exception)
                                        {
                                            Logger.Debug($"Transformation {transformationName} threw an exception.");
                                            Logger.Exception(exception);
                                            ReportError(downloadItem, localization.GetLogLineTransformationError(transformationName));
                                            break;
                                        }
                                        bool isUrlValid;
                                        if (String.IsNullOrWhiteSpace(url))
                                        {
                                            isUrlValid = false;
                                        }
                                        else if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                                        {
                                            isUrlValid = false;
                                        }
                                        else
                                        {
                                            url = EncodeInvalidUrlCharacters(url);
                                            isUrlValid = Uri.IsWellFormedUriString(url, UriKind.Absolute);
                                        }
                                        if (!isUrlValid)
                                        {
                                            Logger.Debug($"Transformation {transformationName} failed, returned string:", url);
                                            Logger.Debug("Page:", pageContent);
                                            ReportError(downloadItem, localization.GetLogLineTransformationReturnedIncorrectUrl(transformationName));
                                            break;
                                        }
                                    }
                                }
                                if (downloadItem.CancellationToken.IsCancellationRequested)
                                {
                                    continue;
                                }
                                bool skipFileDownload = false;
                                lock (downloadQueueLock)
                                {
                                    if (downloadItem.Status == DownloadItemStatus.DOWNLOADING)
                                    {
                                        downloadItem.DirectFileUrl = url;
                                        downloadItem.Referer = referer;
                                        SaveDownloadQueue();
                                    }
                                    else
                                    {
                                        SaveDownloadQueue();
                                        if (downloadItem.Status == DownloadItemStatus.RETRY_DELAY)
                                        {
                                            skipFileDownload = true;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                if (!skipFileDownload)
                                {
                                    await DownloadFileAsync(downloadItem);
                                }
                            }
                            else
                            {
                                await DownloadFileAsync(downloadItem);
                            }
                            bool retryDelay;
                            CancellationToken cancellationToken;
                            int currentAttempt;
                            lock (downloadQueueLock)
                            {
                                retryDelay = downloadItem.Status == DownloadItemStatus.RETRY_DELAY;
                                cancellationToken = downloadItem.CancellationToken;
                                currentAttempt = downloadItem.CurrentAttempt;
                            }
                            if (retryDelay)
                            {
                                if (currentAttempt == downloadSettings.Attempts)
                                {
                                    lock (downloadQueueLock)
                                    {
                                        ReportError(downloadItem, localization.LogLineMaximumDownloadAttempts);
                                        SaveDownloadQueue();
                                    }
                                }
                                else
                                {
                                    ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL,
                                        localization.GetLogLineRetryDelay(downloadSettings.RetryDelay));
                                    bool isCancelled = false;
                                    try
                                    {
                                        await Task.Delay(TimeSpan.FromSeconds(downloadSettings.RetryDelay), cancellationToken);
                                    }
                                    catch (TaskCanceledException)
                                    {
                                        isCancelled = true;
                                    }
                                    if (!isCancelled)
                                    {
                                        lock (downloadQueueLock)
                                        {
                                            downloadItem.Status = DownloadItemStatus.DOWNLOADING;
                                            currentAttempt++;
                                            downloadItem.CurrentAttempt = currentAttempt;
                                            if (downloadItem.RestartSessionOnTimeout)
                                            {
                                                downloadItem.Referer = null;
                                            }
                                            ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL,
                                                localization.GetLogLineAttempt(currentAttempt, downloadSettings.Attempts));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        private async Task<string> DownloadPageAsync(DownloadItem downloadItem, string url, string referer)
        {
            ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.GetLogLineDownloadingPage(Uri.UnescapeDataString(url)));
            bool isRedirect;
            int redirectCount = 0;
            HttpResponseMessage response;
            do
            {
                response = await SendDownloadRequestAsync(downloadItem, url, referer, waitForFullContent: true);
                if (response == null)
                {
                    return null;
                }
                isRedirect = IsRedirect(response.StatusCode);
                if (isRedirect)
                {
                    referer = url;
                    if (!GenerateRedirectUrl(referer, response.Headers.Location, out url))
                    {
                        ReportError(downloadItem, localization.GetLogLineIncorrectRedirectUrl(Uri.UnescapeDataString(response.Headers.Location.ToString())));
                        return null;
                    }
                    else
                    {
                        ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.GetLogLineRedirect(Uri.UnescapeDataString(url)));
                        redirectCount++;
                        if (redirectCount == MAX_DOWNLOAD_REDIRECT_COUNT)
                        {
                            ReportError(downloadItem, localization.LogLineTooManyRedirects);
                            return null;
                        }
                    }
                }
            }
            while (isRedirect);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string statusCode = $"{(int)response.StatusCode} {response.StatusCode}";
                Logger.Debug($"Server returned non-successful status code: {statusCode}.");
                string messageText = localization.GetLogLineNonSuccessfulStatusCode(statusCode);
                if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.BadGateway ||
                    response.StatusCode == HttpStatusCode.ServiceUnavailable || response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    ReportStatusChange(downloadItem, DownloadItemStatus.RETRY_DELAY, DownloadItemLogLineType.INFORMATIONAL, messageText);
                }
                else
                {
                    ReportError(downloadItem, messageText);
                }
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        private async Task DownloadFileAsync(DownloadItem downloadItem)
        {
            try
            {
                ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL,
                    localization.GetLogLineDownloadingFile(Uri.UnescapeDataString(downloadItem.DirectFileUrl)));
                if (!Directory.Exists(downloadItem.DownloadDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(downloadItem.DownloadDirectory);
                    }
                    catch (Exception exception)
                    {
                        Logger.Exception(exception);
                        ReportError(downloadItem, localization.GetLogLineCannotCreateDownloadDirectory(downloadItem.DownloadDirectory));
                    }
                }
                string fileName = downloadItem.FileCreated ? downloadItem.FileName : GenerateFileName(downloadItem.DownloadDirectory, downloadItem.FileName,
                    downloadItem.Md5Hash);
                string targetFilePath = Path.Combine(downloadItem.DownloadDirectory, fileName);
                string partFilePath = targetFilePath + ".part";
                bool isCompleted;
                bool isDeleteRequested;
                FileStream destinationFileStream;
                try
                {
                    destinationFileStream = new FileStream(partFilePath, FileMode.Append, FileAccess.Write, FileShare.None);
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                    ReportError(downloadItem, localization.GetLogLineCannotCreateOrOpenFile(partFilePath));
                    return;
                }
                using (destinationFileStream)
                {
                    lock (downloadQueueLock)
                    {
                        downloadItem.FileName = fileName;
                        downloadItem.FileCreated = true;
                        downloadItem.FileHandleOpened = true;
                        SaveDownloadQueue();
                    }
                    await DownloadFileAsync(downloadItem, destinationFileStream);
                    lock (downloadQueueLock)
                    {
                        isCompleted = downloadItem.Status == DownloadItemStatus.COMPLETED;
                        isDeleteRequested = downloadItem.Status == DownloadItemStatus.REMOVED;
                        SaveDownloadQueue();
                    }
                }
                if (isCompleted)
                {
                    bool moveFileError = false;
                    try
                    {
                        File.Move(partFilePath, targetFilePath);
                    }
                    catch (Exception exception)
                    {
                        Logger.Debug($"File rename error: partFilePath = {partFilePath}, targetFilePath = {targetFilePath}");
                        Logger.Exception(exception);
                        ReportError(downloadItem, localization.GetLogLineCannotRenamePartFile(partFilePath, targetFilePath));
                        moveFileError = true;
                    }
                    if (!moveFileError)
                    {
                        ReportStatusChange(downloadItem, DownloadItemStatus.COMPLETED, DownloadItemLogLineType.COMPLETED, localization.LogLineCompleted);
                    }
                }
                else if (isDeleteRequested)
                {
                    try
                    {
                        File.Delete(partFilePath);
                    }
                    catch (Exception exception)
                    {
                        Logger.Debug($"Part file delete error: partFilePath = {partFilePath}");
                        Logger.Exception(exception);
                    }
                }
                lock (downloadQueueLock)
                {
                    downloadItem.FileHandleOpened = false;
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                ReportError(downloadItem, localization.GetLogLineUnexpectedError(exception.GetInnermostException().Message));
            }
        }

        private async Task DownloadFileAsync(DownloadItem downloadItem, FileStream destinationFileStream)
        {
            string url = downloadItem.DirectFileUrl;
            string referer = downloadItem.Referer;
            long startPosition = destinationFileStream.Position;
            bool partialDownload = startPosition > 0;
            bool isRedirect;
            int redirectCount = 0;
            HttpResponseMessage response;
            do
            {
                response = await SendDownloadRequestAsync(downloadItem, url, referer, waitForFullContent: false, startPosition: startPosition);
                if (response == null)
                {
                    return;
                }
                isRedirect = IsRedirect(response.StatusCode);
                if (isRedirect)
                {
                    referer = url;
                    if (!GenerateRedirectUrl(referer, response.Headers.Location, out url))
                    {
                        ReportError(downloadItem, localization.GetLogLineIncorrectRedirectUrl(Uri.UnescapeDataString(response.Headers.Location.ToString())));
                        return;
                    }
                    else
                    {
                        ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.GetLogLineRedirect(Uri.UnescapeDataString(url)));
                        redirectCount++;
                        if (redirectCount == MAX_DOWNLOAD_REDIRECT_COUNT)
                        {
                            ReportError(downloadItem, localization.LogLineTooManyRedirects);
                            return;
                        }
                    }
                }
            }
            while (isRedirect);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
            {
                string statusCode = $"{(int)response.StatusCode} {response.StatusCode}";
                Logger.Debug($"Server returned non-successful status code: {statusCode}.");
                string messageText = localization.GetLogLineNonSuccessfulStatusCode(statusCode);
                if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.BadGateway ||
                    response.StatusCode == HttpStatusCode.ServiceUnavailable || response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    ReportStatusChange(downloadItem, DownloadItemStatus.RETRY_DELAY, DownloadItemLogLineType.INFORMATIONAL, messageText);
                }
                else
                {
                    ReportError(downloadItem, messageText);
                }
                return;
            }
            if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType.CompareOrdinalIgnoreCase("text/html"))
            {
                Logger.Debug($"Server returned HTML page instead of the file.");
                ReportError(downloadItem, localization.LogLineHtmlPageReturned);
                return;
            }
            if (partialDownload && response.StatusCode == HttpStatusCode.OK)
            {
                Logger.Debug("Server doesn't support partial downloads.");
                ReportError(downloadItem, localization.LogLineNoPartialDownloadSupport);
                return;
            }
            long? contentLength = response.Content.Headers.ContentLength;
            if (!contentLength.HasValue)
            {
                Logger.Debug("Server did not return Content-Length value.");
                ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.LogLineNoContentLengthWarning);
            }
            long? remainingSize = contentLength;
            lock (downloadQueueLock)
            {
                downloadItem.DownloadedFileSize = startPosition;
                if (remainingSize.HasValue)
                {
                    downloadItem.TotalFileSize = startPosition + remainingSize;
                }
                else
                {
                    downloadItem.TotalFileSize = null;
                }
                ReportChange(downloadItem);
            }
            if (remainingSize == 0)
            {
                Logger.Debug($"File download is complete.");
                lock (downloadQueueLock)
                {
                    downloadItem.Status = DownloadItemStatus.COMPLETED;
                }
                return;
            }
            if (partialDownload)
            {
                if (remainingSize.HasValue)
                {
                    Logger.Debug($"Remaining download size is {remainingSize.Value} bytes.");
                    ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL,
                        localization.GetLogLineResumingFileDownloadKnownFileSize(remainingSize.Value));
                }
                else
                {
                    Logger.Debug($"Remaining download size is unknown.");
                    ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.LogLineResumingFileDownloadUnknownFileSize);
                }
            }
            else
            {
                if (remainingSize.HasValue)
                {
                    Logger.Debug($"Download file size is {remainingSize.Value} bytes.");
                    ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL,
                        localization.GetLogLineStartingFileDownloadKnownFileSize(remainingSize.Value));
                }
                else
                {
                    Logger.Debug($"Download file size is unknown.");
                    ReportLogLine(downloadItem, DownloadItemLogLineType.INFORMATIONAL, localization.LogLineStartingFileDownloadUnknownFileSize);
                }
            }
            Stream downloadStream = await response.Content.ReadAsStreamAsync();
            byte[] buffer = new byte[4096];
            long downloadedBytes = 0;
            while (!downloadItem.CancellationToken.IsCancellationRequested)
            {
                int bytesRead;
                try
                {
                    bytesRead = downloadStream.Read(buffer, 0, buffer.Length);
                }
                catch (IOException ioException)
                {
                    bool expectedError = false;
                    if (ioException.InnerException != null)
                    {
                        if (ioException.InnerException is WebException webException)
                        {
                            switch (webException.Status)
                            {
                                case WebExceptionStatus.Timeout:
                                    Logger.Debug("Download timeout.");
                                    ReportStatusChange(downloadItem, DownloadItemStatus.RETRY_DELAY, DownloadItemLogLineType.INFORMATIONAL,
                                        localization.LogLineServerResponseTimeout);
                                    expectedError = true;
                                    break;
                                case WebExceptionStatus.RequestCanceled:
                                    Logger.Debug("File download has been cancelled.");
                                    expectedError = true;
                                    break;
                            }
                        }
                    }
                    if (!expectedError)
                    {
                        Logger.Exception(ioException);
                        ReportError(downloadItem, localization.GetLogLineUnexpectedError(ioException.GetInnermostException().Message));
                    }
                    break;
                }
                catch (Exception exception)
                {
                    if (downloadItem.CancellationToken.IsCancellationRequested)
                    {
                        Logger.Debug("File download has been cancelled.");
                        break;
                    }
                    Logger.Exception(exception);
                    ReportError(downloadItem, localization.GetLogLineUnexpectedError(exception.GetInnermostException().Message));
                    break;
                }
                if (bytesRead == 0)
                {
                    bool isCompleted = !remainingSize.HasValue || downloadedBytes == remainingSize.Value;
                    Logger.Debug($"File download is {(isCompleted ? "complete" : "incomplete")}.");
                    if (isCompleted)
                    {
                        lock (downloadQueueLock)
                        {
                            downloadItem.Status = DownloadItemStatus.COMPLETED;
                        }
                    }
                    else
                    {
                        ReportError(downloadItem, localization.LogLineDownloadIncompleteError);
                    }
                    break;
                }
                try
                {
                    destinationFileStream.Write(buffer, 0, bytesRead);
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                    ReportError(downloadItem, localization.LogLineFileWriteError);
                    break;
                }
                downloadedBytes += bytesRead;
                downloadItem.DownloadedFileSize = startPosition + downloadedBytes;
                ReportChange(downloadItem);
            }
        }

        private bool IsRedirect(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 300 && (int)statusCode <= 399;
        }

        private async Task<HttpResponseMessage> SendDownloadRequestAsync(DownloadItem downloadItem, string url, string referer, bool waitForFullContent,
            long? startPosition = null)
        {
            bool partialDownload = startPosition.HasValue && startPosition.Value > 0;
            if (!partialDownload)
            {
                Logger.Debug($"Requesting {url}");
            }
            else
            {
                Logger.Debug($"Requesting {url}, range: {startPosition.Value} - end.");
            }
            HttpRequestMessage request;
            try
            {
                request = new HttpRequestMessage(HttpMethod.Get, url);
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                ReportError(downloadItem, localization.GetLogLineRequestError(Uri.UnescapeDataString(url)));
                return null;
            }
            request.Headers.UserAgent.ParseAdd(USER_AGENT);
            if (downloadItem.Cookies.Any())
            {
                request.Headers.Add("Cookie", GenerateCookieHeader(downloadItem.Cookies));
            }
            if (partialDownload)
            {
                request.Headers.Range = new RangeHeaderValue(startPosition.Value, null);
            }
            if (!String.IsNullOrEmpty(referer))
            {
                request.Headers.Referrer = new Uri(referer);
            }
            string requestHeaders = request.Headers.ToString().TrimEnd();
            Logger.Debug("Request headers:", requestHeaders);
            StringBuilder requestLogBuilder = new StringBuilder();
            requestLogBuilder.Append(localization.LogLineRequest);
            requestLogBuilder.AppendLine(":");
            requestLogBuilder.Append("GET ");
            requestLogBuilder.AppendLine(url);
            requestLogBuilder.AppendLine(requestHeaders);
            ReportLogLine(downloadItem, DownloadItemLogLineType.DEBUG, requestLogBuilder.ToString().TrimEnd());
            HttpResponseMessage response;
            try
            {
                response = await SendRequestAsync(request, downloadItem.CancellationToken, waitForFullContent);
            }
            catch (TimeoutException)
            {
                Logger.Debug("Download timeout.");
                ReportStatusChange(downloadItem, DownloadItemStatus.RETRY_DELAY, DownloadItemLogLineType.INFORMATIONAL,
                    localization.LogLineServerResponseTimeout);
                return null;
            }
            catch (AggregateException aggregateException)
            {
                if (downloadItem.CancellationToken.IsCancellationRequested)
                {
                    Logger.Debug("File download has been cancelled.");
                }
                else
                {
                    Logger.Exception(aggregateException);
                    ReportError(downloadItem, localization.GetLogLineUnexpectedError(aggregateException.GetInnermostException().Message));
                }
                return null;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                ReportError(downloadItem, localization.GetLogLineUnexpectedError(exception.GetInnermostException().Message));
                return null;
            }
            Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
            string responseHeaders = response.Headers.ToString().TrimEnd();
            string responseContentHeaders = response.Content.Headers.ToString().TrimEnd();
            Logger.Debug("Response headers:", responseHeaders, responseContentHeaders);
            StringBuilder responseLogBuilder = new StringBuilder();
            responseLogBuilder.Append(localization.LogLineResponse);
            responseLogBuilder.AppendLine(":");
            responseLogBuilder.Append((int)response.StatusCode);
            responseLogBuilder.Append(" ");
            responseLogBuilder.AppendLine(response.StatusCode.ToString());
            if (!String.IsNullOrEmpty(responseHeaders))
            {
                responseLogBuilder.AppendLine(responseHeaders);
            }
            if (!String.IsNullOrEmpty(responseContentHeaders))
            {
                responseLogBuilder.AppendLine(responseContentHeaders);
            }
            ReportLogLine(downloadItem, DownloadItemLogLineType.DEBUG, responseLogBuilder.ToString().TrimEnd());
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookieHeaders))
            {
                Uri uri = new Uri(url);
                foreach (string cookieHeader in cookieHeaders)
                {
                    AppendCookies(downloadItem.Cookies, uri, cookieHeader);
                }
            }
            return response;
        }

        private Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool waitForFullContent)
        {
            return Task.Run(() =>
            {
                CancellationTokenSource combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                Task<HttpResponseMessage> innerTask = httpClient.SendAsync(request,
                    waitForFullContent ? HttpCompletionOption.ResponseContentRead : HttpCompletionOption.ResponseHeadersRead,
                    combinedCancellationTokenSource.Token);
                try
                {
                    bool success = innerTask.Wait(TimeSpan.FromSeconds(downloadSettings.Timeout));
                    if (success)
                    {
                        return innerTask.Result;
                    }
                    else
                    {
                        combinedCancellationTokenSource.Cancel();
                        throw new TimeoutException();
                    }
                }
                catch (Exception exception)
                {
                    if (exception.GetInnermostException() is SocketException socketException && socketException.SocketErrorCode == SocketError.TimedOut)
                    {
                        throw new TimeoutException();
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }

        private string ExecuteTransformation(string pageContent, string transformationName)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContent);
            XslCompiledTransform xslTransform = new XslCompiledTransform();
            xslTransform.Load(Path.Combine(Environment.MirrorsDirectoryPath, transformationName + ".xslt"));
            XmlWriterSettings outputSettings = xslTransform.OutputSettings.Clone();
            outputSettings.OmitXmlDeclaration = true;
            outputSettings.Encoding = new UTF8Encoding(false);
            using (MemoryStream memoryStream = new MemoryStream())
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, outputSettings))
            {
                xslTransform.Transform(htmlDocument, xmlWriter);
                string xmlEncodedString = Encoding.UTF8.GetString(memoryStream.ToArray()).Trim();
                return WebUtility.HtmlDecode(xmlEncodedString);
            }
        }

        private void AppendCookies(Dictionary<string, string> existingCookies, Uri uri, string cookieHeader)
        {
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(uri, cookieHeader);
            foreach (Cookie cookie in cookieContainer.GetCookies(uri))
            {
                if (!cookie.Expired)
                {
                    existingCookies[cookie.Name] = cookie.Value;
                }
            }
        }

        private string GenerateCookieHeader(Dictionary<string, string> cookies)
        {
            return String.Join(";", cookies.Select(cookie => $"{cookie.Key}={cookie.Value}"));
        }

        private void StartEventPublisherTask()
        {
            Task.Run(() =>
            {
                while (!eventQueue.IsCompleted)
                {
                    DownloaderBatchEventArgs batchEventArgs;
                    try
                    {
                        batchEventArgs = eventQueue.Take();
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                    DownloaderBatchEvent?.Invoke(this, batchEventArgs);
                }
            });
        }

        private HttpClient CreateNewHttpClient(NetworkSettings networkSettings, DownloadSettings downloadSettings)
        {
            WebRequestHandler webRequestHandler = new WebRequestHandler
            {
                Proxy = NetworkUtils.CreateProxy(networkSettings),
                UseProxy = true,
                AllowAutoRedirect = false,
                UseCookies = false,
                ReadWriteTimeout = downloadSettings.Timeout * 1000
            };
            return new HttpClient(webRequestHandler)
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        private void SwitchToOfflineMode()
        {
            lock (downloadQueueLock)
            {
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                foreach (DownloadItem downloadingItem in downloadQueue.Where(downloadItem => downloadItem.Status == DownloadItemStatus.DOWNLOADING ||
                    downloadItem.Status == DownloadItemStatus.RETRY_DELAY))
                {
                    downloadingItem.CancelDownload();
                    DownloadItemLogLineEventArgs logLineEventArgs =
                        AddLogLine(downloadingItem, DownloadItemLogLineType.INFORMATIONAL, localization.LogLineOfflineModeIsOn);
                    batchEventArgs.Add(logLineEventArgs);
                    downloadingItem.Status = DownloadItemStatus.STOPPED;
                    batchEventArgs.Add(new DownloadItemChangedEventArgs(downloadingItem));
                }
                eventQueue.Add(batchEventArgs);
            }
        }

        private void SaveDownloadQueue()
        {
            try
            {
                DownloadQueueStorage.SaveDownloadQueue(downloadQueueFilePath, downloadQueue);
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }
        }

        private string GenerateFileName(string directory, string fileNameTemplate, string md5Hash)
        {
            string fileName = fileNameTemplate;
            string filePath;
            string fileExtension = null;
            bool isMd5HashFileName = false;
            try
            {
                filePath = Path.GetFullPath(Path.Combine(directory, fileName));
            }
            catch (PathTooLongException)
            {
                fileExtension = Path.GetExtension(fileNameTemplate);
                fileNameTemplate = md5Hash + fileExtension;
                fileName = fileNameTemplate;
                filePath = Path.Combine(directory, fileName);
                isMd5HashFileName = true;
            }
            string fileNameWithoutExtension = null;
            int counter = 0;
            while (File.Exists(filePath) || File.Exists(filePath + ".part"))
            {
                if (fileNameWithoutExtension == null)
                {
                    fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameTemplate);
                    fileExtension = Path.GetExtension(fileNameTemplate);
                }
                counter++;
                fileName = $"{fileNameWithoutExtension} ({counter}){fileExtension}";
                try
                {
                    filePath = Path.GetFullPath(Path.Combine(directory, fileName));
                }
                catch (PathTooLongException)
                {
                    if (isMd5HashFileName)
                    {
                        throw;
                    }
                    fileExtension = Path.GetExtension(fileNameTemplate);
                    fileName = md5Hash + fileExtension;
                    filePath = Path.Combine(directory, fileName);
                    counter = 0;
                    fileNameWithoutExtension = null;
                    isMd5HashFileName = true;
                }
            }
            return fileName;
        }

        private bool GenerateRedirectUrl(string requestUrl, Uri newLocationUri, out string redirectUrl)
        {
            if (newLocationUri.IsAbsoluteUri)
            {
                redirectUrl = EncodeInvalidUrlCharacters(newLocationUri.ToString());
            }
            else
            {
                redirectUrl = EncodeInvalidUrlCharacters(new Uri(new Uri(requestUrl), newLocationUri).ToString());
            }
            return Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute);
        }

        private string EncodeInvalidUrlCharacters(string url)
        {
            return url.Replace(" ", "%20").Replace("^", "%5E").Replace("`", "%60").Replace("<", "%3C").Replace(">", "%3E").
                Replace("[", "%5B").Replace("]", "%5D").Replace("{", "%7B").Replace("}", "%7D").Replace("|", "%7C");
        }

        private DownloadItemLogLineEventArgs AddLogLine(DownloadItem downloadItem, DownloadItemLogLineType logLineType, string logLine)
        {
            lock (downloadQueueLock)
            {
                DownloadItemLogLine downloadItemLogLine = new DownloadItemLogLine(logLineType, DateTime.Now, logLine);
                int lineIndex = downloadItem.Logs.Count;
                downloadItem.Logs.Add(downloadItemLogLine);
                Logger.Debug($"Downloader log line: type = {logLineType}, text = \"{logLine}\".");
                return new DownloadItemLogLineEventArgs(downloadItem.Id, lineIndex, downloadItemLogLine);
            }
        }

        private void ReportLogLine(DownloadItem downloadItem, DownloadItemLogLineType logLineType, string logLine)
        {
            lock (downloadQueueLock)
            {
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                DownloadItemLogLineEventArgs logLineEventArgs = AddLogLine(downloadItem, logLineType, logLine);
                batchEventArgs.Add(logLineEventArgs);
                eventQueue.Add(batchEventArgs);
            }
        }

        private void ReportChange(DownloadItem downloadItem)
        {
            lock (downloadQueueLock)
            {
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                batchEventArgs.Add(new DownloadItemChangedEventArgs(downloadItem));
                eventQueue.Add(batchEventArgs);
            }
        }

        private void ReportStatusChange(DownloadItem downloadItem, DownloadItemStatus newStatus, DownloadItemLogLineType logLineType, string logMessage)
        {
            lock (downloadQueueLock)
            {
                DownloaderBatchEventArgs batchEventArgs = new DownloaderBatchEventArgs();
                DownloadItemLogLineEventArgs logLineEventArgs = AddLogLine(downloadItem, logLineType, logMessage);
                batchEventArgs.Add(logLineEventArgs);
                downloadItem.Status = newStatus;
                batchEventArgs.Add(new DownloadItemChangedEventArgs(downloadItem));
                eventQueue.Add(batchEventArgs);
            }
        }

        private void ReportError(DownloadItem downloadItem, string errorMessage)
        {
            ReportStatusChange(downloadItem, DownloadItemStatus.ERROR, DownloadItemLogLineType.ERROR, errorMessage);
        }
    }
}
