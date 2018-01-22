using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class NonFictionDetailsWindowViewModel : LibgenWindowViewModel
    {
        private readonly MainModel mainModel;
        private readonly NonFictionBook book;
        private readonly bool modalWindow;
        private NonFictionDetailsTabViewModel tabViewModel;

        public NonFictionDetailsWindowViewModel(MainModel mainModel, NonFictionBook book, bool modalWindow)
        {
            this.mainModel = mainModel;
            this.book = book;
            this.modalWindow = modalWindow;
            tabViewModel = null;
            WindowTitle = book.Title;
            WindowWidth = mainModel.AppSettings.NonFiction.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.NonFiction.DetailsWindow.Height;
            WindowClosedCommand = new Command(WindowClosed);
        }

        public string WindowTitle { get; private set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public NonFictionDetailsTabViewModel TabViewModel
        {
            get
            {
                if (tabViewModel == null)
                {
                    tabViewModel = new NonFictionDetailsTabViewModel(mainModel, CurrentWindowContext, book, modalWindow);
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
            mainModel.AppSettings.NonFiction.DetailsWindow = new AppSettings.NonFictionDetailsWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            mainModel.SaveSettings();
        }
    }
}
