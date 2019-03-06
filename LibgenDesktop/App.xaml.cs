using System;
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
        private MainModel mainModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandlers();
            try
            {
                mainModel = new MainModel();
                if (mainModel.LocalDatabaseStatus == MainModel.DatabaseStatus.OPENED)
                {
                    ShowMainWindow(mainModel);
                }
                else
                {
                    ShowCreateDatabaseWindow(mainModel);
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

        private void ShowCreateDatabaseWindow(MainModel mainModel)
        {
            CreateDatabaseWindowViewModel createDatabaseWindowViewModel = new CreateDatabaseWindowViewModel(mainModel);
            IWindowContext windowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.CREATE_DATABASE_WINDOW, createDatabaseWindowViewModel);
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
                mainModel.Downloader.Shutdown();
                mainModel.Dispose();
            }
            Shutdown();
        }
    }
}
