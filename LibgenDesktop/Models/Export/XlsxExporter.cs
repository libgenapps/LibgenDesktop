using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.Models.Export
{
    internal class XlsxExporter : Exporter<XlsxExportWriter>
    {
        public XlsxExporter(string filePathTemplate, string fileExtension, int? rowsPerFile, bool splitIntoMultipleFiles, Language currentLanguage)
            : base(filePathTemplate, rowsPerFile, splitIntoMultipleFiles, fileExtension, currentLanguage, filePath => new XlsxExportWriter(filePath))
        {
        }
    }
}
