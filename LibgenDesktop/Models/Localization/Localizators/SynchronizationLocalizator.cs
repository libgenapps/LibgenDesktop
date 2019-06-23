using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SynchronizationLocalizator : Localizator
    {
        public SynchronizationLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            ErrorMessageTitle = Format(translation => translation?.ErrorMessageTitle);
            ImportRequired = Format(translation => translation?.ImportRequired);
            NoSynchronizationMirror = Format(translation => translation?.NoSynchronizationMirror);
            OfflineModePromptTitle = Format(translation => translation?.OfflineModePromptTitle);
            OfflineModePromptText = Format(translation => translation?.OfflineModePromptText);
            Unknown = Format(translation => translation?.Unknown);
            Interrupt = Format(translation => translation?.Interrupt);
            Interrupting = Format(translation => translation?.Interrupting);
            Close = Format(translation => translation?.Close);
            StatusPreparation = FormatStatus(translation => translation?.Preparation);
            StatusCreatingIndexes = FormatStatus(translation => translation?.CreatingIndexes);
            StatusLoadingIds = FormatStatus(translation => translation?.LoadingIds);
            StatusSynchronizingData = FormatStatus(translation => translation?.SynchronizingData);
            StatusSynchronizationComplete = FormatStatus(translation => translation?.SynchronizationComplete);
            StatusSynchronizationCancelled = FormatStatus(translation => translation?.SynchronizationCancelled);
            StatusSynchronizationError = FormatStatus(translation => translation?.SynchronizationError);
            LogLineCreatingIndexes = FormatLogLine(translation => translation?.CreatingIndexes);
            LogLineLoadingIds = FormatLogLine(translation => translation?.LoadingIds);
            LogLineSynchronizingBookList = FormatLogLine(translation => translation?.SynchronizingBookList);
            LogLineDownloadingNewBooks = FormatLogLine(translation => translation?.DownloadingNewBooks);
            LogLineSynchronizationSuccessful = FormatLogLine(translation => translation?.SynchronizationSuccessful);
            LogLineInsufficientDiskSpace = FormatLogLine(translation => translation?.InsufficientDiskSpace);
            LogLineSynchronizationCancelled = FormatLogLine(translation => translation?.SynchronizationCancelled);
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

        public string GetElapsedString(string elapsed) => Format(translation => translation?.Elapsed, new { elapsed });
        public string GetFreeSpaceString(string freeSpace) => Format(translation => translation?.FreeSpace, new { freeSpace });
        public string GetStatusStep(int current, int total) => FormatStatus(translation => translation?.Step, new { current, total });
        public string GetLogLineStep(int step) => FormatLogLine(translation => translation?.Step, new { step });
        public string GetLogLineCreatingIndexForColumn(string column) => FormatLogLine(translation => translation?.CreatingIndexForColumn, new { column });
        public string GetLogLineLoadingColumnValues(string column) => FormatLogLine(translation => translation?.LoadingColumnValues, new { column });
        public string GetLogLineSynchronizationProgressNoAddedNoUpdated(int downloaded) =>
            FormatLogLine(translation => translation?.SynchronizationProgressNoAddedNoUpdated, new { downloaded });
        public string GetLogLineSynchronizationProgressAdded(int downloaded, int added) =>
            FormatLogLine(translation => translation?.SynchronizationProgressAdded, new { downloaded, added });
        public string GetLogLineSynchronizationProgressUpdated(int downloaded, int updated) =>
            FormatLogLine(translation => translation?.SynchronizationProgressUpdated, new { downloaded, updated });
        public string GetLogLineSynchronizationProgressAddedAndUpdated(int downloaded, int added, int updated) =>
            FormatLogLine(translation => translation?.SynchronizationProgressAddedAndUpdated, new { downloaded, added, updated });
        public string GetLogLineSynchronizationError(string error) => FormatLogLine(translation => translation?.SynchronizationError, new { error });

        private string Format(Func<Translation.SynchronizationTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Synchronization), templateArguments);
        }

        private string FormatStatus(Func<Translation.SynchronizationStatusMessagesTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Synchronization?.StatusMessages), templateArguments);
        }

        private string FormatLogLine(Func<Translation.SynchronizationLogMessagesTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Synchronization?.LogMessages), templateArguments);
        }
    }
}
