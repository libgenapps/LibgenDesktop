using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.Models.ProgressArgs;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class DownloadDumpsPageViewModel : SetupStepViewModel
    {
        internal class DownloadItemViewModel : ViewModel
        {
            private DownloadDumpsSetupStepLocalizator localization;
            private string name;
            private DownloadItemStatus status;
            private long? downloadedSize;
            private long? totalSize;
            private string progressText;
            private double progressValue;

            public DownloadItemViewModel(SharedSetupContext.Collection collection, DownloadDumpsSetupStepLocalizator localization)
            {
                this.localization = localization;
                Collection = collection;
                name = GetDownloadName(collection.Identifier);
                status = collection.DownloadStatus;
                downloadedSize = collection.DownloadedSize;
                totalSize = collection.TotalSize;
                UpdateProgressTextAndValue();
            }

            public SharedSetupContext.Collection Collection { get; }

            public string Name
            {
                get
                {
                    return name;
                }
                private set
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
                private set
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
                private set
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
                private set
                {
                    progressValue = value;
                    NotifyPropertyChanged();
                }
            }

            public void UpdateLocalization(DownloadDumpsSetupStepLocalizator localization)
            {
                this.localization = localization;
                Name = GetDownloadName(Collection.Identifier);
                UpdateProgressTextAndValue();
            }

            public void UpdateProgress(long newDownloadedSize, long newTotalSize)
            {
                downloadedSize = newDownloadedSize;
                totalSize = newTotalSize;
                UpdateProgressTextAndValue();
            }

            public void UpdateStatus(DownloadItemStatus newStatus)
            {
                Status = newStatus;
                if (newStatus == DownloadItemStatus.COMPLETED && !totalSize.HasValue)
                {
                    totalSize = 0;
                }
                UpdateProgressTextAndValue();
            }

            public void SaveStateToCollection()
            {
                Collection.DownloadStatus = Status;
                Collection.DownloadedSize = downloadedSize;
                Collection.TotalSize = totalSize;
            }

            private static double GetDownloadProgressValue(DownloadItemStatus status, long? downloadedSize, long? totalSize)
            {
                if (status == DownloadItemStatus.COMPLETED)
                {
                    return 100;
                }
                else if (downloadedSize.HasValue && totalSize.HasValue)
                {
                    return (double)downloadedSize.Value * 100 / totalSize.Value;
                }
                else
                {
                    return 0;
                }
            }

            private string GetDownloadName(SharedSetupContext.CollectionIdentifier collectionIdentifier)
            {
                switch (collectionIdentifier)
                {
                    case SharedSetupContext.CollectionIdentifier.NON_FICTION:
                        return localization.NonFictionDumpName;
                    case SharedSetupContext.CollectionIdentifier.FICTION:
                        return localization.FictionDumpName;
                    case SharedSetupContext.CollectionIdentifier.SCIMAG:
                        return localization.SciMagArticlesDumpName;
                    default:
                        throw new Exception($"Unexpected collection identifier: {collectionIdentifier}.");
                }
            }

            private void UpdateProgressTextAndValue()
            {
                ProgressValue = GetDownloadProgressValue(status, downloadedSize, totalSize);
                ProgressText = GetDownloadProgressText(status, downloadedSize, totalSize);
            }

            private string GetDownloadProgressText(DownloadItemStatus downloadStatus, long? downloadedSize, long? totalSize)
            {
                if (downloadStatus == DownloadItemStatus.COMPLETED && totalSize.HasValue)
                {
                    return localization.GetDownloadProgress(totalSize.Value, totalSize.Value, 100);
                }
                else if (downloadedSize.HasValue && totalSize.HasValue)
                {
                    int percentage = (int)Math.Truncate(GetDownloadProgressValue(status, downloadedSize, totalSize));
                    return localization.GetDownloadProgress(downloadedSize.Value, totalSize.Value, percentage);
                }
                else
                {
                    switch (downloadStatus)
                    {
                        case DownloadItemStatus.QUEUED:
                            return localization.QueuedStatus;
                        case DownloadItemStatus.DOWNLOADING:
                            return localization.DownloadingStatus;
                        case DownloadItemStatus.STOPPED:
                            return localization.StoppedStatus;
                        case DownloadItemStatus.ERROR:
                            return localization.ErrorStatus;
                        default:
                            throw new Exception($"Unexpected collection download status: {downloadStatus}.");
                    }
                }
            }
        }

        private CancellationTokenSource downloadCancellationTokenSource;
        private bool isHeaderVisible;
        private ObservableCollection<DownloadItemViewModel> downloads;
        private string interruptButtonText;
        private bool isInterruptButtonEnabled;
        private bool isInterruptButtonVisible;
        private bool isRetryButtonVisible;

        public DownloadDumpsPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.DOWNLOAD_DUMPS)
        {
            Localization = windowLocalization.DownloadDumpsStep;
            foreach (SharedSetupContext.Collection collection in SharedSetupContext.Collections)
            {
                collection.DownloadStatus = DownloadItemStatus.QUEUED;
                collection.TotalSize = null;
                collection.DownloadedSize = null;
            }
            isHeaderVisible = true;
            downloads = new ObservableCollection<DownloadItemViewModel>();
            interruptButtonText = Localization.Interrupt;
            isInterruptButtonEnabled = true;
            isInterruptButtonVisible = true;
            isRetryButtonVisible = false;
            InterruptCommand = new Command(InterruptDownloads);
            RetryCommand = new Command(RetryDownloads);
        }

        public DownloadDumpsSetupStepLocalizator Localization { get; private set; }

        public string Header
        {
            get
            {
                return GetHeaderString(SetupStage.DOWNLOADING_DUMPS);
            }
        }

        public bool IsHeaderVisible
        {
            get
            {
                return isHeaderVisible;
            }
            set
            {
                isHeaderVisible = value;
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

        public string InterruptButtonText
        {
            get
            {
                return interruptButtonText;
            }
            set
            {
                interruptButtonText = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsInterruptButtonEnabled
        {
            get
            {
                return isInterruptButtonEnabled;
            }
            set
            {
                isInterruptButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsInterruptButtonVisible
        {
            get
            {
                return isInterruptButtonVisible;
            }
            set
            {
                isInterruptButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsRetryButtonVisible
        {
            get
            {
                return isRetryButtonVisible;
            }
            set
            {
                isRetryButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public Command InterruptCommand { get; }
        public Command RetryCommand { get; }

        public override void OnPageEnter()
        {
            base.OnPageEnter();
            IsHeaderVisible = SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC;
            ConvertCollectionsToDownloadItems();
            DownloadDumpsAsync();
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            SaveDownloadItemStatesToCollections();
            MoveToPage(SetupWizardStep.COLLECTIONS);
        }

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            SaveDownloadItemStatesToCollections();
            MoveToPage(SetupWizardStep.CREATE_DATABASE);
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.DownloadDumpsStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(Header));
            foreach (DownloadItemViewModel downloadItemViewModel in Downloads)
            {
                downloadItemViewModel.UpdateLocalization(Localization);
            }
        }

        private async void DownloadDumpsAsync()
        {
            HideBackButton();
            HideNextButton();
            HideCancelButton();
            HideCloseButton();
            InterruptButtonText = Localization.Interrupt;
            IsInterruptButtonEnabled = true;
            IsInterruptButtonVisible = true;
            IsRetryButtonVisible = false;
            downloadCancellationTokenSource = new CancellationTokenSource();
            foreach (DownloadItemViewModel downloadItemViewModel in Downloads)
            {
                if (downloadItemViewModel.Status != DownloadItemStatus.COMPLETED)
                {
                    downloadItemViewModel.UpdateStatus(DownloadItemStatus.QUEUED);
                }
            }
            while (Downloads.Any(downloadItemViewModel => downloadItemViewModel.Status == DownloadItemStatus.QUEUED))
            {
                await DownloadNextItemAsync();
                if (downloadCancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }
            IsInterruptButtonVisible = false;
            ShowBackButton();
            ShowCancelButton();
            ShowCloseButton();
            if (Downloads.Any(downloadItemViewModel => downloadItemViewModel.Status != DownloadItemStatus.COMPLETED))
            {
                IsRetryButtonVisible = true;
            }
            else
            {
                ShowNextButton();
            }
        }

        private async Task DownloadNextItemAsync()
        {
            DownloadItemViewModel downloadItem = Downloads.FirstOrDefault(downloadItemViewModel => downloadItemViewModel.Status == DownloadItemStatus.QUEUED);
            if (downloadItem == null)
            {
                return;
            }
            string dumpUrl = downloadItem.Collection.DownloadUrl;
            string dumpFilePath = downloadItem.Collection.DownloadFilePath;
            Progress<object> downloadProgressHandler = new Progress<object>(progress => HandleDownloadProgress(downloadItem, progress));
            DownloadUtils.DownloadResult downloadResult = default;
            bool error = false;
            downloadItem.UpdateStatus(DownloadItemStatus.DOWNLOADING);
            try
            {
                downloadResult = await MainModel.LibgenDumpDownloader.DownloadDumpAsync(dumpUrl, dumpFilePath, downloadProgressHandler,
                    downloadCancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                error = true;
            }
            if (!error)
            {
                switch (downloadResult)
                {
                    case DownloadUtils.DownloadResult.COMPLETED:
                        downloadItem.UpdateStatus(DownloadItemStatus.COMPLETED);
                        break;
                    case DownloadUtils.DownloadResult.CANCELLED:
                        downloadItem.UpdateStatus(DownloadItemStatus.STOPPED);
                        break;
                    case DownloadUtils.DownloadResult.INCOMPLETE:
                    case DownloadUtils.DownloadResult.ERROR:
                        error = true;
                        break;
                }
            }
            if (error)
            {
                downloadItem.UpdateStatus(DownloadItemStatus.ERROR);
            }
        }

        private void HandleDownloadProgress(DownloadItemViewModel downloadItemViewModel, object progress)
        {
            if (progress is DownloadFileProgress downloadFileProgress)
            {
                downloadItemViewModel.UpdateProgress(downloadFileProgress.DownloadedBytes, downloadFileProgress.FileSize);
            }
        }

        private void ConvertCollectionsToDownloadItems()
        {
            Downloads = new ObservableCollection<DownloadItemViewModel>(SharedSetupContext.Collections.Where(collection => collection.IsSelected).
                Select(collection => new DownloadItemViewModel(collection, Localization)));
        }

        private void SaveDownloadItemStatesToCollections()
        {
            foreach (DownloadItemViewModel downloadItemViewModel in Downloads)
            {
                downloadItemViewModel.SaveStateToCollection();
            }
        }

        private void InterruptDownloads()
        {
            InterruptButtonText = Localization.Interrupting;
            IsInterruptButtonEnabled = false;
            downloadCancellationTokenSource.Cancel();
        }

        private void RetryDownloads()
        {
            DownloadDumpsAsync();
        }
    }
}
