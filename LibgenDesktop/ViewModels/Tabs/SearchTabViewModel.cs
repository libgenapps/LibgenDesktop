using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization.Localizators.Tabs;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.ViewModels.EventArguments;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal class SearchTabViewModel : TabViewModel
    {
        private SearchTabLocalizator localization;
        private CancellationTokenSource searchCancellationTokenSource;
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
        private string interruptButtonText;
        private bool isInterruptButtonEnabled;
        private bool isImportBlockVisible;

        public SearchTabViewModel(MainModel mainModel, IWindowContext parentWindowContext)
            : base(mainModel, parentWindowContext, mainModel.Localization.CurrentLanguage.SearchTab.TabTitle)
        {
            ImportCommand = new Command(Import);
            SearchCommand = new Command(Search);
            InterruptSearchCommand = new Command(InterruptSearch);
            Initialize();
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
        }

        public SearchTabLocalizator Localization
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

        public string InterruptButtonText
        {
            get
            {
                return interruptButtonText;
            }
            set
            {
                interruptButtonText = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsInterruptButtonEnabled
        {
            get
            {
                return isInterruptButtonEnabled;
            }
            set
            {
                isInterruptButtonEnabled = value;
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
        public Command InterruptSearchCommand { get; }

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
                if (IsNonFictionLibrarySelected && !IsNonFictionLibraryAvailable)
                {
                    IsNonFictionLibrarySelected = false;
                }
                if (IsFictionLibrarySelected && !IsFictionLibraryAvailable)
                {
                    IsFictionLibrarySelected = false;
                }
                if (IsSciMagLibrarySelected && !IsSciMagLibraryAvailable)
                {
                    IsSciMagLibrarySelected = false;
                }
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
                    SetFocus();
                }
            }
            else
            {
                IsSearchBlockVisible = false;
                IsImportBlockVisible = true;
            }
        }

        public void SetFocus()
        {
            Events.RaiseEvent(ViewModelEvent.RegisteredEventId.FOCUS_SEARCH_TEXT_BOX);
        }

        public override void HandleTabClosing()
        {
            MainModel.Localization.LanguageChanged -= LocalizationLanguageChanged;
        }

        private void Initialize()
        {
            localization = MainModel.Localization.CurrentLanguage.SearchTab;
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
                SearchBoxHint = Localization.NonFictionSearchBoxTooltip;
            }
            else if (IsFictionLibrarySelected)
            {
                SearchBoxHint = Localization.FictionSearchBoxTooltip;
            }
            else if (IsSciMagLibrarySelected)
            {
                SearchBoxHint = Localization.SciMagSearchBoxTooltip;
            }
        }

        private void Search()
        {
            if (!String.IsNullOrWhiteSpace(SearchQuery))
            {
                InterruptButtonText = Localization.Interrupt;
                IsInterruptButtonEnabled = true;
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

        private async void SearchItemsAsync<T>(Func<string, IProgress<SearchProgress>, CancellationToken, Task<List<T>>> searchFunction,
            string searchQuery, EventHandler<SearchCompleteEventArgs<T>> searchCompleteEventHandler)
            where T: LibgenObject
        {
            Progress<SearchProgress> searchProgressHandler = new Progress<SearchProgress>(HandleSearchProgress);
            searchCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = searchCancellationTokenSource.Token;
            List<T> result = null;
            bool error = false;
            try
            {
                result = await searchFunction(searchQuery, searchProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, ParentWindowContext);
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
            if (IsNonFictionLibrarySelected)
            {
                SearchProgressStatus = Localization.GetNonFictionSearchProgressText(itemsFound);
            }
            else if (IsFictionLibrarySelected)
            {
                SearchProgressStatus = Localization.GetFictionSearchProgressText(itemsFound);
            }
            else if (IsSciMagLibrarySelected)
            {
                SearchProgressStatus = Localization.GetSciMagSearchProgressText(itemsFound);
            }
        }

        private void InterruptSearch()
        {
            searchCancellationTokenSource.Cancel();
            InterruptButtonText = Localization.Interrupting;
            IsInterruptButtonEnabled = false;
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            Localization = MainModel.Localization.CurrentLanguage.SearchTab;
            Title = Localization.TabTitle;
            UpdateSearchBoxHint();
        }
    }
}
