namespace LibgenDesktop.Models.ProgressArgs
{
    internal class SynchronizationProgress
    {
        public SynchronizationProgress(int objectsDownloaded, int objectsAdded, int objectsUpdated)
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
