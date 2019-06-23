namespace LibgenDesktop.Models.ProgressArgs
{
    internal class SynchronizationObjectsProgress
    {
        public SynchronizationObjectsProgress(int objectsDownloaded, int objectsAdded, int objectsUpdated)
        {
            ObjectsDownloaded = objectsDownloaded;
            ObjectsAdded = objectsAdded;
            ObjectsUpdated = objectsUpdated;
        }

        public int ObjectsDownloaded { get; }
        public int ObjectsAdded { get; }
        public int ObjectsUpdated { get; }
    }
}
