using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Utils;
using LibgenDesktop.ViewModels.Panels;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class SynchronizationWindowViewModel : LibgenWindowViewModel
    {
        private enum Step
        {
            PREPARATION = 1,
            CREATING_INDEXES,
            LOADING_EXISTING_IDS,
            SYNCHRONIZATION
        }

        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Timer elapsedTimer;
        private bool isInProgress;
        private string status;
        private string elapsed;
        private string cancelButtonText;
        private bool isCancelButtonVisible;
        private bool isCancelButtonEnabled;
        private bool isCloseButtonVisible;
        private Step currentStep;
        private int currentStepIndex;
        private int totalSteps;
        private DateTime startDateTime;
        private TimeSpan lastElapsedTime;

        public SynchronizationWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            cancellationTokenSource = new CancellationTokenSource();
            Localization = mainModel.Localization.CurrentLanguage.Synchronization;
            elapsedTimer = new Timer(state => UpdateElapsedTime());
            Logs = new ImportLogPanelViewModel();
            CancelCommand = new Command(Cancel);
            CloseCommand = new Command(Close);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize();
        }

        public SynchronizationLocalizator Localization { get; }
        public ImportLogPanelViewModel Logs { get; }

        public bool IsInProgress
        {
            get
            {
                return isInProgress;
            }
            set
            {
                isInProgress = value;
                NotifyPropertyChanged();
            }
        }

        public string Status
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

        public string Elapsed
        {
            get
            {
                return elapsed;
            }
            set
            {
                elapsed = value;
                NotifyPropertyChanged();
            }
        }

        public string CancelButtonText
        {
            get
            {
                return cancelButtonText;
            }
            set
            {
                cancelButtonText = value;
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

        public bool IsCancelButtonEnabled
        {
            get
            {
                return isCancelButtonEnabled;
            }
            set
            {
                isCancelButtonEnabled = value;
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

        public Command CancelCommand { get; }
        public Command CloseCommand { get; }
        public Command WindowClosedCommand { get; }

        private ImportLogPanelViewModel.ImportLogItemViewModel CurrentLogItem
        {
            get
            {
                return Logs.LogItems[currentStepIndex - 1];
            }
        }

        private void Initialize()
        {
            isInProgress = true;
            currentStep = Step.PREPARATION;
            currentStepIndex = 0;
            totalSteps = 2;
            UpdateStatus(Localization.StatusPreparation);
            startDateTime = DateTime.Now;
            lastElapsedTime = TimeSpan.Zero;
            elapsedTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            elapsed = GetElapsedString(lastElapsedTime);
            cancelButtonText = Localization.Interrupt;
            isCancelButtonVisible = true;
            isCancelButtonEnabled = true;
            isCloseButtonVisible = false;
            Syncrhonize();
        }

        private async void Syncrhonize()
        {
            Progress<object> synchronizationProgressHandler = new Progress<object>(HandleSynchronizationProgress);
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            MainModel.SynchronizationResult synchronizationResult;
            try
            {
                synchronizationResult = await MainModel.SynchronizeNonFictionAsync(synchronizationProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                exception = exception.GetInnermostException();
                elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                Logs.ShowErrorLogLine(Localization.GetLogLineSynchronizationError(exception.Message));
                IsInProgress = false;
                Status = Localization.StatusSynchronizationError;
                IsCancelButtonVisible = false;
                IsCloseButtonVisible = true;
                if (!(exception is TimeoutException) && !(exception is SocketException))
                {
                    ShowErrorWindow(exception, CurrentWindowContext);
                }
                return;
            }
            elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            switch (synchronizationResult)
            {
                case MainModel.SynchronizationResult.COMPLETED:
                    Logs.ShowResultLogLine(Localization.LogLineSynchronizationSuccessful);
                    Status = Localization.StatusSynchronizationComplete;
                    break;
                case MainModel.SynchronizationResult.CANCELLED:
                    Logs.ShowErrorLogLine(Localization.LogLineSynchronizationCancelled);
                    Status = Localization.StatusSynchronizationCancelled;
                    break;
            }
            IsInProgress = false;
            IsCancelButtonVisible = false;
            IsCloseButtonVisible = true;
        }

        private void HandleSynchronizationProgress(object progress)
        {
            try
            {
                switch (progress)
                {
                    case ImportCreateIndexProgress createIndexProgress:
                        if (currentStep != Step.CREATING_INDEXES)
                        {
                            currentStep = Step.CREATING_INDEXES;
                            currentStepIndex++;
                            totalSteps++;
                            UpdateStatus(Localization.StatusCreatingIndexes);
                            Logs.AddLogItem(Localization.GetLogLineStep(currentStepIndex), Localization.LogLineCreatingIndexes);
                        }
                        CurrentLogItem.LogLines.Add(Localization.GetLogLineCreatingIndexForColumn(createIndexProgress.ColumnName));
                        break;
                    case ImportLoadLibgenIdsProgress importLoadLibgenIdsProgress:
                        if (currentStep != Step.LOADING_EXISTING_IDS)
                        {
                            currentStep = Step.LOADING_EXISTING_IDS;
                            currentStepIndex++;
                            UpdateStatus(Localization.StatusLoadingIds);
                            Logs.AddLogItem(Localization.GetLogLineStep(currentStepIndex), Localization.LogLineLoadingIds);
                        }
                        CurrentLogItem.LogLines.Add(Localization.GetLogLineLoadingColumnValues("LibgenId"));
                        break;
                    case SynchronizationProgress synchronizationProgress:
                        string secondLogLine = GetSynchronizedBookCountLogLine(synchronizationProgress.ObjectsDownloaded, synchronizationProgress.ObjectsAdded,
                            synchronizationProgress.ObjectsUpdated);
                        if (currentStep != Step.SYNCHRONIZATION)
                        {
                            currentStep = Step.SYNCHRONIZATION;
                            currentStepIndex++;
                            Logs.AddLogItem(Localization.GetLogLineStep(currentStepIndex), Localization.LogLineSynchronizingBookList,
                                Localization.LogLineDownloadingNewBooks);
                            CurrentLogItem.LogLines.Add(secondLogLine);
                            UpdateStatus(Localization.StatusSynchronizingData);
                        }
                        else
                        {
                            CurrentLogItem.LogLines[1] = secondLogLine;
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                cancellationTokenSource.Cancel();
                ShowErrorWindow(exception, CurrentWindowContext);
            }
        }

        private void UpdateElapsedTime()
        {
            TimeSpan elapsedTime = DateTime.Now - startDateTime;
            if (elapsedTime.Seconds != lastElapsedTime.Seconds)
            {
                lastElapsedTime = elapsedTime;
                Elapsed = GetElapsedString(elapsedTime);
            }
        }

        private void Cancel()
        {
            IsCancelButtonEnabled = false;
            CancelButtonText = Localization.Interrupting;
            cancellationTokenSource.Cancel();
        }

        private void Close()
        {
            CurrentWindowContext.Close();
        }

        private void WindowClosed()
        {
            cancellationTokenSource.Cancel();
        }

        private void UpdateStatus(string statusDescription)
        {
            StringBuilder statusBuilder = new StringBuilder();
            if (currentStepIndex > 0)
            {
                statusBuilder.Append(Localization.GetStatusStep(currentStepIndex, totalSteps));
                statusBuilder.Append(" ");
            }
            statusBuilder.Append(statusDescription);
            statusBuilder.Append("...");
            Status = statusBuilder.ToString();
        }

        private string GetSynchronizedBookCountLogLine(int downloadedObjectCount, int addedObjectCount, int updatedObjectCount)
        {
            if (addedObjectCount > 0)
            {
                if (updatedObjectCount > 0)
                {
                    return Localization.GetLogLineSynchronizationProgressAddedAndUpdated(downloadedObjectCount, addedObjectCount, updatedObjectCount);
                }
                else
                {
                    return Localization.GetLogLineSynchronizationProgressAdded(downloadedObjectCount, addedObjectCount);
                }
            }
            else
            {
                if (updatedObjectCount > 0)
                {
                    return Localization.GetLogLineSynchronizationProgressUpdated(downloadedObjectCount, updatedObjectCount);
                }
                else
                {
                    return Localization.GetLogLineSynchronizationProgressNoAddedNoUpdated(downloadedObjectCount);
                }
            }
        }

        private string GetElapsedString(TimeSpan elapsedTime)
        {
            string elapsed;
            if (elapsedTime.Hours > 0)
            {
                elapsed = $"{Math.Truncate(elapsedTime.TotalHours)}:{elapsedTime:mm\\:ss}";
            }
            else
            {
                elapsed = $"{elapsedTime:mm\\:ss}";
            }
            return Localization.GetElapsedString(elapsed);
        }
    }
}
