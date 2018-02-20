using System;
using System.IO;
using System.Linq;
using System.Text;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class CreateDatabaseWindowViewModel : LibgenWindowViewModel
    {
        internal enum EventType
        {
            DATABASE_NOT_FOUND = 1,
            DATABASE_CORRUPTED,
            FIRST_RUN
        }

        private EventType eventType;
        private string header;
        private bool isCreateDatabaseSelected;
        private bool isOpenDatabaseSelected;

        public CreateDatabaseWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            Localization = mainModel.Localization.CurrentLanguage.DatabaseWindow;
            OkButtonCommand = new Command(OkButtonClick);
            CancelButtonCommand = new Command(CancelButtonClick);
            Initialize();
        }

        public DatabaseWindowLocalizator Localization { get; }

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

        public bool IsCreateDatabaseSelected
        {
            get
            {
                return isCreateDatabaseSelected;
            }
            set
            {
                isCreateDatabaseSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsOpenDatabaseSelected
        {
            get
            {
                return isOpenDatabaseSelected;
            }
            set
            {
                isOpenDatabaseSelected = value;
                NotifyPropertyChanged();
            }
        }

        public Command OkButtonCommand { get; }
        public Command CancelButtonCommand { get; }

        private void Initialize()
        {
            UpdateHeaderAndEventType();
            isCreateDatabaseSelected = true;
            isOpenDatabaseSelected = false;
        }

        private void UpdateHeaderAndEventType(string databaseFileName = null)
        {
            switch (MainModel.LocalDatabaseStatus)
            {
                case MainModel.DatabaseStatus.NOT_SET:
                    eventType = EventType.FIRST_RUN;
                    header = Localization.FirstRunMessage;
                    break;
                case MainModel.DatabaseStatus.NOT_FOUND:
                    eventType = EventType.DATABASE_NOT_FOUND;
                    header = Localization.GetDatabaseNotFoundText(databaseFileName ?? GetDatabaseFileName());
                    break;
                case MainModel.DatabaseStatus.CORRUPTED:
                    eventType = EventType.DATABASE_CORRUPTED;
                    header = Localization.GetDatabaseCorruptedText(databaseFileName ?? GetDatabaseFileName());
                    break;
                default:
                    throw new Exception($"Unexpected database status: {MainModel.LocalDatabaseStatus}.");
            }
            header += " " + Localization.ChooseOption + ":";
        }

        private string GetDatabaseFileName()
        {
            return Path.GetFileName(MainModel.AppSettings.DatabaseFileName);
        }

        private void OkButtonClick()
        {
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(Localization.Databases);
            filterBuilder.Append(" (*.db)|*.db|");
            filterBuilder.Append(Localization.AllFiles);
            filterBuilder.Append(" (*.*)|*.*");
            if (IsCreateDatabaseSelected)
            {
                SaveFileDialogParameters saveFileDialogParameters = new SaveFileDialogParameters
                {
                    DialogTitle = Localization.BrowseNewDatabaseDialogTitle,
                    Filter = filterBuilder.ToString(),
                    OverwritePrompt = true
                };
                if (eventType == EventType.DATABASE_CORRUPTED)
                {
                    string databaseFilePath = MainModel.GetDatabaseFullPath(MainModel.AppSettings.DatabaseFileName);
                    saveFileDialogParameters.InitialDirectory = Path.GetDirectoryName(databaseFilePath);
                    saveFileDialogParameters.InitialFileName = Path.GetFileName(databaseFilePath);
                }
                else
                {
                    saveFileDialogParameters.InitialDirectory = Environment.AppDataDirectory;
                    saveFileDialogParameters.InitialFileName = Constants.DEFAULT_DATABASE_FILE_NAME;
                }
                SaveFileDialogResult saveFileDialogResult = WindowManager.ShowSaveFileDialog(saveFileDialogParameters);
                if (saveFileDialogResult.DialogResult)
                {
                    if (MainModel.CreateDatabase(saveFileDialogResult.SelectedFilePath))
                    {
                        MainModel.AppSettings.DatabaseFileName = MainModel.GetDatabaseNormalizedPath(saveFileDialogResult.SelectedFilePath);
                        MainModel.SaveSettings();
                        CurrentWindowContext.CloseDialog(true);
                    }
                    else
                    {
                        ShowMessage(Localization.Error, Localization.CannotCreateDatabase);
                    }
                }
            }
            else
            {
                OpenFileDialogParameters openFileDialogParameters = new OpenFileDialogParameters
                {
                    DialogTitle = Localization.BrowseExistingDatabaseDialogTitle,
                    Filter = filterBuilder.ToString(),
                    Multiselect = false
                };
                OpenFileDialogResult openFileDialogResult = WindowManager.ShowOpenFileDialog(openFileDialogParameters);
                if (openFileDialogResult.DialogResult)
                {
                    string databaseFilePath = openFileDialogResult.SelectedFilePaths.First();
                    if (MainModel.OpenDatabase(databaseFilePath))
                    {
                        MainModel.AppSettings.DatabaseFileName = MainModel.GetDatabaseNormalizedPath(databaseFilePath);
                        MainModel.SaveSettings();
                        CurrentWindowContext.CloseDialog(true);
                    }
                    else
                    {
                        UpdateHeaderAndEventType(Path.GetFileName(databaseFilePath));
                        NotifyPropertyChanged(nameof(Header));
                    }
                }
            }
        }

        private void CancelButtonClick()
        {
            CurrentWindowContext.CloseDialog(false);
        }
    }
}
