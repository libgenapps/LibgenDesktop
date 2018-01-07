namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportObjectsProgress
    {
        public ImportObjectsProgress(int objectsImported)
        {
            ObjectsImported = objectsImported;
        }

        public int ObjectsImported { get; }
    }
}
