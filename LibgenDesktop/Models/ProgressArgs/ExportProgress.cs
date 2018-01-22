namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ExportProgress
    {
        public ExportProgress(int itemsExported, int filesCreated, bool isWriterDisposing)
        {
            ItemsExported = itemsExported;
            FilesCreated = filesCreated;
            IsWriterDisposing = isWriterDisposing;
        }

        public int ItemsExported { get; }
        public int FilesCreated { get; }
        public bool IsWriterDisposing { get; }
    }
}
