using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class NonFictionDetailsWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private readonly bool modalWindow;

        public NonFictionDetailsWindowViewModel(MainModel mainModel, NonFictionBook book, bool modalWindow)
        {
            this.mainModel = mainModel;
            this.modalWindow = modalWindow;
            WindowTitle = book.Title;
            WindowWidth = mainModel.AppSettings.NonFiction.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.NonFiction.DetailsWindow.Height;
            WindowClosedCommand = new Command(WindowClosed);
            TabViewModel = new NonFictionDetailsTabViewModel(mainModel, book, modalWindow);
            TabViewModel.CloseTabRequested += CloseTabRequested;
        }

        public string WindowTitle { get; private set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public NonFictionDetailsTabViewModel TabViewModel { get; }

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
