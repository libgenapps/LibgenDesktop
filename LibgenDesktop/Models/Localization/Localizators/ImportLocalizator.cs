using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class ImportLocalizator : Localizator
    {
        public ImportLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            BrowseImportFileDialogTitle = Format(translation => translation?.BrowseImportFileDialogTitle);
            AllSupportedFiles = Format(translation => translation?.AllSupportedFiles);
            SqlDumps = Format(translation => translation?.SqlDumps);
            Archives = Format(translation => translation?.Archives);
            AllFiles = Format(translation => translation?.AllFiles);
            Unknown = Format(translation => translation?.Unknown);
            Interrupt = Format(translation => translation?.Interrupt);
            Interrupting = Format(translation => translation?.Interrupting);
            Close = Format(translation => translation?.Close);
            StatusDataLookup = FormatStatus(translation => translation?.DataLookup);
            StatusCreatingIndexes = FormatStatus(translation => translation?.CreatingIndexes);
            StatusLoadingIds = FormatStatus(translation => translation?.LoadingIds);
            StatusImportingData = FormatStatus(translation => translation?.ImportingData);
            StatusImportComplete = FormatStatus(translation => translation?.ImportComplete);
            StatusImportCancelled = FormatStatus(translation => translation?.ImportCancelled);
            StatusDataNotFound = FormatStatus(translation => translation?.DataNotFound);
            StatusImportError = FormatStatus(translation => translation?.ImportError);
            LogLineDataLookup = FormatLogLine(translation => translation?.DataLookup);
            LogLineScanning = FormatLogLine(translation => translation?.Scanning);
            LogLineNonFictionTableFound = FormatLogLine(translation => translation?.NonFictionTableFound);
            LogLineFictionTableFound = FormatLogLine(translation => translation?.FictionTableFound);
            LogLineSciMagTableFound = FormatLogLine(translation => translation?.SciMagTableFound);
            LogLineCreatingIndexes = FormatLogLine(translation => translation?.CreatingIndexes);
            LogLineLoadingIds = FormatLogLine(translation => translation?.LoadingIds);
            LogLineImportingData = FormatLogLine(translation => translation?.ImportingData);
            LogLineImportSuccessful = FormatLogLine(translation => translation?.ImportSuccessful);
            LogLineImportCancelled = FormatLogLine(translation => translation?.ImportCancelled);
            LogLineDataNotFound = FormatLogLine(translation => translation?.DataNotFound);
            LogLineInsufficientDiskSpace = FormatLogLine(translation => translation?.InsufficientDiskSpace);
            LogLineImportError = FormatLogLine(translation => translation?.ImportError);
        }

        public string WindowTitle { get; }
        public string BrowseImportFileDialogTitle { get; }
        public string AllSupportedFiles { get; }
        public string SqlDumps { get; }
        public string Archives { get; }
        public string AllFiles { get; }
        public string Unknown { get; set; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string Close { get; }
        public string StatusDataLookup { get; }
        public string StatusCreatingIndexes { get; }
        public string StatusLoadingIds { get; }
        public string StatusImportingData { get; }
        public string StatusImportComplete { get; }
        public string StatusImportCancelled { get; }
        public string StatusDataNotFound { get; }
        public string StatusImportError { get; }
        public string LogLineDataLookup { get; }
        public string LogLineScanning { get; }
        public string LogLineNonFictionTableFound { get; }
        public string LogLineFictionTableFound { get; }
        public string LogLineSciMagTableFound { get; }
        public string LogLineCreatingIndexes { get; }
        public string LogLineLoadingIds { get; }
        public string LogLineImportingData { get; }
        public string LogLineImportSuccessful { get; }
        public string LogLineImportCancelled { get; }
        public string LogLineDataNotFound { get; }
        public string LogLineInsufficientDiskSpace { get; }
        public string LogLineImportError { get; }

        public string GetElapsedString(string elapsed) => Format(translation => translation?.Elapsed, new { elapsed });
        public string GetFreeSpaceString(string freeSpace) => Format(translation => translation?.FreeSpace, new { freeSpace });
        public string GetStatusStep(int current, int total) => FormatStatus(translation => translation?.Step, new { current, total });
        public string GetLogLineStep(int step) => FormatLogLine(translation => translation?.Step, new { step });
        public string GetLogLineScannedProgress(decimal percent) =>
            FormatLogLine(translation => translation?.ScannedProgress, new { percent = Formatter.ToFormattedString(percent) });
        public string GetLogLineCreatingIndexForColumn(string column) => FormatLogLine(translation => translation?.CreatingIndexForColumn, new { column });
        public string GetLogLineLoadingColumnValues(string column) => FormatLogLine(translation => translation?.LoadingColumnValues, new { column });
        public string GetLogLineImportBooksProgressNoUpdate(int added) =>
            FormatLogLine(translation => translation?.ImportBooksProgressNoUpdate, new { added = Formatter.ToFormattedString(added) });
        public string GetLogLineImportBooksProgressWithUpdate(int added, int updated) =>
            FormatLogLine(translation => translation?.ImportBooksProgressWithUpdate,
                new { added = Formatter.ToFormattedString(added), updated = Formatter.ToFormattedString(updated) });
        public string GetLogLineImportArticlesProgressNoUpdate(int added) =>
            FormatLogLine(translation => translation?.ImportArticlesProgressNoUpdate, new { added = Formatter.ToFormattedString(added) });
        public string GetLogLineImportArticlesProgressWithUpdate(int added, int updated) =>
            FormatLogLine(translation => translation?.ImportArticlesProgressWithUpdate,
                new { added = Formatter.ToFormattedString(added), updated = Formatter.ToFormattedString(updated) });

        private string Format(Func<Translation.ImportTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Import), templateArguments);
        }

        private string FormatStatus(Func<Translation.ImportStatusMessagesTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Import?.StatusMessages), templateArguments);
        }

        private string FormatLogLine(Func<Translation.ImportLogMessagesTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Import?.LogMessages), templateArguments);
        }
    }
}
