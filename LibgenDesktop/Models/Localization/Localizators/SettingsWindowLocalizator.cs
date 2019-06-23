using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SettingsWindowLocalizator : Localizator
    {
        public SettingsWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            Ok = Format(translation => translation?.Ok);
            Cancel = Format(translation => translation?.Cancel);
            DiscardChangesPromptTitle = Format(translation => translation?.DiscardChangesPromptTitle);
            DiscardChangesPromptText = Format(translation => translation?.DiscardChangesPromptText);
            GeneralTabHeader = Format(translation => translation?.General?.TabHeader);
            GeneralLanguage = Format(translation => translation?.General?.Language);
            GeneralCheckUpdates = Format(translation => translation?.General?.CheckUpdates);
            GeneralUpdateCheckIntervalNever = Format(translation => translation?.General?.UpdateCheckIntervals?.Never);
            GeneralUpdateCheckIntervalDaily = Format(translation => translation?.General?.UpdateCheckIntervals?.Daily);
            GeneralUpdateCheckIntervalWeekly = Format(translation => translation?.General?.UpdateCheckIntervals?.Weekly);
            GeneralUpdateCheckIntervalMonthly = Format(translation => translation?.General?.UpdateCheckIntervals?.Monthly);
            NetworkTabHeader = Format(translation => translation?.Network?.TabHeader);
            NetworkOfflineMode = Format(translation => translation?.Network?.OfflineMode);
            NetworkUseHttpProxy = Format(translation => translation?.Network?.UseHttpProxy);
            NetworkProxyAddress = Format(translation => translation?.Network?.ProxyAddress);
            NetworkProxyAddressRequired = Format(translation => translation?.Network?.ProxyAddressRequired);
            NetworkProxyPort = Format(translation => translation?.Network?.ProxyPort);
            NetworkProxyUserName = Format(translation => translation?.Network?.ProxyUserName);
            NetworkProxyPassword = Format(translation => translation?.Network?.ProxyPassword);
            NetworkProxyPasswordWarning = Format(translation => translation?.Network?.ProxyPasswordWarning);
            DownloadTabHeader = Format(translation => translation?.Download?.TabHeader);
            DownloadDownloadMode = Format(translation => translation?.Download?.DownloadMode) + ":";
            DownloadOpenInBrowser = Format(translation => translation?.Download?.OpenInBrowser);
            DownloadUseDownloadManager = Format(translation => translation?.Download?.UseDownloadManager);
            DownloadDownloadDirectory = Format(translation => translation?.Download?.DownloadDirectory) + ":";
            DownloadBrowseDirectoryDialogTitle = Format(translation => translation?.Download?.BrowseDirectoryDialogTitle);
            DownloadDownloadDirectoryNotFound = Format(translation => translation?.Download?.DownloadDirectoryNotFound);
            DownloadTimeout = Format(translation => translation?.Download?.Timeout);
            DownloadSeconds = Format(translation => translation?.Download?.Seconds);
            DownloadDownloadAttempts = Format(translation => translation?.Download?.DownloadAttempts);
            DownloadTimes = Format(translation => translation?.Download?.Times);
            DownloadRetryDelay = Format(translation => translation?.Download?.RetryDelay);
            MirrorsTabHeader = Format(translation => translation?.Mirrors?.TabHeader);
            MirrorsNonFiction = Format(translation => translation?.Mirrors?.NonFiction);
            MirrorsFiction = Format(translation => translation?.Mirrors?.Fiction);
            MirrorsSciMagArticles = Format(translation => translation?.Mirrors?.SciMagArticles);
            MirrorsBooks = Format(translation => translation?.Mirrors?.Books);
            MirrorsArticles = Format(translation => translation?.Mirrors?.Articles);
            MirrorsCovers = Format(translation => translation?.Mirrors?.Covers);
            MirrorsSynchronization = Format(translation => translation?.Mirrors?.Synchronization);
            MirrorsNoMirror = Format(translation => translation?.Mirrors?.NoMirror);
            SearchTabHeader = Format(translation => translation?.Search?.TabHeader);
            SearchLimitResults = Format(translation => translation?.Search?.LimitResults);
            SearchMaximumResults = Format(translation => translation?.Search?.MaximumResults);
            SearchPositiveNumbersOnly = Format(translation => translation?.Search?.PositiveNumbersOnly);
            SearchOpenDetails = Format(translation => translation?.Search?.OpenDetails) + ":";
            SearchInModalWindow = Format(translation => translation?.Search?.InModalWindow);
            SearchInNonModalWindow = Format(translation => translation?.Search?.InNonModalWindow);
            SearchInNewTab = Format(translation => translation?.Search?.InNewTab);
            ExportTabHeader = Format(translation => translation?.Export?.TabHeader);
            ExportOpenResults = Format(translation => translation?.Export?.OpenResults);
            ExportSplitIntoMultipleFiles = Format(translation => translation?.Export?.SplitIntoMultipleFiles);
            ExportMaximumRowsPerFile = Format(translation => translation?.Export?.MaximumRowsPerFile);
            AdvancedTabHeader = Format(translation => translation?.Advanced?.TabHeader);
            AdvancedUseLogging = Format(translation => translation?.Advanced?.UseLogging);
            AdvancedEnableSqlDebugger = Format(translation => translation?.Advanced?.EnableSqlDebugger);
        }

        public string WindowTitle { get; }
        public string Ok { get; }
        public string Cancel { get; }
        public string DiscardChangesPromptTitle { get; }
        public string DiscardChangesPromptText { get; }
        public string GeneralTabHeader { get; }
        public string GeneralLanguage { get; }
        public string GeneralCheckUpdates { get; }
        public string GeneralUpdateCheckIntervalNever { get; }
        public string GeneralUpdateCheckIntervalDaily { get; }
        public string GeneralUpdateCheckIntervalWeekly { get; }
        public string GeneralUpdateCheckIntervalMonthly { get; }
        public string NetworkTabHeader { get; }
        public string NetworkOfflineMode { get; }
        public string NetworkUseHttpProxy { get; }
        public string NetworkProxyAddress { get; }
        public string NetworkProxyAddressRequired { get; }
        public string NetworkProxyPort { get; }
        public string NetworkProxyUserName { get; }
        public string NetworkProxyPassword { get; }
        public string NetworkProxyPasswordWarning { get; }
        public string DownloadTabHeader { get; }
        public string DownloadDownloadMode { get; }
        public string DownloadOpenInBrowser { get; }
        public string DownloadUseDownloadManager { get; }
        public string DownloadDownloadDirectory { get; }
        public string DownloadBrowseDirectoryDialogTitle { get; }
        public string DownloadDownloadDirectoryNotFound { get; }
        public string DownloadTimeout { get; }
        public string DownloadSeconds { get; }
        public string DownloadDownloadAttempts { get; }
        public string DownloadTimes { get; }
        public string DownloadRetryDelay { get; }
        public string MirrorsTabHeader { get; }
        public string MirrorsNonFiction { get; }
        public string MirrorsFiction { get; }
        public string MirrorsSciMagArticles { get; }
        public string MirrorsBooks { get; }
        public string MirrorsArticles { get; }
        public string MirrorsCovers { get; }
        public string MirrorsSynchronization { get; }
        public string MirrorsNoMirror { get; }
        public string SearchTabHeader { get; }
        public string SearchLimitResults { get; }
        public string SearchMaximumResults { get; }
        public string SearchPositiveNumbersOnly { get; }
        public string SearchOpenDetails { get; }
        public string SearchInModalWindow { get; }
        public string SearchInNonModalWindow { get; }
        public string SearchInNewTab { get; }
        public string ExportTabHeader { get; }
        public string ExportOpenResults { get; }
        public string ExportSplitIntoMultipleFiles { get; }
        public string ExportMaximumRowsPerFile { get; }
        public string AdvancedTabHeader { get; }
        public string AdvancedUseLogging { get; }
        public string AdvancedEnableSqlDebugger { get; }

        public string GetNetworkProxyPortValidation(int min, int max) => Format(translation => translation?.Network?.ProxyPortValidation, new { min, max });
        public string GetDownloadTimeoutValidation(int min, int max) => Format(translation => translation?.Download?.TimeoutValidation, new { min, max });
        public string GetDownloadDownloadAttemptsValidation(int min, int max) =>
            Format(translation => translation?.Download?.DownloadAttemptsValidation, new { min, max });
        public string GetDownloadRetryDelayValidation(int min, int max) => Format(translation => translation?.Download?.RetryDelayValidation, new { min, max });
        public string GetExportMaximumRowsPerFileValidation(int min, int max) =>
            Format(translation => translation?.Export?.MaximumRowsPerFileValidation, new { min, max });
        public string GetExportExcelLimitNote(int count) =>
            Format(translation => translation?.Export?.ExcelLimitNote, new { count = Formatter.ToFormattedString(count) });

        private string Format(Func<Translation.SettingsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Settings), templateArguments);
        }
    }
}
