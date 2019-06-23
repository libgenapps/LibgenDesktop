using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LibgenDesktop.Common;

namespace LibgenDesktop.Models.Download
{
    internal class CoverCache
    {
        private struct CoverCacheItem
        {
            public string md5;
            public byte[] coverImage;
        }

        private readonly int capacity;
        private readonly Dictionary<string, int> md5ToCoverCacheIndexes;
        private readonly List<CoverCacheItem> coverCache;
        private readonly object cacheLockObject;
        private int cacheTailIndex;
        private HttpClient httpClient;
        private bool isInOfflineMode;

        public CoverCache(int capacity, HttpClient httpClient, bool isInOfflineMode)
        {
            this.capacity = capacity;
            this.httpClient = httpClient;
            this.isInOfflineMode = isInOfflineMode;
            md5ToCoverCacheIndexes = new Dictionary<string, int>(capacity);
            coverCache = new List<CoverCacheItem>(capacity);
            cacheLockObject = new object();
            cacheTailIndex = 0;
            Logger.Debug($"Cover cache initialization complete. Capacity = {capacity} items.");
        }

        public void Configure(HttpClient httpClient, bool isInOfflineMode)
        {
            this.httpClient = httpClient;
            this.isInOfflineMode = isInOfflineMode;
            Logger.Debug($"Cover cache configuration complete. Offline mode is {(isInOfflineMode ? "on" : "off")}");
        }

        public byte[] TryGetCoverFromCache(string md5)
        {
            Logger.Debug($"Retrieving cover: MD5 = {md5}, cache only = true.");
            lock (cacheLockObject)
            {
                if (!md5ToCoverCacheIndexes.TryGetValue(md5, out int coverCacheIndex))
                {
                    Logger.Debug($"Cover for MD5 = {md5} not found in the cache.");
                    return null;
                }
                Logger.Debug($"Cover for MD5 = {md5} has been retrieved from the cache.");
                return coverCache[coverCacheIndex].coverImage;
            }
        }

        public async Task<byte[]> GetCoverAsync(string md5, string coverUrl)
        {
            md5 = md5.ToUpperInvariant();
            Logger.Debug($"Retrieving cover: MD5 = {md5}, cache only = false, cover URL = {coverUrl}");
            lock (cacheLockObject)
            {
                if (md5ToCoverCacheIndexes.TryGetValue(md5, out int coverCacheIndex))
                {
                    Logger.Debug($"Cover for MD5 = {md5} has been retrieved from the cache.");
                    return coverCache[coverCacheIndex].coverImage;
                }
            }
            if (isInOfflineMode)
            {
                Logger.Debug($"Cover for MD5 = {md5} not found in the cache. Offline mode is on.");
                return null;
            }
            else
            {
                Logger.Debug($"Cover for MD5 = {md5} not found in the cache. Loading it from the server...");
                byte[] coverImage = await LoadCoverAsync(coverUrl);
                lock (cacheLockObject)
                {
                    if (!md5ToCoverCacheIndexes.ContainsKey(md5))
                    {
                        if (cacheTailIndex == capacity)
                        {
                            cacheTailIndex = 0;
                        }
                        CoverCacheItem newCoverCacheItem = new CoverCacheItem
                        {
                            md5 = md5,
                            coverImage = coverImage
                        };
                        if (coverCache.Count > cacheTailIndex)
                        {
                            CoverCacheItem oldCoverCacheItem = coverCache[cacheTailIndex];
                            md5ToCoverCacheIndexes.Remove(md5);
                            Logger.Debug($"Cache is full. Old cover for MD5 = {oldCoverCacheItem.md5} has been removed from the cache.");
                            coverCache[cacheTailIndex] = newCoverCacheItem;
                        }
                        else
                        {
                            coverCache.Add(newCoverCacheItem);
                        }
                        md5ToCoverCacheIndexes.Add(md5, cacheTailIndex);
                        Logger.Debug($"Cover for MD5 = {md5} has been saved into the cache at the index {cacheTailIndex}.");
                        cacheTailIndex++;
                    }
                }
                return coverImage;
            }
        }

        private Task<byte[]> LoadCoverAsync(string coverUrl)
        {
            return httpClient.GetByteArrayAsync(coverUrl);
        }
    }
}
