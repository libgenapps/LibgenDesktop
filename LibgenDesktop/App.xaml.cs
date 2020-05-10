using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.ViewModels.Windows;

namespace LibgenDesktop
{
    public partial class App : Application
    {
        private enum DatabaseOpenResult
        {
            IN_PROGRESS = 1,
            DATABASE_OPENED,
            SHOW_SETUP_WIZARD,
            EXIT_REQUESTED
        }

        private MainModel mainModel;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandlers();
            try
            {
                mainModel = new MainModel();
                string databaseFilePath = mainModel.AppSettings.DatabaseFileName;
                bool saveDatabaseFilePathAfterSuccessfulOpen = false;
                MainModel.OpenDatabaseOptions openDatabaseOptions = MainModel.OpenDatabaseOptions.NONE;
                DatabaseOpenResult databaseOpenResult = DatabaseOpenResult.IN_PROGRESS;
                while (databaseOpenResult == DatabaseOpenResult.IN_PROGRESS)
                {
                    MainModel.DatabaseStatus databaseStatus = await mainModel.OpenDatabase(databaseFilePath, openDatabaseOptions);
                    if (databaseStatus == MainModel.DatabaseStatus.OPENED)
                    {
                        databaseOpenResult = DatabaseOpenResult.DATABASE_OPENED;
                        if (saveDatabaseFilePathAfterSuccessfulOpen)
                        {
                            mainModel.AppSettings.DatabaseFileName = MainModel.GetDatabaseNormalizedPath(databaseFilePath);
                            mainModel.SaveSettings();
                        }
                    }
                    else if (databaseStatus == MainModel.DatabaseStatus.NOT_SET)
                    {
                        databaseOpenResult = DatabaseOpenResult.SHOW_SETUP_WIZARD;
                    }
                    else
                    {
                        DatabaseErrorWindowViewModel.OptionSet optionSet;
                        switch (databaseStatus)
                        {
                            case MainModel.DatabaseStatus.NOT_FOUND:
                                optionSet = DatabaseErrorWindowViewModel.OptionSet.DATABASE_NOT_FOUND;
                                break;
                            case MainModel.DatabaseStatus.POSSIBLE_DUMP_FILE:
                                optionSet = DatabaseErrorWindowViewModel.OptionSet.DATABASE_DUMP_FILE;
                                break;
                            case MainModel.DatabaseStatus.OLD_FICTION_SCHEMA:
                                optionSet = DatabaseErrorWindowViewModel.OptionSet.OLD_FICTION_SCHEMA;
                                break;
                            case MainModel.DatabaseStatus.CORRUPTED:
                                optionSet = DatabaseErrorWindowViewModel.OptionSet.DATABASE_NOT_VALID;
                                break;
                            case MainModel.DatabaseStatus.SERVER_DATABASE:
                                optionSet = DatabaseErrorWindowViewModel.OptionSet.SERVER_DATABASE;
                                break;
                            default:
                                throw new Exception($"Unknown database status: {databaseStatus}.");
                        }
                        DatabaseErrorWindowViewModel databaseErrorWindowViewModel = new DatabaseErrorWindowViewModel(mainModel, optionSet, databaseFilePath);
                        IWindowContext windowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.DATABASE_ERROR_WINDOW,
                            databaseErrorWindowViewModel);
                        windowContext.ShowDialog();
                        switch (databaseErrorWindowViewModel.Result)
                        {
                            case DatabaseErrorWindowViewModel.DatabaseErrorWindowResult.OPEN_ANOTHER_DATABASE:
                                OpenFileDialogResult selectDatabaseFileDialogResult = DatabaseWindowViewModel.SelectDatabaseFile(mainModel);
                                if (selectDatabaseFileDialogResult.DialogResult)
                                {
                                    databaseFilePath = selectDatabaseFileDialogResult.SelectedFilePaths.First();
                                    saveDatabaseFilePathAfterSuccessfulOpen = true;
                                }
                                break;
                            case DatabaseErrorWindowViewModel.DatabaseErrorWindowResult.START_SETUP_WIZARD:
                                databaseOpenResult = DatabaseOpenResult.SHOW_SETUP_WIZARD;
                                break;
                            case DatabaseErrorWindowViewModel.DatabaseErrorWindowResult.DELETE_FICTION:
                                openDatabaseOptions = MainModel.OpenDatabaseOptions.MIGRATE_FICTION;
                                break;
                            case DatabaseErrorWindowViewModel.DatabaseErrorWindowResult.EXIT:
                            case DatabaseErrorWindowViewModel.DatabaseErrorWindowResult.CANCEL:
                                databaseOpenResult = DatabaseOpenResult.EXIT_REQUESTED;
                                break;
                            default:
                                throw new Exception($"Unknown database error view model result: {databaseErrorWindowViewModel.Result}.");
                        }
                    }
                }
                switch (databaseOpenResult)
                {
                    case DatabaseOpenResult.DATABASE_OPENED:
                        ShowMainWindow(mainModel);
                        break;
                    case DatabaseOpenResult.SHOW_SETUP_WIZARD:
                        ShowSetupWizardWindow(mainModel);
                        break;
                    default:
                        Close();
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.EnableLogging();
                ShowErrorWindow(exception);
                Close();
            }
        }

        private void ShowMainWindow(MainModel mainModel)
        {
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(mainModel);
            IWindowContext windowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.MAIN_WINDOW, mainWindowViewModel);
            windowContext.Closed += (sender, args) => Close();
            windowContext.Show();
        }

        private void ShowSetupWizardWindow(MainModel mainModel)
        {
            SetupWizardWindowViewModel setupWizardWindowViewModel = new SetupWizardWindowViewModel(mainModel);
            IWindowContext windowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SETUP_WIZARD_WINDOW, setupWizardWindowViewModel);
            bool? result = windowContext.ShowDialog();
            if (result == true)
            {
                ShowMainWindow(mainModel);
            }
            else
            {
                Close();
            }
        }

        private void SetupExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => ShowErrorWindow(e.ExceptionObject as Exception);
            DispatcherUnhandledException += (sender, e) =>
            {
                ShowErrorWindow(e.Exception);
                e.Handled = true;
            };
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                ShowErrorWindow(e.Exception);
                e.SetObserved();
            };
        }

        private void ShowErrorWindow(Exception exception)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowErrorWindow(exception));
            }
            else
            {
                Logger.Exception(exception);
                try
                {
                    ErrorWindowViewModel errorWindowViewModel = new ErrorWindowViewModel(exception?.ToString() ?? "(null)",
                        mainModel?.Localization?.CurrentLanguage);
                    IWindowContext errorWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.ERROR_WINDOW, errorWindowViewModel);
                    errorWindowContext.ShowDialog();
                }
                catch (Exception errorWindowException)
                {
                    Logger.Exception(errorWindowException);
                }
            }
        }

        private void Close()
        {
            if (mainModel != null)
            {
                mainModel.DownloadManager.Shutdown();
                mainModel.Dispose();
            }
            Shutdown();
        }
    }
}
