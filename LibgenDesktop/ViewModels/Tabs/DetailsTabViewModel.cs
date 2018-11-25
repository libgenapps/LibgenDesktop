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
        private enum MainActionButtonMode
        {
            START_DOWNLOAD = 1,
            SELECT_DOWNLOAD,
            OPEN_FILE
        }

        private enum MainActionButtonCaptionOption
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

        private enum MainActionButtonTooltipOption
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
        private MainActionButtonCaptionOption mainActionButtonCaptionOption;
        private DownloadActionTextOption downloadActionTextOption;
        private MainActionButtonTooltipOption mainActionButtonTooltipOption;
        private CoverNotificationOption coverNotificationOption;
        private string mainActionButtonCaption;
        private double mainActionProgress;
        private bool isMainActionButtonEnabled;
        private string disabledMainActionButtonTooltip;
        private bool isCoverNotificationVisible;
        private string coverNotification;
        private bool coverVisible;
        private BitmapImage cover;
        private MainActionButtonMode mainActionButtonMode;
        private string downloadUrl;
        private Guid? downloadId;
        private string localFilePath;

        protected DetailsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, T libgenObject, string libgenObjectTitle, bool isInModalWindow,
            string downloadMirrorName, string coverMirrorName)
            : base(mainModel, parentWindowContext, libgenObjectTitle)
        {
            localization = mainModel.Localization.CurrentLanguage.CommonDetailsTab;
            LibgenObject = libgenObject;
            IsInModalWindow = isInModalWindow;
            this.downloadMirrorName = downloadMirrorName;
            this.coverMirrorName = coverMirrorName;
            MainActionCommand = new Command(MainActionButtonClick);
            Initialize();
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
        }

        public bool IsInModalWindow { get; }

        public string MainActionButtonCaption
        {
            get
            {
                return mainActionButtonCaption;
            }
            set
            {
                mainActionButtonCaption = value;
                NotifyPropertyChanged();
            }
        }

        public double MainActionProgress
        {
            get
            {
                return mainActionProgress;
            }
            set
            {
                mainActionProgress = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsMainActionButtonEnabled
        {
            get
            {
                return isMainActionButtonEnabled;
            }
            set
            {
                isMainActionButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string DisabledMainActionButtonTooltip
        {
            get
            {
                return disabledMainActionButtonTooltip;
            }
            set
            {
                disabledMainActionButtonTooltip = value;
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

        public Command MainActionCommand { get; }

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
            mainActionProgress = 0;
            downloadId = null;
            if (LibgenObject.FileId.HasValue)
            {
                LibraryFile file = await MainModel.LoadFileAsync(LibgenObject.FileId.Value);
                localFilePath = file.FilePath;
            }
            else
            {
                localFilePath = null;
            }
            if (localFilePath != null)
            {
                downloadUrl = null;
                IsMainActionButtonEnabled = true;
                mainActionButtonMode = MainActionButtonMode.OPEN_FILE;
                mainActionButtonCaptionOption = MainActionButtonCaptionOption.OPEN;
                mainActionButtonTooltipOption = MainActionButtonTooltipOption.NO_TOOLTIP;
            }
            else
            {
                mainActionButtonMode = MainActionButtonMode.START_DOWNLOAD;
                mainActionButtonCaptionOption = MainActionButtonCaptionOption.DOWNLOAD;
                if (downloadMirrorName == null)
                {
                    downloadActionTextOption = DownloadActionTextOption.DOWNLOAD;
                    IsMainActionButtonEnabled = false;
                    mainActionButtonTooltipOption = MainActionButtonTooltipOption.NO_DOWNLOAD_MIRROR;
                    downloadUrl = null;
                }
                else
                {
                    downloadActionTextOption = DownloadActionTextOption.DOWNLOAD_FROM_MIRROR;
                    if (IsInOfflineMode)
                    {
                        IsMainActionButtonEnabled = false;
                        mainActionButtonTooltipOption = MainActionButtonTooltipOption.OFFLINE_MODE_IS_ON;
                        downloadUrl = null;
                    }
                    else
                    {
                        IsMainActionButtonEnabled = true;
                        mainActionButtonTooltipOption = MainActionButtonTooltipOption.NO_TOOLTIP;
                        downloadUrl = GenerateDownloadUrl(MainModel.Mirrors[downloadMirrorName]);
                    }
                }
            }
            UpdateMainActionButtonCaption();
            UpdateMainActionButtonTooltip();
            UpdateDownloadStatus(MainModel.Downloader.GetDownloadItemByDownloadPageUrl(downloadUrl));
            MainModel.Downloader.DownloaderEvent += DownloaderEvent;
            await InitializeCoverAsync();
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

        private void MainActionButtonClick()
        {
            switch (mainActionButtonMode)
            {
                case MainActionButtonMode.START_DOWNLOAD:
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
                case MainActionButtonMode.SELECT_DOWNLOAD:
                    SelectDownloadRequested?.Invoke(this, new SelectDownloadEventArgs(downloadId.Value));
                    break;
                case MainActionButtonMode.OPEN_FILE:
                    if (File.Exists(localFilePath))
                    {
                        Process.Start(localFilePath);
                    }
                    else
                    {
                        ShowMessage(localization.ErrorMessageTitle, localization.GetFileNotFoundErrorText(localFilePath));
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
                            mainActionButtonCaptionOption = MainActionButtonCaptionOption.QUEUED;
                            break;
                        case DownloadItemStatus.DOWNLOADING:
                        case DownloadItemStatus.RETRY_DELAY:
                            mainActionButtonCaptionOption = MainActionButtonCaptionOption.DOWNLOADING;
                            break;
                        case DownloadItemStatus.STOPPED:
                            mainActionButtonCaptionOption = MainActionButtonCaptionOption.STOPPED;
                            break;
                        case DownloadItemStatus.ERROR:
                            mainActionButtonCaptionOption = MainActionButtonCaptionOption.ERROR;
                            break;
                        case DownloadItemStatus.COMPLETED:
                            mainActionButtonCaptionOption = MainActionButtonCaptionOption.OPEN;
                            break;
                        case DownloadItemStatus.REMOVED:
                            mainActionButtonCaptionOption = MainActionButtonCaptionOption.DOWNLOAD;
                            break;
                    }
                    UpdateMainActionButtonCaption();
                    switch (downloadItem.Status)
                    {
                        case DownloadItemStatus.QUEUED:
                        case DownloadItemStatus.DOWNLOADING:
                        case DownloadItemStatus.RETRY_DELAY:
                        case DownloadItemStatus.STOPPED:
                        case DownloadItemStatus.ERROR:
                            downloadId = downloadItem.Id;
                            mainActionButtonMode = MainActionButtonMode.SELECT_DOWNLOAD;
                            break;
                        case DownloadItemStatus.COMPLETED:
                            localFilePath = Path.Combine(downloadItem.DownloadDirectory, downloadItem.FileName);
                            mainActionButtonMode = MainActionButtonMode.OPEN_FILE;
                            break;
                        case DownloadItemStatus.REMOVED:
                            downloadId = null;
                            localFilePath = null;
                            mainActionButtonMode = MainActionButtonMode.START_DOWNLOAD;
                            break;
                    }
                    if (downloadItem.Status != DownloadItemStatus.REMOVED && downloadItem.DownloadedFileSize.HasValue &&
                        downloadItem.TotalFileSize.HasValue && downloadItem.DownloadedFileSize.Value < downloadItem.TotalFileSize.Value)
                    {
                        MainActionProgress = (double)downloadItem.DownloadedFileSize.Value * 100 / downloadItem.TotalFileSize.Value;
                    }
                    else
                    {
                        MainActionProgress = 0;
                    }
                });
            }
        }

        private void UpdateMainActionButtonCaption()
        {
            switch (mainActionButtonCaptionOption)
            {
                case MainActionButtonCaptionOption.DOWNLOAD:
                    switch (downloadActionTextOption)
                    {
                        case DownloadActionTextOption.DOWNLOAD:
                            MainActionButtonCaption = localization.Download;
                            break;
                        case DownloadActionTextOption.DOWNLOAD_FROM_MIRROR:
                            MainActionButtonCaption = localization.GetDownloadFromMirrorText(downloadMirrorName.ToUpper());
                            break;
                    }
                    break;
                case MainActionButtonCaptionOption.QUEUED:
                    MainActionButtonCaption = localization.Queued;
                    break;
                case MainActionButtonCaptionOption.DOWNLOADING:
                    MainActionButtonCaption = localization.Downloading;
                    break;
                case MainActionButtonCaptionOption.STOPPED:
                    MainActionButtonCaption = localization.Stopped;
                    break;
                case MainActionButtonCaptionOption.ERROR:
                    MainActionButtonCaption = localization.Error;
                    break;
                case MainActionButtonCaptionOption.OPEN:
                    MainActionButtonCaption = localization.Open;
                    break;
            }
        }

        private void UpdateMainActionButtonTooltip()
        {
            switch (mainActionButtonTooltipOption)
            {
                case MainActionButtonTooltipOption.NO_TOOLTIP:
                    DisabledMainActionButtonTooltip = null;
                    break;
                case MainActionButtonTooltipOption.NO_DOWNLOAD_MIRROR:
                    DisabledMainActionButtonTooltip = localization.NoDownloadMirrorTooltip;
                    break;
                case MainActionButtonTooltipOption.OFFLINE_MODE_IS_ON:
                    DisabledMainActionButtonTooltip = localization.OfflineModeIsOnTooltip;
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
            UpdateMainActionButtonCaption();
            UpdateMainActionButtonTooltip();
            UpdateCoverNotification();
            UpdateLocalization(MainModel.Localization.CurrentLanguage);
        }
    }
}
