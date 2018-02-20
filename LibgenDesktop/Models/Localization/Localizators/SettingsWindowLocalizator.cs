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
        }

        public string WindowTitle { get; set; }
        public string Ok { get; set; }
        public string Cancel { get; set; }
        public string DiscardChangesPromptTitle { get; set; }
        public string DiscardChangesPromptText { get; set; }
        public string GeneralTabHeader { get; set; }
        public string GeneralLanguage { get; set; }
        public string GeneralCheckUpdates { get; set; }
        public string GeneralUpdateCheckIntervalNever { get; set; }
        public string GeneralUpdateCheckIntervalDaily { get; set; }
        public string GeneralUpdateCheckIntervalWeekly { get; set; }
        public string GeneralUpdateCheckIntervalMonthly { get; set; }
        public string NetworkTabHeader { get; set; }
        public string NetworkOfflineMode { get; set; }
        public string NetworkUseHttpProxy { get; set; }
        public string NetworkProxyAddress { get; set; }
        public string NetworkProxyAddressRequired { get; set; }
        public string NetworkProxyPort { get; set; }
        public string NetworkProxyUserName { get; set; }
        public string NetworkProxyPassword { get; set; }
        public string NetworkProxyPasswordWarning { get; set; }
        public string DownloadTabHeader { get; set; }
        public string DownloadDownloadMode { get; set; }
        public string DownloadOpenInBrowser { get; set; }
        public string DownloadUseDownloadManager { get; set; }
        public string DownloadDownloadDirectory { get; set; }
        public string DownloadBrowseDirectoryDialogTitle { get; set; }
        public string DownloadDownloadDirectoryNotFound { get; set; }
        public string DownloadTimeout { get; set; }
        public string DownloadSeconds { get; set; }
        public string DownloadDownloadAttempts { get; set; }
        public string DownloadTimes { get; set; }
        public string DownloadRetryDelay { get; set; }
        public string MirrorsTabHeader { get; set; }
        public string MirrorsNonFiction { get; set; }
        public string MirrorsFiction { get; set; }
        public string MirrorsSciMagArticles { get; set; }
        public string MirrorsBooks { get; set; }
        public string MirrorsArticles { get; set; }
        public string MirrorsCovers { get; set; }
        public string MirrorsSynchronization { get; set; }
        public string MirrorsNoMirror { get; set; }
        public string SearchTabHeader { get; set; }
        public string SearchLimitResults { get; set; }
        public string SearchMaximumResults { get; set; }
        public string SearchPositiveNumbersOnly { get; set; }
        public string SearchOpenDetails { get; set; }
        public string SearchInModalWindow { get; set; }
        public string SearchInNonModalWindow { get; set; }
        public string SearchInNewTab { get; set; }
        public string ExportTabHeader { get; set; }
        public string ExportOpenResults { get; set; }
        public string ExportSplitIntoMultipleFiles { get; set; }
        public string ExportMaximumRowsPerFile { get; set; }
        public string AdvancedTabHeader { get; set; }
        public string AdvancedUseLogging { get; set; }

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
