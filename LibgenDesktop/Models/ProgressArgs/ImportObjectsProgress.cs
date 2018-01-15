namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportObjectsProgress
    {
        public ImportObjectsProgress(int objectsAdded, int objectsUpdated)
        {
            ObjectsAdded = objectsAdded;
            ObjectsUpdated = objectsUpdated;
        }

        public int ObjectsAdded { get; }
        public int ObjectsUpdated { get; }
    }
}
