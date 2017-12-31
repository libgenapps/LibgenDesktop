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
    }
}
