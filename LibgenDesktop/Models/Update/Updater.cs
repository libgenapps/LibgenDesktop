using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Download;
using Newtonsoft.Json;
using static LibgenDesktop.Common.Constants;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.Models.Update
{
    internal class Updater : IDisposable
    {
        private const string ASSET_NAME_SETUP_64BIT = "LibgenDesktop.Setup.64-bit.msi";
        private const string ASSET_NAME_SETUP_32BIT = "LibgenDesktop.Setup.32-bit.msi";
        private const string ASSET_NAME_PORTABLE_64BIT = "LibgenDesktop.Portable.64-bit.zip";
        private const string ASSET_NAME_PORTABLE_32BIT = "LibgenDesktop.Portable.32-bit.zip";

        internal class UpdateCheckResult
        {
            public UpdateCheckResult(string newReleaseName, DateTime publishedAt, string fileName, int fileSize, string downloadUrl)
            {
                NewReleaseName = newReleaseName;
                PublishedAt = publishedAt;
                FileName = fileName;
                FileSize = fileSize;
                DownloadUrl = downloadUrl;
            }

            public string NewReleaseName { get; }
            public DateTime PublishedAt { get; }
            public string FileName { get; }
            public int FileSize { get; }
            public string DownloadUrl { get; }
        }

        internal class UpdateCheckEventArgs : EventArgs
        {
            public UpdateCheckEventArgs(UpdateCheckResult result)
            {
                Result = result;
            }

            public UpdateCheckResult Result { get; }
        }

        internal class UpdateDownloadResult
        {
            public DownloadUtils.DownloadResult DownloadResult { get; set; }
            public string DownloadFilePath { get; set; }
        }

        private readonly string expectedAssetName;
        private HttpClient httpClient;
        private string updateUrl;
        private Timer timer;
        private string ignoreReleaseName;
        private bool disposed;

        public Updater()
        {
            expectedAssetName = GetExpectedAssetName();
            httpClient = null;
            updateUrl = null;
            timer = null;
            ignoreReleaseName = null;
            disposed = false;
        }

        public event EventHandler<UpdateCheckEventArgs> UpdateCheck;

        public void Configure(HttpClient httpClient, string updateUrl, DateTime? nextUpdateCheck, string ignoreReleaseName)
        {
            this.httpClient = httpClient;
            this.updateUrl = updateUrl;
            this.ignoreReleaseName = ignoreReleaseName;
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            if (nextUpdateCheck.HasValue)
            {
                TimeSpan nextUpdateTimeSpan = nextUpdateCheck.Value - DateTime.Now;
                if (nextUpdateTimeSpan.TotalSeconds < 0)
                {
                    nextUpdateTimeSpan = TimeSpan.Zero;
                }
                timer = new Timer(state => TimerTick(), null, nextUpdateTimeSpan, Timeout.InfiniteTimeSpan);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                disposed = true;
            }
        }

        public async Task<UpdateCheckResult> CheckForUpdateAsync(bool ignoreSpecifiedRelease)
        {
            UpdateCheckResult result = null;
            if (httpClient != null)
            {
                DownloadUtils.DownloadPageResult downloadPageResult = await DownloadUtils.DownloadPageAsync(httpClient, updateUrl,
                    CancellationToken.None);
                if (downloadPageResult.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"GitHub API returned {(int)downloadPageResult.HttpStatusCode} {downloadPageResult.HttpStatusCode}.");
                }
                List<GitHubApiRelease> releases;
                try
                {
                    releases = JsonConvert.DeserializeObject<List<GitHubApiRelease>>(downloadPageResult.PageContent);
                }
                catch (Exception exception)
                {
                    throw new Exception("GitHub API response is not a valid JSON string.", exception);
                }
                Logger.Debug($"{releases.Count} releases have been parsed from the GitHub API response.");
                if (releases.Any())
                {
                    GitHubApiRelease latestRelease = releases.First();
                    Logger.Debug($@"Latest release is ""{latestRelease.Name}"".");
                    if (latestRelease.Name != CURRENT_GITHUB_RELEASE_NAME && (!ignoreSpecifiedRelease || latestRelease.Name != ignoreReleaseName))
                    {
                        GitHubApiRelease.Asset asset = latestRelease.Assets.FirstOrDefault(assetItem => assetItem.Name == expectedAssetName);
                        if (asset != null)
                        {
                            Logger.Debug($"New asset is {asset.Name} ({asset.Size} bytes), download URL = {asset.DownloadUrl}.");
                            result = new UpdateCheckResult(latestRelease.Name, latestRelease.PublishedAt, asset.Name, asset.Size, asset.DownloadUrl);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<UpdateDownloadResult> DownloadUpdateAsync(UpdateCheckResult updateCheckResult, IProgress<object> progressHandler,
            CancellationToken cancellationToken)
        {
            UpdateDownloadResult result = new UpdateDownloadResult();
            string downloadDirectory = Path.Combine(Environment.AppDataDirectory, "Updates");
            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }
            result.DownloadFilePath = Path.Combine(downloadDirectory, updateCheckResult.FileName);
            result.DownloadResult = await DownloadUtils.DownloadFileAsync(httpClient, updateCheckResult.DownloadUrl, result.DownloadFilePath, false,
                progressHandler, cancellationToken);
            return result;
        }

        private void TimerTick()
        {
            UpdateCheckResult updateCheckResult;
            try
            {
                updateCheckResult = CheckForUpdateAsync(ignoreSpecifiedRelease: true).Result;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                updateCheckResult = null;
            }
            UpdateCheck?.Invoke(this, new UpdateCheckEventArgs(updateCheckResult));
        }

        private string GetExpectedAssetName()
        {
            if (Environment.IsInPortableMode)
            {
                return Environment.IsIn64BitProcess ? ASSET_NAME_PORTABLE_64BIT : ASSET_NAME_PORTABLE_32BIT;
            }
            else
            {
                return Environment.IsIn64BitProcess ? ASSET_NAME_SETUP_64BIT : ASSET_NAME_SETUP_32BIT;
            }
        }
    }
}
