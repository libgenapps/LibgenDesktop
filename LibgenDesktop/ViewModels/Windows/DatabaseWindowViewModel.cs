using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class DatabaseWindowViewModel : LibgenWindowViewModel
    {
        private readonly LanguageFormatter formatter;
        private bool isCreatingIndexesMessageVisible;
        private bool areDatabaseStatsVisible;
        private string nonFictionTotalBooks;
        private string nonFictionLastUpdate;
        private string fictionTotalBooks;
        private string fictionLastUpdate;
        private string sciMagTotalArticles;
        private string sciMagLastUpdate;
        private string databaseFilePath;
        private bool isDatabaseOperationInProgress;

        public DatabaseWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            Localization = mainModel.Localization.CurrentLanguage.Database;
            formatter = mainModel.Localization.CurrentLanguage.Formatter;
            WindowClosingCommand = new FuncCommand<bool?, bool>(WindowClosing);
            ChangeDatabaseCommand = new Command(ChangeDatabase);
            CloseButtonCommand = new Command(CloseButtonClick);
            GetStats();
        }

        public DatabaseWindowLocalizator Localization { get; }

        public bool IsCreatingIndexesMessageVisible
        {
            get
            {
                return isCreatingIndexesMessageVisible;
            }
            set
            {
                isCreatingIndexesMessageVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool AreDatabaseStatsVisible
        {
            get
            {
                return areDatabaseStatsVisible;
            }
            set
            {
                areDatabaseStatsVisible = value;
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

        public string NonFictionTotalBooks
        {
            get
            {
                return nonFictionTotalBooks;
            }
            set
            {
                nonFictionTotalBooks = value;
                NotifyPropertyChanged();
            }
        }

        public string NonFictionLastUpdate
        {
            get
            {
                return nonFictionLastUpdate;
            }
            set
            {
                nonFictionLastUpdate = value;
                NotifyPropertyChanged();
            }
        }

        public string FictionTotalBooks
        {
            get
            {
                return fictionTotalBooks;
            }
            set
            {
                fictionTotalBooks = value;
                NotifyPropertyChanged();
            }
        }

        public string FictionLastUpdate
        {
            get
            {
                return fictionLastUpdate;
            }
            set
            {
                fictionLastUpdate = value;
                NotifyPropertyChanged();
            }
        }

        public string SciMagTotalArticles
        {
            get
            {
                return sciMagTotalArticles;
            }
            set
            {
                sciMagTotalArticles = value;
                NotifyPropertyChanged();
            }
        }

        public string SciMagLastUpdate
        {
            get
            {
                return sciMagLastUpdate;
            }
            set
            {
                sciMagLastUpdate = value;
                NotifyPropertyChanged();
            }
        }

        public FuncCommand<bool?, bool> WindowClosingCommand { get; }
        public Command ChangeDatabaseCommand { get; }
        public Command CloseButtonCommand { get; }

        public static async Task<bool> OpenDatabase(MainModel mainModel, IWindowContext parentWindowContext)
        {
            DatabaseWindowLocalizator databaseLocalizator = mainModel.Localization.CurrentLanguage.Database;
            MessageBoxLocalizator messageBoxLocalizator = mainModel.Localization.CurrentLanguage.MessageBox;
            OpenFileDialogResult openFileDialogResult = SelectDatabaseFile(mainModel);
            if (openFileDialogResult.DialogResult)
            {
                string databaseFilePath = openFileDialogResult.SelectedFilePaths.First();
                MainModel.OpenDatabaseOptions openDatabaseOptions = MainModel.OpenDatabaseOptions.NONE;
                MainModel.DatabaseStatus databaseStatus;
                bool stopOpenAttempts = false;
                bool openedSuccessfully = false;
                while (!stopOpenAttempts)
                {
                    databaseStatus = await mainModel.OpenDatabase(databaseFilePath, openDatabaseOptions);
                    switch (databaseStatus)
                    {
                        case MainModel.DatabaseStatus.OPENED:
                            mainModel.AppSettings.DatabaseFileName = MainModel.GetDatabaseNormalizedPath(databaseFilePath);
                            mainModel.SaveSettings();
                            openedSuccessfully = true;
                            stopOpenAttempts = true;
                            break;
                        case MainModel.DatabaseStatus.POSSIBLE_DUMP_FILE:
                            WindowManager.ShowMessage(databaseLocalizator.Error, databaseLocalizator.GetDatabaseDumpFileText(databaseFilePath),
                                messageBoxLocalizator.Ok, parentWindowContext);
                            stopOpenAttempts = true;
                            break;
                        case MainModel.DatabaseStatus.OLD_FICTION_SCHEMA:
                            if (WindowManager.ShowPrompt(databaseLocalizator.OldFictionSchemaTitle, databaseLocalizator.GetOldFictionSchemaText(databaseFilePath),
                                messageBoxLocalizator.Yes, messageBoxLocalizator.No, parentWindowContext))
                            {
                                openDatabaseOptions = MainModel.OpenDatabaseOptions.MIGRATE_FICTION;
                            }
                            else
                            {
                                stopOpenAttempts = true;
                            }
                            break;
                        case MainModel.DatabaseStatus.CORRUPTED:
                            WindowManager.ShowMessage(databaseLocalizator.Error, databaseLocalizator.GetDatabaseNotValidText(databaseFilePath),
                                messageBoxLocalizator.Ok, parentWindowContext);
                            stopOpenAttempts = true;
                            break;
                        case MainModel.DatabaseStatus.SERVER_DATABASE:
                            WindowManager.ShowMessage(databaseLocalizator.Error, databaseLocalizator.GetLibgenServerDatabaseText(databaseFilePath),
                                messageBoxLocalizator.Ok, parentWindowContext);
                            stopOpenAttempts = true;
                            break;
                        default:
                            throw new Exception($"Unexpected database status: {databaseStatus}.");
                    }
                }
                if (openedSuccessfully)
                {
                    return true;
                }
                else
                {
                    await mainModel.OpenDatabase(mainModel.AppSettings.DatabaseFileName);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static OpenFileDialogResult SelectDatabaseFile(MainModel mainModel)
        {
            DatabaseWindowLocalizator databaseLocalizator = mainModel.Localization.CurrentLanguage.Database;
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(databaseLocalizator.Databases);
            filterBuilder.Append(" (*.db)|*.db|");
            filterBuilder.Append(databaseLocalizator.AllFiles);
            filterBuilder.Append(" (*.*)|*.*");
            OpenFileDialogParameters openFileDialogParameters = new OpenFileDialogParameters
            {
                DialogTitle = databaseLocalizator.BrowseDatabaseDialogTitle,
                Filter = filterBuilder.ToString(),
                Multiselect = false
            };
            return WindowManager.ShowOpenFileDialog(openFileDialogParameters);
        }

        private async void GetStats()
        {
            isDatabaseOperationInProgress = true; 
            AreDatabaseStatsVisible = false;
            DatabaseStats databaseStats;
            try
            {
                bool databaseStatsIndexesCreated = await MainModel.CheckIfDatabaseStatsIndexesCreated();
                if (!databaseStatsIndexesCreated)
                {
                    if (!ShowPrompt(Localization.IndexesRequiredTitle, Localization.IndexesRequiredText))
                    {
                        isDatabaseOperationInProgress = false;
                        CurrentWindowContext.CloseDialog(false);
                        return;
                    }
                }
                IsCreatingIndexesMessageVisible = true;
                databaseStats = await MainModel.GetDatabaseStatsAsync();
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, CurrentWindowContext);
                isDatabaseOperationInProgress = false;
                CurrentWindowContext.CloseDialog(false);
                return;
            }
            DatabaseFilePath = MainModel.GetDatabaseFullPath(MainModel.AppSettings.DatabaseFileName);
            NonFictionTotalBooks = formatter.ToFormattedString(databaseStats.NonFictionBookCount);
            NonFictionLastUpdate = databaseStats.NonFictionLastUpdate.HasValue ?
                formatter.ToFormattedDateTimeString(databaseStats.NonFictionLastUpdate.Value) : Localization.Never;
            FictionTotalBooks = formatter.ToFormattedString(databaseStats.FictionBookCount);
            FictionLastUpdate = databaseStats.FictionLastUpdate.HasValue ? formatter.ToFormattedDateTimeString(databaseStats.FictionLastUpdate.Value) :
                Localization.Never;
            SciMagTotalArticles = formatter.ToFormattedString(databaseStats.SciMagArticleCount);
            SciMagLastUpdate = databaseStats.SciMagLastUpdate.HasValue ? formatter.ToFormattedDateTimeString(databaseStats.SciMagLastUpdate.Value) :
                Localization.Never;
            IsCreatingIndexesMessageVisible = false;
            AreDatabaseStatsVisible = true;
            isDatabaseOperationInProgress = false;
        }

        private async void ChangeDatabase()
        {
            if (await OpenDatabase(MainModel, CurrentWindowContext))
            {
                GetStats();
            }
        }

        private bool WindowClosing(bool? dialogResult)
        {
            return !isDatabaseOperationInProgress;
        }

        private void CloseButtonClick()
        {
            CurrentWindowContext.CloseDialog(false);
        }
    }
}
