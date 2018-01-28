using System;
using System.Text;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
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

        private readonly MainModel mainModel;
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
        {
            this.mainModel = mainModel;
            cancellationTokenSource = new CancellationTokenSource();
            elapsedTimer = new Timer(state => UpdateElapsedTime());
            Logs = new ImportLogPanelViewModel();
            CancelCommand = new Command(Cancel);
            CloseCommand = new Command(Close);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize();
        }

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
            UpdateStatus("Подготовка к синхронизации");
            startDateTime = DateTime.Now;
            lastElapsedTime = TimeSpan.Zero;
            elapsedTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            elapsed = GetElapsedString(lastElapsedTime);
            cancelButtonText = "ПРЕРВАТЬ";
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
                synchronizationResult = await mainModel.SynchronizeNonFictionAsync(synchronizationProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                Logs.ShowErrorLogLine("Синхронизация завершилась с ошибками.");
                IsInProgress = false;
                Status = "Синхронизация завершилась с ошибками";
                IsCancelButtonVisible = false;
                IsCloseButtonVisible = true;
                ShowErrorWindow(exception, CurrentWindowContext);
                return;
            }
            elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            switch (synchronizationResult)
            {
                case MainModel.SynchronizationResult.COMPLETED:
                    Logs.ShowResultLogLine("Синхронизация выполнена успешно.");
                    Status = "Синхронизация завершена";
                    break;
                case MainModel.SynchronizationResult.CANCELLED:
                    Logs.ShowErrorLogLine("Синхронизация была прервана пользователем.");
                    Status = "Синхронизация прервана";
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
                            UpdateStatus("Создание индексов");
                            Logs.AddLogItem(currentStepIndex, "Создание недостающих индексов");
                        }
                        CurrentLogItem.LogLines.Add($"Создается индекс для столбца {createIndexProgress.ColumnName}...");
                        break;
                    case ImportLoadLibgenIdsProgress importLoadLibgenIdsProgress:
                        if (currentStep != Step.LOADING_EXISTING_IDS)
                        {
                            currentStep = Step.LOADING_EXISTING_IDS;
                            currentStepIndex++;
                            UpdateStatus("Загрузка идентификаторов");
                            Logs.AddLogItem(currentStepIndex, "Загрузка идентификаторов существующих данных");
                        }
                        CurrentLogItem.LogLines.Add($"Загрузка значений столбца LibgenId...");
                        break;
                    case SynchronizationProgress synchronizationProgress:
                        string secondLogLine = GetSynchronizedBookCountLogLine(synchronizationProgress.ObjectsDownloaded, synchronizationProgress.ObjectsAdded,
                            synchronizationProgress.ObjectsUpdated);
                        if (currentStep != Step.SYNCHRONIZATION)
                        {
                            currentStep = Step.SYNCHRONIZATION;
                            currentStepIndex++;
                            Logs.AddLogItem(currentStepIndex, "Синхронизация списка книг", "Скачивание информации о новых книгах");
                            CurrentLogItem.LogLines.Add(secondLogLine);
                            UpdateStatus("Синхронизация");
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
            CancelButtonText = "ПРЕРЫВАЕТСЯ...";
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
                statusBuilder.Append("Шаг ");
                statusBuilder.Append(currentStepIndex);
                statusBuilder.Append(" из ");
                statusBuilder.Append(totalSteps);
                statusBuilder.Append(". ");
            }
            statusBuilder.Append(statusDescription);
            statusBuilder.Append("...");
            Status = statusBuilder.ToString();
        }

        private string GetSynchronizedBookCountLogLine(int downloadedObjectCount, int addedObjectCount, int updatedObjectCount)
        {
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("Скачано книг: ");
            resultBuilder.Append(downloadedObjectCount.ToFormattedString());
            if (addedObjectCount > 0)
            {
                resultBuilder.Append(", добавлено книг: ");
                resultBuilder.Append(addedObjectCount.ToFormattedString());
            }
            if (updatedObjectCount > 0)
            {
                resultBuilder.Append(", обновлено книг: ");
                resultBuilder.Append(updatedObjectCount.ToFormattedString());
            }
            resultBuilder.Append(".");
            return resultBuilder.ToString();
        }

        private string GetElapsedString(TimeSpan elapsedTime)
        {
            if (elapsedTime.Hours > 0)
            {
                return $"Прошло {Math.Truncate(elapsedTime.TotalHours)}:{elapsedTime:mm\\:ss}";
            }
            else
            {
                return $"Прошло {elapsedTime:mm\\:ss}";
            }
        }
    }
}
