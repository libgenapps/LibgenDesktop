namespace LibgenDesktop.Models.Export
{
    internal class CsvExporter : Exporter<CsvExportWriter>
    {
        public CsvExporter(string filePathTemplate, string fileExtenstion, int? rowsPerFile, bool splitIntoMultipleFiles, char separator)
            : base(filePathTemplate, rowsPerFile, splitIntoMultipleFiles, fileExtenstion, filePath => new CsvExportWriter(filePath, separator))
        {
        }
    }
}
