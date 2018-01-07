namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportSearchTableDefinitionProgress
    {
        public ImportSearchTableDefinitionProgress(long bytesParsed, long totalBytes)
        {
            BytesParsed = bytesParsed;
            TotalBytes = totalBytes;
        }

        public long BytesParsed { get; }
        public long TotalBytes { get; }
    }
}
