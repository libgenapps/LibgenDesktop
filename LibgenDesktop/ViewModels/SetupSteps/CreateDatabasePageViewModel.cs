using System;
using System.IO;
using System.Text;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using static LibgenDesktop.Common.Constants;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class CreateDatabasePageViewModel : SetupStepViewModel
    {
        private bool isHeaderVisible;
        private string promptText;
        private bool areDatabaseFilePathControlsEnabled;
        private string databaseFilePath;
        private string diskSpaceRequirementsNote;
        private string createDatabaseButtonText;
        private bool isCreateDatabaseButtonEnabled;
        private bool isCreateDatabaseButtonVisible;

        public CreateDatabasePageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.CREATE_DATABASE)
        {
            Localization = windowLocalization.CreateDatabaseStep;
            isHeaderVisible = true;
            promptText = Localization.DatabaseFilePathPrompt;
            areDatabaseFilePathControlsEnabled = true;
            databaseFilePath = Path.Combine(Environment.AppDataDirectory, DEFAULT_DATABASE_FILE_NAME);
            diskSpaceRequirementsNote = String.Empty;
            createDatabaseButtonText = Localization.CreateDatabase;
            isCreateDatabaseButtonEnabled = true;
            isCreateDatabaseButtonVisible = true;
            SelectDatabaseFilePathCommand = new Command(SelectDatabaseFilePath);
            CreateDatabaseCommand = new Command(CreateDatabase);
        }

        public CreateDatabaseSetupStepLocalizator Localization { get; private set; }

        public string Header
        {
            get
            {
                return GetHeaderString(SetupStage.CREATING_DATABASE);
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

        public string PromptText
        {
            get
            {
                return promptText;
            }
            set
            {
                promptText = value;
                NotifyPropertyChanged();
            }
        }

        public bool AreDatabaseFilePathControlsEnabled
        {
            get
            {
                return areDatabaseFilePathControlsEnabled;
            }
            set
            {
                areDatabaseFilePathControlsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string DatabaseFilePath
        {
            get
            {
                return databaseFilePath;
            }
            set
            {
                databaseFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string DiskSpaceRequirementsNote
        {
            get
            {
                return diskSpaceRequirementsNote;
            }
            set
            {
                diskSpaceRequirementsNote = value;
                NotifyPropertyChanged();
            }
        }

        public string CreateDatabaseButtonText
        {
            get
            {
                return createDatabaseButtonText;
            }
            set
            {
                createDatabaseButtonText = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCreateDatabaseButtonEnabled
        {
            get
            {
                return isCreateDatabaseButtonEnabled;
            }
            set
            {
                isCreateDatabaseButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCreateDatabaseButtonVisible
        {
            get
            {
                return isCreateDatabaseButtonVisible;
            }
            set
            {
                isCreateDatabaseButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public Command SelectDatabaseFilePathCommand { get; }
        public Command CreateDatabaseCommand { get; }

        public override void OnPageEnter()
        {
            base.OnPageEnter();
            IsHeaderVisible = SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC;
            UpdateDiskSpaceRequirementsNote();
            if (SharedSetupContext.IsDatabaseCreated)
            {
                PromptText = Localization.DatabaseCreated;
                AreDatabaseFilePathControlsEnabled = false;
                IsCreateDatabaseButtonVisible = false;
                ShowNextButton();
            }
            else
            {
                PromptText = Localization.DatabaseFilePathPrompt;
                AreDatabaseFilePathControlsEnabled = true;
                CreateDatabaseButtonText = Localization.CreateDatabase;
                IsCreateDatabaseButtonEnabled = true;
                IsCreateDatabaseButtonVisible = true;
                HideNextButton();
            }
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SharedSetupContext.SelectedDownloadMode == SharedSetupContext.DownloadMode.DOWNLOAD_MANAGER ?
                SetupWizardStep.DOWNLOAD_DUMPS : SetupWizardStep.DOWNLOAD_DUMP_LINKS);
        }

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            MoveToPage(SetupWizardStep.IMPORT_DUMPS);
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.CreateDatabaseStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(Header));
            UpdateDiskSpaceRequirementsNote();
        }

        private void UpdateDiskSpaceRequirementsNote()
        {
            decimal totalDiskSpaceRequiredInGigabytes = 0;
            if (SharedSetupContext.NonFictionCollection.IsSelected)
            {
                totalDiskSpaceRequiredInGigabytes += NON_FICTION_APPROXIMATE_DATABASE_SIZE_IN_GB;
            }
            if (SharedSetupContext.FictionCollection.IsSelected)
            {
                totalDiskSpaceRequiredInGigabytes += FICTION_APPROXIMATE_DATABASE_SIZE_IN_GB;
            }
            if (SharedSetupContext.SciMagCollection.IsSelected)
            {
                totalDiskSpaceRequiredInGigabytes += SCIMAG_APPROXIMATE_DATABASE_SIZE_IN_GB;
            }
            DiskSpaceRequirementsNote = Localization.GetDiskSpaceRequirementsNoteString(totalDiskSpaceRequiredInGigabytes);
        }

        private void SelectDatabaseFilePath()
        {
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(Localization.Databases);
            filterBuilder.Append(" (*.db)|*.db|");
            filterBuilder.Append(Localization.AllFiles);
            filterBuilder.Append(" (*.*)|*.*");
            SaveFileDialogParameters saveFileDialogParameters = new SaveFileDialogParameters
            {
                DialogTitle = Localization.SelectDatabaseFilePathDialogTitle,
                Filter = filterBuilder.ToString(),
                OverwritePrompt = true
            };
            saveFileDialogParameters.InitialDirectory = Path.GetDirectoryName(DatabaseFilePath);
            saveFileDialogParameters.InitialFileName = Path.GetFileName(DatabaseFilePath);
            SaveFileDialogResult saveFileDialogResult = WindowManager.ShowSaveFileDialog(saveFileDialogParameters);
            if (saveFileDialogResult.DialogResult)
            {
                DatabaseFilePath = saveFileDialogResult.SelectedFilePath;
            }
        }

        private async void CreateDatabase()
        {
            if (File.Exists(DatabaseFilePath) && !ShowPrompt(Localization.DatabaseFileOverwritePromptTitle,
                Localization.GetDatabaseFileOverwritePromptTextString(DatabaseFilePath)))
            {
                return;
            }
            CreateDatabaseButtonText = Localization.CreatingDatabase;
            IsCreateDatabaseButtonEnabled = false;
            if (await MainModel.CreateDatabaseAsync(DatabaseFilePath))
            {
                MainModel.AppSettings.DatabaseFileName = MainModel.GetDatabaseNormalizedPath(DatabaseFilePath);
                MainModel.SaveSettings();
                SharedSetupContext.IsDatabaseCreated = true;
                PromptText = Localization.DatabaseCreated;
                AreDatabaseFilePathControlsEnabled = false;
                IsCreateDatabaseButtonVisible = false;
                ShowNextButton();
            }
            else
            {
                PromptText = Localization.CannotCreateDatabase;
                CreateDatabaseButtonText = Localization.CreateDatabase;
                IsCreateDatabaseButtonEnabled = true;
            }
        }
    }
}
