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
    internal class ImportWindowViewModel : LibgenWindowViewModel
    {
        private enum Step
        {
            SEARCHING_TABLE_DEFINITION = 1,
            CREATING_INDEXES,
            IMPORTING_DATA
        }

        private readonly MainModel mainModel;
        private readonly string dumpFilePath;
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
        private decimal lastScannedPercentage;
        private TableType tableType;
        private DateTime startDateTime;
        private TimeSpan lastElapsedTime;

        public ImportWindowViewModel(MainModel mainModel, string dumpFilePath)
        {
            this.mainModel = mainModel;
            this.dumpFilePath = dumpFilePath;
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
            currentStep = Step.SEARCHING_TABLE_DEFINITION;
            currentStepIndex = 1;
            totalSteps = 2;
            UpdateStatus("Поиск данных для импорта");
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
            lastScannedPercentage = 0;
            ProgressLogItemViewModel searchHeaderLogItem = new ProgressLogItemViewModel(currentStepIndex, "Поиск данных для импорта в файле");
            searchHeaderLogItem.LogLine = "Идет сканирование файла...";
            logs.Add(searchHeaderLogItem);
            Import();
        }

        private async void Import()
        {
            Progress<object> importProgressHandler = new Progress<object>(HandleImportProgress);
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            MainModel.ImportSqlDumpResult importResult;
            try
            {
                importResult = await mainModel.ImportSqlDumpAsync(dumpFilePath, importProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                ErrorLogLine = "Импорт завершился с ошибками.";
                IsErrorLogLineVisible = true;
                IsInProgress = false;
                Status = "Импорт завершен с ошибками";
                IsCancelButtonVisible = false;
                IsCloseButtonVisible = true;
                ShowErrorWindow(exception, CurrentWindowContext);
                return;
            }
            elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            switch (importResult)
            {
                case MainModel.ImportSqlDumpResult.COMPLETED:
                    ResultLogLine = "Импорт выполнен успешно.";
                    IsResultLogLineVisible = true;
                    Status = "Импорт завершен";
                    break;
                case MainModel.ImportSqlDumpResult.CANCELLED:
                    ErrorLogLine = "Импорт был прерван пользователем.";
                    IsErrorLogLineVisible = true;
                    Status = "Импорт прерван";
                    break;
                case MainModel.ImportSqlDumpResult.DATA_NOT_FOUND:
                    ErrorLogLine = "Не найдены данные для импорта.";
                    IsErrorLogLineVisible = true;
                    Status = "Данные не найдены";
                    break;
            }
            IsInProgress = false;
            IsCancelButtonVisible = false;
            IsCloseButtonVisible = true;
        }

        private void HandleImportProgress(object progress)
        {
            try
            {
                switch (progress)
                {
                    case ImportSearchTableDefinitionProgress searchTableDefinitionProgress:
                        if (searchTableDefinitionProgress.TotalBytes != 0)
                        {
                            decimal scannedPercentage = Math.Round((decimal)searchTableDefinitionProgress.BytesParsed * 100 / searchTableDefinitionProgress.TotalBytes, 1);
                            if (scannedPercentage != lastScannedPercentage)
                            {
                                CurrentLogItem.LogLine = GetScannedPercentageStatusString(scannedPercentage);
                                lastScannedPercentage = scannedPercentage;
                            }
                        }
                        break;
                    case ImportTableDefinitionFoundProgress tableDefinitionFoundProgress:
                        tableType = tableDefinitionFoundProgress.TableFound;
                        string tableFoundStatusString = "Найдена таблица с ";
                        switch (tableDefinitionFoundProgress.TableFound)
                        {
                            case TableType.NON_FICTION:
                                tableFoundStatusString += "нехудожественными книгами";
                                break;
                            case TableType.FICTION:
                                tableFoundStatusString += "художественными книгами";
                                break;
                            case TableType.SCI_MAG:
                                tableFoundStatusString += "научными статьями";
                                break;
                        }
                        CurrentLogItem.LogLine = tableFoundStatusString + ".";
                        break;
                    case ImportCreateIndexProgress createIndexProgress:
                        if (currentStep != Step.CREATING_INDEXES)
                        {
                            currentStep = Step.CREATING_INDEXES;
                            currentStepIndex++;
                            totalSteps++;
                            UpdateStatus("Создание индексов");
                            logs.Add(new ProgressLogItemViewModel(currentStepIndex, "Создание недостающих индексов"));
                        }
                        CurrentLogItem.LogLines.Add($"Создается индекс для столбца {createIndexProgress.ColumnName}...");
                        break;
                    case ImportObjectsProgress importObjectsProgress:
                        if (currentStep != Step.IMPORTING_DATA)
                        {
                            currentStep = Step.IMPORTING_DATA;
                            currentStepIndex++;
                            UpdateStatus("Импорт данных");
                            logs.Add(new ProgressLogItemViewModel(currentStepIndex, "Импорт данных"));
                        }
                        string logLine = GetImportedObjectCountLogLine(importObjectsProgress.ObjectsAdded, importObjectsProgress.ObjectsUpdated);
                        CurrentLogItem.LogLine = logLine;
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
            Status = $"Шаг {currentStepIndex} из {totalSteps}. {statusDescription}...";
        }

        private string GetScannedPercentageStatusString(decimal scannedPercentage)
        {
            return $"Просканировано {scannedPercentage}% файла...";
        }

        private string GetImportedObjectCountLogLine(int addedObjectCount, int updatedObjectCount)
        {
            string objectType;
            switch (tableType)
            {
                case TableType.NON_FICTION:
                case TableType.FICTION:
                    objectType = "книг";
                    break;
                case TableType.SCI_MAG:
                    objectType = "статей";
                    break;
                default:
                    throw new Exception($"Unknown table type: {tableType}.");
            }
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("Добавлено ");
            resultBuilder.Append(objectType);
            resultBuilder.Append(": ");
            resultBuilder.Append(addedObjectCount.ToFormattedString());
            if (updatedObjectCount > 0)
            {
                resultBuilder.Append(", обновлено ");
                resultBuilder.Append(objectType);
                resultBuilder.Append(": ");
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
