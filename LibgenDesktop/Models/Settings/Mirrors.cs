using System.Collections.Generic;

namespace LibgenDesktop.Models.Settings
{
    internal class Mirrors : Dictionary<string, Mirrors.MirrorConfiguration>
    {
        internal class MirrorConfiguration
        {
            public string NonFictionDownloadUrl { get; set; }
            public string NonFictionCoverUrl { get; set; }
            public string NonFictionSynchronizationUrl { get; set; }
            public string FictionDownloadUrl { get; set; }
            public string FictionCoverUrl { get; set; }
            public string SciMagDownloadUrl { get; set; }
        }
    }
}
