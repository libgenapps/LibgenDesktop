using System;
using System.IO;
using System.Linq;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Views;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.ViewModels
{
    internal class CreateDatabaseWindowViewModel : LibgenWindowViewModel
    {
        internal enum EventType
        {
            DATABASE_NOT_FOUND = 1,
            DATABASE_CORRUPTED,
            FIRST_RUN
        }

        private readonly MainModel mainModel;
        private EventType eventType;
        private string header;
        private bool isCreateDatabaseSelected;
        private bool isOpenDatabaseSelected;

        public CreateDatabaseWindowViewModel(MainModel mainModel)
        {
            this.mainModel = mainModel;
            OkButtonCommand = new Command(OkButtonClick);
            CancelButtonCommand = new Command(CancelButtonClick);
            Initialize();
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
            switch (mainModel.LocalDatabaseStatus)
            {
                case MainModel.DatabaseStatus.NOT_SET:
                    eventType = EventType.FIRST_RUN;
                    header = "Добро пожаловать в Libgen Desktop";
                    break;
                case MainModel.DatabaseStatus.NOT_FOUND:
                    eventType = EventType.DATABASE_NOT_FOUND;
                    header = $"База данных {databaseFileName ?? GetDatabaseFileName()} не найдена";
                    break;
                case MainModel.DatabaseStatus.CORRUPTED:
                    eventType = EventType.DATABASE_CORRUPTED;
                    header = $"База данных {databaseFileName ?? GetDatabaseFileName()} повреждена";
                    break;
                default:
                    throw new Exception($"Unexpected database status: {mainModel.LocalDatabaseStatus}.");
            }
            header += ". Выберите действие:";
        }

        private string GetDatabaseFileName()
        {
            return Path.GetFileName(mainModel.AppSettings.DatabaseFileName);
        }

        private void OkButtonClick()
        {
            if (IsCreateDatabaseSelected)
            {
                SaveFileDialogParameters saveFileDialogParameters = new SaveFileDialogParameters
                {
                    DialogTitle = "Сохранение новой базы данных",
                    Filter = "Базы данных (*.db)|*.db|Все файлы (*.*)|*.*",
                    OverwritePrompt = true
                };
                if (eventType == EventType.DATABASE_CORRUPTED)
                {
                    string databaseFilePath = mainModel.GetDatabaseFullPath(mainModel.AppSettings.DatabaseFileName);
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
                    if (mainModel.CreateDatabase(saveFileDialogResult.SelectedFilePath))
                    {
                        mainModel.AppSettings.DatabaseFileName = mainModel.GetDatabaseNormalizedPath(saveFileDialogResult.SelectedFilePath);
                        mainModel.SaveSettings();
                        CurrentWindowContext.CloseDialog(true);
                    }
                    else
                    {
                        MessageBoxWindow.ShowMessage("Ошибка", "Не удалось создать базу данных.", CurrentWindowContext);
                    }
                }
            }
            else
            {
                OpenFileDialogParameters openFileDialogParameters = new OpenFileDialogParameters
                {
                    DialogTitle = "Выбор базы данных",
                    Filter = "Базы данных (*.db)|*.db|Все файлы (*.*)|*.*",
                    Multiselect = false
                };
                OpenFileDialogResult openFileDialogResult = WindowManager.ShowOpenFileDialog(openFileDialogParameters);
                if (openFileDialogResult.DialogResult)
                {
                    string databaseFilePath = openFileDialogResult.SelectedFilePaths.First();
                    if (mainModel.OpenDatabase(databaseFilePath))
                    {
                        mainModel.AppSettings.DatabaseFileName = mainModel.GetDatabaseNormalizedPath(databaseFilePath);
                        mainModel.SaveSettings();
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
