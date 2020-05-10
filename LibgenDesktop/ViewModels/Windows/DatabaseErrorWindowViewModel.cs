using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class DatabaseErrorWindowViewModel : LibgenWindowViewModel
    {
        internal enum OptionSet
        {
            DATABASE_NOT_FOUND = 1,
            DATABASE_NOT_VALID,
            DATABASE_DUMP_FILE,
            SERVER_DATABASE,
            OLD_FICTION_SCHEMA
        }

        internal enum DatabaseErrorWindowResult
        {
            OPEN_ANOTHER_DATABASE = 1,
            START_SETUP_WIZARD,
            DELETE_FICTION,
            EXIT,
            CANCEL
        }

        internal class OptionViewModel : ViewModel
        {
            private string text;
            private bool isSelected;

            public OptionViewModel(string text, DatabaseErrorWindowResult result)
            {
                this.text = text;
                Result = result;
                isSelected = false;
            }

            public DatabaseErrorWindowResult Result { get; }

            public string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    text = value;
                    NotifyPropertyChanged();
                }
            }

            public bool IsSelected
            {
                get
                {
                    return isSelected;
                }
                set
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private readonly OptionSet optionSet;
        private readonly string databaseFilePath;
        private string header;
        private ObservableCollection<OptionViewModel> options;

        public DatabaseErrorWindowViewModel(MainModel mainModel, OptionSet optionSet, string databaseFilePath)
            : base(mainModel)
        {
            this.optionSet = optionSet;
            this.databaseFilePath = databaseFilePath;
            Localization = mainModel.Localization.CurrentLanguage.DatabaseError;
            OpenAnotherDatabaseOption = new OptionViewModel(Localization.OpenAnotherDatabase, DatabaseErrorWindowResult.OPEN_ANOTHER_DATABASE);
            StartSetupWizardOption = new OptionViewModel(Localization.StartSetupWizard, DatabaseErrorWindowResult.START_SETUP_WIZARD);
            DeleteFictionOption = new OptionViewModel(Localization.DeleteFiction, DatabaseErrorWindowResult.DELETE_FICTION);
            ExitOption = new OptionViewModel(Localization.Exit, DatabaseErrorWindowResult.EXIT);
            OkButtonCommand = new Command(OkButtonClick);
            CancelButtonCommand = new Command(CancelButtonClick);
            Initialize();
        }

        public DatabaseErrorWindowResult Result { get; private set; }

        public DatabaseErrorWindowLocalizator Localization { get; }

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

        public ObservableCollection<OptionViewModel> Options
        {
            get
            {
                return options;
            }
            set
            {
                options = value;
                NotifyPropertyChanged();
            }
        }

        public Command OkButtonCommand { get; }
        public Command CancelButtonCommand { get; }

        private OptionViewModel OpenAnotherDatabaseOption { get; }
        private OptionViewModel StartSetupWizardOption { get; }
        private OptionViewModel DeleteFictionOption { get; }
        private OptionViewModel ExitOption { get; }

        private void Initialize()
        {
            options = new ObservableCollection<OptionViewModel>();
            string error;
            switch (optionSet)
            {
                case OptionSet.DATABASE_NOT_FOUND:
                    error = Localization.GetDatabaseNotFoundText(databaseFilePath);
                    options.Add(OpenAnotherDatabaseOption);
                    options.Add(StartSetupWizardOption);
                    options.Add(ExitOption);
                    break;
                case OptionSet.DATABASE_NOT_VALID:
                    error = Localization.GetDatabaseNotValidText(databaseFilePath);
                    options.Add(OpenAnotherDatabaseOption);
                    options.Add(StartSetupWizardOption);
                    options.Add(ExitOption);
                    break;
                case OptionSet.DATABASE_DUMP_FILE:
                    error = Localization.GetDatabaseDumpFileText(databaseFilePath);
                    options.Add(OpenAnotherDatabaseOption);
                    options.Add(StartSetupWizardOption);
                    options.Add(ExitOption);
                    break;
                case OptionSet.SERVER_DATABASE:
                    error = Localization.GetLibgenServerDatabaseText(databaseFilePath);
                    options.Add(OpenAnotherDatabaseOption);
                    options.Add(StartSetupWizardOption);
                    options.Add(ExitOption);
                    break;
                case OptionSet.OLD_FICTION_SCHEMA:
                    error = Localization.GetOldFictionSchemaText(databaseFilePath);
                    options.Add(DeleteFictionOption);
                    options.Add(OpenAnotherDatabaseOption);
                    options.Add(StartSetupWizardOption);
                    options.Add(ExitOption);
                    break;
                default:
                    throw new Exception($"Unexpected option set: {optionSet}.");
            }
            header = Localization.GetHeader(error);
            options.First().IsSelected = true;
        }

        private void OkButtonClick()
        {
            Result = Options.First(option => option.IsSelected).Result;
            CurrentWindowContext.CloseDialog(true);
        }

        private void CancelButtonClick()
        {
            Result = DatabaseErrorWindowResult.CANCEL;
            CurrentWindowContext.CloseDialog(false);
        }
    }
}
