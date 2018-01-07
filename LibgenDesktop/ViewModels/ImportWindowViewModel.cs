using System;
using System.Collections.ObjectModel;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.SqlDump;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
{
    internal class ImportWindowViewModel : ViewModel
    {
        internal class LogLine : ViewModel
        {
            private string step;
            private string header;
            private string status;

            public LogLine(string step, string header, string status)
            {
                this.step = step;
                this.header = header;
                this.status = status;
            }

            public string Step
            {
                get
                {
                    return step;
                }
                set
                {
                    step = value;
                    NotifyPropertyChanged();
                }
            }

            public string Header
            {
                get
                {
                    return header;
                }
                set
                {
                    header = value;
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
        }

        private readonly MainModel mainModel;
        private readonly string dumpFilePath;
        private readonly CancellationTokenSource cancellationTokenSource;
        private bool isInProgress;
        private string status;
        private ObservableCollection<LogLine> logs;
        private string resultLogLine;
        private bool isResultLogLineVisible;
        private string errorLogLine;
        private bool isErrorLogLineVisible;
        private string elapsed;
        private bool isCancellationEnabled;
        private bool isCancelButtonVisible;
        private bool isCloseButtonVisible;
        private decimal lastScannedPercentage;
        private TableType tableType;
        private DateTime startDateTime;
        private TimeSpan lastElapsedTime;

        public ImportWindowViewModel(MainModel mainModel, string dumpFilePath)
        {
            this.mainModel = mainModel;
            this.dumpFilePath = dumpFilePath;
            cancellationTokenSource = new CancellationTokenSource();
            CancelCommand = new Command(Cancel);
            CloseCommand = new Command(Close);
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

        public ObservableCollection<LogLine> Logs
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

        public bool IsCancellationEnabled
        {
            get
            {
                return isCancellationEnabled;
            }
            set
            {
                isCancellationEnabled = value;
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

        private void Initialize()
        {
            isInProgress = true;
            status = "Шаг 1 из 2. Поиск данных для импорта...";
            logs = new ObservableCollection<LogLine>();
            isResultLogLineVisible = false;
            isErrorLogLineVisible = false;
            startDateTime = DateTime.Now;
            lastElapsedTime = TimeSpan.Zero;
            elapsed = GetElapsedString(lastElapsedTime);
            isCancellationEnabled = true;
            isCancelButtonVisible = true;
            isCloseButtonVisible = false;
            lastScannedPercentage = 0;
            LogLine searchHeaderLogLine = new LogLine("Шаг 1", "Поиск данных для импорта в файле", "Идет сканирование файла...");
            logs.Add(searchHeaderLogLine);
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
                ErrorLogLine = "Импорт завершился с ошибками.";
                IsErrorLogLineVisible = true;
                IsInProgress = false;
                Status = "Импорт завершен с ошибками";
                IsCancelButtonVisible = false;
                IsCloseButtonVisible = true;
                ShowErrorWindow(exception);
                return;
            }
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
            DateTime now = DateTime.Now;
            switch (progress)
            {
                case ImportSearchTableDefinitionProgress searchTableDefinitionProgress:
                    if (searchTableDefinitionProgress.TotalBytes != 0)
                    {
                        decimal scannedPercentage = Math.Round((decimal)searchTableDefinitionProgress.BytesParsed * 100 / searchTableDefinitionProgress.TotalBytes, 1);
                        if (scannedPercentage != lastScannedPercentage)
                        {
                            logs[0].Status = GetScannedPercentageStatusString(scannedPercentage);
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
                    logs[0].Status = tableFoundStatusString + ".";
                    Status = "Шаг 2 из 2. Импорт данных...";
                    logs.Add(new LogLine("Шаг 2", "Импорт данных", GetImportedObjectCountStatusString(0)));
                    break;
                case ImportObjectsProgress importObjectsProgress:
                    logs[1].Status = GetImportedObjectCountStatusString(importObjectsProgress.ObjectsImported);
                    break;
            }
            TimeSpan elapsedTime = now - startDateTime;
            if (elapsedTime.Seconds != lastElapsedTime.Seconds)
            {
                lastElapsedTime = elapsedTime;
                Elapsed = GetElapsedString(elapsedTime);
            }
        }

        private void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        private void Close()
        {
            CurrentWindowContext.Close();
        }

        private string GetScannedPercentageStatusString(decimal scannedPercentage)
        {
            return $"Просканировано {scannedPercentage}% файла...";
        }

        private string GetImportedObjectCountStatusString(int importedObjectCount)
        {
            string result = "Импортировано ";
            switch (tableType)
            {
                case TableType.NON_FICTION:
                case TableType.FICTION:
                    result += "книг";
                    break;
                case TableType.SCI_MAG:
                    result += "статей";
                    break;
                default:
                    throw new Exception($"Unknown table type: {tableType}.");
            }
            return $"{result}: {importedObjectCount.ToFormattedString()}.";
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
