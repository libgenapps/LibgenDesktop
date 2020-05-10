using System;
using System.Threading;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class DownloadDumpInfoPageViewModel : SetupStepViewModel
    {
        private CancellationTokenSource downloadCancellationTokenSource;
        private bool isHeaderVisible;
        private bool isDownloadingMessageVisible;
        private bool isErrorMessageVisible;
        private string interruptButtonText;
        private bool isInterruptButtonEnabled;
        private bool isInterruptButtonVisible;
        private bool isRetryButtonVisible;

        public DownloadDumpInfoPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.DOWNLOAD_DUMP_INFO)
        {
            Localization = windowLocalization.DownloadDumpInfoStep;
            isHeaderVisible = true;
            isDownloadingMessageVisible = false;
            isErrorMessageVisible = false;
            interruptButtonText = Localization.Interrupt;
            isInterruptButtonEnabled = true;
            isInterruptButtonVisible = true;
            isRetryButtonVisible = false;
            InterruptCommand = new Command(InterruptDownload);
            RetryCommand = new Command(RetryDownload);
        }

        public DownloadDumpInfoSetupStepLocalizator Localization { get; private set; }

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

        public bool IsDownloadingMessageVisible
        {
            get
            {
                return isDownloadingMessageVisible;
            }
            set
            {
                isDownloadingMessageVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsErrorMessageVisible
        {
            get
            {
                return isErrorMessageVisible;
            }
            set
            {
                isErrorMessageVisible = value;
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

        public override bool IsBackButtonVisible => false;
        public override bool IsNextButtonVisible => false;

        public override void OnPageEnter()
        {
            base.OnPageEnter();
            IsHeaderVisible = SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC;
            HideBackButton();
            HideNextButton();
            HideCancelButton();
            HideCloseButton();
            DownloadDumpInfoAsync();
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SetupWizardStep.DOWNLOAD_MODE);
        }

        public async void DownloadDumpInfoAsync()
        {
            IsDownloadingMessageVisible = true;
            IsErrorMessageVisible = false;
            InterruptButtonText = Localization.Interrupt;
            IsInterruptButtonEnabled = true;
            IsInterruptButtonVisible = true;
            IsRetryButtonVisible = false;
            downloadCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = downloadCancellationTokenSource.Token;
            LibgenDumpDownloader.LoadAndParseResult loadAndParseResult = null;
            bool error = false;
            try
            {
                loadAndParseResult = await MainModel.LibgenDumpDownloader.LoadAndParseDumpPageAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                error = true;
            }
            if (loadAndParseResult != null && (loadAndParseResult.Status == LibgenDumpDownloader.LoadAndParseStatus.LOAD_FAILED ||
                loadAndParseResult.Status == LibgenDumpDownloader.LoadAndParseStatus.PARSE_FAILED))
            {
                error = true;
            }
            ShowCancelButton();
            ShowCloseButton();
            if (error)
            {
                IsDownloadingMessageVisible = false;
                IsInterruptButtonVisible = false;
                IsErrorMessageVisible = true;
                IsRetryButtonVisible = true;
                ShowBackButton();
                return;
            }
            if (loadAndParseResult != null)
            {
                if (loadAndParseResult.Status == LibgenDumpDownloader.LoadAndParseStatus.CANCELLED)
                {
                    MoveToPage(SetupWizardStep.DOWNLOAD_MODE);
                }
                else if (loadAndParseResult.Status == LibgenDumpDownloader.LoadAndParseStatus.COMPLETED)
                {
                    LibgenDumpDownloader.Dumps dumps = loadAndParseResult.Dumps;
                    SharedSetupContext.DumpsMetadata = dumps;
                    SharedSetupContext.NonFictionCollection.DownloadUrl = dumps.NonFiction.Url;
                    SharedSetupContext.FictionCollection.DownloadUrl = dumps.Fiction.Url;
                    SharedSetupContext.SciMagCollection.DownloadUrl = dumps.SciMag.Url;
                    MoveToPage(SetupWizardStep.COLLECTIONS);
                }
            }
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.DownloadDumpInfoStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(Header));
        }

        private void InterruptDownload()
        {
            downloadCancellationTokenSource.Cancel();
            InterruptButtonText = Localization.Interrupting;
            IsInterruptButtonEnabled = false;
        }

        private void RetryDownload()
        {
            HideBackButton();
            HideCancelButton();
            HideCloseButton();
            DownloadDumpInfoAsync();
        }
    }
}
