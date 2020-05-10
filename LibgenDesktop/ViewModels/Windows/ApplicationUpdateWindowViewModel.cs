using System;
using System.Diagnostics;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Update;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class ApplicationUpdateWindowViewModel : LibgenWindowViewModel
    {
        private readonly Updater.UpdateCheckResult updateCheckResult;
        private readonly CancellationTokenSource cancellationTokenSource;
        private string newVersionText;
        private double downloadProgress;
        private bool isSkipButtonVisible;
        private string downloadButtonText;
        private bool isDownloadButtonVisible;
        private bool isCancelButtonVisible;
        private bool isInterruptButtonVisible;
        private bool isCloseButtonVisible;
        private bool error;
        private bool isInterruptButtonEnabled;
        private string interruptButtonText;

        public ApplicationUpdateWindowViewModel(MainModel mainModel, Updater.UpdateCheckResult updateCheckResult, bool showSkipVersionButton)
            : base(mainModel)
        {
            this.updateCheckResult = updateCheckResult;
            isSkipButtonVisible = showSkipVersionButton;
            cancellationTokenSource = new CancellationTokenSource();
            Localization = mainModel.Localization.CurrentLanguage.ApplicationUpdate;
            WindowClosingCommand = new FuncCommand<bool?, bool>(WindowClosing);
            SkipVersionCommand = new Command(SkipVersion);
            DownloadCommand = new Command(DownloadAsync);
            CancelCommand = new Command(Cancel);
            InterruptDownloadCommand = new Command(InterruptDownload);
            CloseCommand = new Command(Close);
            Initialize();
        }

        public ApplicationUpdateWindowLocalizator Localization { get; }

        public string NewVersionText
        {
            get
            {
                return newVersionText;
            }
            set
            {
                newVersionText = value;
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

        public bool IsSkipButtonVisible
        {
            get
            {
                return isSkipButtonVisible;
            }
            set
            {
                isSkipButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string DownloadButtonText
        {
            get
            {
                return downloadButtonText;
            }
            set
            {
                downloadButtonText = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDownloadButtonVisible
        {
            get
            {
                return isDownloadButtonVisible;
            }
            set
            {
                isDownloadButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCancelButtonVisible
        {
            get
            {
                return isCancelButtonVisible;
            }
            set
            {
                isCancelButtonVisible = value;
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

        public bool IsCloseButtonVisible
        {
            get
            {
                return isCloseButtonVisible;
            }
            set
            {
                isCloseButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public FuncCommand<bool?, bool> WindowClosingCommand { get; }
        public Command SkipVersionCommand { get; }
        public Command DownloadCommand { get; }
        public Command CancelCommand { get; }
        public Command InterruptDownloadCommand { get; }
        public Command CloseCommand { get; }

        public event EventHandler ApplicationShutdownRequested;

        private void Initialize()
        {
            newVersionText = Localization.GetNewVersionString(updateCheckResult.NewReleaseName, updateCheckResult.PublishedAt);
            downloadProgress = 0;
            downloadButtonText = Environment.IsInPortableMode ? Localization.Download : Localization.DownloadAndInstall;
            isDownloadButtonVisible = true;
            isCancelButtonVisible = true;
            interruptButtonText = Localization.Interrupt;
            isInterruptButtonEnabled = true;
            isInterruptButtonVisible = false;
            isCloseButtonVisible = false;
            error = false;
        }

        private void SkipVersion()
        {
            MainModel.AppSettings.LastUpdate.IgnoreReleaseName = updateCheckResult.NewReleaseName;
            MainModel.SaveSettings();
            MainModel.LastApplicationUpdateCheckResult = null;
            CurrentWindowContext.CloseDialog(true);
        }

        private async void DownloadAsync()
        {
            IsSkipButtonVisible = false;
            IsDownloadButtonVisible = false;
            IsCancelButtonVisible = false;
            IsInterruptButtonVisible = true;
            Updater.UpdateDownloadResult result = null;
            try
            {
                Progress<object> downloadProgressHandler = new Progress<object>(HandleDownloadProgress);
                result = await MainModel.Updater.DownloadUpdateAsync(updateCheckResult, downloadProgressHandler, cancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, CurrentWindowContext);
                error = true;
            }
            if (!error && result.DownloadResult != DownloadUtils.DownloadResult.COMPLETED)
            {
                if ((result.DownloadResult == DownloadUtils.DownloadResult.INCOMPLETE) || (result.DownloadResult == DownloadUtils.DownloadResult.ERROR))
                {
                    ShowMessage(Localization.Error, Localization.IncompleteDownload);
                }
                error = true;
            }
            IsInterruptButtonVisible = false;
            IsCloseButtonVisible = true;
            if (!error)
            {
                if (Environment.IsInPortableMode)
                {
                    Process.Start("explorer.exe", $@"/select, ""{result.DownloadFilePath}""");
                    CurrentWindowContext.CloseDialog(false);
                }
                else
                {
                    Process.Start(result.DownloadFilePath);
                    CurrentWindowContext.CloseDialog(false);
                    ApplicationShutdownRequested?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void HandleDownloadProgress(object progress)
        {
            if (progress is DownloadFileProgress downloadFileProgress)
            {
                DownloadProgress = (double)downloadFileProgress.DownloadedBytes * 100 / downloadFileProgress.FileSize;
            }
        }

        private void Cancel()
        {
            CurrentWindowContext.CloseDialog(false);
        }

        private void InterruptDownload()
        {
            IsInterruptButtonEnabled = false;
            InterruptButtonText = Localization.Interrupting;
            cancellationTokenSource.Cancel();
        }

        private void Close()
        {
            CurrentWindowContext.CloseDialog(!error);
        }

        private bool WindowClosing(bool? dialogResult)
        {
            if (IsInterruptButtonVisible)
            {
                if (ShowPrompt(Localization.InterruptPromptTitle, Localization.InterruptPromptText))
                {
                    cancellationTokenSource.Cancel();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
