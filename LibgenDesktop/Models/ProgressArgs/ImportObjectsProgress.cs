using LibgenDesktop.Models.SqlDump;

namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportWrongTableDefinitionProgress
    {
        public ImportWrongTableDefinitionProgress(TableType expectedTableType, TableType actualTableType)
        {
            ExpectedTableType = expectedTableType;
            ActualTableType = actualTableType;
        }

        public TableType ExpectedTableType { get; }
        public TableType ActualTableType { get; }
    }
}
