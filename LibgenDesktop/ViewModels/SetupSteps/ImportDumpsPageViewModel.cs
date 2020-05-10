using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.Models.SqlDump;
using LibgenDesktop.ViewModels.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class ImportDumpsPageViewModel : SetupStepViewModel
    {
        internal enum ImportQueueItemStatus
        {
            NOT_IMPORTED = 1,
            IMPORTING,
            IMPORT_SUCCESSFUL,
            IMPORT_CANCELLED,
            IMPORT_ERROR
        }

        internal class ImportQueueItemViewModel : ViewModel
        {
            private ImportDumpsSetupStepLocalizator localization;
            private string name;
            private ImportQueueItemStatus status;
            private bool isNextInQueue;

            public ImportQueueItemViewModel(SharedSetupContext.Collection collection, ImportDumpsSetupStepLocalizator localization)
            {
                this.localization = localization;
                Collection = collection;
                name = GetImportQueueItemName(collection.Identifier);
                status = collection.IsImported ? ImportQueueItemStatus.IMPORT_SUCCESSFUL : ImportQueueItemStatus.NOT_IMPORTED;
                isNextInQueue = false;
            }

            public SharedSetupContext.Collection Collection { get; }

            public string Name
            {
                get
                {
                    return name;
                }
                private set
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }

            public ImportQueueItemStatus Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                    NotifyPropertyChanged(nameof(StatusText));
                    NotifyPropertyChanged(nameof(ImportButtonText));
                    NotifyPropertyChanged(nameof(IsImportButtonEnabled));
                }
            }

            public string StatusText
            {
                get
                {
                    string statusValueString;
                    switch (Status)
                    {
                        case ImportQueueItemStatus.NOT_IMPORTED:
                            statusValueString = localization.NotImported;
                            break;
                        case ImportQueueItemStatus.IMPORTING:
                            statusValueString = localization.Importing;
                            break;
                        case ImportQueueItemStatus.IMPORT_SUCCESSFUL:
                            statusValueString = localization.ImportSuccessful;
                            break;
                        case ImportQueueItemStatus.IMPORT_CANCELLED:
                            statusValueString = localization.ImportCancelled;
                            break;
                        case ImportQueueItemStatus.IMPORT_ERROR:
                            statusValueString = localization.ImportError;
                            break;
                        default:
                            throw new Exception($"Unexpected import queue item status: {status}.");
                    }
                    return localization.GetStatusString(statusValueString);
                }
            }

            public string ImportButtonText
            {
                get
                {
                    return Status == ImportQueueItemStatus.IMPORTING ? localization.ImportingButton : localization.ImportButton;
                }
            }

            public bool IsImportButtonEnabled
            {
                get
                {
                    return IsNextInQueue && (Status == ImportQueueItemStatus.NOT_IMPORTED || Status == ImportQueueItemStatus.IMPORT_ERROR ||
                        Status == ImportQueueItemStatus.IMPORT_CANCELLED);
                }
            }

            public bool IsNextInQueue
            {
                get
                {
                    return isNextInQueue;
                }
                set
                {
                    isNextInQueue = value;
                    NotifyPropertyChanged(nameof(IsImportButtonEnabled));
                }
            }

            public void UpdateLocalization(ImportDumpsSetupStepLocalizator localization)
            {
                this.localization = localization;
                Name = GetImportQueueItemName(Collection.Identifier);
                NotifyPropertyChanged(nameof(StatusText));
                NotifyPropertyChanged(nameof(ImportButtonText));
            }

            public void SaveStateToCollection()
            {
                Collection.IsImported = Status == ImportQueueItemStatus.IMPORT_SUCCESSFUL;
            }

            private string GetImportQueueItemName(SharedSetupContext.CollectionIdentifier collectionIdentifier)
            {
                switch (collectionIdentifier)
                {
                    case SharedSetupContext.CollectionIdentifier.NON_FICTION:
                        return localization.NonFictionDumpName;
                    case SharedSetupContext.CollectionIdentifier.FICTION:
                        return localization.FictionDumpName;
                    case SharedSetupContext.CollectionIdentifier.SCIMAG:
                        return localization.SciMagArticlesDumpName;
                    default:
                        throw new Exception($"Unexpected collection identifier: {collectionIdentifier}.");
                }
            }
        }

        private bool isHeaderVisible;
        private ObservableCollection<ImportQueueItemViewModel> importQueue;
        private bool isDeleteDumpsChecked;

        public ImportDumpsPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.IMPORT_DUMPS)
        {
            Localization = windowLocalization.ImportDumpsStep;
            foreach (SharedSetupContext.Collection collection in SharedSetupContext.Collections)
            {
                collection.IsImported = false;
            }
            isHeaderVisible = true;
            importQueue = new ObservableCollection<ImportQueueItemViewModel>();
            isDeleteDumpsChecked = false;
            ImportCommand = new Command(param => Import(param as ImportQueueItemViewModel));
        }

        public ImportDumpsSetupStepLocalizator Localization { get; private set; }

        public string Header
        {
            get
            {
                return GetHeaderString(SetupStage.IMPORTING_DUMPS);
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

        public ObservableCollection<ImportQueueItemViewModel> ImportQueue
        {
            get
            {
                return importQueue;
            }
            set
            {
                importQueue = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDeleteDumpsChecked
        {
            get
            {
                return isDeleteDumpsChecked;
            }
            set
            {
                isDeleteDumpsChecked = value;
                NotifyPropertyChanged();
            }
        }

        public Command ImportCommand { get; }

        public override void OnPageEnter()
        {
            base.OnPageEnter();
            IsHeaderVisible = SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC;
            ConvertCollectionsToImportQueueItems();
            IsDeleteDumpsChecked = SharedSetupContext.SelectedDownloadMode == SharedSetupContext.DownloadMode.DOWNLOAD_MANAGER;
            SelectNextImportQueueItem();
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SetupWizardStep.CREATE_DATABASE);
        }

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            SaveImportQueueItemStatesToCollections();
            if (IsDeleteDumpsChecked)
            {
                DeleteDumpFiles();
            }
            MoveToPage(SetupWizardStep.CONFIRMATION);
        }

        public void Import(ImportQueueItemViewModel importQueueItemViewModel)
        {
            string databaseDumpFilePath;
            if (SharedSetupContext.SelectedDownloadMode == SharedSetupContext.DownloadMode.DOWNLOAD_MANAGER)
            {
                databaseDumpFilePath = importQueueItemViewModel.Collection.DownloadFilePath;
            }
            else
            {
                OpenFileDialogResult selectDatabaseDumpFileDialogResult = ImportWindowViewModel.SelectDatabaseDumpFile(MainModel);
                if (!selectDatabaseDumpFileDialogResult.DialogResult)
                {
                    return;
                }
                databaseDumpFilePath = selectDatabaseDumpFileDialogResult.SelectedFilePaths.First();
            }
            importQueueItemViewModel.Status = ImportQueueItemStatus.IMPORTING;
            TableType expectedTableType;
            switch (importQueueItemViewModel.Collection.Identifier)
            {
                case SharedSetupContext.CollectionIdentifier.NON_FICTION:
                    expectedTableType = TableType.NON_FICTION;
                    break;
                case SharedSetupContext.CollectionIdentifier.FICTION:
                    expectedTableType = TableType.FICTION;
                    break;
                case SharedSetupContext.CollectionIdentifier.SCIMAG:
                    expectedTableType = TableType.SCI_MAG;
                    break;
                default:
                    throw new Exception($"Unexpected collection identifier: {importQueueItemViewModel.Collection.Identifier}.");
            }
            ImportWindowViewModel importWindowViewModel = new ImportWindowViewModel(MainModel, databaseDumpFilePath, expectedTableType);
            IWindowContext importWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.IMPORT_WINDOW, importWindowViewModel,
                SetupWizardWindowContext);
            importWindowContext.ShowDialog();
            switch (importWindowViewModel.Status)
            {
                case ImportWindowViewModel.ImportStatus.IMPORT_COMPLETE:
                    importQueueItemViewModel.Status = ImportQueueItemStatus.IMPORT_SUCCESSFUL;
                    SelectNextImportQueueItem();
                    break;
                case ImportWindowViewModel.ImportStatus.IMPORT_CANCELLED:
                    importQueueItemViewModel.Status = ImportQueueItemStatus.IMPORT_CANCELLED;
                    break;
                default:
                    importQueueItemViewModel.Status = ImportQueueItemStatus.IMPORT_ERROR;
                    break;
            }
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.ImportDumpsStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(Header));
        }

        private void SelectNextImportQueueItem()
        {
            foreach (ImportQueueItemViewModel importQueueItemViewModel in ImportQueue)
            {
                importQueueItemViewModel.IsNextInQueue = false;
            }
            ImportQueueItemViewModel nextImportQueueItem =
                ImportQueue.FirstOrDefault(importQueueItem => importQueueItem.Status == ImportQueueItemStatus.NOT_IMPORTED ||
                importQueueItem.Status == ImportQueueItemStatus.IMPORT_CANCELLED || importQueueItem.Status == ImportQueueItemStatus.IMPORT_ERROR);
            if (nextImportQueueItem != null)
            {
                nextImportQueueItem.IsNextInQueue = true;
                HideNextButton();
            }
            else
            {
                ShowNextButton();
            }
        }

        private void ConvertCollectionsToImportQueueItems()
        {
            ImportQueue = new ObservableCollection<ImportQueueItemViewModel>(SharedSetupContext.Collections.Where(collection => collection.IsSelected).
                Select(collection => new ImportQueueItemViewModel(collection, Localization)));
        }

        private void SaveImportQueueItemStatesToCollections()
        {
            foreach (ImportQueueItemViewModel importQueueItemViewModel in ImportQueue)
            {
                importQueueItemViewModel.SaveStateToCollection();
            }
        }

        private void DeleteDumpFiles()
        {
            foreach (ImportQueueItemViewModel importQueueItemViewModel in
                ImportQueue.Where(importQueueItem => importQueueItem.Status == ImportQueueItemStatus.IMPORT_SUCCESSFUL))
            {
                SharedSetupContext.Collection collection = importQueueItemViewModel.Collection;
                if (collection.IsDeleted)
                {
                    File.Delete(importQueueItemViewModel.Collection.DownloadFilePath);
                    collection.IsDeleted = true;
                }
            }
        }
    }
}
