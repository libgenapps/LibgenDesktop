namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportSqlDumpProgress
    {
        public ImportSqlDumpProgress(int booksImported, long bytesParsed, long totalBytes)
        {
            BooksImported = booksImported;
            BytesParsed = bytesParsed;
            TotalBytes = totalBytes;
        }

        public int BooksImported { get; }
        public long BytesParsed { get; }
        public long TotalBytes { get; }
    }
}
