using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class SynchronizationWindowLocalizator : Localizator<Translation.SynchronizationTranslation>
    {
        public SynchronizationWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Synchronization)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            ErrorMessageTitle = Format(section => section?.ErrorMessageTitle);
            ImportRequired = Format(section => section?.ImportRequired);
            NoSynchronizationMirror = Format(section => section?.NoSynchronizationMirror);
            OfflineModePromptTitle = Format(section => section?.OfflineModePromptTitle);
            OfflineModePromptText = Format(section => section?.OfflineModePromptText);
            Unknown = Format(section => section?.Unknown);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            Close = Format(section => section?.Close);
            StatusPreparation = FormatStatus(section => section?.Preparation);
            StatusCreatingIndexes = FormatStatus(section => section?.CreatingIndexes);
            StatusLoadingIds = FormatStatus(section => section?.LoadingIds);
            StatusSynchronizingData = FormatStatus(section => section?.SynchronizingData);
            StatusSynchronizationComplete = FormatStatus(section => section?.SynchronizationComplete);
            StatusSynchronizationCancelled = FormatStatus(section => section?.SynchronizationCancelled);
            StatusSynchronizationError = FormatStatus(section => section?.SynchronizationError);
            LogLineCreatingIndexes = FormatLogLine(section => section?.CreatingIndexes);
            LogLineLoadingIds = FormatLogLine(section => section?.LoadingIds);
            LogLineSynchronizingBookList = FormatLogLine(section => section?.SynchronizingBookList);
            LogLineDownloadingNewBooks = FormatLogLine(section => section?.DownloadingNewBooks);
            LogLineSynchronizationSuccessful = FormatLogLine(section => section?.SynchronizationSuccessful);
            LogLineInsufficientDiskSpace = FormatLogLine(section => section?.InsufficientDiskSpace);
            LogLineSynchronizationCancelled = FormatLogLine(section => section?.SynchronizationCancelled);
        }

        public string WindowTitle { get; }
        public string ErrorMessageTitle { get; }
        public string ImportRequired { get; }
        public string NoSynchronizationMirror { get; }
        public string OfflineModePromptTitle { get; }
        public string OfflineModePromptText { get; }
        public string Unknown { get; set; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string Close { get; }
        public string StatusPreparation { get; set; }
        public string StatusCreatingIndexes { get; set; }
        public string StatusLoadingIds { get; set; }
        public string StatusSynchronizingData { get; set; }
        public string StatusSynchronizationComplete { get; set; }
        public string StatusSynchronizationCancelled { get; set; }
        public string StatusSynchronizationError { get; set; }
        public string LogLineCreatingIndexes { get; set; }
        public string LogLineLoadingIds { get; set; }
        public string LogLineSynchronizingBookList { get; set; }
        public string LogLineDownloadingNewBooks { get; set; }
        public string LogLineSynchronizationSuccessful { get; set; }
        public string LogLineInsufficientDiskSpace { get; }
        public string LogLineSynchronizationCancelled { get; set; }

        public string GetElapsedString(string elapsed) => Format(section => section?.Elapsed, new { elapsed });

        public string GetFreeSpaceString(string freeSpace) => Format(section => section?.FreeSpace, new { freeSpace });

        public string GetStatusStep(int current, int total) =>
            FormatStatus(section => section?.Step, new { current = Formatter.ToFormattedString(current), total = Formatter.ToFormattedString(total) });

        public string GetLogLineStep(int step) => FormatLogLine(section => section?.Step, new { step = Formatter.ToDecimalFormattedString(step) });

        public string GetLogLineCreatingIndexForColumn(string column) => FormatLogLine(section => section?.CreatingIndexForColumn, new { column });

        public string GetLogLineLoadingColumnValues(string column) => FormatLogLine(section => section?.LoadingColumnValues, new { column });

        public string GetLogLineSynchronizationProgressNoAddedNoUpdated(int downloaded) =>
            FormatLogLine(section => section?.SynchronizationProgressNoAddedNoUpdated, new { downloaded = Formatter.ToFormattedString(downloaded) });

        public string GetLogLineSynchronizationProgressAdded(int downloaded, int added) =>
            FormatLogLine(section => section?.SynchronizationProgressAdded,
                new { downloaded = Formatter.ToFormattedString(downloaded), added = Formatter.ToFormattedString(added) });

        public string GetLogLineSynchronizationProgressUpdated(int downloaded, int updated) =>
            FormatLogLine(section => section?.SynchronizationProgressUpdated,
                new { downloaded = Formatter.ToFormattedString(downloaded), updated = Formatter.ToFormattedString(updated) });

        public string GetLogLineSynchronizationProgressAddedAndUpdated(int downloaded, int added, int updated) =>
            FormatLogLine(section => section?.SynchronizationProgressAddedAndUpdated,
                new { downloaded = Formatter.ToFormattedString(downloaded), added = Formatter.ToFormattedString(added),
                    updated = Formatter.ToFormattedString(updated) });

        public string GetLogLineSynchronizationError(string error) => FormatLogLine(section => section?.SynchronizationError, new { error });

        private string FormatStatus(Func<Translation.SynchronizationStatusMessagesTranslation, string> field, object templateArguments = null) =>
            Format(translation => field(translation?.Synchronization?.StatusMessages), templateArguments);

        private string FormatLogLine(Func<Translation.SynchronizationLogMessagesTranslation, string> field, object templateArguments = null) =>
            Format(translation => field(translation?.Synchronization?.LogMessages), templateArguments);
    }
}
