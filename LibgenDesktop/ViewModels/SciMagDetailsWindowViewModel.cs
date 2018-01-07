using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class SciMagDetailsWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private readonly bool modalWindow;

        public SciMagDetailsWindowViewModel(MainModel mainModel, SciMagArticle article, bool modalWindow)
        {
            this.mainModel = mainModel;
            this.modalWindow = modalWindow;
            WindowTitle = article.Title;
            WindowWidth = mainModel.AppSettings.SciMag.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.SciMag.DetailsWindow.Height;
            WindowClosedCommand = new Command(WindowClosed);
            TabViewModel = new SciMagDetailsTabViewModel(mainModel, article, modalWindow);
            TabViewModel.CloseTabRequested += CloseTabRequested;
        }

        public string WindowTitle { get; private set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public SciMagDetailsTabViewModel TabViewModel { get; }

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
            mainModel.AppSettings.SciMag.DetailsWindow = new AppSettings.SciMagDetailsWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight
            };
            mainModel.SaveSettings();
        }
    }
}
