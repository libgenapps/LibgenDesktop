using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
{
    internal class SearchTabViewModel : TabViewModel
    {
        internal class SearchCompleteEventArgs<T> : EventArgs
        {
            public SearchCompleteEventArgs(string searchQuery, ObservableCollection<T> searchResult)
            {
                SearchQuery = searchQuery;
                SearchResult = searchResult;
            }

            public string SearchQuery { get; }
            public ObservableCollection<T> SearchResult { get; }
        }

        private bool isSearchBlockVisible;
        private bool isSearchParamsPanelVisible;
        private string searchQuery;
        private string searchBoxHint;
        private bool isLibrarySelectorVisible;
        private bool isNonFictionLibraryAvailable;
        private bool isFictionLibraryAvailable;
        private bool isSciMagLibraryAvailable;
        private bool isNonFictionLibrarySelected;
        private bool isFictionLibrarySelected;
        private bool isSciMagLibrarySelected;
        private bool isSearchProgressPanelVisible;
        private string searchProgressStatus;
        private bool isImportBlockVisible;

        public SearchTabViewModel(MainModel mainModel)
            : base(mainModel, "Поиск")
        {
            ImportCommand = new Command(Import);
            SearchCommand = new Command(Search);
            Initialize();
        }

        public bool IsSearchBlockVisible
        {
            get
            {
                return isSearchBlockVisible;
            }
            set
            {
                isSearchBlockVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSearchParamsPanelVisible
        {
            get
            {
                return isSearchParamsPanelVisible;
            }
            set
            {
                isSearchParamsPanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get
            {
                return searchQuery;
            }
            set
            {
                searchQuery = value;
                NotifyPropertyChanged();
            }
        }

        public string SearchBoxHint
        {
            get
            {
                return searchBoxHint;
            }
            set
            {
                searchBoxHint = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsLibrarySelectorVisible
        {
            get
            {
                return isLibrarySelectorVisible;
            }
            set
            {
                isLibrarySelectorVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsNonFictionLibraryAvailable
        {
            get
            {
                return isNonFictionLibraryAvailable;
            }
            set
            {
                isNonFictionLibraryAvailable = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsFictionLibraryAvailable
        {
            get
            {
                return isFictionLibraryAvailable;
            }
            set
            {
                isFictionLibraryAvailable = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSciMagLibraryAvailable
        {
            get
            {
                return isSciMagLibraryAvailable;
            }
            set
            {
                isSciMagLibraryAvailable = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsNonFictionLibrarySelected
        {
            get
            {
                return isNonFictionLibrarySelected;
            }
            set
            {
                isNonFictionLibrarySelected = value;
                NotifyPropertyChanged();
                UpdateSearchBoxHint();
            }
        }

        public bool IsFictionLibrarySelected
        {
            get
            {
                return isFictionLibrarySelected;
            }
            set
            {
                isFictionLibrarySelected = value;
                NotifyPropertyChanged();
                UpdateSearchBoxHint();
            }
        }

        public bool IsSciMagLibrarySelected
        {
            get
            {
                return isSciMagLibrarySelected;
            }
            set
            {
                isSciMagLibrarySelected = value;
                NotifyPropertyChanged();
                UpdateSearchBoxHint();
            }
        }

        public bool IsSearchProgressPanelVisible
        {
            get
            {
                return isSearchProgressPanelVisible;
            }
            set
            {
                isSearchProgressPanelVisible = value;
                NotifyPropertyChanged();
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

        public bool IsImportBlockVisible
        {
            get
            {
                return isImportBlockVisible;
            }
            set
            {
                isImportBlockVisible = value;
                NotifyPropertyChanged();
            }
        }

        public Command ImportCommand { get; }
        public Command SearchCommand { get; }

        public event EventHandler ImportRequested;
        public event EventHandler<SearchCompleteEventArgs<NonFictionBook>> NonFictionSearchComplete;
        public event EventHandler<SearchCompleteEventArgs<FictionBook>> FictionSearchComplete;
        public event EventHandler<SearchCompleteEventArgs<SciMagArticle>> SciMagSearchComplete;

        public void Refresh(bool setFocus)
        {
            int nonFictionBookCount = MainModel.NonFictionBookCount;
            int fictionBookCount = MainModel.FictionBookCount;
            int sciMagArticleCount = MainModel.SciMagArticleCount;
            int availableLibraryCount = 0;
            if (nonFictionBookCount > 0)
            {
                availableLibraryCount++;
            }
            if (fictionBookCount > 0)
            {
                availableLibraryCount++;
            }
            if (sciMagArticleCount > 0)
            {
                availableLibraryCount++;
            }
            if (availableLibraryCount > 0)
            {
                IsImportBlockVisible = false;
                IsSearchBlockVisible = true;
                IsSearchParamsPanelVisible = true;
                IsNonFictionLibraryAvailable = nonFictionBookCount > 0;
                IsFictionLibraryAvailable = fictionBookCount > 0;
                IsSciMagLibraryAvailable = sciMagArticleCount > 0;
                if (!IsNonFictionLibrarySelected && !IsFictionLibrarySelected && !IsSciMagLibrarySelected)
                {
                    if (IsNonFictionLibraryAvailable)
                    {
                        IsNonFictionLibrarySelected = true;
                    }
                    else if (IsFictionLibraryAvailable)
                    {
                        IsFictionLibrarySelected = true;
                    }
                    else
                    {
                        IsSciMagLibrarySelected = true;
                    }
                }
                IsLibrarySelectorVisible = availableLibraryCount > 1;
                if (setFocus)
                {
                    Events.RaiseEvent(ViewModelEvent.RegisteredEventId.FOCUS_SEARCH_TEXT_BOX);
                }
            }
            else
            {
                IsSearchBlockVisible = false;
                IsImportBlockVisible = true;
            }
        }

        private void Initialize()
        {
            isSearchBlockVisible = false;
            isSearchParamsPanelVisible = false;
            isLibrarySelectorVisible = false;
            isNonFictionLibraryAvailable = false;
            isFictionLibraryAvailable = false;
            isSciMagLibraryAvailable = false;
            isNonFictionLibrarySelected = false;
            isFictionLibrarySelected = false;
            isSciMagLibrarySelected = false;
            isSearchProgressPanelVisible = false;
            isImportBlockVisible = false;
            Refresh(setFocus: true);
        }

        private void Import()
        {
            ImportRequested?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateSearchBoxHint()
        {
            if (IsNonFictionLibrarySelected)
            {
                SearchBoxHint = "Поиск по наименованию, авторам, серии, издателю и ISBN без дефисов";
            }
            else if (IsFictionLibrarySelected)
            {
                SearchBoxHint = "Поиск по наименованию, авторам, серии, издателю и ISBN с дефисами";
            }
            else if (IsSciMagLibrarySelected)
            {
                SearchBoxHint = "Поиск по наименованию, авторам, журналу, DOI, Pubmed ID и ISSN (p/e)";
            }
        }

        private void Search()
        {
            if (!String.IsNullOrWhiteSpace(SearchQuery))
            {
                IsSearchParamsPanelVisible = false;
                IsSearchProgressPanelVisible = true;
                UpdateSearchProgressStatus(0);
                if (IsNonFictionLibrarySelected)
                {
                    SearchItemsAsync(MainModel.SearchNonFictionAsync, SearchQuery, NonFictionSearchComplete);
                }
                else if (IsFictionLibrarySelected)
                {
                    SearchItemsAsync(MainModel.SearchFictionAsync, SearchQuery, FictionSearchComplete);
                }
                else if (IsSciMagLibrarySelected)
                {
                    SearchItemsAsync(MainModel.SearchSciMagAsync, SearchQuery, SciMagSearchComplete);
                }
            }
        }

        private async void SearchItemsAsync<T>(Func<string, IProgress<SearchProgress>, CancellationToken, Task<ObservableCollection<T>>> searchFunction,
            string searchQuery, EventHandler<SearchCompleteEventArgs<T>> searchCompleteEventHandler)
        {
            Progress<SearchProgress> searchProgressHandler = new Progress<SearchProgress>(HandleSearchProgress);
            CancellationToken cancellationToken = new CancellationToken();
            ObservableCollection<T> result = null;
            bool error = false;
            try
            {
                result = await searchFunction(searchQuery, searchProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception);
                error = true;
            }
            IsSearchProgressPanelVisible = false;
            IsSearchParamsPanelVisible = true;
            if (error)
            {
                Events.RaiseEvent(ViewModelEvent.RegisteredEventId.FOCUS_SEARCH_TEXT_BOX);
            }
            else
            {
                searchCompleteEventHandler?.Invoke(this, new SearchCompleteEventArgs<T>(searchQuery, result));
                SearchQuery = String.Empty;
            }
        }

        private void HandleSearchProgress(SearchProgress searchProgress)
        {
            UpdateSearchProgressStatus(searchProgress.ItemsFound);
        }

        private void UpdateSearchProgressStatus(int itemsFound)
        {
            string itemsFoundString = itemsFound.ToFormattedString();
            if (IsNonFictionLibrarySelected || IsFictionLibrarySelected)
            {
                SearchProgressStatus = $"Найдено книг: {itemsFoundString}";
            }
            else if (IsSciMagLibrarySelected)
            {
                SearchProgressStatus = $"Найдено статей: {itemsFoundString}";
            }
        }
    }
}
