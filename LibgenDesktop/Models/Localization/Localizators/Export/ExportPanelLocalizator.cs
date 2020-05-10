using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Export
{
    internal class ExportPanelLocalizator : Localizator<Translation.ExportPanelTranslation>
    {
        public ExportPanelLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.ExportPanel)
        {
            Header = Format(section => section?.Header);
            FormatLabel = Format(section => section?.Format) + ":";
            Excel = Format(section => section?.Excel);
            Csv = Format(section => section?.Csv);
            Separator = Format(section => section?.Separator) + ":";
            Comma = Format(section => section?.Comma);
            Semicolon = Format(section => section?.Semicolon);
            Tab = Format(section => section?.Tab);
            SaveAs = Format(section => section?.SaveAs) + ":";
            Browse = Format(section => section?.Browse);
            BrowseDialogTitle = Format(section => section?.BrowseDialogTitle);
            ExcelFiles = Format(section => section?.ExcelFiles);
            CsvFiles = Format(section => section?.CsvFiles);
            TsvFiles = Format(section => section?.TsvFiles);
            AllFiles = Format(section => section?.AllFiles);
            ExportRange = Format(section => section?.ExportRange) + ":";
            NoLimit = Format(section => section?.NoLimit);
            Export = Format(section => section?.Export);
            Cancel = Format(section => section?.Cancel);
            SavingFile = Format(section => section?.SavingFile);
            ErrorWarningTitle = Format(section => section?.ErrorWarningTitle);
            InvalidExportPath = Format(section => section?.InvalidExportPath);
            InvalidExportFileName = Format(section => section?.InvalidExportFileName);
            OverwritePromptTitle = Format(section => section?.OverwritePromptTitle);
            RowLimitWarningTitle = Format(section => section?.RowLimitWarningTitle);
            RowLimitWarningText = Format(section => section?.RowLimitWarningText);
            ExportError = Format(section => section?.ExportError);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            ExportInterrupted = Format(section => section?.ExportInterrupted);
            Results = Format(section => section?.Results);
            Close = Format(section => section?.Close);
        }

        public string Header { get; }
        public string FormatLabel { get; }
        public string Excel { get; }
        public string Csv { get; }
        public string Separator { get; }
        public string Comma { get; }
        public string Semicolon { get; }
        public string Tab { get; }
        public string SaveAs { get; }
        public string Browse { get; }
        public string BrowseDialogTitle { get; }
        public string ExcelFiles { get; }
        public string CsvFiles { get; }
        public string TsvFiles { get; }
        public string AllFiles { get; }
        public string ExportRange { get; }
        public string NoLimit { get; }
        public string Export { get; }
        public string Cancel { get; }
        public string SavingFile { get; }
        public string ErrorWarningTitle { get; }
        public string InvalidExportPath { get; }
        public string InvalidExportFileName { get; }
        public string OverwritePromptTitle { get; }
        public string RowLimitWarningTitle { get; }
        public string RowLimitWarningText { get; }
        public string ExportError { get; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string ExportInterrupted { get; }
        public string Results { get; }
        public string Close { get; }

        public string GetLimitString(int count) => Format(section => section?.Limit, new { count = Formatter.ToFormattedString(count) });

        public string GetRowCountSingleFileString(int rows) =>
            Format(section => section?.RowCountSingleFile, new { rows = Formatter.ToFormattedString(rows) });

        public string GetRowCountMultipleFilesString(int rows, int files) =>
            Format(section => section?.RowCountMultipleFiles, new { rows = Formatter.ToFormattedString(rows), files = Formatter.ToFormattedString(files) });

        public string GetDirectoryNotFoundString(string directory) => Format(section => section?.DirectoryNotFound, new { directory });

        public string GetOverwritePromptTextString(string file) => Format(section => section?.OverwritePromptText, new { file });
    }
}
