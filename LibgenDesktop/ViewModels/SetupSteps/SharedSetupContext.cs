using System.Collections.Generic;
using LibgenDesktop.Models.Download;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class SharedSetupContext
    {
        internal enum SetupMode
        {
            BASIC = 1,
            ADVANCED
        }

        internal enum DownloadMode
        {
            DOWNLOAD_MANAGER = 1,
            MANUAL
        }

        internal enum DatabaseOperation
        {
            CREATE_DATABASE = 1,
            OPEN_DATABASE
        }

        internal enum CollectionIdentifier
        {
            NON_FICTION = 1,
            FICTION,
            SCIMAG
        }

        internal class Collection
        {
            public Collection(CollectionIdentifier identifier)
            {
                Identifier = identifier;
                IsSelected = false;
                DownloadStatus = DownloadItemStatus.QUEUED;
                TotalSize = null;
                DownloadedSize = null;
                DownloadUrl = null;
                DownloadFilePath = null;
                IsImported = false;
                IsDeleted = false;
            }

            public CollectionIdentifier Identifier { get; }
            public bool IsSelected { get; set; }
            public DownloadItemStatus DownloadStatus { get; set; }
            public long? TotalSize { get; set; }
            public long? DownloadedSize { get; set; }
            public string DownloadUrl { get; set; }
            public string DownloadFilePath { get; set; }
            public bool IsImported { get; set; }
            public bool IsDeleted { get; set; }
        }

        public SharedSetupContext()
        {
            SelectedSetupMode = SetupMode.BASIC;
            SelectedDownloadMode = DownloadMode.DOWNLOAD_MANAGER;
            SelectedDatabaseOperation = DatabaseOperation.CREATE_DATABASE;
            DumpsMetadata = null;
            NonFictionCollection = new Collection(CollectionIdentifier.NON_FICTION);
            FictionCollection = new Collection(CollectionIdentifier.FICTION);
            SciMagCollection = new Collection(CollectionIdentifier.SCIMAG);
            Collections = new List<Collection> { NonFictionCollection, FictionCollection, SciMagCollection };
            IsDatabaseCreated = false;
        }

        public SetupMode SelectedSetupMode { get; set; }
        public DownloadMode SelectedDownloadMode { get; set; }
        public DatabaseOperation SelectedDatabaseOperation { get; set; }
        public LibgenDumpDownloader.Dumps DumpsMetadata { get; set; }
        public Collection NonFictionCollection { get; }
        public Collection FictionCollection { get; }
        public Collection SciMagCollection { get; }
        public List<Collection> Collections { get; }
        public bool IsDatabaseCreated { get; set; }
    }
}
