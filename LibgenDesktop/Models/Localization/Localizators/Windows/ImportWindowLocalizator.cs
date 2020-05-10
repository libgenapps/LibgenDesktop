using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class ImportWindowLocalizator : Localizator<Translation.ImportTranslation>
    {
        public ImportWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Import)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            BrowseImportFileDialogTitle = Format(section => section?.BrowseImportFileDialogTitle);
            AllSupportedFiles = Format(section => section?.AllSupportedFiles);
            SqlDumps = Format(section => section?.SqlDumps);
            Archives = Format(section => section?.Archives);
            AllFiles = Format(section => section?.AllFiles);
            Unknown = Format(section => section?.Unknown);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            Close = Format(section => section?.Close);
            StatusDataLookup = FormatStatus(section => section?.DataLookup);
            StatusCreatingIndexes = FormatStatus(section => section?.CreatingIndexes);
            StatusLoadingIds = FormatStatus(section => section?.LoadingIds);
            StatusImportingData = FormatStatus(section => section?.ImportingData);
            StatusImportComplete = FormatStatus(section => section?.ImportComplete);
            StatusImportCancelled = FormatStatus(section => section?.ImportCancelled);
            StatusDataNotFound = FormatStatus(section => section?.DataNotFound);
            StatusImportError = FormatStatus(section => section?.ImportError);
            LogLineDataLookup = FormatLogLine(section => section?.DataLookup);
            LogLineScanning = FormatLogLine(section => section?.Scanning);
            LogLineNonFictionTableFound = FormatLogLine(section => section?.NonFictionTableFound);
            LogLineFictionTableFound = FormatLogLine(section => section?.FictionTableFound);
            LogLineSciMagTableFound = FormatLogLine(section => section?.SciMagTableFound);
            LogLineCreatingIndexes = FormatLogLine(section => section?.CreatingIndexes);
            LogLineLoadingIds = FormatLogLine(section => section?.LoadingIds);
            LogLineImportingData = FormatLogLine(section => section?.ImportingData);
            LogLineImportSuccessful = FormatLogLine(section => section?.ImportSuccessful);
            LogLineImportCancelled = FormatLogLine(section => section?.ImportCancelled);
            LogLineDataNotFound = FormatLogLine(section => section?.DataNotFound);
            LogLineInsufficientDiskSpace = FormatLogLine(section => section?.InsufficientDiskSpace);
            LogLineExpectedNonFictionTable = FormatLogLine(section => section?.ExpectedNonFictionTable);
            LogLineExpectedFictionTable = FormatLogLine(section => section?.ExpectedFictionTable);
            LogLineExpectedSciMagTable = FormatLogLine(section => section?.ExpectedSciMagTable);
            LogLineFoundNonFictionTable = FormatLogLine(section => section?.FoundNonFictionTable);
            LogLineFoundFictionTable = FormatLogLine(section => section?.FoundFictionTable);
            LogLineFoundSciMagTable = FormatLogLine(section => section?.FoundSciMagTable);
            LogLineImportError = FormatLogLine(section => section?.ImportError);
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
        public string LogLineExpectedNonFictionTable { get; }
        public string LogLineExpectedFictionTable { get; }
        public string LogLineExpectedSciMagTable { get; }
        public string LogLineFoundNonFictionTable { get; }
        public string LogLineFoundFictionTable { get; }
        public string LogLineFoundSciMagTable { get; }
        public string LogLineImportError { get; }

        public string GetElapsedString(string elapsed) => Format(section => section?.Elapsed, new { elapsed });

        public string GetFreeSpaceString(string freeSpace) => Format(section => section?.FreeSpace, new { freeSpace });

        public string GetStatusStep(int current, int total) =>
            FormatStatus(section => section?.Step, new { current = Formatter.ToFormattedString(current), total = Formatter.ToFormattedString(total) });

        public string GetLogLineStep(int step) => FormatLogLine(section => section?.Step, new { step = Formatter.ToDecimalFormattedString(step) });

        public string GetLogLineScannedProgress(decimal percent) =>
            FormatLogLine(section => section?.ScannedProgress, new { percent = Formatter.ToFormattedString(percent) });

        public string GetLogLineCreatingIndexForColumn(string column) => FormatLogLine(section => section?.CreatingIndexForColumn, new { column });

        public string GetLogLineLoadingColumnValues(string column) => FormatLogLine(section => section?.LoadingColumnValues, new { column });

        public string GetLogLineImportBooksProgressNoUpdate(int added) =>
            FormatLogLine(section => section?.ImportBooksProgressNoUpdate, new { added = Formatter.ToFormattedString(added) });

        public string GetLogLineImportBooksProgressWithUpdate(int added, int updated) =>
            FormatLogLine(section => section?.ImportBooksProgressWithUpdate,
                new { added = Formatter.ToFormattedString(added), updated = Formatter.ToFormattedString(updated) });

        public string GetLogLineImportArticlesProgressNoUpdate(int added) =>
            FormatLogLine(section => section?.ImportArticlesProgressNoUpdate, new { added = Formatter.ToFormattedString(added) });

        public string GetLogLineImportArticlesProgressWithUpdate(int added, int updated) =>
            FormatLogLine(section => section?.ImportArticlesProgressWithUpdate,
                new { added = Formatter.ToFormattedString(added), updated = Formatter.ToFormattedString(updated) });

        public string GetLogLineWrongTableFound(string expected, string found) => FormatLogLine(section => section?.WrongTableFound, new { expected, found });

        private string FormatStatus(Func<Translation.ImportStatusMessagesTranslation, string> statusFieldSelector, object templateArguments = null) =>
            Format(translation => statusFieldSelector(translation?.Import?.StatusMessages), templateArguments);

        private string FormatLogLine(Func<Translation.ImportLogMessagesTranslation, string> logLineFieldSelector, object templateArguments = null) =>
            Format(translation => logLineFieldSelector(translation?.Import?.LogMessages), templateArguments);
    }
}
