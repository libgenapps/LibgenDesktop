using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;
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

        private readonly string expectedAssetName;
        private HttpClient httpClient;
        private Timer timer;
        private string ignoreReleaseName;
        private bool disposed;

        public Updater()
        {
            expectedAssetName = GetExpectedAssetName();
            httpClient = null;
            timer = null;
            ignoreReleaseName = null;
            disposed = false;
        }

        public event EventHandler<UpdateCheckEventArgs> UpdateCheck;

        public void Configure(HttpClient httpClient, DateTime? nextUpdateCheck, string ignoreReleaseName)
        {
            this.httpClient = httpClient;
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
                string url = GITHUB_RELEASE_API_URL;
                Logger.Debug($"Sending a request to {url}");
                HttpResponseMessage response = await httpClient.GetAsync(url);
                Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
                Logger.Debug("Response headers:", response.Headers.ToString().TrimEnd(), response.Content.Headers.ToString().TrimEnd());
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"GitHub API returned {(int)response.StatusCode} {response.StatusCode}.");
                }
                string responseContent = await response.Content.ReadAsStringAsync();
                Logger.Debug("Response content:", responseContent);
                List<GitHubApiRelease> releases;
                try
                {
                    releases = JsonConvert.DeserializeObject<List<GitHubApiRelease>>(responseContent);
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
