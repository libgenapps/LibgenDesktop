namespace LibgenDesktop.Models.SqlDump
{
    internal class ColumnDefinition
    {
        public ColumnDefinition(string columnName, ColumnType columnType)
        {
            ColumnName = columnName.ToLower();
            ColumnType = columnType;
        }

        public string ColumnName { get; }
        public ColumnType ColumnType { get; }
    }
}