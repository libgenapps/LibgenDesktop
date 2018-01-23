using System;
using System.Diagnostics;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
{
    internal class SciMagDetailsTabViewModel : TabViewModel
    {
        private SciMagArticle article;
        private string downloadButtonCaption;
        private bool isDownloadButtonEnabled;
        private string disabledDownloadButtonTooltip;
        private string articleDownloadUrl;

        public SciMagDetailsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, SciMagArticle article, bool isInModalWindow)
            : base(mainModel, parentWindowContext, article.Title)
        {
            this.article = article;
            IsInModalWindow = isInModalWindow;
            DownloadArticleCommand = new Command(DownloadArticle);
            CloseCommand = new Command(CloseTab);
            Initialize();
        }

        public bool IsInModalWindow { get; }

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

        public string DownloadButtonCaption
        {
            get
            {
                return downloadButtonCaption;
            }
            set
            {
                downloadButtonCaption = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDownloadButtonEnabled
        {
            get
            {
                return isDownloadButtonEnabled;
            }
            set
            {
                isDownloadButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string DisabledDownloadButtonTooltip
        {
            get
            {
                return disabledDownloadButtonTooltip;
            }
            set
            {
                disabledDownloadButtonTooltip = value;
                NotifyPropertyChanged();
            }
        }

        public Command DownloadArticleCommand { get; }
        public Command CloseCommand { get; }

        public event EventHandler CloseTabRequested;

        private void Initialize()
        {
            bool isInOfflineMode = MainModel.AppSettings.Network.OfflineMode;
            string downloadMirrorName = MainModel.AppSettings.Mirrors.ArticlesMirrorMirrorName;
            if (downloadMirrorName == null)
            {
                DownloadButtonCaption = "СКАЧАТЬ";
                IsDownloadButtonEnabled = false;
                DisabledDownloadButtonTooltip = "Не выбрано зеркало для загрузки статей";
                articleDownloadUrl = null;
            }
            else
            {
                DownloadButtonCaption = "СКАЧАТЬ С " + downloadMirrorName.ToUpper();
                if (isInOfflineMode)
                {
                    IsDownloadButtonEnabled = false;
                    DisabledDownloadButtonTooltip = "Включен автономный режим";
                    articleDownloadUrl = null;
                }
                else
                {
                    IsDownloadButtonEnabled = true;
                    articleDownloadUrl = Article.Env(MainModel.Mirrors[downloadMirrorName].SciMagDownloadUrl);
                }
            }
        }

        private void DownloadArticle()
        {
            Process.Start(articleDownloadUrl);
        }

        private void CloseTab()
        {
            CloseTabRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
