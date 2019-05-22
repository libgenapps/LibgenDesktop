using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.Utils;
using LibgenDesktop.ViewModels.EventArguments;
using LibgenDesktop.ViewModels.SearchResultItems;
using static LibgenDesktop.Models.Settings.AppSettings;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal class SciMagSearchResultsTabViewModel : SearchResultsTabViewModel
    {
        private readonly SciMagColumnSettings columnSettings;
        private ObservableCollection<SciMagSearchResultItemViewModel> articles;
        private SciMagSearchResultsTabLocalizator localization;
        private string articleCount;
        private string searchProgressStatus;

        public SciMagSearchResultsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, string searchQuery,
            List<SciMagArticle> searchResults)
            : base(mainModel, parentWindowContext, LibgenObjectType.SCIMAG_ARTICLE, searchQuery)
        {
            columnSettings = mainModel.AppSettings.SciMag.Columns;
            LanguageFormatter formatter = MainModel.Localization.CurrentLanguage.Formatter;
            articles = new ObservableCollection<SciMagSearchResultItemViewModel>(searchResults.Select(article =>
                new SciMagSearchResultItemViewModel(article, formatter)));
            OpenDetailsCommand = new Command(param => OpenDetails((param as SciMagSearchResultItemViewModel)?.Article));
            ArticleDataGridEnterKeyCommand = new Command(ArticleDataGridEnterKeyPressed);
            Initialize();
        }

        public SciMagSearchResultsTabLocalizator Localization
        {
            get
            {
                return localization;
            }
            set
            {
                localization = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<SciMagSearchResultItemViewModel> Articles
        {
            get
            {
                return articles;
            }
            set
            {
                articles = value;
                NotifyPropertyChanged();
            }
        }

        public int TitleColumnWidth
        {
            get
            {
                return columnSettings.TitleColumnWidth;
            }
            set
            {
                columnSettings.TitleColumnWidth = value;
            }
        }

        public int AuthorsColumnWidth
        {
            get
            {
                return columnSettings.AuthorsColumnWidth;
            }
            set
            {
                columnSettings.AuthorsColumnWidth = value;
            }
        }

        public int JournalColumnWidth
        {
            get
            {
                return columnSettings.JournalColumnWidth;
            }
            set
            {
                columnSettings.JournalColumnWidth = value;
            }
        }

        public int YearColumnWidth
        {
            get
            {
                return columnSettings.YearColumnWidth;
            }
            set
            {
                columnSettings.YearColumnWidth = value;
            }
        }

        public int FileSizeColumnWidth
        {
            get
            {
                return columnSettings.FileSizeColumnWidth;
            }
            set
            {
                columnSettings.FileSizeColumnWidth = value;
            }
        }

        public int DoiColumnWidth
        {
            get
            {
                return columnSettings.DoiColumnWidth;
            }
            set
            {
                columnSettings.DoiColumnWidth = value;
            }
        }

        public int ExistsInLibraryColumnWidth
        {
            get
            {
                return columnSettings.ExistsInLibraryColumnWidth;
            }
            set
            {
                columnSettings.ExistsInLibraryColumnWidth = value;
            }
        }

        public string SearchProgressStatus
        {
            get
            {
                return searchProgressStatus;
            }
            set
            {
                searchProgressStatus = value;
                NotifyPropertyChanged();
            }
        }

        public string ArticleCount
        {
            get
            {
                return articleCount;
            }
            set
            {
                articleCount = value;
                NotifyPropertyChanged();
            }
        }

        public SciMagSearchResultItemViewModel SelectedArticle { get; set; }

        public Command OpenDetailsCommand { get; }
        public Command ArticleDataGridEnterKeyCommand { get; }

        protected override string FileNameWithoutExtension => $"{SelectedArticle.Article.Authors} - {SelectedArticle.Article.Title}";
        protected override string FileExtension => "pdf";
        protected override string Md5Hash => SelectedArticle.Article.Md5Hash;

        public event EventHandler<OpenSciMagDetailsEventArgs> OpenSciMagDetailsRequested;

        protected override SearchResultsTabLocalizator GetLocalization()
        {
            return localization;
        }

        protected override LibgenObject GetSelectedLibgenObject()
        {
            return SelectedArticle.Article;
        }

        protected override async Task SearchAsync(string searchQuery, CancellationToken cancellationToken)
        {
            IsSearchResultsGridVisible = false;
            IsStatusBarVisible = false;
            UpdateSearchProgressStatus(0);
            Progress<SearchProgress> searchProgressHandler = new Progress<SearchProgress>(HandleSearchProgress);
            List<SciMagArticle> result = new List<SciMagArticle>();
            try
            {
                result = await MainModel.SearchSciMagAsync(SearchQuery, searchProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, ParentWindowContext);
            }
            LanguageFormatter formatter = MainModel.Localization.CurrentLanguage.Formatter;
            Articles = new ObservableCollection<SciMagSearchResultItemViewModel>(result.Select(article =>
                new SciMagSearchResultItemViewModel(article, formatter)));
            UpdateArticleCount();
            IsSearchResultsGridVisible = true;
            IsStatusBarVisible = true;
        }

        protected override void UpdateLocalization(Language newLanguage)
        {
            base.UpdateLocalization(newLanguage);
            Localization = newLanguage.SciMagSearchResultsTab;
            UpdateArticleCount();
            LanguageFormatter newFormatter = newLanguage.Formatter;
            foreach (SciMagSearchResultItemViewModel article in Articles)
            {
                article.UpdateLocalization(newFormatter);
            }
        }

        protected override string GetDownloadMirrorName()
        {
            return MainModel.AppSettings.Mirrors.ArticlesMirrorName;
        }

        protected override string GenerateDownloadUrl(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return UrlGenerator.GetSciMagDownloadUrl(mirrorConfiguration, SelectedArticle.Article);
        }

        protected override string GetDownloadTransformations(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return mirrorConfiguration.SciMagDownloadTransformations;
        }

        private void Initialize()
        {
            localization = MainModel.Localization.CurrentLanguage.SciMagSearchResultsTab;
            UpdateArticleCount();
        }

        private void UpdateArticleCount()
        {
            ArticleCount = Localization.GetStatusBarText(Articles.Count);
        }

        private void ArticleDataGridEnterKeyPressed()
        {
            OpenDetails(SelectedArticle.Article);
        }

        private void OpenDetails(SciMagArticle article)
        {
            OpenSciMagDetailsRequested?.Invoke(this, new OpenSciMagDetailsEventArgs(article));
        }

        private void HandleSearchProgress(SearchProgress searchProgress)
        {
            UpdateSearchProgressStatus(searchProgress.ItemsFound);
        }

        private void UpdateSearchProgressStatus(int articlesFound)
        {
            SearchProgressStatus = Localization.GetSearchProgressText(articlesFound);
        }
    }
}
