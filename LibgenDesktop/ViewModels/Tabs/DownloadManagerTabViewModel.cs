using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Localization.Localizators;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal class DownloadManagerTabViewModel : TabViewModel
    {
        internal class DownloadItemViewModel : ViewModel
        {
            private string name;
            private DownloadItemStatus status;
            private string progressText;
            private double progressValue;
            private bool isSelected;
            private ObservableCollection<DownloadItemLogLineViewModel> logs;

            public DownloadItemViewModel(Guid id, string name, DownloadItemStatus status, string progressText, double progressValue,
                string downloadDirectory, IEnumerable<DownloadItemLogLineViewModel> logs)
            {
                Id = id;
                this.name = name;
                this.status = status;
                this.progressText = progressText;
                this.progressValue = progressValue;
                isSelected = false;
                DownloadDirectory = downloadDirectory;
                this.logs = new ObservableCollection<DownloadItemLogLineViewModel>(logs);
            }

            public Guid Id { get; }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }

            public DownloadItemStatus Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                    NotifyPropertyChanged();
                }
            }

            public string ProgressText
            {
                get
                {
                    return progressText;
                }
                set
                {
                    progressText = value;
                    NotifyPropertyChanged();
                }
            }

            public double ProgressValue
            {
                get
                {
                    return progressValue;
                }
                set
                {
                    progressValue = value;
                    NotifyPropertyChanged();
                }
            }

            public bool IsSelected
            {
                get
                {
                    return isSelected;
                }
                set
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                }
            }

            public ObservableCollection<DownloadItemLogLineViewModel> Logs
            {
                get
                {
                    return logs;
                }
                set
                {
                    logs = value;
                    NotifyPropertyChanged();
                }
            }

            public string DownloadDirectory { get; }
        }

        internal class DownloadItemLogLineViewModel : ViewModel
        {
            private DownloadItemLogLineType type;
            private string timeStamp;
            private string text;

            public DownloadItemLogLineViewModel(DownloadItemLogLineType type, string timeStamp, string text)
            {
                this.type = type;
                this.timeStamp = timeStamp;
                this.text = text;
            }

            public DownloadItemLogLineType Type
            {
                get
                {
                    return type;
                }
                set
                {
                    type = value;
                    NotifyPropertyChanged();
                }
            }

            public string TimeStamp
            {
                get
                {
                    return timeStamp;
                }
                set
                {
                    timeStamp = value;
                    NotifyPropertyChanged();
                }
            }

            public string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private readonly Dictionary<Guid, DownloadItemViewModel> downloadDictionary;
        private DownloadManagerLocalizator localization;
        private bool isStartButtonEnabled;
        private bool isStopButtonEnabled;
        private bool isRemoveButtonEnabled;
        private bool isStartAllButtonEnabled;
        private bool isStopAllButtonEnabled;
        private bool isRemoveAllCompletedButtonEnabled;
        private int logPanelHeight;
        private bool showDebugDownloadLogs;
        private ObservableCollection<DownloadItemViewModel> downloads;
        private DownloadItemViewModel selectedDownload;
        private ObservableCollection<DownloadItemLogLineViewModel> selectedDownloadLogs;
        private bool isSelectionChangedHandlerEnabled;

        public DownloadManagerTabViewModel(MainModel mainModel, IWindowContext parentWindowContext)
            : base(mainModel, parentWindowContext, mainModel.Localization.CurrentLanguage.DownloadManager.TabTitle)
        {
            localization = mainModel.Localization.CurrentLanguage.DownloadManager;
            Downloads = new ObservableCollection<DownloadItemViewModel>(mainModel.Downloader.GetDownloadQueueSnapshot().Select(ToDownloadItemViewModel));
            downloadDictionary = Downloads.ToDictionary(downloadItem => downloadItem.Id);
            SelectionChangedCommand = new Command(param => SelectionChangedHandler(param as SelectionChangedCommandArgs));
            DownloaderListBoxDoubleClickCommand = new Command(DownloaderListBoxDoubleClick);
            StartSelectedDownloadsCommand = new Command(StartSelectedDownloads);
            StopSelectedDownloadsCommand = new Command(StopSelectedDownloads);
            RemoveSelectedDownloadsCommand = new Command(RemoveSelectedDownloads);
            StartAllDownloadsCommand = new Command(StartAllDownloads);
            StopAllDownloadsCommand = new Command(StopAllDownloads);
            RemoveAllCompletedDownloadsCommand = new Command(RemoveAllCompletedDownloads);
            CopyDownloadLogCommand = new Command(CopyDownloadLog);
            Initialize();
            mainModel.Downloader.DownloaderBatchEvent += DownloaderBatchEvent;
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
        }

        public DownloadManagerLocalizator Localization
        {
            get
            {
                return localization;
            }
            set
            {
                localization = value;
                NotifyPropertyChanged();
            }
        }

        public IList SelectedRows { get; set; }

        public bool IsStartButtonEnabled
        {
            get
            {
                return isStartButtonEnabled;
            }
            set
            {
                isStartButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsStopButtonEnabled
        {
            get
            {
                return isStopButtonEnabled;
            }
            set
            {
                isStopButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsRemoveButtonEnabled
        {
            get
            {
                return isRemoveButtonEnabled;
            }
            set
            {
                isRemoveButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsStartAllButtonEnabled
        {
            get
            {
                return isStartAllButtonEnabled;
            }
            set
            {
                isStartAllButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsStopAllButtonEnabled
        {
            get
            {
                return isStopAllButtonEnabled;
            }
            set
            {
                isStopAllButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsRemoveAllCompletedButtonEnabled
        {
            get
            {
                return isRemoveAllCompletedButtonEnabled;
            }
            set
            {
                isRemoveAllCompletedButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public int LogPanelHeight
        {
            get
            {
                return logPanelHeight;
            }
            set
            {
                logPanelHeight = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShowDebugDownloadLogs
        {
            get
            {
                return showDebugDownloadLogs;
            }
            set
            {
                showDebugDownloadLogs = value;
                NotifyPropertyChanged();
            }
        }

        public DownloadItemViewModel SelectedDownload
        {
            get
            {
                return selectedDownload;
            }
            set
            {
                selectedDownload = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<DownloadItemLogLineViewModel> SelectedDownloadLogs
        {
            get
            {
                return selectedDownloadLogs;
            }
            set
            {
                selectedDownloadLogs = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<DownloadItemViewModel> Downloads
        {
            get
            {
                return downloads;
            }
            set
            {
                downloads = value;
                NotifyPropertyChanged();
            }
        }

        public Command SelectionChangedCommand { get; }
        public Command DownloaderListBoxDoubleClickCommand { get; }
        public Command StartSelectedDownloadsCommand { get; }
        public Command StopSelectedDownloadsCommand { get; }
        public Command RemoveSelectedDownloadsCommand { get; }
        public Command StartAllDownloadsCommand { get; }
        public Command StopAllDownloadsCommand { get; }
        public Command RemoveAllCompletedDownloadsCommand { get; }
        public Command CopyDownloadLogCommand { get; }

        private IEnumerable<DownloadItemViewModel> SelectedDownloads
        {
            get
            {
                return SelectedRows.OfType<DownloadItemViewModel>();
            }
        }

        public void SelectDownload(Guid downloadId)
        {
            ExecuteInUiThread(() =>
            {
                DownloadItemViewModel downloadItemToSelect = Downloads.FirstOrDefault(downloadItemViewModel => downloadItemViewModel.Id == downloadId);
                if (downloadItemToSelect != null)
                {
                    SelectedDownload = downloadItemToSelect;
                    Events.RaiseEvent(ViewModelEvent.RegisteredEventId.SCROLL_TO_SELECTION);
                }
            });
        }

        public override void HandleTabClosing()
        {
            MainModel.Downloader.DownloaderBatchEvent -= DownloaderBatchEvent;
            MainModel.Localization.LanguageChanged -= LocalizationLanguageChanged;
        }

        private void Initialize()
        {
            SelectedRows = null;
            isStartButtonEnabled = false;
            isStopButtonEnabled = false;
            isRemoveButtonEnabled = false;
            isStartAllButtonEnabled = false;
            isStopAllButtonEnabled = false;
            isRemoveAllCompletedButtonEnabled = false;
            logPanelHeight = MainModel.AppSettings.DownloadManagerTab.LogPanelHeight;
            showDebugDownloadLogs = MainModel.AppSettings.DownloadManagerTab.ShowDebugLogs;
            selectedDownloadLogs = null;
            isSelectionChangedHandlerEnabled = true;
            UpdateNonSelectionButtonStates();
        }

        private void DownloaderBatchEvent(object sender, DownloaderBatchEventArgs e)
        {
            if (e.AddEventCount + e.RemoveEventCount < LARGE_DOWNLOADER_BATCH_UPDATE_ITEM_COUNT)
            {
                ExecuteInUiThread(() =>
                {
                    isSelectionChangedHandlerEnabled = false;
                    ApplyDownloaderBatchEventArgs(Downloads, e, out bool selectionButtonStatesUpdateRequired, out bool nonSelectionButtonStatesUpdateRequired);
                    if (selectionButtonStatesUpdateRequired)
                    {
                        UpdateSelectionButtonStates();
                        UpdateSelectedDownloadLogs();
                    }
                    if (nonSelectionButtonStatesUpdateRequired)
                    {
                        UpdateNonSelectionButtonStates();
                    }
                    isSelectionChangedHandlerEnabled = true;
                });
            }
            else
            {
                ObservableCollection<DownloadItemViewModel> newDownloads = new ObservableCollection<DownloadItemViewModel>(Downloads);
                ApplyDownloaderBatchEventArgs(newDownloads, e, out bool selectionButtonStatesUpdateRequired, out bool nonSelectionButtonStatesUpdateRequired);
                ExecuteInUiThread(() =>
                {
                    Downloads = newDownloads;
                    if (selectionButtonStatesUpdateRequired)
                    {
                        UpdateSelectionButtonStates();
                        UpdateSelectedDownloadLogs();
                    }
                    if (nonSelectionButtonStatesUpdateRequired)
                    {
                        UpdateNonSelectionButtonStates();
                    }
                });
            }
        }

        private void ApplyDownloaderBatchEventArgs(ObservableCollection<DownloadItemViewModel> downloads, DownloaderBatchEventArgs downloaderBatchEventArgs,
            out bool selectionButtonStatesUpdateRequired, out bool nonSelectionButtonStatesUpdateRequired)
        {
            selectionButtonStatesUpdateRequired = false;
            nonSelectionButtonStatesUpdateRequired = false;
            foreach (DownloadItemEventArgs downloadItemEventArgs in downloaderBatchEventArgs.BatchEvents)
            {
                switch (downloadItemEventArgs)
                {
                    case DownloadItemAddedEventArgs downloadItemAddedEvent:
                        DownloadItemViewModel newDownloadItemViewModel = ToDownloadItemViewModel(downloadItemAddedEvent.AddedDownloadItem);
                        downloadDictionary[newDownloadItemViewModel.Id] = newDownloadItemViewModel;
                        downloads.Add(newDownloadItemViewModel);
                        nonSelectionButtonStatesUpdateRequired = true;
                        break;
                    case DownloadItemChangedEventArgs downloadItemChangedEvent:
                        DownloadItem changedDownloadItem = downloadItemChangedEvent.ChangedDownloadItem;
                        DownloadItemViewModel changedDownloadItemViewModel = downloadDictionary[changedDownloadItem.Id];
                        bool statusChanged = changedDownloadItemViewModel.Status != changedDownloadItem.Status;
                        changedDownloadItemViewModel.Name = changedDownloadItem.FileName;
                        if (statusChanged)
                        {
                            changedDownloadItemViewModel.Status = changedDownloadItem.Status;
                        }
                        changedDownloadItemViewModel.ProgressText = GetDownloadProgressText(changedDownloadItem);
                        changedDownloadItemViewModel.ProgressValue = GetDownloadProgressValue(changedDownloadItem);
                        if (statusChanged)
                        {
                            if (changedDownloadItemViewModel.IsSelected)
                            {
                                selectionButtonStatesUpdateRequired = true;
                            }
                            nonSelectionButtonStatesUpdateRequired = true;
                        }
                        break;
                    case DownloadItemRemovedEventArgs downloadItemRemovedEvent:
                        DownloadItem removedDownloadItem = downloadItemRemovedEvent.RemovedDownloadItem;
                        DownloadItemViewModel removedDownloadItemViewModel = downloadDictionary[removedDownloadItem.Id];
                        bool wasSelected = removedDownloadItemViewModel.IsSelected;
                        downloads.Remove(removedDownloadItemViewModel);
                        if (wasSelected)
                        {
                            selectionButtonStatesUpdateRequired = true;
                        }
                        nonSelectionButtonStatesUpdateRequired = true;
                        break;
                    case DownloadItemLogLineEventArgs downloadItemLogLineEvent:
                        DownloadItemViewModel downloadItemViewModel = downloadDictionary[downloadItemLogLineEvent.DownloadItemId];
                        if (downloadItemLogLineEvent.LineIndex >= downloadItemViewModel.Logs.Count)
                        {
                            downloadItemViewModel.Logs.Add(ToDownloadItemLogLineViewModel(downloadItemLogLineEvent.LogLine));
                        }
                        break;
                }
            }
        }

        private void SelectionChangedHandler(SelectionChangedCommandArgs selectionChangedCommandArgs)
        {
            if (isSelectionChangedHandlerEnabled)
            {
                if (selectionChangedCommandArgs != null)
                {
                    if (selectionChangedCommandArgs.AddedItems != null)
                    {
                        foreach (DownloadItemViewModel selectedDownloadItem in selectionChangedCommandArgs.AddedItems.OfType<DownloadItemViewModel>())
                        {
                            selectedDownloadItem.IsSelected = true;
                        }
                    }
                    if (selectionChangedCommandArgs.RemovedItems != null)
                    {
                        foreach (DownloadItemViewModel unselectedDownloadItem in selectionChangedCommandArgs.RemovedItems.OfType<DownloadItemViewModel>())
                        {
                            unselectedDownloadItem.IsSelected = false;
                        }
                    }
                }
                UpdateSelectionButtonStates();
                UpdateSelectedDownloadLogs();
            }
        }

        private void UpdateSelectionButtonStates()
        {
            bool enableStartButton = false;
            bool enableStopButton = false;
            bool enableRemoveButton = false;
            foreach (DownloadItemViewModel downloadItemViewModel in SelectedDownloads)
            {
                enableRemoveButton = true;
                if (CanBeStarted(downloadItemViewModel.Status))
                {
                    enableStartButton = true;
                }
                else if (CanBeStopped(downloadItemViewModel.Status))
                {
                    enableStopButton = true;
                }
                if (enableStartButton && enableStopButton)
                {
                    break;
                }
            }
            IsStartButtonEnabled = enableStartButton;
            IsStopButtonEnabled = enableStopButton;
            IsRemoveButtonEnabled = enableRemoveButton;
        }

        private void UpdateSelectedDownloadLogs()
        {
            if (SelectedRows.Count == 1)
            {
                SelectedDownloadLogs = SelectedDownloads.First().Logs;
            }
            else
            {
                SelectedDownloadLogs = null;
            }
        }

        private void UpdateNonSelectionButtonStates()
        {
            bool enableStartAllButton = false;
            bool enableStopAllButton = false;
            bool enableRemoveAllCompletedButton = false;
            foreach (DownloadItemViewModel downloadItemViewModel in Downloads)
            {
                if (CanBeStarted(downloadItemViewModel.Status))
                {
                    enableStartAllButton = true;
                }
                else if (CanBeStopped(downloadItemViewModel.Status))
                {
                    enableStopAllButton = true;
                }
                else if (IsCompleted(downloadItemViewModel.Status))
                {
                    enableRemoveAllCompletedButton = true;
                }
                if (enableStartAllButton && enableStopAllButton && enableRemoveAllCompletedButton)
                {
                    break;
                }
            }
            IsStartAllButtonEnabled = enableStartAllButton;
            IsStopAllButtonEnabled = enableStopAllButton;
            IsRemoveAllCompletedButtonEnabled = enableRemoveAllCompletedButton;
        }

        private void DownloaderListBoxDoubleClick()
        {
            if (SelectedRows.Count == 1)
            {
                DownloadItemViewModel selectedDownload = SelectedDownloads.First();
                if (selectedDownload.Status == DownloadItemStatus.COMPLETED)
                {
                    string downloadedFilePath = Path.Combine(selectedDownload.DownloadDirectory, selectedDownload.Name);
                    if (File.Exists(downloadedFilePath))
                    {
                        Process.Start(downloadedFilePath);
                    }
                    else
                    {
                        ShowMessage(Localization.FileNotFoundErrorTitle, Localization.GetFileNotFoundErrorText(downloadedFilePath));
                    }
                }
            }
        }

        private void StartSelectedDownloads()
        {
            MainModel.Downloader.StartDownloads(SelectedDownloads.Where(downloadItem => CanBeStarted(downloadItem.Status)).
                Select(downloadItem => downloadItem.Id));
        }

        private void StopSelectedDownloads()
        {
            MainModel.Downloader.StopDownloads(SelectedDownloads.Where(downloadItem => CanBeStopped(downloadItem.Status)).
                Select(downloadItem => downloadItem.Id));
        }

        private void RemoveSelectedDownloads()
        {
            MainModel.Downloader.RemoveDownloads(SelectedDownloads.Select(downloadItem => downloadItem.Id));
        }

        private void StartAllDownloads()
        {
            MainModel.Downloader.StartDownloads(Downloads.Where(downloadItem => CanBeStarted(downloadItem.Status)).Select(downloadItem => downloadItem.Id));
        }

        private void StopAllDownloads()
        {
            MainModel.Downloader.StopDownloads(Downloads.Where(downloadItem => CanBeStopped(downloadItem.Status)).Select(downloadItem => downloadItem.Id));
        }

        private void RemoveAllCompletedDownloads()
        {
            MainModel.Downloader.RemoveDownloads(Downloads.Where(downloadItem => IsCompleted(downloadItem.Status)).Select(downloadItem => downloadItem.Id));
        }

        private void CopyDownloadLog()
        {
            if (SelectedDownloadLogs != null)
            {
                StringBuilder clipboardTextBuilder = new StringBuilder();
                foreach (DownloadItemLogLineViewModel logLine in SelectedDownloadLogs)
                {
                    if (logLine.Type != DownloadItemLogLineType.DEBUG || ShowDebugDownloadLogs)
                    {
                        clipboardTextBuilder.Append("[");
                        clipboardTextBuilder.Append(logLine.TimeStamp);
                        clipboardTextBuilder.Append("] ");
                        clipboardTextBuilder.AppendLine(logLine.Text);
                    }
                }
                WindowManager.SetClipboardText(clipboardTextBuilder.ToString());
            }
        }

        private DownloadItemViewModel ToDownloadItemViewModel(DownloadItem downloadItem)
        {
            return new DownloadItemViewModel(downloadItem.Id, downloadItem.FileName, downloadItem.Status,
                GetDownloadProgressText(downloadItem), GetDownloadProgressValue(downloadItem), downloadItem.DownloadDirectory,
                downloadItem.Logs.Select(downloadItemLogLine => ToDownloadItemLogLineViewModel(downloadItemLogLine)));
        }

        private string GetDownloadProgressText(DownloadItem downloadItem)
        {
            if (downloadItem.DownloadedFileSize.HasValue)
            {
                if (downloadItem.TotalFileSize.HasValue)
                {
                    int percentage = (int)Math.Truncate(GetDownloadProgressValue(downloadItem));
                    return Localization.GetDownloadProgressKnownFileSize(downloadItem.DownloadedFileSize.Value, downloadItem.TotalFileSize.Value, percentage);
                }
                else if (downloadItem.Status == DownloadItemStatus.COMPLETED)
                {
                    return Localization.GetDownloadProgressKnownFileSize(downloadItem.DownloadedFileSize.Value, downloadItem.DownloadedFileSize.Value, 100);
                }
                else
                {
                    return Localization.GetDownloadProgressUnknownFileSize(downloadItem.DownloadedFileSize.Value);
                }
            }
            else
            {
                switch (downloadItem.Status)
                {
                    case DownloadItemStatus.QUEUED:
                        return Localization.QueuedStatus;
                    case DownloadItemStatus.DOWNLOADING:
                        return Localization.DownloadingStatus;
                    case DownloadItemStatus.STOPPED:
                        return Localization.StoppedStatus;
                    case DownloadItemStatus.RETRY_DELAY:
                        return Localization.RetryDelayStatus;
                    case DownloadItemStatus.ERROR:
                        return Localization.ErrorStatus;
                    default:
                        throw new Exception($"Unexpected download item status: {downloadItem.Status}.");
                }
            }
        }

        private double GetDownloadProgressValue(DownloadItem downloadItem)
        {
            if (downloadItem.DownloadedFileSize.HasValue && downloadItem.TotalFileSize.HasValue)
            {
                return (double)downloadItem.DownloadedFileSize.Value * 100 / downloadItem.TotalFileSize.Value;
            }
            else if (downloadItem.Status == DownloadItemStatus.COMPLETED)
            {
                return 100;
            }
            else
            {
                return 0;
            }
        }

        private DownloadItemLogLineViewModel ToDownloadItemLogLineViewModel(DownloadItemLogLine downloadItemLogLine)
        {
            return new DownloadItemLogLineViewModel(downloadItemLogLine.Type,
                MainModel.Localization.CurrentLanguage.Formatter.ToFormattedTimeString(downloadItemLogLine.TimeStamp), downloadItemLogLine.Text);
        }

        private bool CanBeStarted(DownloadItemStatus downloadItemStatus)
        {
            return downloadItemStatus == DownloadItemStatus.STOPPED || downloadItemStatus == DownloadItemStatus.ERROR;
        }

        private bool CanBeStopped(DownloadItemStatus downloadItemStatus)
        {
            return downloadItemStatus == DownloadItemStatus.QUEUED || downloadItemStatus == DownloadItemStatus.DOWNLOADING ||
                downloadItemStatus == DownloadItemStatus.RETRY_DELAY;
        }

        private bool IsCompleted(DownloadItemStatus downloadItemStatus)
        {
            return downloadItemStatus == DownloadItemStatus.COMPLETED;
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            Localization = MainModel.Localization.CurrentLanguage.DownloadManager;
            Title = Localization.TabTitle;
            foreach (DownloadItem downloadItem in MainModel.Downloader.GetDownloadQueueSnapshot())
            {
                if (downloadDictionary.TryGetValue(downloadItem.Id, out DownloadItemViewModel downloadItemViewModel))
                {
                    downloadItemViewModel.ProgressText = GetDownloadProgressText(downloadItem);
                }
            }
        }
    }
}
