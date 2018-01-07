using System;
using System.Diagnostics;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels
{
    internal class SciMagDetailsTabViewModel : TabViewModel
    {
        private SciMagArticle article;
        private string downloadButtonCaption;
        private bool isDownloadButtonEnabled;
        private string disabledDownloadButtonTooltip;
        private string articleDownloadUrl;

        public SciMagDetailsTabViewModel(MainModel mainModel, SciMagArticle article, bool isInModalWindow)
            : base(mainModel, article.Title)
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
            DownloadButtonCaption = "СКАЧАТЬ С " + MainModel.AppSettings.Network.MirrorName.ToUpper();
            articleDownloadUrl = null;
            if (MainModel.AppSettings.Network.OfflineMode)
            {
                IsDownloadButtonEnabled = false;
                DisabledDownloadButtonTooltip = "Включен автономный режим";
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(MainModel.CurrentMirror.SciMagDownloadUrl))
                {
                    IsDownloadButtonEnabled = true;
                    articleDownloadUrl = MainModel.CurrentMirror.SciMagDownloadUrl + Article.Doi;
                }
                else
                {
                    IsDownloadButtonEnabled = false;
                    DisabledDownloadButtonTooltip = "Выбранное зеркало не поддерживает загрузку статей";
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
