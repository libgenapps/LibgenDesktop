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
using LibgenDesktop.Models.Localization.Localizators.Tabs;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.Utils;
using LibgenDesktop.ViewModels.EventArguments;
using LibgenDesktop.ViewModels.SearchResultItems;
using static LibgenDesktop.Models.Settings.AppSettings;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal class NonFictionSearchResultsTabViewModel : SearchResultsTabViewModel<NonFictionBook>
    {
        private readonly NonFictionColumnSettings columnSettings;
        private ObservableCollection<NonFictionSearchResultItemViewModel> books;
        private NonFictionSearchResultsTabLocalizator localization;
        private string bookCount;
        private string searchProgressStatus;

        public NonFictionSearchResultsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, string searchQuery,
            List<NonFictionBook> searchResults)
            : base(mainModel, parentWindowContext, LibgenObjectType.NON_FICTION_BOOK, searchQuery)
        {
            columnSettings = mainModel.AppSettings.NonFiction.Columns;
            LanguageFormatter formatter = MainModel.Localization.CurrentLanguage.Formatter;
            books = new ObservableCollection<NonFictionSearchResultItemViewModel>(searchResults.Select(book =>
                new NonFictionSearchResultItemViewModel(book, formatter)));
            Initialize();
        }

        public NonFictionSearchResultsTabLocalizator Localization
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

        public ObservableCollection<NonFictionSearchResultItemViewModel> Books
        {
            get
            {
                return books;
            }
            set
            {
                books = value;
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

        public int SeriesColumnWidth
        {
            get
            {
                return columnSettings.SeriesColumnWidth;
            }
            set
            {
                columnSettings.SeriesColumnWidth = value;
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

        public int LanguageColumnWidth
        {
            get
            {
                return columnSettings.LanguageColumnWidth;
            }
            set
            {
                columnSettings.LanguageColumnWidth = value;
            }
        }

        public int PublisherColumnWidth
        {
            get
            {
                return columnSettings.PublisherColumnWidth;
            }
            set
            {
                columnSettings.PublisherColumnWidth = value;
            }
        }

        public int FormatColumnWidth
        {
            get
            {
                return columnSettings.FormatColumnWidth;
            }
            set
            {
                columnSettings.FormatColumnWidth = value;
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

        public int OcrColumnWidth
        {
            get
            {
                return columnSettings.OcrColumnWidth;
            }
            set
            {
                columnSettings.OcrColumnWidth = value;
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

        public string BookCount
        {
            get
            {
                return bookCount;
            }
            set
            {
                bookCount = value;
                NotifyPropertyChanged();
            }
        }

        public event EventHandler<OpenNonFictionDetailsEventArgs> OpenNonFictionDetailsRequested;

        protected override SearchResultsTabLocalizator GetLocalization()
        {
            return localization;
        }

        protected override async Task SearchAsync(string searchQuery, CancellationToken cancellationToken)
        {
            IsSearchResultsGridVisible = false;
            IsStatusBarVisible = false;
            UpdateSearchProgressStatus(0);
            Progress<SearchProgress> searchProgressHandler = new Progress<SearchProgress>(HandleSearchProgress);
            List<NonFictionBook> result = new List<NonFictionBook>();
            try
            {
                result = await MainModel.SearchNonFictionAsync(searchQuery, searchProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, ParentWindowContext);
            }
            LanguageFormatter formatter = MainModel.Localization.CurrentLanguage.Formatter;
            Books = new ObservableCollection<NonFictionSearchResultItemViewModel>(result.Select(book =>
                new NonFictionSearchResultItemViewModel(book, formatter)));
            UpdateBookCount();
            IsSearchResultsGridVisible = true;
            IsStatusBarVisible = true;
        }

        protected override void UpdateLocalization(Language newLanguage)
        {
            base.UpdateLocalization(newLanguage);
            Localization = newLanguage.NonFictionSearchResultsTab;
            UpdateBookCount();
            LanguageFormatter newFormatter = newLanguage.Formatter;
            foreach (NonFictionSearchResultItemViewModel book in Books)
            {
                book.UpdateLocalization(newFormatter);
            }
        }

        protected override void OpenDetails(NonFictionBook book)
        {
            OpenNonFictionDetailsRequested?.Invoke(this, new OpenNonFictionDetailsEventArgs(book));
        }

        protected override string GetDownloadMirrorName()
        {
            return MainModel.AppSettings.Mirrors.NonFictionBooksMirrorName;
        }

        protected override string GenerateDownloadUrl(Mirrors.MirrorConfiguration mirrorConfiguration, NonFictionBook book)
        {
            return UrlGenerator.GetNonFictionDownloadUrl(mirrorConfiguration, book);
        }

        protected override string GetDownloadTransformations(Mirrors.MirrorConfiguration mirrorConfiguration)
        {
            return mirrorConfiguration.NonFictionDownloadTransformations;
        }

        private void Initialize()
        {
            localization = MainModel.Localization.CurrentLanguage.NonFictionSearchResultsTab;
            UpdateBookCount();
        }

        private void UpdateBookCount()
        {
            BookCount = Localization.GetStatusBarText(Books.Count);
        }

        private void HandleSearchProgress(SearchProgress searchProgress)
        {
            UpdateSearchProgressStatus(searchProgress.ItemsFound);
        }

        private void UpdateSearchProgressStatus(int booksFound)
        {
            SearchProgressStatus = Localization.GetSearchProgressText(booksFound);
        }
    }
}
