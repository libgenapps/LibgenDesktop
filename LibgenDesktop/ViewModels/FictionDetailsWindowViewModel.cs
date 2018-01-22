using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class FictionDetailsWindowViewModel : LibgenWindowViewModel
    {
        private readonly MainModel mainModel;
        private readonly FictionBook book;
        private readonly bool modalWindow;
        private FictionDetailsTabViewModel tabViewModel;

        public FictionDetailsWindowViewModel(MainModel mainModel, FictionBook book, bool modalWindow)
        {
            this.mainModel = mainModel;
            this.book = book;
            this.modalWindow = modalWindow;
            tabViewModel = null;
            WindowTitle = book.Title;
            WindowWidth = mainModel.AppSettings.Fiction.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.Fiction.DetailsWindow.Height;
            WindowClosedCommand = new Command(WindowClosed);
        }

        public string WindowTitle { get; private set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public FictionDetailsTabViewModel TabViewModel
        {
            get
            {
                if (tabViewModel == null)
                {
                    tabViewModel = new FictionDetailsTabViewModel(mainModel, CurrentWindowContext, book, modalWindow);
                    tabViewModel.CloseTabRequested += CloseTabRequested;
                }
                return tabViewModel;
            }
        }

        public Command WindowClosedCommand { get; }

        private void CloseTabRequested(object sender, EventArgs e)
        {
            TabViewModel.CloseTabRequested -= CloseTabRequested;
            IWindowContext currentWindowContext = WindowManager.GetWindowContext(this);
            if (modalWindow)
            {
                currentWindowContext.CloseDialog(false);
            }
            else
            {
                currentWindowContext.Close();
            }
        }

        private void WindowClosed()
        {
            mainModel.AppSettings.Fiction.DetailsWindow = new AppSettings.FictionDetailsWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            mainModel.SaveSettings();
        }
    }
}
