using System;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.SqlDump;
using LibgenDesktop.ViewModels.Panels;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class ImportWindowViewModel : LibgenWindowViewModel
    {
        private enum Step
        {
            SEARCHING_TABLE_DEFINITION = 1,
            CREATING_INDEXES,
            LOADING_EXISTING_IDS,
            IMPORTING_DATA
        }

        private readonly string dumpFilePath;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly LanguageFormatter languageFormatter;
        private readonly Timer elapsedTimer;
        private bool isInProgress;
        private string status;
        private string elapsed;
        private string freeSpace;
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
            : base(mainModel)
        {
            this.dumpFilePath = dumpFilePath;
            cancellationTokenSource = new CancellationTokenSource();
            Localization = mainModel.Localization.CurrentLanguage.Import;
            languageFormatter = mainModel.Localization.CurrentLanguage.Formatter;
            elapsedTimer = new Timer(state => UpdateElapsedTime());
            Logs = new ImportLogPanelViewModel();
            CancelCommand = new Command(Cancel);
            CloseCommand = new Command(Close);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize();
        }

        public ImportLocalizator Localization { get; }
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

        public string FreeSpace
        {
            get
            {
                return freeSpace;
            }
            set
            {
                freeSpace = value;
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
            currentStep = Step.SEARCHING_TABLE_DEFINITION;
            currentStepIndex = 1;
            totalSteps = 2;
            UpdateStatus(Localization.StatusDataLookup);
            startDateTime = DateTime.Now;
            lastElapsedTime = TimeSpan.Zero;
            elapsedTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            elapsed = GetElapsedString(lastElapsedTime);
            freeSpace = String.Empty;
            cancelButtonText = Localization.Interrupt;
            isCancelButtonVisible = true;
            isCancelButtonEnabled = true;
            isCloseButtonVisible = false;
            lastScannedPercentage = 0;
            Logs.AddLogItem(Localization.GetLogLineStep(currentStepIndex), Localization.LogLineDataLookup, Localization.LogLineScanning);
            Import();
        }

        private async void Import()
        {
            Progress<object> importProgressHandler = new Progress<object>(HandleImportProgress);
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            MainModel.ImportSqlDumpResult importResult;
            try
            {
                importResult = await MainModel.ImportSqlDumpAsync(dumpFilePath, importProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                Logs.ShowErrorLogLine(Localization.LogLineImportError);
                IsInProgress = false;
                Status = Localization.StatusImportError;
                IsCancelButtonVisible = false;
                IsCloseButtonVisible = true;
                ShowErrorWindow(exception, CurrentWindowContext);
                return;
            }
            elapsedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            switch (importResult)
            {
                case MainModel.ImportSqlDumpResult.COMPLETED:
                    Logs.ShowResultLogLine(Localization.LogLineImportSuccessful);
                    Status = Localization.StatusImportComplete;
                    break;
                case MainModel.ImportSqlDumpResult.CANCELLED:
                    Logs.ShowErrorLogLine(Localization.LogLineImportCancelled);
                    Status = Localization.StatusImportCancelled;
                    break;
                case MainModel.ImportSqlDumpResult.DATA_NOT_FOUND:
                    Logs.ShowErrorLogLine(Localization.LogLineDataNotFound);
                    Status = Localization.StatusDataNotFound;
                    break;
                case MainModel.ImportSqlDumpResult.LOW_DISK_SPACE:
                    Logs.ShowErrorLogLine(Localization.LogLineInsufficientDiskSpace);
                    Status = Localization.StatusImportCancelled;
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
                        switch (tableDefinitionFoundProgress.TableFound)
                        {
                            case TableType.NON_FICTION:
                                CurrentLogItem.LogLine = Localization.LogLineNonFictionTableFound;
                                break;
                            case TableType.FICTION:
                                CurrentLogItem.LogLine = Localization.LogLineFictionTableFound;
                                break;
                            case TableType.SCI_MAG:
                                CurrentLogItem.LogLine = Localization.LogLineSciMagTableFound;
                                break;
                        }
                        break;
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
                            totalSteps++;
                            UpdateStatus(Localization.StatusLoadingIds);
                            Logs.AddLogItem(Localization.GetLogLineStep(currentStepIndex), Localization.LogLineLoadingIds);
                        }
                        CurrentLogItem.LogLines.Add(Localization.GetLogLineLoadingColumnValues("LibgenId"));
                        break;
                    case ImportObjectsProgress importObjectsProgress:
                        if (currentStep != Step.IMPORTING_DATA)
                        {
                            currentStep = Step.IMPORTING_DATA;
                            currentStepIndex++;
                            UpdateStatus(Localization.StatusImportingData);
                            Logs.AddLogItem(Localization.GetLogLineStep(currentStepIndex), Localization.LogLineImportingData);
                        }
                        string logLine = GetImportedObjectCountLogLine(importObjectsProgress.ObjectsAdded, importObjectsProgress.ObjectsUpdated);
                        CurrentLogItem.LogLine = logLine;
                        break;
                    case ImportDiskSpaceProgress importDiskSpaceProgress:
                        string freeSpaceInBytesString = importDiskSpaceProgress.FreeSpaceInBytes.HasValue ?
                            languageFormatter.FileSizeToString(importDiskSpaceProgress.FreeSpaceInBytes.Value, false) : Localization.Unknown;
                        FreeSpace = Localization.GetFreeSpaceString(freeSpaceInBytesString);
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
            Status = $"{Localization.GetStatusStep(currentStepIndex, totalSteps)} {statusDescription}...";
        }

        private string GetScannedPercentageStatusString(decimal scannedPercentage)
        {
            return Localization.GetLogLineScannedProgress(scannedPercentage);
        }

        private string GetImportedObjectCountLogLine(int addedObjectCount, int updatedObjectCount)
        {
            switch (tableType)
            {
                case TableType.NON_FICTION:
                case TableType.FICTION:
                    return updatedObjectCount > 0 ? Localization.GetLogLineImportBooksProgressWithUpdate(addedObjectCount, updatedObjectCount) :
                        Localization.GetLogLineImportBooksProgressNoUpdate(addedObjectCount);
                case TableType.SCI_MAG:
                    return updatedObjectCount > 0 ? Localization.GetLogLineImportArticlesProgressWithUpdate(addedObjectCount, updatedObjectCount) :
                        Localization.GetLogLineImportArticlesProgressNoUpdate(addedObjectCount);
                default:
                    throw new Exception($"Unknown table type: {tableType}.");
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
