using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.SqlDump;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
{
    internal class SynchronizationWindowViewModel : ViewModel
    {
        private enum Step
        {
            PREPARATION = 1,
            CREATING_INDEXES,
            DOWNLOADING_BOOKS,
            IMPORTING_DATA
        }

        private readonly MainModel mainModel;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Timer elapsedTimer;
        private bool isInProgress;
        private string status;
        private ObservableCollection<ProgressLogItemViewModel> logs;
        private string resultLogLine;
        private bool isResultLogLineVisible;
        private string errorLogLine;
        private bool isErrorLogLineVisible;
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
            CancelCommand = new Command(Cancel);
            CloseCommand = new Command(Close);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize();
        }

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

        public ObservableCollection<ProgressLogItemViewModel> Logs
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

        public string ResultLogLine
        {
            get
            {
                return resultLogLine;
            }
            set
            {
                resultLogLine = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsResultLogLineVisible
        {
            get
            {
                return isResultLogLineVisible;
            }
            set
            {
                isResultLogLineVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string ErrorLogLine
        {
            get
            {
                return errorLogLine;
            }
            set
            {
                errorLogLine = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsErrorLogLineVisible
        {
            get
            {
                return isErrorLogLineVisible;
            }
            set
            {
                isErrorLogLineVisible = value;
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

        private ProgressLogItemViewModel CurrentLogItem
        {
            get
            {
                return logs[currentStepIndex - 1];
            }
        }

        private void Initialize()
        {
            isInProgress = true;
            currentStep = Step.PREPARATION;
            currentStepIndex = 1;
            totalSteps = 2;
            UpdateStatus("Подготовка к синхронизации");
            logs = new ObservableCollection<ProgressLogItemViewModel>();
            isResultLogLineVisible = false;
            isErrorLogLineVisible = false;
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
                synchronizationResult = await mainModel.SynchronizeNonFiction(synchronizationProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                ErrorLogLine = "Синхронизация завершилась с ошибками.";
                IsErrorLogLineVisible = true;
                IsInProgress = false;
                Status = "Синхронизация завершилась с ошибками";
                IsCancelButtonVisible = false;
                IsCloseButtonVisible = true;
                ShowErrorWindow(exception);
                return;
            }
            elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            switch (synchronizationResult)
            {
                case MainModel.SynchronizationResult.COMPLETED:
                    ResultLogLine = "Синхронизация выполнена успешно.";
                    IsResultLogLineVisible = true;
                    Status = "Синхронизация завершена";
                    break;
                case MainModel.SynchronizationResult.CANCELLED:
                    ErrorLogLine = "Синхронизация была прервана пользователем.";
                    IsErrorLogLineVisible = true;
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
                            totalSteps++;
                            UpdateStatus("Создание индексов");
                            logs.Add(new ProgressLogItemViewModel(currentStepIndex, "Создание недостающих индексов"));
                        }
                        CurrentLogItem.LogLines.Add($"Создается индекс для столбца {createIndexProgress.ColumnName}...");
                        break;
                    case JsonApiDownloadProgress jsonApiDownloadProgress:
                        if (currentStep != Step.DOWNLOADING_BOOKS)
                        {
                            if (currentStep == Step.CREATING_INDEXES)
                            {
                                currentStepIndex++;
                            }
                            currentStep = Step.DOWNLOADING_BOOKS;
                            UpdateStatus("Скачивание данных");
                            logs.Add(new ProgressLogItemViewModel(currentStepIndex, "Скачивание информации о новых книгах"));
                        }
                        CurrentLogItem.LogLine = $"Скачано книг: {jsonApiDownloadProgress.BooksDownloaded}.";
                        break;
                    case ImportObjectsProgress importObjectsProgress:
                        if (currentStep != Step.IMPORTING_DATA)
                        {
                            currentStep = Step.IMPORTING_DATA;
                            currentStepIndex++;
                            UpdateStatus("Импорт данных");
                            logs.Add(new ProgressLogItemViewModel(currentStepIndex, "Импорт данных"));
                        }
                        string logLine = GetSynchronizedBookCountLogLine(importObjectsProgress.ObjectsAdded, importObjectsProgress.ObjectsUpdated);
                        CurrentLogItem.LogLine = logLine;
                        break;
                }
            }
            catch (Exception exception)
            {
                cancellationTokenSource.Cancel();
                ShowErrorWindow(exception);
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
            Status = $"Шаг {currentStepIndex} из {totalSteps}. {statusDescription}...";
        }

        private string GetSynchronizedBookCountLogLine(int addedObjectCount, int updatedObjectCount)
        {
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("Добавлено книг: ");
            resultBuilder.Append(addedObjectCount.ToFormattedString());
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
