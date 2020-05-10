using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.Models.Export
{
    internal class CsvExporter : Exporter<CsvExportWriter>
    {
        public CsvExporter(string filePathTemplate, string fileExtension, int? rowsPerFile, bool splitIntoMultipleFiles, char separator,
            Language currentLanguage)
            : base(filePathTemplate, rowsPerFile, splitIntoMultipleFiles, fileExtension, currentLanguage,
                  filePath => new CsvExportWriter(filePath, separator))
        {
        }
    }
}
