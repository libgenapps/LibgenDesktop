namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportDiskSpaceProgress
    {
        public ImportDiskSpaceProgress(long? freeSpaceInBytes)
        {
            FreeSpaceInBytes = freeSpaceInBytes;
        }

        public long? FreeSpaceInBytes { get; }
    }
}
