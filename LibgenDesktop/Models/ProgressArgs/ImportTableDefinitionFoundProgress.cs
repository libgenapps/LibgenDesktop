using LibgenDesktop.Models.SqlDump;

namespace LibgenDesktop.Models.ProgressArgs
{
    internal class ImportTableDefinitionFoundProgress
    {
        public ImportTableDefinitionFoundProgress(TableType tableFound)
        {
            TableFound = tableFound;
        }

        public TableType TableFound { get; }
    }
}
