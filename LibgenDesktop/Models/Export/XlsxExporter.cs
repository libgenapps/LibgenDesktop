namespace LibgenDesktop.Models.Export
{
    internal class XlsxExporter : Exporter<XlsxExportWriter>
    {
        public XlsxExporter(string filePathTemplate, string fileExtenstion, int? rowsPerFile, bool splitIntoMultipleFiles)
            : base(filePathTemplate, rowsPerFile, splitIntoMultipleFiles, fileExtenstion, filePath => new XlsxExportWriter(filePath))
        {
        }
    }
}
