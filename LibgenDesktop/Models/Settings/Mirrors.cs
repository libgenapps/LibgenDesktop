using System.Collections.Generic;

namespace LibgenDesktop.Models.Settings
{
    internal class Mirrors : Dictionary<string, Mirrors.MirrorConfiguration>
    {
        internal class DatabaseDumpManualDownloadConfiguration
        {
            public string NonFictionFileNameTemplate { get; set; }
            public string FictionFileNameTemplate { get; set; }
            public string SciMagFileNameTemplate { get; set; }
            public string DateTemplate { get; set; }
        }

        internal class MirrorConfiguration
        {
            public string NonFictionDownloadUrl { get; set; }
            public string NonFictionDownloadTransformations { get; set; }
            public string NonFictionCoverUrl { get; set; }
            public string NonFictionSynchronizationUrl { get; set; }
            public string FictionDownloadUrl { get; set; }
            public string FictionDownloadTransformations { get; set; }
            public string FictionCoverUrl { get; set; }
            public string SciMagDownloadUrl { get; set; }
            public string SciMagDownloadTransformations { get; set; }
            public bool RestartSessionOnTimeout { get; set; }
            public string DatabaseDumpPageUrl { get; set; }
            public string DatabaseDumpPageTransformation { get; set; }
            public DatabaseDumpManualDownloadConfiguration DatabaseDumpManualDownload { get; set; }
        }
    }
}
