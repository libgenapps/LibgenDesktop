using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Update;
using LibgenDesktop.Views;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.ViewModels
{
    internal class ApplicationUpdateWindowViewModel : LibgenWindowViewModel
    {
        private readonly MainModel mainModel;
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

        public ApplicationUpdateWindowViewModel(MainModel mainModel, Updater.UpdateCheckResult updateCheckResult)
        {
            this.mainModel = mainModel;
            this.updateCheckResult = updateCheckResult;
            cancellationTokenSource = new CancellationTokenSource();
            WindowClosingCommand = new FuncCommand<bool>(WindowClosing);
            SkipVersionCommand = new Command(SkipVersion);
            DownloadCommand = new Command(DownloadAsync);
            CancelCommand = new Command(Cancel);
            InterruptDownloadCommand = new Command(InterruptDownload);
            CloseCommand = new Command(Close);
            Initialize();
        }

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

        public FuncCommand<bool> WindowClosingCommand { get; }
        public Command SkipVersionCommand { get; }
        public Command DownloadCommand { get; }
        public Command CancelCommand { get; }
        public Command InterruptDownloadCommand { get; }
        public Command CloseCommand { get; }

        public event EventHandler ApplicationShutdownRequested;

        private void Initialize()
        {
            newVersionText = $"Новая версия: {updateCheckResult.NewReleaseName} от {updateCheckResult.PublishedAt:dd.MM.yyyy}";
            isSkipButtonVisible = true;
            downloadProgress = 0;
            downloadButtonText = Environment.IsInPortableMode ? "СКАЧАТЬ" : "СКАЧАТЬ И УСТАНОВИТЬ";
            isDownloadButtonVisible = true;
            isCancelButtonVisible = true;
            interruptButtonText = "ПРЕРВАТЬ";
            isInterruptButtonEnabled = true;
            isInterruptButtonVisible = false;
            isCloseButtonVisible = false;
            error = false;
        }

        private void SkipVersion()
        {
            mainModel.AppSettings.LastUpdate.IgnoreReleaseName = updateCheckResult.NewReleaseName;
            mainModel.SaveSettings();
            mainModel.LastApplicationUpdateCheckResult = null;
            CurrentWindowContext.CloseDialog(true);
        }

        private async void DownloadAsync()
        {
            IsSkipButtonVisible = false;
            IsDownloadButtonVisible = false;
            IsCancelButtonVisible = false;
            IsInterruptButtonVisible = true;
            string downloadFilePath = null;
            MainModel.DownloadFileResult? result = null;
            try
            {
                string downloadDirectory = Path.Combine(Environment.AppDataDirectory, "Updates");
                if (!Directory.Exists(downloadDirectory))
                {
                    Directory.CreateDirectory(downloadDirectory);
                }
                downloadFilePath = Path.Combine(downloadDirectory, updateCheckResult.FileName);
                Progress<object> downloadProgressHandler = new Progress<object>(HandleDownloadProgress);
                result = await mainModel.DownloadFileAsync(updateCheckResult.DownloadUrl, downloadFilePath, downloadProgressHandler,
                    cancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, CurrentWindowContext);
                error = true;
            }
            if (!error && result != MainModel.DownloadFileResult.COMPLETED)
            {
                if (result == MainModel.DownloadFileResult.INCOMPLETE)
                {
                    MessageBoxWindow.ShowMessage("Ошибка", "Файл обновления не был загружен полностью.", CurrentWindowContext);
                }
                error = true;
            }
            IsInterruptButtonVisible = false;
            IsCloseButtonVisible = true;
            if (!error)
            {
                if (Environment.IsInPortableMode)
                {
                    Process.Start("explorer.exe", $@"/select, ""{downloadFilePath}""");
                    CurrentWindowContext.CloseDialog(false);
                }
                else
                {
                    Process.Start(downloadFilePath);
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
            InterruptButtonText = "ПРЕРЫВАЕТСЯ...";
            cancellationTokenSource.Cancel();
        }

        private void Close()
        {
            CurrentWindowContext.CloseDialog(!error);
        }

        private bool WindowClosing()
        {
            if (IsInterruptButtonVisible)
            {
                if (MessageBoxWindow.ShowPrompt("Прервать загрузку?", "Прервать загрузку обновления?", CurrentWindowContext))
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
