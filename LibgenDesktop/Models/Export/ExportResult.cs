namespace LibgenDesktop.Models.Export
{
    internal class ExportResult
    {
        public ExportResult(int itemsExported, int filesCreated, string firstFilePath, bool isExportCancelled, bool isRowsPerFileLimitReached)
        {
            ItemsExported = itemsExported;
            FilesCreated = filesCreated;
            FirstFilePath = firstFilePath;
            IsExportCancelled = isExportCancelled;
            IsRowsPerFileLimitReached = isRowsPerFileLimitReached;
        }

        public int ItemsExported { get; }
        public int FilesCreated { get; }
        public string FirstFilePath { get; }
        public bool IsExportCancelled { get; }
        public bool IsRowsPerFileLimitReached { get; }
    }
}
