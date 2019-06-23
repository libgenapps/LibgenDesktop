using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.Settings;
using static LibgenDesktop.Common.Constants;
using static LibgenDesktop.Models.Settings.AppSettings;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class SettingsWindowViewModel : LibgenWindowViewModel, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, string> errors;
        private bool isGeneralTabSelected;
        private bool isNetworkTabSelected;
        private bool isDownloadTabSelected;
        private bool isMirrorsTabSelected;
        private bool isSearchTabSelected;
        private bool isExportTabSelected;
        private bool isAdvancedTabSelected;
        private Dictionary<string, Language> generalLanguagesList;
        private KeyValuePair<string, Language> generalSelectedLanguage;
        private Dictionary<GeneralSettings.UpdateCheckInterval, string> generalUpdateCheckIntervalList;
        private KeyValuePair<GeneralSettings.UpdateCheckInterval, string> generalSelectedUpdateCheckInterval;
        private bool networkIsOfflineModeOn;
        private bool networkUseProxy;
        private string networkProxyAddress;
        private string networkProxyPort;
        private string networkProxyUserName;
        private string networkProxyPassword;
        private bool downloadIsOpenInBrowserSelected;
        private bool downloadIsUseDownloadManagerSelected;
        private string downloadDirectory;
        private ObservableCollection<string> downloadTimeoutDefaultValues;
        private string downloadTimeout;
        private ObservableCollection<string> downloadAttemptCountDefaultValues;
        private string downloadAttemptCount;
        private ObservableCollection<string> downloadRetryDelayDefaultValues;
        private string downloadRetryDelay;
        private ObservableCollection<string> mirrorsNonFictionBooksMirrorList;
        private string mirrorsSelectedNonFictionBooksMirror;
        private ObservableCollection<string> mirrorsNonFictionCoversMirrorList;
        private string mirrorsSelectedNonFictionCoversMirror;
        private ObservableCollection<string> mirrorsNonFictionSynchronizationMirrorList;
        private string mirrorsSelectedNonFictionSynchronizationMirror;
        private ObservableCollection<string> mirrorsFictionBooksMirrorList;
        private string mirrorsSelectedFictionBooksMirror;
        private ObservableCollection<string> mirrorsFictionCoversMirrorList;
        private string mirrorsSelectedFictionCoversMirror;
        private ObservableCollection<string> mirrorsArticlesMirrorList;
        private string mirrorsSelectedArticlesMirror;
        private bool searchIsLimitResultsOn;
        private ObservableCollection<string> searchMaximumResultCountDefaultValues;
        private string searchMaximumResultCount;
        private bool searchIsOpenInModalWindowSelected;
        private bool searchIsOpenInNonModalWindowSelected;
        private bool searchIsOpenInNewTabSelected;
        private bool exportIsOpenResultsAfterExportEnabled;
        private bool exportIsSplitIntoMultipleFilesEnabled;
        private ObservableCollection<string> exportMaximumRowsPerFileDefaultValues;
        private string exportMaximumRowsPerFile;
        private bool advancedIsLoggingEnabled;
        private bool advancedIsSqlDebuggerEnabled;
        private bool isOkButtonEnabled;
        private bool settingsChanged;

        public SettingsWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            errors = new Dictionary<string, string>();
            Localization = mainModel.Localization.CurrentLanguage.Settings;
            DownloadSelectDirectoryCommand = new Command(DownloadSelectDirectory);
            OkCommand = new Command(OkButtonClick);
            CancelCommand = new Command(CancelButtonClick);
            WindowClosingCommand = new FuncCommand<bool>(WindowClosing);
            Initialize();
        }

        public SettingsWindowLocalizator Localization { get; }

        public bool IsGeneralTabSelected
        {
            get
            {
                return isGeneralTabSelected;
            }
            set
            {
                isGeneralTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsNetworkTabSelected
        {
            get
            {
                return isNetworkTabSelected;
            }
            set
            {
                isNetworkTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDownloadTabSelected
        {
            get
            {
                return isDownloadTabSelected;
            }
            set
            {
                isDownloadTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsMirrorsTabSelected
        {
            get
            {
                return isMirrorsTabSelected;
            }
            set
            {
                isMirrorsTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSearchTabSelected
        {
            get
            {
                return isSearchTabSelected;
            }
            set
            {
                isSearchTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsExportTabSelected
        {
            get
            {
                return isExportTabSelected;
            }
            set
            {
                isExportTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsAdvancedTabSelected
        {
            get
            {
                return isAdvancedTabSelected;
            }
            set
            {
                isAdvancedTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<string, Language> GeneralLanguagesList
        {
            get
            {
                return generalLanguagesList;
            }
            set
            {
                generalLanguagesList = value;
                NotifyPropertyChanged();
            }
        }

        public KeyValuePair<string, Language> GeneralSelectedLanguage
        {
            get
            {
                return generalSelectedLanguage;
            }
            set
            {
                generalSelectedLanguage = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public Dictionary<GeneralSettings.UpdateCheckInterval, string> GeneralUpdateCheckIntervalList
        {
            get
            {
                return generalUpdateCheckIntervalList;
            }
            set
            {
                generalUpdateCheckIntervalList = value;
                NotifyPropertyChanged();
            }
        }

        public KeyValuePair<GeneralSettings.UpdateCheckInterval, string> GeneralSelectedUpdateCheckInterval
        {
            get
            {
                return generalSelectedUpdateCheckInterval;
            }
            set
            {
                generalSelectedUpdateCheckInterval = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool NetworkIsOfflineModeOn
        {
            get
            {
                return networkIsOfflineModeOn;
            }
            set
            {
                networkIsOfflineModeOn = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool NetworkUseProxy
        {
            get
            {
                return networkUseProxy;
            }
            set
            {
                networkUseProxy = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public string NetworkProxyAddress
        {
            get
            {
                return networkProxyAddress;
            }
            set
            {
                networkProxyAddress = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public string NetworkProxyPort
        {
            get
            {
                return networkProxyPort;
            }
            set
            {
                networkProxyPort = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public string NetworkProxyUserName
        {
            get
            {
                return networkProxyUserName;
            }
            set
            {
                networkProxyUserName = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public string NetworkProxyPassword
        {
            get
            {
                return networkProxyPassword;
            }
            set
            {
                networkProxyPassword = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool DownloadIsOpenInBrowserSelected
        {
            get
            {
                return downloadIsOpenInBrowserSelected;
            }
            set
            {
                downloadIsOpenInBrowserSelected = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public bool DownloadIsUseDownloadManagerSelected
        {
            get
            {
                return downloadIsUseDownloadManagerSelected;
            }
            set
            {
                downloadIsUseDownloadManagerSelected = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public string DownloadDirectory
        {
            get
            {
                return downloadDirectory;
            }
            set
            {
                downloadDirectory = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }
                                   
        public ObservableCollection<string> DownloadTimeoutDefaultValues
        {
            get
            {
                return downloadTimeoutDefaultValues;
            }
            set
            {
                downloadTimeoutDefaultValues = value;
                NotifyPropertyChanged();
            }
        }

        public string DownloadTimeout
        {
            get
            {
                return downloadTimeout;
            }
            set
            {
                downloadTimeout = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public ObservableCollection<string> DownloadAttemptCountDefaultValues
        {
            get
            {
                return downloadAttemptCountDefaultValues;
            }
            set
            {
                downloadAttemptCountDefaultValues = value;
                NotifyPropertyChanged();
            }
        }

        public string DownloadAttemptCount
        {
            get
            {
                return downloadAttemptCount;
            }
            set
            {
                downloadAttemptCount = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public ObservableCollection<string> DownloadRetryDelayDefaultValues
        {
            get
            {
                return downloadRetryDelayDefaultValues;
            }
            set
            {
                downloadRetryDelayDefaultValues = value;
                NotifyPropertyChanged();
            }
        }

        public string DownloadRetryDelay
        {
            get
            {
                return downloadRetryDelay;
            }
            set
            {
                downloadRetryDelay = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public ObservableCollection<string> MirrorsNonFictionBooksMirrorList
        {
            get
            {
                return mirrorsNonFictionBooksMirrorList;
            }
            set
            {
                mirrorsNonFictionBooksMirrorList = value;
                NotifyPropertyChanged();
            }
        }

        public string MirrorsSelectedNonFictionBooksMirror
        {
            get
            {
                return mirrorsSelectedNonFictionBooksMirror;
            }
            set
            {
                mirrorsSelectedNonFictionBooksMirror = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public ObservableCollection<string> MirrorsNonFictionCoversMirrorList
        {
            get
            {
                return mirrorsNonFictionCoversMirrorList;
            }
            set
            {
                mirrorsNonFictionCoversMirrorList = value;
                NotifyPropertyChanged();
            }
        }

        public string MirrorsSelectedNonFictionCoversMirror
        {
            get
            {
                return mirrorsSelectedNonFictionCoversMirror;
            }
            set
            {
                mirrorsSelectedNonFictionCoversMirror = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public ObservableCollection<string> MirrorsNonFictionSynchronizationMirrorList
        {
            get
            {
                return mirrorsNonFictionSynchronizationMirrorList;
            }
            set
            {
                mirrorsNonFictionSynchronizationMirrorList = value;
                NotifyPropertyChanged();
            }
        }

        public string MirrorsSelectedNonFictionSynchronizationMirror
        {
            get
            {
                return mirrorsSelectedNonFictionSynchronizationMirror;
            }
            set
            {
                mirrorsSelectedNonFictionSynchronizationMirror = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public ObservableCollection<string> MirrorsFictionBooksMirrorList
        {
            get
            {
                return mirrorsFictionBooksMirrorList;
            }
            set
            {
                mirrorsFictionBooksMirrorList = value;
                NotifyPropertyChanged();
            }
        }

        public string MirrorsSelectedFictionBooksMirror
        {
            get
            {
                return mirrorsSelectedFictionBooksMirror;
            }
            set
            {
                mirrorsSelectedFictionBooksMirror = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public ObservableCollection<string> MirrorsFictionCoversMirrorList
        {
            get
            {
                return mirrorsFictionCoversMirrorList;
            }
            set
            {
                mirrorsFictionCoversMirrorList = value;
                NotifyPropertyChanged();
            }
        }

        public string MirrorsSelectedFictionCoversMirror
        {
            get
            {
                return mirrorsSelectedFictionCoversMirror;
            }
            set
            {
                mirrorsSelectedFictionCoversMirror = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public ObservableCollection<string> MirrorsArticlesMirrorList
        {
            get
            {
                return mirrorsArticlesMirrorList;
            }
            set
            {
                mirrorsArticlesMirrorList = value;
                NotifyPropertyChanged();
            }
        }

        public string MirrorsSelectedArticlesMirror
        {
            get
            {
                return mirrorsSelectedArticlesMirror;
            }
            set
            {
                mirrorsSelectedArticlesMirror = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool SearchIsLimitResultsOn
        {
            get
            {
                return searchIsLimitResultsOn;
            }
            set
            {
                searchIsLimitResultsOn = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public ObservableCollection<string> SearchMaximumResultCountDefaultValues
        {
            get
            {
                return searchMaximumResultCountDefaultValues;
            }
            set
            {
                searchMaximumResultCountDefaultValues = value;
                NotifyPropertyChanged();
            }
        }

        public string SearchMaximumResultCount
        {
            get
            {
                return searchMaximumResultCount;
            }
            set
            {
                searchMaximumResultCount = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public bool SearchIsOpenInModalWindowSelected
        {
            get
            {
                return searchIsOpenInModalWindowSelected;
            }
            set
            {
                searchIsOpenInModalWindowSelected = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool SearchIsOpenInNonModalWindowSelected
        {
            get
            {
                return searchIsOpenInNonModalWindowSelected;
            }
            set
            {
                searchIsOpenInNonModalWindowSelected = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool SearchIsOpenInNewTabSelected
        {
            get
            {
                return searchIsOpenInNewTabSelected;
            }
            set
            {
                searchIsOpenInNewTabSelected = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool ExportIsOpenResultsAfterExportEnabled
        {
            get
            {
                return exportIsOpenResultsAfterExportEnabled;
            }
            set
            {
                exportIsOpenResultsAfterExportEnabled = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool ExportIsSplitIntoMultipleFilesEnabled
        {
            get
            {
                return exportIsSplitIntoMultipleFilesEnabled;
            }
            set
            {
                exportIsSplitIntoMultipleFilesEnabled = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public ObservableCollection<string> ExportMaximumRowsPerFileDefaultValues
        {
            get
            {
                return exportMaximumRowsPerFileDefaultValues;
            }
            set
            {
                exportMaximumRowsPerFileDefaultValues = value;
                NotifyPropertyChanged();
            }
        }

        public string ExportMaximumRowsPerFile
        {
            get
            {
                return exportMaximumRowsPerFile;
            }
            set
            {
                exportMaximumRowsPerFile = value;
                NotifyPropertyChanged();
                settingsChanged = true;
                Validate();
            }
        }

        public string ExportExcelLimitNote
        {
            get
            {
                return Localization.GetExportExcelLimitNote(MAX_EXPORT_ROWS_PER_FILE + 1);
            }
        }

        public bool AdvancedIsLoggingEnabled
        {
            get
            {
                return advancedIsLoggingEnabled;
            }
            set
            {
                advancedIsLoggingEnabled = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool AdvancedIsSqlDebuggerEnabled
        {
            get
            {
                return advancedIsSqlDebuggerEnabled;
            }
            set
            {
                advancedIsSqlDebuggerEnabled = value;
                NotifyPropertyChanged();
                settingsChanged = true;
            }
        }

        public bool IsOkButtonEnabled
        {
            get
            {
                return isOkButtonEnabled;
            }
            set
            {
                isOkButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool HasErrors => errors.Any();

        public Command DownloadSelectDirectoryCommand { get; }
        public Command OkCommand { get; }
        public Command CancelCommand { get; }
        public FuncCommand<bool> WindowClosingCommand { get; }

        private int? NetworkProxyPortValue
        {
            get
            {
                if (Int32.TryParse(NetworkProxyPort, out int value))
                {
                    if (value >= MIN_PROXY_PORT && value <= MAX_PROXY_PORT)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        private int? DownloadTimeoutValue
        {
            get
            {
                if (Int32.TryParse(DownloadTimeout, out int value))
                {
                    if (value >= MIN_DOWNLOAD_TIMEOUT && value <= MAX_DOWNLOAD_TIMEOUT)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        private int? DownloadAttemptCountValue
        {
            get
            {
                if (Int32.TryParse(DownloadAttemptCount, out int value))
                {
                    if (value >= 1 && value <= MAX_DOWNLOAD_ATTEMPT_COUNT)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        private int? DownloadRetryDelayValue
        {
            get
            {
                if (Int32.TryParse(DownloadRetryDelay, out int value))
                {
                    if (value >= 0 && value <= MAX_DOWNLOAD_RETRY_DELAY)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        private int? SearchMaximumResultCountValue
        {
            get
            {
                if (Int32.TryParse(SearchMaximumResultCount, out int value))
                {
                    if (value > 0)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        private int? ExportMaximumRowsPerFileValue
        {
            get
            {
                if (Int32.TryParse(ExportMaximumRowsPerFile, out int value))
                {
                    if (value > 0 && value <= MAX_EXPORT_ROWS_PER_FILE)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (errors.TryGetValue(propertyName, out string error))
            {
                yield return error;
            }
        }

        private void Initialize()
        {
            AppSettings appSettings = MainModel.AppSettings;
            settingsChanged = false;
            isGeneralTabSelected = true;
            isNetworkTabSelected = false;
            isDownloadTabSelected = false;
            isMirrorsTabSelected = false;
            isSearchTabSelected = false;
            isExportTabSelected = false;
            isAdvancedTabSelected = false;
            generalLanguagesList = MainModel.Localization.Languages.ToDictionary(language => language.DisplayName);
            generalSelectedLanguage = generalLanguagesList.First(language => language.Value == MainModel.Localization.CurrentLanguage);
            generalUpdateCheckIntervalList = new Dictionary<GeneralSettings.UpdateCheckInterval, string>
            {
                { GeneralSettings.UpdateCheckInterval.NEVER, Localization.GeneralUpdateCheckIntervalNever },
                { GeneralSettings.UpdateCheckInterval.DAILY, Localization.GeneralUpdateCheckIntervalDaily },
                { GeneralSettings.UpdateCheckInterval.WEEKLY, Localization.GeneralUpdateCheckIntervalWeekly },
                { GeneralSettings.UpdateCheckInterval.MONTHLY, Localization.GeneralUpdateCheckIntervalMonthly }
            };
            generalSelectedUpdateCheckInterval = generalUpdateCheckIntervalList.First(interval => interval.Key == appSettings.General.UpdateCheck);
            networkIsOfflineModeOn = appSettings.Network.OfflineMode;
            networkUseProxy = appSettings.Network.UseProxy;
            networkProxyAddress = appSettings.Network.ProxyAddress;
            networkProxyPort = appSettings.Network.ProxyPort?.ToString() ?? String.Empty;
            networkProxyUserName = appSettings.Network.ProxyUserName;
            networkProxyPassword = appSettings.Network.ProxyPassword;
            downloadIsOpenInBrowserSelected = !appSettings.Download.UseDownloadManager;
            downloadIsUseDownloadManagerSelected = !downloadIsOpenInBrowserSelected;
            downloadDirectory = appSettings.Download.DownloadDirectory;
            downloadTimeoutDefaultValues = new ObservableCollection<string> { "30", "60", "90", "120", "180", "300", "600", "1200" };
            downloadTimeout = appSettings.Download.Timeout.ToString();
            downloadAttemptCountDefaultValues = new ObservableCollection<string> { "0", "1", "2", "3", "5", "10", "15", "20" };
            downloadAttemptCount = appSettings.Download.Attempts.ToString();
            downloadRetryDelayDefaultValues = new ObservableCollection<string> { "60", "120", "180", "300", "600", "1200" };
            downloadRetryDelay = appSettings.Download.RetryDelay.ToString();
            mirrorsNonFictionBooksMirrorList = BuildMirrorList(mirror => mirror.NonFictionDownloadUrl);
            mirrorsNonFictionCoversMirrorList = BuildMirrorList(mirror => mirror.NonFictionCoverUrl);
            mirrorsNonFictionSynchronizationMirrorList = BuildMirrorList(mirror => mirror.NonFictionSynchronizationUrl);
            mirrorsFictionBooksMirrorList = BuildMirrorList(mirror => mirror.FictionDownloadUrl);
            mirrorsFictionCoversMirrorList = BuildMirrorList(mirror => mirror.FictionCoverUrl);
            mirrorsArticlesMirrorList = BuildMirrorList(mirror => mirror.SciMagDownloadUrl);
            mirrorsSelectedNonFictionBooksMirror = GetDisplayMirrorName(appSettings.Mirrors.NonFictionBooksMirrorName);
            mirrorsSelectedNonFictionCoversMirror = GetDisplayMirrorName(appSettings.Mirrors.NonFictionCoversMirrorName);
            mirrorsSelectedNonFictionSynchronizationMirror = GetDisplayMirrorName(appSettings.Mirrors.NonFictionSynchronizationMirrorName);
            mirrorsSelectedFictionBooksMirror = GetDisplayMirrorName(appSettings.Mirrors.FictionBooksMirrorName);
            mirrorsSelectedFictionCoversMirror = GetDisplayMirrorName(appSettings.Mirrors.FictionCoversMirrorName);
            mirrorsSelectedArticlesMirror = GetDisplayMirrorName(appSettings.Mirrors.ArticlesMirrorName);
            searchMaximumResultCountDefaultValues = new ObservableCollection<string> { "100", "250", "500", "1000", "2500", "5000", "10000", "25000", "50000", "100000", "250000", "500000", "1000000" };
            searchIsLimitResultsOn = appSettings.Search.LimitResults;
            searchMaximumResultCount = appSettings.Search.MaximumResultCount.ToString();
            searchIsOpenInModalWindowSelected = false;
            searchIsOpenInNonModalWindowSelected = false;
            searchIsOpenInNewTabSelected = false;
            switch (appSettings.Search.OpenDetailsMode)
            {
                case SearchSettings.DetailsMode.NEW_MODAL_WINDOW:
                    searchIsOpenInModalWindowSelected = true;
                    break;
                case SearchSettings.DetailsMode.NEW_NON_MODAL_WINDOW:
                    searchIsOpenInNonModalWindowSelected = true;
                    break;
                case SearchSettings.DetailsMode.NEW_TAB:
                    searchIsOpenInNewTabSelected = true;
                    break;
            }
            exportIsOpenResultsAfterExportEnabled = appSettings.Export.OpenResultsAfterExport;
            exportIsSplitIntoMultipleFilesEnabled = appSettings.Export.SplitIntoMultipleFiles;
            exportMaximumRowsPerFileDefaultValues = new ObservableCollection<string> { "50000", "100000", "250000", "500000", "1000000" };
            exportMaximumRowsPerFile = appSettings.Export.MaximumRowsPerFile.ToString();
            advancedIsLoggingEnabled = appSettings.Advanced.LoggingEnabled;
            advancedIsSqlDebuggerEnabled = appSettings.Advanced.SqlDebuggerEnabled;
            Validate();
        }

        private ObservableCollection<string> BuildMirrorList(Func<Mirrors.MirrorConfiguration, string> mirrorProperty)
        {
            return new ObservableCollection<string>(new[] { Localization.MirrorsNoMirror }.
                Concat(MainModel.Mirrors.Where(mirror => !String.IsNullOrWhiteSpace(mirrorProperty(mirror.Value))).Select(mirror => mirror.Key)));
        }

        private void Validate()
        {
            bool networkProxyAddressValid = !NetworkUseProxy || !String.IsNullOrWhiteSpace(NetworkProxyAddress);
            bool networkProxyPortValid = !NetworkUseProxy || NetworkProxyPortValue.HasValue;
            bool downloadDirectoryValid = DownloadIsOpenInBrowserSelected ||
                (!String.IsNullOrWhiteSpace(DownloadDirectory) && Directory.Exists(DownloadDirectory));
            bool downloadTimeoutValid = DownloadIsOpenInBrowserSelected || DownloadTimeoutValue.HasValue;
            bool downloadAttemptCountValid = DownloadIsOpenInBrowserSelected || DownloadAttemptCountValue.HasValue;
            bool downloadRetryDelayValid = DownloadIsOpenInBrowserSelected || DownloadRetryDelayValue.HasValue;
            bool searchMaximumResultCountValid = !SearchIsLimitResultsOn || SearchMaximumResultCountValue.HasValue;
            bool exportMaximumRowsPerFileValid = !ExportIsSplitIntoMultipleFilesEnabled || ExportMaximumRowsPerFileValue.HasValue;
            UpdateValidationState(nameof(NetworkProxyAddress), networkProxyAddressValid, Localization.NetworkProxyAddressRequired);
            UpdateValidationState(nameof(NetworkProxyPort), networkProxyPortValid, Localization.GetNetworkProxyPortValidation(MIN_PROXY_PORT, MAX_PROXY_PORT));
            UpdateValidationState(nameof(DownloadDirectory), downloadDirectoryValid, Localization.DownloadDownloadDirectoryNotFound);
            UpdateValidationState(nameof(DownloadTimeout), downloadTimeoutValid,
                Localization.GetDownloadTimeoutValidation(MIN_DOWNLOAD_TIMEOUT, MAX_DOWNLOAD_TIMEOUT));
            UpdateValidationState(nameof(DownloadAttemptCount), downloadAttemptCountValid,
                Localization.GetDownloadDownloadAttemptsValidation(1, MAX_DOWNLOAD_ATTEMPT_COUNT));
            UpdateValidationState(nameof(DownloadRetryDelay), downloadRetryDelayValid,
                Localization.GetDownloadRetryDelayValidation(0, MAX_DOWNLOAD_RETRY_DELAY));
            UpdateValidationState(nameof(SearchMaximumResultCount), searchMaximumResultCountValid, Localization.SearchPositiveNumbersOnly);
            UpdateValidationState(nameof(ExportMaximumRowsPerFile), exportMaximumRowsPerFileValid,
                Localization.GetExportMaximumRowsPerFileValidation(1, MAX_EXPORT_ROWS_PER_FILE));
            IsOkButtonEnabled = networkProxyAddressValid && networkProxyPortValid && downloadDirectoryValid && downloadTimeoutValid &&
                downloadAttemptCountValid && downloadRetryDelayValid && searchMaximumResultCountValid && exportMaximumRowsPerFileValid;
        }

        private void UpdateValidationState(string propertyName, bool isValid, string errorText)
        {
            if (isValid && errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else if (!isValid && !errors.ContainsKey(propertyName))
            {
                errors.Add(propertyName, errorText);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void DownloadSelectDirectory()
        {
            SelectFolderDialogParameters selectFolderDialogParameters = new SelectFolderDialogParameters
            {
                DialogTitle = Localization.DownloadBrowseDirectoryDialogTitle,
                InitialDirectory = DownloadDirectory
            };
            SelectFolderDialogResult selectFolderDialogResult = WindowManager.ShowSelectFolderDialog(selectFolderDialogParameters);
            if (selectFolderDialogResult.DialogResult)
            {
                DownloadDirectory = selectFolderDialogResult.SelectedFolderPath;
            }
        }

        private void OkButtonClick()
        {
            MainModel.AppSettings.General = new GeneralSettings
            {
                Language = GeneralSelectedLanguage.Value.Name,
                UpdateCheck = GeneralSelectedUpdateCheckInterval.Key
            };
            MainModel.AppSettings.Network = new NetworkSettings
            {
                OfflineMode = NetworkIsOfflineModeOn,
                UseProxy = NetworkUseProxy,
                ProxyAddress = NetworkProxyAddress,
                ProxyPort = NetworkProxyPortValue,
                ProxyUserName = NetworkProxyUserName ?? String.Empty,
                ProxyPassword = NetworkProxyPassword ?? String.Empty
            };
            MainModel.AppSettings.Download = new DownloadSettings
            {
                UseDownloadManager = DownloadIsUseDownloadManagerSelected,
                DownloadDirectory = DownloadDirectory,
                Timeout = DownloadTimeoutValue ?? DEFAULT_DOWNLOAD_TIMEOUT,
                Attempts = DownloadAttemptCountValue ?? DEFAULT_DOWNLOAD_ATTEMPT_COUNT,
                RetryDelay = DownloadRetryDelayValue ?? DEFAULT_DOWNLOAD_RETRY_DELAY
            };
            MainModel.AppSettings.Mirrors = new MirrorSettings
            {
                NonFictionBooksMirrorName = ParseDisplayMirrorName(MirrorsSelectedNonFictionBooksMirror),
                NonFictionCoversMirrorName = ParseDisplayMirrorName(MirrorsSelectedNonFictionCoversMirror),
                NonFictionSynchronizationMirrorName = ParseDisplayMirrorName(MirrorsSelectedNonFictionSynchronizationMirror),
                FictionBooksMirrorName = ParseDisplayMirrorName(MirrorsSelectedFictionBooksMirror),
                FictionCoversMirrorName = ParseDisplayMirrorName(MirrorsSelectedFictionCoversMirror),
                ArticlesMirrorName = ParseDisplayMirrorName(MirrorsSelectedArticlesMirror)
            };
            MainModel.AppSettings.Search = new SearchSettings
            {
                LimitResults = SearchIsLimitResultsOn,
                MaximumResultCount = SearchMaximumResultCountValue ?? DEFAULT_MAXIMUM_SEARCH_RESULT_COUNT
            };
            if (SearchIsOpenInModalWindowSelected)
            {
                MainModel.AppSettings.Search.OpenDetailsMode = SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
            }
            else if (SearchIsOpenInNonModalWindowSelected)
            {
                MainModel.AppSettings.Search.OpenDetailsMode = SearchSettings.DetailsMode.NEW_NON_MODAL_WINDOW;
            }
            else if (SearchIsOpenInNewTabSelected)
            {
                MainModel.AppSettings.Search.OpenDetailsMode = SearchSettings.DetailsMode.NEW_TAB;
            }
            MainModel.AppSettings.Export = new ExportSettings
            {
                OpenResultsAfterExport = ExportIsOpenResultsAfterExportEnabled,
                SplitIntoMultipleFiles = ExportIsSplitIntoMultipleFilesEnabled,
                MaximumRowsPerFile = ExportMaximumRowsPerFileValue ?? MAX_EXPORT_ROWS_PER_FILE
            };
            if (advancedIsLoggingEnabled != MainModel.AppSettings.Advanced.LoggingEnabled)
            {
                MainModel.AppSettings.Advanced.LoggingEnabled = advancedIsLoggingEnabled;
                if (advancedIsLoggingEnabled)
                {
                    MainModel.EnableLogging();
                }
                else
                {
                    MainModel.DisableLogging();
                }
            }
            MainModel.AppSettings.Advanced.SqlDebuggerEnabled = AdvancedIsSqlDebuggerEnabled;
            MainModel.SaveSettings();
            MainModel.ReconfigureSettingsDependencies();
            settingsChanged = false;
            CurrentWindowContext.CloseDialog(true);
        }

        private void CancelButtonClick()
        {
            CurrentWindowContext.CloseDialog(true);
        }

        private string GetDisplayMirrorName(string mirrorName)
        {
            return mirrorName ?? Localization.MirrorsNoMirror;
        }

        private string ParseDisplayMirrorName(string displayMirrorName)
        {
            return displayMirrorName != Localization.MirrorsNoMirror ? displayMirrorName : null;
        }

        private bool WindowClosing()
        {
            return !settingsChanged || ShowPrompt(Localization.DiscardChangesPromptTitle, Localization.DiscardChangesPromptText);
        }
    }
}
