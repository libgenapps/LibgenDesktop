using System;
using System.Linq;
using System.Text;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;

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

        public DatabaseWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            Localization = mainModel.Localization.CurrentLanguage.Database;
            formatter = mainModel.Localization.CurrentLanguage.Formatter;
            WindowClosingCommand = new FuncCommand<bool>(WindowClosing);
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

        public FuncCommand<bool> WindowClosingCommand { get; }
        public Command ChangeDatabaseCommand { get; }
        public Command CloseButtonCommand { get; }

        private async void GetStats()
        {
            IsCreatingIndexesMessageVisible = true;
            AreDatabaseStatsVisible = false;
            DatabaseStats databaseStats;
            try
            {
                databaseStats = await MainModel.GetDatabaseStatsAsync();
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, CurrentWindowContext);
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
        }

        private void ChangeDatabase()
        {
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(Localization.Databases);
            filterBuilder.Append(" (*.db)|*.db|");
            filterBuilder.Append(Localization.AllFiles);
            filterBuilder.Append(" (*.*)|*.*");
            OpenFileDialogParameters openFileDialogParameters = new OpenFileDialogParameters
            {
                DialogTitle = Localization.BrowseDatabaseDialogTitle,
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
                    GetStats();
                }
                else
                {
                    ShowMessage(Localization.Error, Localization.GetCannotOpenDatabase(databaseFilePath));
                    MainModel.OpenDatabase(MainModel.AppSettings.DatabaseFileName);
                }
            }
        }

        private bool WindowClosing()
        {
            return !IsCreatingIndexesMessageVisible;
        }

        private void CloseButtonClick()
        {
            CurrentWindowContext.CloseDialog(false);
        }
    }
}
