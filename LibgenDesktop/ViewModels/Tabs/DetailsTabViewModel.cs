using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.ViewModels.EventArguments;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal abstract class DetailsTabViewModel<T> : TabViewModel where T: LibgenObject
    {
        private enum DownloadButtonAction
        {
            START_DOWNLOAD = 1,
            SELECT_DOWNLOAD,
            OPEN_FILE
        }

        private enum DownloadButtonCaptionOption
        {
            DOWNLOAD = 1,
            QUEUED,
            DOWNLOADING,
            STOPPED,
            ERROR,
            OPEN
        }

        private enum DownloadActionTextOption
        {
            DOWNLOAD = 1,
            DOWNLOAD_FROM_MIRROR
        }

        private enum DownloadButtonTooltipOption
        {
            NO_TOOLTIP = 1,
            NO_DOWNLOAD_MIRROR,
            OFFLINE_MODE_IS_ON
        }

        private enum CoverNotificationOption
        {
            NO_NOTIFICATION = 1,
            COVER_IS_LOADING,
            NO_COVER,
            NO_COVER_MIRROR,
            NO_COVER_DUE_TO_OFFLINE_MODE,
            COVER_LOADING_ERROR
        }

        private readonly string downloadMirrorName;
        private readonly string coverMirrorName;
        private CommonDetailsTabLocalizator localization;
        private DownloadButtonCaptionOption downloadButtonCaptionOption;
        private DownloadActionTextOption downloadActionTextOption;
        private DownloadButtonTooltipOption downloadButtonTooltipOption;
        private CoverNotificationOption coverNotificationOption;
        private string downloadButtonCaption;
        private double downloadProgress;
        private bool isDownloadButtonEnabled;
        private string disabledDownloadButtonTooltip;
        private bool isCoverNotificationVisible;
        private string coverNotification;
        private bool coverVisible;
        private BitmapImage cover;
        private DownloadButtonAction downloadButtonAction;
        private string downloadUrl;
        private Guid? downloadId;
        private string downloadedFilePath;

        protected DetailsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, T libgenObject, string libgenObjectTitle, bool isInModalWindow,
            string downloadMirrorName, string coverMirrorName)
            : base(mainModel, parentWindowContext, libgenObjectTitle)
        {
            localization = mainModel.Localization.CurrentLanguage.CommonDetailsTab;
            LibgenObject = libgenObject;
            IsInModalWindow = isInModalWindow;
            this.downloadMirrorName = downloadMirrorName;
            this.coverMirrorName = coverMirrorName;
            DownloadCommand = new Command(DownloadButtonClick);
            Initialize();
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
        }

        public bool IsInModalWindow { get; }

        public string DownloadButtonCaption
        {
            get
            {
                return downloadButtonCaption;
            }
            set
            {
                downloadButtonCaption = value;
                NotifyPropertyChanged();
            }
        }

        public double DownloadProgress
        {
            get
            {
                return downloadProgress;
            }
            set
            {
                downloadProgress = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDownloadButtonEnabled
        {
            get
            {
                return isDownloadButtonEnabled;
            }
            set
            {
                isDownloadButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string DisabledDownloadButtonTooltip
        {
            get
            {
                return disabledDownloadButtonTooltip;
            }
            set
            {
                disabledDownloadButtonTooltip = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCoverNotificationVisible
        {
            get
            {
                return isCoverNotificationVisible;
            }
            set
            {
                isCoverNotificationVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string CoverNotification
        {
            get
            {
                return coverNotification;
            }
            set
            {
                coverNotification = value;
                NotifyPropertyChanged();
            }
        }

        public bool CoverVisible
        {
            get
            {
                return coverVisible;
            }
            private set
            {
                coverVisible = value;
                NotifyPropertyChanged();
            }
        }

        public BitmapImage Cover
        {
            get
            {
                return cover;
            }
            private set
            {
                cover = value;
                NotifyPropertyChanged();
            }
        }

        public Command DownloadCommand { get; }

        protected abstract string FileNameWithoutExtension { get; }
        protected abstract string FileExtension { get; }
        protected abstract string Md5Hash { get; }
        protected abstract bool HasCover { get; }

        protected T LibgenObject { get; }
        protected bool IsInOfflineMode => MainModel.AppSettings.Network.OfflineMode;

        public event EventHandler<SelectDownloadEventArgs> SelectDownloadRequested;

        public override void HandleTabClosing()
        {
            MainModel.Downloader.DownloaderEvent -= DownloaderEvent;
            MainModel.Localization.LanguageChanged -= LocalizationLanguageChanged;
        }

        protected abstract string GenerateDownloadUrl(Mirrors.MirrorConfiguration mirrorConfiguration);
        protected abstract string GenerateCoverUrl(Mirrors.MirrorConfiguration mirrorConfiguration);
        protected abstract string GetDownloadTransformations(Mirrors.MirrorConfiguration mirrorConfiguration);
        protected abstract void UpdateLocalization(Language newLanguage);

        private async void Initialize()
        {
            downloadProgress = 0;
            downloadButtonAction = DownloadButtonAction.START_DOWNLOAD;
            downloadId = null;
            downloadedFilePath = null;
            InitializeLibgenObject();
            UpdateDownloadStatus(MainModel.Downloader.GetDownloadItemByDownloadPageUrl(downloadUrl));
            MainModel.Downloader.DownloaderEvent += DownloaderEvent;
            await InitializeCoverAsync();
        }

        private void InitializeLibgenObject()
        {
            downloadButtonCaptionOption = DownloadButtonCaptionOption.DOWNLOAD;
            if (downloadMirrorName == null)
            {
                downloadActionTextOption = DownloadActionTextOption.DOWNLOAD;
                IsDownloadButtonEnabled = false;
                downloadButtonTooltipOption = DownloadButtonTooltipOption.NO_DOWNLOAD_MIRROR;
                downloadUrl = null;
            }
            else
            {
                downloadActionTextOption = DownloadActionTextOption.DOWNLOAD_FROM_MIRROR;
                if (IsInOfflineMode)
                {
                    IsDownloadButtonEnabled = false;
                    downloadButtonTooltipOption = DownloadButtonTooltipOption.OFFLINE_MODE_IS_ON;
                    downloadUrl = null;
                }
                else
                {
                    IsDownloadButtonEnabled = true;
                    downloadButtonTooltipOption = DownloadButtonTooltipOption.NO_TOOLTIP;
                    downloadUrl = GenerateDownloadUrl(MainModel.Mirrors[downloadMirrorName]);
                }
            }
            UpdateDownloadButtonCaption();
            UpdateDownloadButtonTooltip();
        }

        private async Task InitializeCoverAsync()
        {
            CoverVisible = false;
            Cover = null;
            if (!HasCover)
            {
                coverNotificationOption = CoverNotificationOption.NO_COVER;
                UpdateCoverNotification();
                IsCoverNotificationVisible = true;
            }
            else
            {
                if (coverMirrorName == null)
                {
                    coverNotificationOption = CoverNotificationOption.NO_COVER_MIRROR;
                    UpdateCoverNotification();
                    IsCoverNotificationVisible = true;
                }
                else
                {
                    if (IsInOfflineMode)
                    {
                        coverNotificationOption = CoverNotificationOption.NO_COVER_DUE_TO_OFFLINE_MODE;
                        UpdateCoverNotification();
                        IsCoverNotificationVisible = true;
                    }
                    else
                    {
                        string coverUrl = GenerateCoverUrl(MainModel.Mirrors[coverMirrorName]);
                        coverNotificationOption = CoverNotificationOption.COVER_IS_LOADING;
                        UpdateCoverNotification();
                        IsCoverNotificationVisible = true;
                        try
                        {
                            Cover = await LoadCoverAsync(coverUrl);
                            CoverVisible = true;
                            coverNotificationOption = CoverNotificationOption.NO_NOTIFICATION;
                            UpdateCoverNotification();
                            IsCoverNotificationVisible = false;
                        }
                        catch (Exception exception)
                        {
                            Logger.Exception(exception);
                            coverNotificationOption = CoverNotificationOption.COVER_LOADING_ERROR;
                            UpdateCoverNotification();
                        }
                    }
                }
            }
        }

        private async Task<BitmapImage> LoadCoverAsync(string coverUrl)
        {
            byte[] imageData = await MainModel.HttpClient.GetByteArrayAsync(coverUrl);
            BitmapImage result = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = memoryStream;
                result.EndInit();
                result.Freeze();
            }
            return result;
        }

        private void DownloadButtonClick()
        {
            switch (downloadButtonAction)
            {
                case DownloadButtonAction.START_DOWNLOAD:
                    if (MainModel.AppSettings.Download.UseDownloadManager)
                    {
                        MainModel.Downloader.EnqueueDownloadItem(downloadUrl, FileNameWithoutExtension, FileExtension.ToLower(), Md5Hash,
                            GetDownloadTransformations(MainModel.Mirrors[downloadMirrorName]));
                    }
                    else
                    {
                        Process.Start(downloadUrl);
                    }
                    break;
                case DownloadButtonAction.SELECT_DOWNLOAD:
                    SelectDownloadRequested?.Invoke(this, new SelectDownloadEventArgs(downloadId.Value));
                    break;
                case DownloadButtonAction.OPEN_FILE:
                    if (File.Exists(downloadedFilePath))
                    {
                        Process.Start(downloadedFilePath);
                    }
                    else
                    {
                        ShowMessage(localization.ErrorMessageTitle, localization.GetFileNotFoundErrorText(downloadedFilePath));
                    }
                    break;
            }
        }

        private void DownloaderEvent(object sender, EventArgs e)
        {
            DownloadItem downloadItem;
            switch (e)
            {
                case DownloadItemAddedEventArgs downloadItemAddedEvent:
                    downloadItem = downloadItemAddedEvent.AddedDownloadItem;
                    break;
                case DownloadItemChangedEventArgs downloadItemChangedEvent:
                    downloadItem = downloadItemChangedEvent.ChangedDownloadItem;
                    break;
                case DownloadItemRemovedEventArgs downloadItemRemovedEvent:
                    downloadItem = downloadItemRemovedEvent.RemovedDownloadItem;
                    break;
                default:
                    downloadItem = null;
                    break;
            }
            UpdateDownloadStatus(downloadItem);
        }

        private void UpdateDownloadStatus(DownloadItem downloadItem)
        {
            if (downloadItem != null && downloadItem.DownloadPageUrl == downloadUrl)
            {
                ExecuteInUiThread(() =>
                {
                    switch (downloadItem.Status)
                    {
                        case DownloadItemStatus.QUEUED:
                            downloadButtonCaptionOption = DownloadButtonCaptionOption.QUEUED;
                            break;
                        case DownloadItemStatus.DOWNLOADING:
                        case DownloadItemStatus.RETRY_DELAY:
                            downloadButtonCaptionOption = DownloadButtonCaptionOption.DOWNLOADING;
                            break;
                        case DownloadItemStatus.STOPPED:
                            downloadButtonCaptionOption = DownloadButtonCaptionOption.STOPPED;
                            break;
                        case DownloadItemStatus.ERROR:
                            downloadButtonCaptionOption = DownloadButtonCaptionOption.ERROR;
                            break;
                        case DownloadItemStatus.COMPLETED:
                            downloadButtonCaptionOption = DownloadButtonCaptionOption.OPEN;
                            break;
                        case DownloadItemStatus.REMOVED:
                            downloadButtonCaptionOption = DownloadButtonCaptionOption.DOWNLOAD;
                            break;
                    }
                    UpdateDownloadButtonCaption();
                    switch (downloadItem.Status)
                    {
                        case DownloadItemStatus.QUEUED:
                        case DownloadItemStatus.DOWNLOADING:
                        case DownloadItemStatus.RETRY_DELAY:
                        case DownloadItemStatus.STOPPED:
                        case DownloadItemStatus.ERROR:
                            downloadId = downloadItem.Id;
                            downloadButtonAction = DownloadButtonAction.SELECT_DOWNLOAD;
                            break;
                        case DownloadItemStatus.COMPLETED:
                            downloadedFilePath = Path.Combine(downloadItem.DownloadDirectory, downloadItem.FileName);
                            downloadButtonAction = DownloadButtonAction.OPEN_FILE;
                            break;
                        case DownloadItemStatus.REMOVED:
                            downloadId = null;
                            downloadedFilePath = null;
                            downloadButtonAction = DownloadButtonAction.START_DOWNLOAD;
                            break;
                    }
                    if (downloadItem.Status != DownloadItemStatus.REMOVED && downloadItem.DownloadedFileSize.HasValue &&
                        downloadItem.TotalFileSize.HasValue && downloadItem.DownloadedFileSize.Value < downloadItem.TotalFileSize.Value)
                    {
                        DownloadProgress = (double)downloadItem.DownloadedFileSize.Value * 100 / downloadItem.TotalFileSize.Value;
                    }
                    else
                    {
                        DownloadProgress = 0;
                    }
                });
            }
        }

        private void UpdateDownloadButtonCaption()
        {
            switch (downloadButtonCaptionOption)
            {
                case DownloadButtonCaptionOption.DOWNLOAD:
                    switch (downloadActionTextOption)
                    {
                        case DownloadActionTextOption.DOWNLOAD:
                            DownloadButtonCaption = localization.Download;
                            break;
                        case DownloadActionTextOption.DOWNLOAD_FROM_MIRROR:
                            DownloadButtonCaption = localization.GetDownloadFromMirrorText(downloadMirrorName.ToUpper());
                            break;
                    }
                    break;
                case DownloadButtonCaptionOption.QUEUED:
                    DownloadButtonCaption = localization.Queued;
                    break;
                case DownloadButtonCaptionOption.DOWNLOADING:
                    DownloadButtonCaption = localization.Downloading;
                    break;
                case DownloadButtonCaptionOption.STOPPED:
                    DownloadButtonCaption = localization.Stopped;
                    break;
                case DownloadButtonCaptionOption.ERROR:
                    DownloadButtonCaption = localization.Error;
                    break;
                case DownloadButtonCaptionOption.OPEN:
                    DownloadButtonCaption = localization.Open;
                    break;
            }
        }

        private void UpdateDownloadButtonTooltip()
        {
            switch (downloadButtonTooltipOption)
            {
                case DownloadButtonTooltipOption.NO_TOOLTIP:
                    DisabledDownloadButtonTooltip = null;
                    break;
                case DownloadButtonTooltipOption.NO_DOWNLOAD_MIRROR:
                    DisabledDownloadButtonTooltip = localization.NoDownloadMirrorTooltip;
                    break;
                case DownloadButtonTooltipOption.OFFLINE_MODE_IS_ON:
                    DisabledDownloadButtonTooltip = localization.OfflineModeIsOnTooltip;
                    break;
            }
        }

        private void UpdateCoverNotification()
        {
            switch (coverNotificationOption)
            {
                case CoverNotificationOption.NO_NOTIFICATION:
                    CoverNotification = null;
                    break;
                case CoverNotificationOption.COVER_IS_LOADING:
                    CoverNotification = localization.CoverIsLoading;
                    break;
                case CoverNotificationOption.NO_COVER:
                    CoverNotification = localization.NoCover;
                    break;
                case CoverNotificationOption.NO_COVER_MIRROR:
                    CoverNotification = localization.NoCoverMirror;
                    break;
                case CoverNotificationOption.NO_COVER_DUE_TO_OFFLINE_MODE:
                    CoverNotification = localization.NoCoverDueToOfflineMode;
                    break;
                case CoverNotificationOption.COVER_LOADING_ERROR:
                    CoverNotification = localization.CoverLoadingError;
                    break;
            }
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            localization = MainModel.Localization.CurrentLanguage.CommonDetailsTab;
            UpdateDownloadButtonCaption();
            UpdateDownloadButtonTooltip();
            UpdateCoverNotification();
            UpdateLocalization(MainModel.Localization.CurrentLanguage);
        }
    }
}
