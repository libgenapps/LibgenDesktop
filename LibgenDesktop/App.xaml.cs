using System;
using System.Threading.Tasks;
using System.Windows;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.ViewModels;

namespace LibgenDesktop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandlers();
            try
            {
                MainModel mainModel = new MainModel();
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
                ShowErrorWindow(exception);
                Shutdown();
            }
        }

        private void ShowMainWindow(MainModel mainModel)
        {
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(mainModel);
            IWindowContext windowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.MAIN_WINDOW, mainWindowViewModel);
            windowContext.Closed += (sender, args) => Shutdown();
            windowContext.Show();
        }

        private void ShowCreateDatabaseWindow(MainModel mainModel)
        {
            CreateDatabaseViewModel createDatabaseViewModel = new CreateDatabaseViewModel(mainModel);
            IWindowContext windowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.CREATE_DATABASE_WINDOW, createDatabaseViewModel);
            bool? result = windowContext.ShowDialog();
            if (result == true)
            {
                ShowMainWindow(mainModel);
            }
            else
            {
                Shutdown();
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
            ErrorWindowViewModel errorWindowViewModel = new ErrorWindowViewModel(exception?.ToString() ?? "(null)");
            IWindowContext errorWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.ERROR_WINDOW, errorWindowViewModel);
            errorWindowContext.ShowDialog();
        }
    }
}
