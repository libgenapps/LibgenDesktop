namespace LibgenDesktop.Models.ProgressArgs
{
    internal class SynchronizationDiskSpaceProgress
    {
        public SynchronizationDiskSpaceProgress(long? freeSpaceInBytes)
        {
            FreeSpaceInBytes = freeSpaceInBytes;
        }

        public long? FreeSpaceInBytes { get; }
    }
}
