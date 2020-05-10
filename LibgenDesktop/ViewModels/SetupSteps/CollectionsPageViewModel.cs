using System;
using System.IO;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using static LibgenDesktop.Common.Constants;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class CollectionsPageViewModel : SetupStepViewModel
    {
        private bool isHeaderVisible;
        private bool isNonFictionCollectionSelected;
        private string nonFictionDownloadSize;
        private string nonFictionImportTime;
        private string nonFictionDatabaseSize;
        private bool isFictionCollectionSelected;
        private string fictionDownloadSize;
        private string fictionImportTime;
        private string fictionDatabaseSize;
        private bool isSciMagCollectionSelected;
        private string sciMagDownloadSize;
        private string sciMagImportTime;
        private string sciMagDatabaseSize;
        private bool isDownloadDirectoryPanelVisible;
        private string downloadDirectory;
        private bool isDefaultDownloadDirectory;

        public CollectionsPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.COLLECTIONS)
        {
            Localization = windowLocalization.CollectionsStep;
            isHeaderVisible = true;
            isNonFictionCollectionSelected = false;
            isFictionCollectionSelected = false;
            isSciMagCollectionSelected = false;
            UpdateDownloadSizes();
            UpdateImportTimes();
            UpdateDatabaseSizes();
            isDownloadDirectoryPanelVisible = false;
            downloadDirectory = Environment.TempDirectoryPath;
            isDefaultDownloadDirectory = true;
            NonFictionCollectionDetailsClickCommand = new Command(NonFictionCollectionDetailsClick);
            FictionCollectionDetailsClickCommand = new Command(FictionCollectionDetailsClick);
            SciMagCollectionDetailsClickCommand = new Command(SciMagCollectionDetailsClick);
            SelectDownloadDirectoryCommand = new Command(SelectDownloadDirectory);
        }

        public CollectionsSetupStepLocalizator Localization { get; private set; }

        public string Header
        {
            get
            {
                return GetHeaderString(SetupStage.DOWNLOADING_DUMPS);
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

        public bool IsNonFictionCollectionSelected
        {
            get
            {
                return isNonFictionCollectionSelected;
            }
            set
            {
                isNonFictionCollectionSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string NonFictionDownloadSize
        {
            get
            {
                return nonFictionDownloadSize;
            }
            set
            {
                nonFictionDownloadSize = value;
                NotifyPropertyChanged();
            }
        }

        public string NonFictionImportTime
        {
            get
            {
                return nonFictionImportTime;
            }
            set
            {
                nonFictionImportTime = value;
                NotifyPropertyChanged();
            }
        }

        public string NonFictionDatabaseSize
        {
            get
            {
                return nonFictionDatabaseSize;
            }
            set
            {
                nonFictionDatabaseSize = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsFictionCollectionSelected
        {
            get
            {
                return isFictionCollectionSelected;
            }
            set
            {
                isFictionCollectionSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string FictionDownloadSize
        {
            get
            {
                return fictionDownloadSize;
            }
            set
            {
                fictionDownloadSize = value;
                NotifyPropertyChanged();
            }
        }

        public string FictionImportTime
        {
            get
            {
                return fictionImportTime;
            }
            set
            {
                fictionImportTime = value;
                NotifyPropertyChanged();
            }
        }

        public string FictionDatabaseSize
        {
            get
            {
                return fictionDatabaseSize;
            }
            set
            {
                fictionDatabaseSize = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSciMagCollectionSelected
        {
            get
            {
                return isSciMagCollectionSelected;
            }
            set
            {
                isSciMagCollectionSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string SciMagDownloadSize
        {
            get
            {
                return sciMagDownloadSize;
            }
            set
            {
                sciMagDownloadSize = value;
                NotifyPropertyChanged();
            }
        }

        public string SciMagImportTime
        {
            get
            {
                return sciMagImportTime;
            }
            set
            {
                sciMagImportTime = value;
                NotifyPropertyChanged();
            }
        }

        public string SciMagDatabaseSize
        {
            get
            {
                return sciMagDatabaseSize;
            }
            set
            {
                sciMagDatabaseSize = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDownloadDirectoryPanelVisible
        {
            get
            {
                return isDownloadDirectoryPanelVisible;
            }
            set
            {
                isDownloadDirectoryPanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string DownloadDirectory
        {
            get
            {
                return downloadDirectory;
            }
            set
            {
                downloadDirectory = value;
                NotifyPropertyChanged();
            }
        }

        public Command NonFictionCollectionDetailsClickCommand { get; }
        public Command FictionCollectionDetailsClickCommand { get; }
        public Command SciMagCollectionDetailsClickCommand { get; }
        public Command SelectDownloadDirectoryCommand { get; }

        public override void OnPageEnter()
        {
            base.OnPageEnter();
            IsHeaderVisible = SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC;
            UpdateDownloadSizes();
            IsDownloadDirectoryPanelVisible = SharedSetupContext.SelectedDownloadMode == SharedSetupContext.DownloadMode.DOWNLOAD_MANAGER;
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SetupWizardStep.DOWNLOAD_MODE);
        }

        public override void OnNextButtonClick()
        {
            if (!IsNonFictionCollectionSelected && !IsFictionCollectionSelected && !IsSciMagCollectionSelected)
            {
                ShowMessage(Localization.ErrorWarningTitle, Localization.NoCollectionsSelected);
            }
            else if (!isDefaultDownloadDirectory && !Directory.Exists(DownloadDirectory))
            {
                ShowMessage(Localization.ErrorWarningTitle, Localization.GetDirectoryNotFoundString(DownloadDirectory));
            }
            else
            {
                base.OnNextButtonClick();
                SharedSetupContext.NonFictionCollection.IsSelected = IsNonFictionCollectionSelected;
                SharedSetupContext.FictionCollection.IsSelected = IsFictionCollectionSelected;
                SharedSetupContext.SciMagCollection.IsSelected = IsSciMagCollectionSelected;
                if (SharedSetupContext.SelectedDownloadMode == SharedSetupContext.DownloadMode.DOWNLOAD_MANAGER)
                {
                    if (isDefaultDownloadDirectory && !Directory.Exists(DownloadDirectory))
                    {
                        Directory.CreateDirectory(DownloadDirectory);
                    }
                    LibgenDumpDownloader.Dumps dumps = SharedSetupContext.DumpsMetadata;
                    SharedSetupContext.NonFictionCollection.DownloadFilePath = Path.Combine(DownloadDirectory, dumps.NonFiction.FileName);
                    SharedSetupContext.FictionCollection.DownloadFilePath = Path.Combine(DownloadDirectory, dumps.Fiction.FileName);
                    SharedSetupContext.SciMagCollection.DownloadFilePath = Path.Combine(DownloadDirectory, dumps.SciMag.FileName);
                    MoveToPage(SetupWizardStep.DOWNLOAD_DUMPS);
                }
                else
                {
                    MoveToPage(SetupWizardStep.DOWNLOAD_DUMP_LINKS);
                }
            }
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.CollectionsStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(Header));
            UpdateDownloadSizes();
            UpdateImportTimes();
            UpdateDatabaseSizes();
        }

        private void UpdateDownloadSizes()
        {
            LibgenDumpDownloader.Dumps databaseDumpsMetadata = SharedSetupContext?.DumpsMetadata;
            if (databaseDumpsMetadata != null)
            {
                NonFictionDownloadSize = Localization.GetDownloadSizeExactString(databaseDumpsMetadata.NonFiction.RoundedSize,
                    RoundedSizeUnitToString(databaseDumpsMetadata.NonFiction.RoundedSizeUnit));
                FictionDownloadSize = Localization.GetDownloadSizeExactString(databaseDumpsMetadata.Fiction.RoundedSize,
                    RoundedSizeUnitToString(databaseDumpsMetadata.Fiction.RoundedSizeUnit));
                SciMagDownloadSize = Localization.GetDownloadSizeExactString(databaseDumpsMetadata.SciMag.RoundedSize,
                    RoundedSizeUnitToString(databaseDumpsMetadata.SciMag.RoundedSizeUnit));
            }
            else
            {
                LanguageFormatter languageFormatter = MainModel.Localization.CurrentLanguage.Formatter;
                NonFictionDownloadSize = Localization.GetDownloadSizeApproximateString(NON_FICTION_APPROXIMATE_DOWNLOAD_SIZE_IN_MB,
                    languageFormatter.MegabytePostfix);
                FictionDownloadSize = Localization.GetDownloadSizeApproximateString(FICTION_APPROXIMATE_DOWNLOAD_SIZE_IN_MB,
                    languageFormatter.MegabytePostfix);
                SciMagDownloadSize = Localization.GetDownloadSizeApproximateString(SCIMAG_APPROXIMATE_DOWNLOAD_SIZE_IN_GB,
                    languageFormatter.GigabytePostfix);
            }
        }

        private void UpdateImportTimes()
        {
            NonFictionImportTime = Localization.GetImportTimeInMinutesString(NON_FICTION_IMPORT_TIME_IN_MINUTES_FROM, NON_FICTION_IMPORT_TIME_IN_MINUTES_TO);
            FictionImportTime = Localization.GetImportTimeInMinutesString(FICTION_IMPORT_TIME_IN_MINUTES_FROM, FICTION_IMPORT_TIME_IN_MINUTES_TO);
            SciMagImportTime = Localization.GetImportTimeInHoursString(SCIMAG_IMPORT_TIME_IN_HOURS_FROM, SCIMAG_IMPORT_TIME_IN_HOURS_TO);
        }

        private void UpdateDatabaseSizes()
        {
            string gigabytePostfix = MainModel.Localization.CurrentLanguage.Formatter.GigabytePostfix;
            NonFictionDatabaseSize = Localization.GetDatabaseSizeString(NON_FICTION_APPROXIMATE_DATABASE_SIZE_IN_GB, gigabytePostfix);
            FictionDatabaseSize = Localization.GetDatabaseSizeString(FICTION_APPROXIMATE_DATABASE_SIZE_IN_GB, gigabytePostfix);
            SciMagDatabaseSize = Localization.GetDatabaseSizeString(SCIMAG_APPROXIMATE_DATABASE_SIZE_IN_GB, gigabytePostfix);
        }

        private void NonFictionCollectionDetailsClick()
        {
            IsNonFictionCollectionSelected = !IsNonFictionCollectionSelected;
        }

        private void FictionCollectionDetailsClick()
        {
            IsFictionCollectionSelected = !IsFictionCollectionSelected;
        }

        private void SciMagCollectionDetailsClick()
        {
            IsSciMagCollectionSelected = !IsSciMagCollectionSelected;
        }

        private void SelectDownloadDirectory()
        {
            SelectFolderDialogParameters selectFolderDialogParameters = new SelectFolderDialogParameters
            {
                DialogTitle = Localization.BrowseDirectoryDialogTitle,
                InitialDirectory = DownloadDirectory
            };
            SelectFolderDialogResult selectFolderDialogResult = WindowManager.ShowSelectFolderDialog(selectFolderDialogParameters);
            if (selectFolderDialogResult.DialogResult)
            {
                DownloadDirectory = selectFolderDialogResult.SelectedFolderPath;
                isDefaultDownloadDirectory = false;
            }
        }
    }
}
