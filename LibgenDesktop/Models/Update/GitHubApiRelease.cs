using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LibgenDesktop.Models.Update
{
    internal class GitHubApiRelease
    {
        internal class Asset
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("size")]
            public int Size { get; set; }

            [JsonProperty("browser_download_url")]
            public string DownloadUrl { get; set; }
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }
    }
}
