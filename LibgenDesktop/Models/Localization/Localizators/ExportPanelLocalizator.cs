using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class ExportPanelLocalizator : Localizator
    {
        public ExportPanelLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Header = Format(translation => translation?.Header);
            FormatLabel = Format(translation => translation?.Format) + ":";
            Excel = Format(translation => translation?.Excel);
            Csv = Format(translation => translation?.Csv);
            Separator = Format(translation => translation?.Separator) + ":";
            Comma = Format(translation => translation?.Comma);
            Semicolon = Format(translation => translation?.Semicolon);
            Tab = Format(translation => translation?.Tab);
            SaveAs = Format(translation => translation?.SaveAs) + ":";
            Browse = Format(translation => translation?.Browse);
            BrowseDialogTitle = Format(translation => translation?.BrowseDialogTitle);
            ExcelFiles = Format(translation => translation?.ExcelFiles);
            CsvFiles = Format(translation => translation?.CsvFiles);
            TsvFiles = Format(translation => translation?.TsvFiles);
            AllFiles = Format(translation => translation?.AllFiles);
            ExportRange = Format(translation => translation?.ExportRange) + ":";
            NoLimit = Format(translation => translation?.NoLimit);
            Export = Format(translation => translation?.Export);
            Cancel = Format(translation => translation?.Cancel);
            SavingFile = Format(translation => translation?.SavingFile);
            ErrorWarningTitle = Format(translation => translation?.ErrorWarningTitle);
            InvalidExportPath = Format(translation => translation?.InvalidExportPath);
            InvalidExportFileName = Format(translation => translation?.InvalidExportFileName);
            OverwritePromptTitle = Format(translation => translation?.OverwritePromptTitle);
            RowLimitWarningTitle = Format(translation => translation?.RowLimitWarningTitle);
            RowLimitWarningText = Format(translation => translation?.RowLimitWarningText);
            ExportError = Format(translation => translation?.ExportError);
            Interrupt = Format(translation => translation?.Interrupt);
            Interrupting = Format(translation => translation?.Interrupting);
            ExportInterrupted = Format(translation => translation?.ExportInterrupted);
            Results = Format(translation => translation?.Results);
            Close = Format(translation => translation?.Close);
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

        public string GetLimitString(int count) => Format(translation => translation?.Limit, new { count = Formatter.ToFormattedString(count) });
        public string GetRowCountSingleFileString(int rows) =>
            Format(translation => translation?.RowCountSingleFile, new { rows = Formatter.ToFormattedString(rows) });
        public string GetRowCountMultipleFilesString(int rows, int files) =>
            Format(translation => translation?.RowCountMultipleFiles, new { rows = Formatter.ToFormattedString(rows), files = Formatter.ToFormattedString(files) });
        public string GetDirectoryNotFoundString(string directory) => Format(translation => translation?.DirectoryNotFound, new { directory });
        public string GetOverwritePromptTextString(string file) => Format(translation => translation?.OverwritePromptText, new { file });

        private string Format(Func<Translation.ExportPanelTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.ExportPanel), templateArguments);
        }
    }
}
