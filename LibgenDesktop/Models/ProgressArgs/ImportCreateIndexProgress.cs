namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportCreateIndexProgress
    {
        public ImportCreateIndexProgress(string columnName)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; }
    }
}
