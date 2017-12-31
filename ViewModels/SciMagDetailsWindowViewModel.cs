using System.Diagnostics;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class SciMagDetailsWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private SciMagArticle article;

        public SciMagDetailsWindowViewModel(MainModel mainModel, SciMagArticle article)
        {
            this.mainModel = mainModel;
            this.article = article;
            DownloadArticleCommand = new Command(DownloadArticle);
            CloseCommand = new Command(CloseWindow);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize();
        }

        public string WindowTitle { get; private set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public bool IsInOfflineMode { get; private set; }

        public SciMagArticle Article
        {
            get
            {
                return article;
            }
            private set
            {
                article = value;
                NotifyPropertyChanged();
            }
        }

        public Command DownloadArticleCommand { get; }
        public Command CloseCommand { get; }
        public Command WindowClosedCommand { get; }

        private void Initialize()
        {
            WindowTitle = Article.Title;
            WindowWidth = mainModel.AppSettings.SciMag.DetailsWindow.Width;
            WindowHeight = mainModel.AppSettings.SciMag.DetailsWindow.Height;
            IsInOfflineMode = mainModel.AppSettings.Network.OfflineMode;
        }

        private void DownloadArticle()
        {
            Process.Start(Constants.SCI_MAG_DOWNLOAD_URL_PREFIX + Article.Doi);
        }

        private void CloseWindow()
        {
            IWindowContext currentWindowContext = WindowManager.GetWindowContext(this);
            currentWindowContext.CloseDialog(false);
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
