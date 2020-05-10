using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using HtmlAgilityPack;
using LibgenDesktop.Common;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Utils;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.Models.Download
{
    internal static class DownloadUtils
    {
        internal enum DownloadResult
        {
            COMPLETED = 1,
            INCOMPLETE,
            CANCELLED,
            ERROR
        }

        internal class DownloadPageResult
        {
            public DownloadResult DownloadResult { get; set; }
            public HttpStatusCode HttpStatusCode { get; set; }
            public string PageContent { get; set; }
        }

        public static async Task<DownloadPageResult> DownloadPageAsync(HttpClient httpClient, string pageUrl, CancellationToken cancellationToken)
        {
            DownloadPageResult result = new DownloadPageResult();
            Logger.Debug($"Sending a request to {pageUrl}");
            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(pageUrl, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Logger.Debug("Page download has been cancelled.");
                result.DownloadResult = DownloadResult.CANCELLED;
                return result;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                result.DownloadResult = DownloadResult.ERROR;
                return result;
            }
            Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
            Logger.Debug("Response headers:", response.Headers.ToString().TrimEnd(), response.Content.Headers.ToString().TrimEnd());
            result.HttpStatusCode = response.StatusCode;
            result.PageContent = await response.Content.ReadAsStringAsync();
            Logger.Debug("Response content:", result.PageContent);
            result.DownloadResult = DownloadResult.COMPLETED;
            return result;
        }

        public static Task<DownloadResult> DownloadFileAsync(HttpClient httpClient, string fileUrl, string destinationPath, bool resumeDownload,
            IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                Logger.Debug($"Requesting {fileUrl}");
                long? startPosition = null;
                HttpRequestMessage request;
                try
                {
                    request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                    if (resumeDownload && File.Exists(destinationPath))
                    {
                        startPosition = new FileInfo(destinationPath).Length;
                        if (startPosition > 0)
                        {
                            long? totalDownloadSize;
                            try
                            {
                                totalDownloadSize = await GetDownloadContentLength(httpClient, fileUrl, cancellationToken);
                            }
                            catch (TaskCanceledException)
                            {
                                Logger.Debug("File download has been cancelled.");
                                return DownloadResult.CANCELLED;
                            }
                            if (!totalDownloadSize.HasValue)
                            {
                                Logger.Debug("Couldn't retrieve file size.");
                                return DownloadResult.ERROR;
                            }
                            if (startPosition >= totalDownloadSize.Value)
                            {
                                if (startPosition > totalDownloadSize.Value)
                                {
                                    Logger.Debug($"Current file size: {startPosition} bytes is larger " +
                                        "than the total download size: {totalDownloadSize.Value} bytes.");
                                }
                                else
                                {
                                    Logger.Debug("File has already been downloaded.");
                                }
                                progressHandler.Report(new DownloadFileProgress(totalDownloadSize.Value, totalDownloadSize.Value));
                                return DownloadResult.COMPLETED;
                            }
                            request.Headers.Range = new RangeHeaderValue(startPosition.Value, null);
                            Logger.Debug($"Resuming download from {startPosition.Value} bytes.");
                        }
                        else
                        {
                            startPosition = null;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                    return DownloadResult.ERROR;
                }
                HttpResponseMessage response;
                try
                {
                    response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Logger.Debug("File download has been cancelled.");
                    return DownloadResult.CANCELLED;
                }
                catch (TimeoutException)
                {
                    Logger.Debug("Download timeout.");
                    return DownloadResult.ERROR;
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                    return DownloadResult.ERROR;
                }
                Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
                Logger.Debug("Response headers:", response.Headers.ToString().TrimEnd(), response.Content.Headers.ToString().TrimEnd());
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                {
                    Logger.Debug($"Server returned {(int)response.StatusCode} {response.StatusCode}.");
                    return DownloadResult.ERROR;
                }
                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType.CompareOrdinalIgnoreCase("text/html"))
                {
                    Logger.Debug($"Server returned HTML page instead of the file.");
                    return DownloadResult.ERROR;
                }
                if (startPosition.HasValue && response.StatusCode == HttpStatusCode.OK)
                {
                    Logger.Debug("Server doesn't support partial downloads.");
                    return DownloadResult.ERROR;
                }
                else if (!startPosition.HasValue)
                {
                    startPosition = 0;
                }
                long? contentLength = response.Content.Headers.ContentLength;
                if (!contentLength.HasValue)
                {
                    Logger.Debug("Server did not return Content-Length value.");
                    return DownloadResult.ERROR;
                }
                long remainingSize = contentLength.Value;
                progressHandler.Report(new DownloadFileProgress(startPosition.Value, startPosition.Value + remainingSize));
                if (remainingSize == 0)
                {
                    return DownloadResult.COMPLETED;
                }
                Logger.Debug($"Remaining download size is {remainingSize} bytes.");
                using (Stream downloadStream = await response.Content.ReadAsStreamAsync())
                {
                    byte[] buffer = new byte[4096];
                    long downloadedBytes = 0;
                    using (FileStream destinationFileStream = new FileStream(destinationPath, resumeDownload ? FileMode.Append : FileMode.Create))
                    {
                        while (true)
                        {
                            int bytesRead;
                            try
                            {
                                bytesRead = downloadStream.Read(buffer, 0, buffer.Length);
                            }
                            catch (Exception exception)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    Logger.Debug("File download has been cancelled.");
                                    return DownloadResult.CANCELLED;
                                }
                                Logger.Exception(exception);
                                return DownloadResult.ERROR;
                            }
                            if (bytesRead == 0)
                            {
                                bool isCompleted = downloadedBytes == remainingSize;
                                Logger.Debug($"File download is {(isCompleted ? "complete" : "incomplete")}.");
                                return isCompleted ? DownloadResult.COMPLETED : DownloadResult.INCOMPLETE;
                            }
                            destinationFileStream.Write(buffer, 0, bytesRead);
                            downloadedBytes += bytesRead;
                            progressHandler.Report(new DownloadFileProgress(startPosition.Value + downloadedBytes, startPosition.Value + remainingSize));
                        }
                    }
                }
            });
        }

        public static string ExecuteTransformation(string input, string transformationName, bool htmlDecode)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);
            XslCompiledTransform xslTransform = new XslCompiledTransform();
            xslTransform.Load(Path.Combine(Environment.MirrorsDirectoryPath, transformationName + ".xslt"));
            XmlWriterSettings outputSettings = xslTransform.OutputSettings.Clone();
            outputSettings.OmitXmlDeclaration = true;
            outputSettings.Encoding = new UTF8Encoding(false);
            using (MemoryStream memoryStream = new MemoryStream())
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, outputSettings))
            {
                xslTransform.Transform(htmlDocument, xmlWriter);
                string transformationOutput = Encoding.UTF8.GetString(memoryStream.ToArray()).Trim();
                return htmlDecode ? WebUtility.HtmlDecode(transformationOutput) : transformationOutput;
            }
        }

        private static async Task<long?> GetDownloadContentLength(HttpClient httpClient, string fileUrl, CancellationToken cancellationToken)
        {
            Logger.Debug("Retrieving download size.");
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, fileUrl))
            using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
                Logger.Debug("Response headers:", response.Headers.ToString().TrimEnd(), response.Content.Headers.ToString().TrimEnd());
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Debug($"Server returned {(int)response.StatusCode} {response.StatusCode}.");
                    return null;
                }
                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType.CompareOrdinalIgnoreCase("text/html"))
                {
                    Logger.Debug($"Server returned HTML page instead of the file.");
                    return null;
                }
                long? contentLength = response.Content.Headers.ContentLength;
                if (!contentLength.HasValue)
                {
                    Logger.Debug("Server did not return Content-Length value.");
                    return null;
                }
                return contentLength.Value;
            }
        }
    }
}
