using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Export;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.Utils;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.ViewModels.Panels
{
    internal class ExportPanelViewModel : ContainerViewModel
    {
        private readonly LibgenObjectType libgenObjectType;
        private readonly IWindowContext parentWindowContext;
        private ExportPanelLocalizator localization;
        private CancellationTokenSource cancellationTokenSource;
        private bool isSeparatorPanelVisible;
        private bool isXlsxSelected;
        private bool isCsvSelected;
        private bool isSettingsPanelVisible;
        private bool isCommaSeparatorSelected;
        private bool isSemicolonSeparatorSelected;
        private bool isTabSeparatorSelected;
        private string searchQuery;
        private string filePathTemplate;
        private bool isLimitPanelVisible;
        private bool isNoLimitSelected;
        private bool isLimitSelected;
        private bool isExportButtonEnabled;
        private bool isProgressPanelVisible;
        private string progressStatus;
        private bool isCancelExportButtonEnabled;
        private bool isCancelExportButtonVisible;
        private bool areExportResultButtonsVisible;
        private bool isShowResultButtonVisible;
        private ExportResult exportResult;

        public ExportPanelViewModel(MainModel mainModel, LibgenObjectType libgenObjectType, IWindowContext parentWindowContext)
            : base(mainModel)
        {
            this.libgenObjectType = libgenObjectType;
            this.parentWindowContext = parentWindowContext;
            localization = mainModel.Localization.CurrentLanguage.ExportPanel;
            SelectPathCommand = new Command(SelectPath);
            ExportCommand = new Command(Export);
            CancelCommand = new Command(Cancel);
            CancelExportCommand = new Command(CancelExport);
            ShowResultCommand = new Command(ShowResult);
            CloseCommand = new Command(Close);
            Initialize();
        }

        public bool IsExportInProgress { get; private set; }

        public ExportPanelLocalizator Localization
        {
            get
            {
                return localization;
            }
            set
            {
                localization = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSettingsPanelVisible
        {
            get
            {
                return isSettingsPanelVisible;
            }
            set
            {
                isSettingsPanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsXlsxSelected
        {
            get
            {
                return isXlsxSelected;
            }
            set
            {
                isXlsxSelected = value;
                NotifyPropertyChanged();
                IsSeparatorPanelVisible = !value;
                UpdateFilePathTemplate(useSearchQuery: false);
            }
        }

        public bool IsCsvSelected
        {
            get
            {
                return isCsvSelected;
            }
            set
            {
                isCsvSelected = value;
                NotifyPropertyChanged();
                IsSeparatorPanelVisible = value;
                UpdateFilePathTemplate(useSearchQuery: false);
            }
        }

        public bool IsSeparatorPanelVisible
        {
            get
            {
                return isSeparatorPanelVisible;
            }
            set
            {
                isSeparatorPanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCommaSeparatorSelected
        {
            get
            {
                return isCommaSeparatorSelected;
            }
            set
            {
                isCommaSeparatorSelected = value;
                NotifyPropertyChanged();
                UpdateFilePathTemplate(useSearchQuery: false);
            }
        }

        public bool IsSemicolonSeparatorSelected
        {
            get
            {
                return isSemicolonSeparatorSelected;
            }
            set
            {
                isSemicolonSeparatorSelected = value;
                NotifyPropertyChanged();
                UpdateFilePathTemplate(useSearchQuery: false);
            }
        }

        public bool IsTabSeparatorSelected
        {
            get
            {
                return isTabSeparatorSelected;
            }
            set
            {
                isTabSeparatorSelected = value;
                NotifyPropertyChanged();
                UpdateFilePathTemplate(useSearchQuery: false);
            }
        }

        public string FilePathTemplate
        {
            get
            {
                return filePathTemplate;
            }
            set
            {
                filePathTemplate = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsLimitPanelVisible
        {
            get
            {
                return isLimitPanelVisible;
            }
            set
            {
                isLimitPanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsNoLimitSelected
        {
            get
            {
                return isNoLimitSelected;
            }
            set
            {
                isNoLimitSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsLimitSelected
        {
            get
            {
                return isLimitSelected;
            }
            set
            {
                isLimitSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string LimitString
        {
            get
            {
                return Localization.GetLimitString(MainModel.AppSettings.Search.MaximumResultCount);
            }
        }

        public bool IsExportButtonEnabled
        {
            get
            {
                return isExportButtonEnabled;
            }
            set
            {
                isExportButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProgressPanelVisible
        {
            get
            {
                return isProgressPanelVisible;
            }
            set
            {
                isProgressPanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string ProgressStatus
        {
            get
            {
                return progressStatus;
            }
            set
            {
                progressStatus = value;
                NotifyPropertyChanged();
            }
        }

        public string CancelExportButtonText
        {
            get
            {
                return IsCancelExportButtonEnabled ? Localization.Interrupt : Localization.Interrupting;
            }
        }

        public bool IsCancelExportButtonEnabled
        {
            get
            {
                return isCancelExportButtonEnabled;
            }
            set
            {
                isCancelExportButtonEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CancelExportButtonText));
            }
        }

        public bool IsCancelExportButtonVisible
        {
            get
            {
                return isCancelExportButtonVisible;
            }
            set
            {
                isCancelExportButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool AreExportResultButtonsVisible
        {
            get
            {
                return areExportResultButtonsVisible;
            }
            set
            {
                areExportResultButtonsVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsShowResultButtonVisible
        {
            get
            {
                return isShowResultButtonVisible;
            }
            set
            {
                isShowResultButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public Command SelectPathCommand { get; }
        public Command ExportCommand { get; }
        public Command CancelCommand { get; }
        public Command CancelExportCommand { get; }
        public Command ShowResultCommand { get; }
        public Command CloseCommand { get; }

        public event EventHandler ClosePanel;

        public void ShowPanel(string searchQuery)
        {
            UpdateSearchQuery(searchQuery);
            IsProgressPanelVisible = false;
            IsSettingsPanelVisible = true;
        }

        public void UpdateSearchQuery(string searchQuery)
        {
            this.searchQuery = searchQuery;
            UpdateFilePathTemplate(useSearchQuery: true);
            IsExportButtonEnabled = !String.IsNullOrWhiteSpace(searchQuery);
        }

        public void UpdateLocalization(Language newLanguage)
        {
            Localization = newLanguage.ExportPanel;
            NotifyPropertyChanged(nameof(LimitString));
            NotifyPropertyChanged(nameof(CancelExportButtonText));
        }

        private void Initialize()
        {
            AppSettings.ExportPanelSettngs exportPanelSettngs;
            switch (libgenObjectType)
            {
                case LibgenObjectType.NON_FICTION_BOOK:
                    exportPanelSettngs = MainModel.AppSettings.NonFiction.ExportPanel;
                    break;
                case LibgenObjectType.FICTION_BOOK:
                    exportPanelSettngs = MainModel.AppSettings.Fiction.ExportPanel;
                    break;
                case LibgenObjectType.SCIMAG_ARTICLE:
                    exportPanelSettngs = MainModel.AppSettings.SciMag.ExportPanel;
                    break;
                default:
                    throw new Exception($"Unknown object type: {libgenObjectType}.");
            }
            cancellationTokenSource = null;
            IsExportInProgress = false;
            isSettingsPanelVisible = true;
            isXlsxSelected = exportPanelSettngs.Format == AppSettings.ExportPanelSettngs.ExportFormat.XLSX;
            isCsvSelected = exportPanelSettngs.Format == AppSettings.ExportPanelSettngs.ExportFormat.CSV;
            isSeparatorPanelVisible = isCsvSelected;
            isCommaSeparatorSelected = exportPanelSettngs.Separator == AppSettings.ExportPanelSettngs.CsvSeparator.COMMA;
            isSemicolonSeparatorSelected = exportPanelSettngs.Separator == AppSettings.ExportPanelSettngs.CsvSeparator.SEMICOLON;
            isTabSeparatorSelected = exportPanelSettngs.Separator == AppSettings.ExportPanelSettngs.CsvSeparator.TAB;
            filePathTemplate = (exportPanelSettngs.ExportDirectory ?? Environment.AppDataDirectory) + @"\";
            isExportButtonEnabled = false;
            if (MainModel.AppSettings.Search.LimitResults)
            {
                isLimitPanelVisible = true;
                isLimitSelected = exportPanelSettngs.LimitSearchResults;
                isNoLimitSelected = !isLimitSelected;
            }
            else
            {
                isLimitPanelVisible = false;
                isLimitSelected = false;
                isNoLimitSelected = true;
            }
            isProgressPanelVisible = false;
            progressStatus = String.Empty;
            isCancelExportButtonEnabled = true;
            isCancelExportButtonVisible = false;
            areExportResultButtonsVisible = false;
            isShowResultButtonVisible = false;
            exportResult = null;
        }

        private void UpdateFilePathTemplate(bool useSearchQuery)
        {
            string fileName = useSearchQuery ? GenerateFileName(searchQuery) : GetFileNameWithoutExtension(correctFileName: true);
            FilePathTemplate = Path.Combine(GetDirectoryPath(correctPath: true), fileName + "." + GenerateFileExtension());
        }

        private void SelectPath()
        {
            string initialDirectory = GetDirectoryPath(correctPath: true);
            while (!Directory.Exists(initialDirectory) && !String.IsNullOrEmpty(initialDirectory))
            {
                initialDirectory = Path.GetDirectoryName(initialDirectory);
            }
            if (String.IsNullOrEmpty(initialDirectory))
            {
                initialDirectory = Environment.AppDataDirectory;
            }
            SaveFileDialogParameters saveFileDialogParameters = new SaveFileDialogParameters
            {
                DialogTitle = Localization.BrowseDialogTitle,
                Filter = GetFileFilter(),
                OverwritePrompt = false,
                InitialDirectory = initialDirectory,
                InitialFileName = GetFileNameWithoutExtension(correctFileName: true) + "." + GenerateFileExtension()
            };
            SaveFileDialogResult saveFileDialogResult = WindowManager.ShowSaveFileDialog(saveFileDialogParameters);
            if (saveFileDialogResult.DialogResult)
            {
                FilePathTemplate = saveFileDialogResult.SelectedFilePath;
            }
        }

        private string GetDirectoryPath(bool correctPath)
        {
            string result;
            if (String.IsNullOrWhiteSpace(FilePathTemplate))
            {
                result = correctPath ? Environment.AppDataDirectory : null;
            }
            else
            {
                try
                {
                    result = Path.GetDirectoryName(FilePathTemplate);
                }
                catch
                {
                    result = correctPath ? Environment.AppDataDirectory : null;
                }
            }
            return result;
        }

        private string GenerateFileName(string searchQuery)
        {
            return FileUtils.RemoveInvalidFileNameCharacters(searchQuery, "export");
        }

        private string GetFileNameWithoutExtension(bool correctFileName)
        {
            string result;
            if (String.IsNullOrWhiteSpace(FilePathTemplate))
            {
                result = correctFileName ? GenerateFileName(searchQuery) : null;
            }
            else
            {
                try
                {
                    result = Path.GetFileNameWithoutExtension(filePathTemplate);
                    if (String.IsNullOrWhiteSpace(result))
                    {
                        result = correctFileName ? GenerateFileName(searchQuery) : null;
                    }
                }
                catch
                {
                    result = correctFileName ? GenerateFileName(searchQuery) : null;
                }
            }
            return result;
        }

        private string GenerateFileExtension()
        {
            if (IsXlsxSelected)
            {
                return "xlsx";
            }
            else if (IsTabSeparatorSelected)
            {
                return "tsv";
            }
            else
            {
                return "csv";
            }
        }

        private string GetFileExtension()
        {
            string result;
            if (String.IsNullOrWhiteSpace(FilePathTemplate))
            {
                result = null;
            }
            else
            {
                try
                {
                    result = Path.GetExtension(FilePathTemplate);
                }
                catch
                {
                    result = null;
                }
            }
            if (result != null && result.StartsWith("."))
            {
                result = result.Substring(1);
            }
            return result;
        }

        private string GetFileFilter()
        {
            StringBuilder resultBuilder = new StringBuilder();
            if (IsXlsxSelected)
            {
                resultBuilder.Append(Localization.ExcelFiles);
                resultBuilder.Append(" (*.xlsx)|*.xlsx");
            }
            else if (IsTabSeparatorSelected)
            {
                resultBuilder.Append(Localization.TsvFiles);
                resultBuilder.Append(" (*.tsv)|*.tsv");
            }
            else
            {
                resultBuilder.Append(Localization.CsvFiles);
                resultBuilder.Append(" (*.csv)|*.csv");
            }
            resultBuilder.Append("|");
            resultBuilder.Append(Localization.AllFiles);
            resultBuilder.Append(" (*.*)|*.*");
            return resultBuilder.ToString();
        }

        private async void Export()
        {
            string directory = GetDirectoryPath(correctPath: false);
            if (directory == null)
            {
                ShowMessage(Localization.ErrorWarningTitle, Localization.InvalidExportPath, parentWindowContext);
                return;
            }
            if (!Directory.Exists(directory))
            {
                ShowMessage(Localization.ErrorWarningTitle, Localization.GetDirectoryNotFoundString(directory), parentWindowContext);
                return;
            }
            string fileNameWithoutExtension = GetFileNameWithoutExtension(correctFileName: false);
            if (fileNameWithoutExtension == null)
            {
                ShowMessage(Localization.ErrorWarningTitle, Localization.InvalidExportFileName, parentWindowContext);
                return;
            }
            string fileExtension = GetFileExtension();
            if (File.Exists(FilePathTemplate) &&
                !ShowPrompt(Localization.OverwritePromptTitle, Localization.GetOverwritePromptTextString(FilePathTemplate), parentWindowContext))
            {
                return;
            }
            int? searchResultLimit = isNoLimitSelected ? (int?)null : MainModel.AppSettings.Search.MaximumResultCount;
            Progress <ExportProgress> exportProgressHandler = new Progress<ExportProgress>(HandleExportProgress);
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Func<MainModel, Task<ExportResult>> exportTask = null;
            if (IsXlsxSelected)
            {
                switch (libgenObjectType)
                {
                    case LibgenObjectType.NON_FICTION_BOOK:
                        exportTask = mainModel => mainModel.ExportNonFictionToXlsxAsync(Path.Combine(directory, fileNameWithoutExtension), fileExtension,
                            searchQuery, searchResultLimit, exportProgressHandler, cancellationToken);
                        break;
                    case LibgenObjectType.FICTION_BOOK:
                        exportTask = mainModel => mainModel.ExportFictionToXlsxAsync(Path.Combine(directory, fileNameWithoutExtension), fileExtension,
                            searchQuery, searchResultLimit, exportProgressHandler, cancellationToken);
                        break;
                    case LibgenObjectType.SCIMAG_ARTICLE:
                        exportTask = mainModel => mainModel.ExportSciMagToXlsxAsync(Path.Combine(directory, fileNameWithoutExtension), fileExtension,
                            searchQuery, searchResultLimit, exportProgressHandler, cancellationToken);
                        break;
                }
            }
            else
            {
                char separator;
                if (IsCommaSeparatorSelected)
                {
                    separator = ',';
                }
                else if (IsSemicolonSeparatorSelected)
                {
                    separator = ';';
                }
                else
                {
                    separator = '\t';
                }
                switch (libgenObjectType)
                {
                    case LibgenObjectType.NON_FICTION_BOOK:
                        exportTask = mainModel => mainModel.ExportNonFictionToCsvAsync(Path.Combine(directory, fileNameWithoutExtension), fileExtension,
                            separator, searchQuery, searchResultLimit, exportProgressHandler, cancellationToken);
                        break;
                    case LibgenObjectType.FICTION_BOOK:
                        exportTask = mainModel => mainModel.ExportFictionToCsvAsync(Path.Combine(directory, fileNameWithoutExtension), fileExtension,
                            separator, searchQuery, searchResultLimit, exportProgressHandler, cancellationToken);
                        break;
                    case LibgenObjectType.SCIMAG_ARTICLE:
                        exportTask = mainModel => mainModel.ExportSciMagToCsvAsync(Path.Combine(directory, fileNameWithoutExtension), fileExtension,
                            separator, searchQuery, searchResultLimit, exportProgressHandler, cancellationToken);
                        break;
                }
            }
            if (exportTask == null)
            {
                throw new Exception("Export task is null.");
            }
            IsExportInProgress = true;
            UpdateProgressStatus(null, 0, 0);
            IsSettingsPanelVisible = false;
            IsProgressPanelVisible = true;
            AreExportResultButtonsVisible = false;
            IsCancelExportButtonEnabled = true;
            IsCancelExportButtonVisible = true;
            SaveSettings();
            bool error = false;
            exportResult = null;
            try
            {
                exportResult = await exportTask(MainModel);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, parentWindowContext);
                error = true;
            }
            IsCancelExportButtonVisible = false;
            if (!error)
            {
                if (exportResult.IsExportCancelled && exportResult.ItemsExported == 0)
                {
                    IsSettingsPanelVisible = true;
                    IsProgressPanelVisible = false;
                }
                else
                {
                    string status = null;
                    if (exportResult.IsExportCancelled || exportResult.IsRowsPerFileLimitReached)
                    {
                        status = Localization.ExportInterrupted;
                    }
                    UpdateProgressStatus(status, exportResult.ItemsExported, exportResult.FilesCreated);
                    if (exportResult.IsRowsPerFileLimitReached)
                    {
                        ShowMessage(Localization.RowLimitWarningTitle, Localization.RowLimitWarningText, parentWindowContext);
                    }
                    AreExportResultButtonsVisible = true;
                    IsShowResultButtonVisible = exportResult.ItemsExported > 0 && exportResult.FirstFilePath != null;
                    if (IsShowResultButtonVisible && MainModel.AppSettings.Export.OpenResultsAfterExport && !exportResult.IsExportCancelled &&
                        !exportResult.IsRowsPerFileLimitReached)
                    {
                        ShowResult();
                    }
                }
            }
            else
            {
                ProgressStatus = Localization.ExportError;
                AreExportResultButtonsVisible = true;
                IsShowResultButtonVisible = false;
            }
            IsExportInProgress = false;
        }

        private void HandleExportProgress(ExportProgress exportProgress)
        {
            string status = exportProgress.IsWriterDisposing ? Localization.SavingFile : null;
            UpdateProgressStatus(status, exportProgress.ItemsExported, exportProgress.FilesCreated);
        }

        private void UpdateProgressStatus(string status, int itemsExported, int filesCreated)
        {
            StringBuilder statusBuilder = new StringBuilder();
            if (status != null)
            {
                statusBuilder.Append(status);
                statusBuilder.Append(" ");
            }
            if (filesCreated > 1)
            {
                statusBuilder.Append(Localization.GetRowCountMultipleFilesString(itemsExported, filesCreated));
            }
            else
            {
                statusBuilder.Append(Localization.GetRowCountSingleFileString(itemsExported));
            }
            ProgressStatus = statusBuilder.ToString();
        }

        private void SaveSettings()
        {
            AppSettings.ExportPanelSettngs exportPanelSettngs = new AppSettings.ExportPanelSettngs
            {
                ExportDirectory = GetDirectoryPath(correctPath: true),
                Format = IsXlsxSelected ? AppSettings.ExportPanelSettngs.ExportFormat.XLSX : AppSettings.ExportPanelSettngs.ExportFormat.CSV,
                LimitSearchResults = IsLimitSelected
            };
            if (IsCommaSeparatorSelected)
            {
                exportPanelSettngs.Separator = AppSettings.ExportPanelSettngs.CsvSeparator.COMMA;
            }
            else if (IsSemicolonSeparatorSelected)
            {
                exportPanelSettngs.Separator = AppSettings.ExportPanelSettngs.CsvSeparator.SEMICOLON;
            }
            else
            {
                exportPanelSettngs.Separator = AppSettings.ExportPanelSettngs.CsvSeparator.TAB;
            }
            switch (libgenObjectType)
            {
                case LibgenObjectType.NON_FICTION_BOOK:
                    MainModel.AppSettings.NonFiction.ExportPanel = exportPanelSettngs;
                    break;
                case LibgenObjectType.FICTION_BOOK:
                    MainModel.AppSettings.Fiction.ExportPanel = exportPanelSettngs;
                    break;
                case LibgenObjectType.SCIMAG_ARTICLE:
                    MainModel.AppSettings.SciMag.ExportPanel = exportPanelSettngs;
                    break;
            }
            MainModel.SaveSettings();
        }

        private void Cancel()
        {
            ClosePanel?.Invoke(this, EventArgs.Empty);
        }

        private void CancelExport()
        {
            IsCancelExportButtonEnabled = false;
            cancellationTokenSource.Cancel();
        }

        private void ShowResult()
        {
            if (exportResult.FilesCreated == 1)
            {
                Process.Start(exportResult.FirstFilePath);
            }
            else
            {
                Process.Start("explorer.exe", $@"/select, ""{exportResult.FirstFilePath}""");
            }
        }

        private void Close()
        {
            ClosePanel?.Invoke(this, EventArgs.Empty);
        }
    }
}
