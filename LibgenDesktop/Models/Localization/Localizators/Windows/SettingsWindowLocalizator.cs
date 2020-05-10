using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class SettingsWindowLocalizator : Localizator<Translation.SettingsTranslation>
    {
        public SettingsWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Settings)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            Ok = Format(section => section?.Ok);
            Cancel = Format(section => section?.Cancel);
            DiscardChangesPromptTitle = Format(section => section?.DiscardChangesPromptTitle);
            DiscardChangesPromptText = Format(section => section?.DiscardChangesPromptText);
            GeneralTabHeader = Format(section => section?.General?.TabHeader);
            GeneralLanguage = Format(section => section?.General?.Language);
            GeneralCheckUpdates = Format(section => section?.General?.CheckUpdates);
            GeneralUpdateCheckIntervalNever = Format(section => section?.General?.UpdateCheckIntervals?.Never);
            GeneralUpdateCheckIntervalDaily = Format(section => section?.General?.UpdateCheckIntervals?.Daily);
            GeneralUpdateCheckIntervalWeekly = Format(section => section?.General?.UpdateCheckIntervals?.Weekly);
            GeneralUpdateCheckIntervalMonthly = Format(section => section?.General?.UpdateCheckIntervals?.Monthly);
            NetworkTabHeader = Format(section => section?.Network?.TabHeader);
            NetworkOfflineMode = Format(section => section?.Network?.OfflineMode);
            NetworkUseHttpProxy = Format(section => section?.Network?.UseHttpProxy);
            NetworkProxyAddress = Format(section => section?.Network?.ProxyAddress);
            NetworkProxyAddressRequired = Format(section => section?.Network?.ProxyAddressRequired);
            NetworkProxyPort = Format(section => section?.Network?.ProxyPort);
            NetworkProxyUserName = Format(section => section?.Network?.ProxyUserName);
            NetworkProxyPassword = Format(section => section?.Network?.ProxyPassword);
            NetworkProxyPasswordWarning = Format(section => section?.Network?.ProxyPasswordWarning);
            DownloadTabHeader = Format(section => section?.Download?.TabHeader);
            DownloadDownloadMode = Format(section => section?.Download?.DownloadMode) + ":";
            DownloadOpenInBrowser = Format(section => section?.Download?.OpenInBrowser);
            DownloadUseDownloadManager = Format(section => section?.Download?.UseDownloadManager);
            DownloadDownloadDirectory = Format(section => section?.Download?.DownloadDirectory) + ":";
            DownloadBrowseDirectoryDialogTitle = Format(section => section?.Download?.BrowseDirectoryDialogTitle);
            DownloadDownloadDirectoryNotFound = Format(section => section?.Download?.DownloadDirectoryNotFound);
            DownloadTimeout = Format(section => section?.Download?.Timeout);
            DownloadSeconds = Format(section => section?.Download?.Seconds);
            DownloadDownloadAttempts = Format(section => section?.Download?.DownloadAttempts);
            DownloadTimes = Format(section => section?.Download?.Times);
            DownloadRetryDelay = Format(section => section?.Download?.RetryDelay);
            MirrorsTabHeader = Format(section => section?.Mirrors?.TabHeader);
            MirrorsNonFiction = Format(section => section?.Mirrors?.NonFiction);
            MirrorsFiction = Format(section => section?.Mirrors?.Fiction);
            MirrorsSciMagArticles = Format(section => section?.Mirrors?.SciMagArticles);
            MirrorsBooks = Format(section => section?.Mirrors?.Books);
            MirrorsArticles = Format(section => section?.Mirrors?.Articles);
            MirrorsCovers = Format(section => section?.Mirrors?.Covers);
            MirrorsSynchronization = Format(section => section?.Mirrors?.Synchronization);
            MirrorsNoMirror = Format(section => section?.Mirrors?.NoMirror);
            SearchTabHeader = Format(section => section?.Search?.TabHeader);
            SearchLimitResults = Format(section => section?.Search?.LimitResults);
            SearchMaximumResults = Format(section => section?.Search?.MaximumResults);
            SearchPositiveNumbersOnly = Format(section => section?.Search?.PositiveNumbersOnly);
            SearchOpenDetails = Format(section => section?.Search?.OpenDetails) + ":";
            SearchInModalWindow = Format(section => section?.Search?.InModalWindow);
            SearchInNonModalWindow = Format(section => section?.Search?.InNonModalWindow);
            SearchInNewTab = Format(section => section?.Search?.InNewTab);
            ExportTabHeader = Format(section => section?.Export?.TabHeader);
            ExportOpenResults = Format(section => section?.Export?.OpenResults);
            ExportSplitIntoMultipleFiles = Format(section => section?.Export?.SplitIntoMultipleFiles);
            ExportMaximumRowsPerFile = Format(section => section?.Export?.MaximumRowsPerFile);
            AdvancedTabHeader = Format(section => section?.Advanced?.TabHeader);
            AdvancedUseLogging = Format(section => section?.Advanced?.UseLogging);
            AdvancedEnableSqlDebugger = Format(section => section?.Advanced?.EnableSqlDebugger);
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

        public string GetGeneralPercentTranslated(decimal percent)
        {
            percent = Decimal.Truncate(percent * 10) / 10;
            return Format(section => section?.General?.PercentTranslated, new { percent = Formatter.ToFormattedString(percent) });
        }

        public string GetNetworkProxyPortValidation(int min, int max) =>
            Format(section => section?.Network?.ProxyPortValidation,
                new { min = Formatter.ToDecimalFormattedString(min), max = Formatter.ToDecimalFormattedString(max) });

        public string GetDownloadTimeoutValidation(int min, int max) =>
            Format(section => section?.Download?.TimeoutValidation,
                new { min = Formatter.ToDecimalFormattedString(min), max = Formatter.ToDecimalFormattedString(max) });

        public string GetDownloadDownloadAttemptsValidation(int min, int max) =>
            Format(section => section?.Download?.DownloadAttemptsValidation,
                new { min = Formatter.ToDecimalFormattedString(min), max = Formatter.ToDecimalFormattedString(max) });

        public string GetDownloadRetryDelayValidation(int min, int max) =>
            Format(section => section?.Download?.RetryDelayValidation,
                new { min = Formatter.ToDecimalFormattedString(min), max = Formatter.ToDecimalFormattedString(max) });

        public string GetExportMaximumRowsPerFileValidation(int min, int max) =>
            Format(section => section?.Export?.MaximumRowsPerFileValidation,
                new { min = Formatter.ToDecimalFormattedString(min), max = Formatter.ToDecimalFormattedString(max) });

        public string GetExportExcelLimitNote(int count) =>
            Format(section => section?.Export?.ExcelLimitNote, new { count = Formatter.ToFormattedString(count) });
    }
}
