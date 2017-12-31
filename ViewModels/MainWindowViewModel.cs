using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private SearchTabViewModel defaultSearchTabViewModel;
        private TabViewModel selectedTabViewModel;

        public MainWindowViewModel(MainModel mainModel)
        {
            this.mainModel = mainModel;
            NewTabCommand = new Command(NewTab);
            CloseTabCommand = new Command(param => CloseTab(param as TabViewModel));
            CloseCurrentTabCommand = new Command(CloseCurrentTab);
            DownloadManagerCommand = new Command(ShowDownloadManager);
            ImportCommand = new Command(Import);
            SettingsCommand = new Command(SettingsMenuItemClick);
            WindowClosedCommand = new Command(WindowClosed);
            DefaultSearchTabViewModel = new SearchTabViewModel(mainModel, null);
            TabViewModels = new ObservableCollection<TabViewModel>();
            Initialize();
        }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public int WindowLeft { get; set; }
        public int WindowTop { get; set; }
        public bool IsWindowMaximized { get; set; }

        public ObservableCollection<TabViewModel> TabViewModels { get; }

        public SearchTabViewModel DefaultSearchTabViewModel
        {
            get
            {
                return defaultSearchTabViewModel;
            }
            set
            {
                defaultSearchTabViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public TabViewModel SelectedTabViewModel
        {
            get
            {
                return selectedTabViewModel;
            }
            set
            {
                selectedTabViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDefaultSearchTabVisible
        {
            get
            {
                return TabViewModels == null || !TabViewModels.Any();
            }
        }

        public bool AreTabsVisible
        {
            get
            {
                return !IsDefaultSearchTabVisible;
            }
        }

        public bool IsNewTabButtonVisible
        {
            get
            {
                return TabViewModels.Any() && !TabViewModels.Any(tabViewModel => tabViewModel is SearchTabViewModel);
            }
        }

        public Command NewTabCommand { get; }
        public Command CloseTabCommand { get; }
        public Command CloseCurrentTabCommand { get; }
        public Command DownloadManagerCommand { get; }
        public Command ImportCommand { get; }
        public Command SettingsCommand { get; }
        public Command WindowClosedCommand { get; }

        private void Initialize()
        {
            AppSettings appSettings = mainModel.AppSettings;
            AppSettings.MainWindowSettings mainWindowSettings = appSettings.MainWindow;
            WindowWidth = mainWindowSettings.Width;
            WindowHeight = mainWindowSettings.Height;
            WindowLeft = mainWindowSettings.Left;
            WindowTop = mainWindowSettings.Top;
            IsWindowMaximized = mainWindowSettings.Maximized;
            DefaultSearchTabViewModel.ImportRequested += DefaultSearchTabViewModel_ImportRequested;
            DefaultSearchTabViewModel.NonFictionSearchComplete += SearchTabNonFictionSearchComplete;
            DefaultSearchTabViewModel.FictionSearchComplete += SearchTabFictionSearchComplete;
            DefaultSearchTabViewModel.SciMagSearchComplete += SearchTabSciMagSearchComplete;
            selectedTabViewModel = null;
        }

        private void DefaultSearchTabViewModel_ImportRequested(object sender, EventArgs e)
        {
            Import();
        }

        private void SearchTabNonFictionSearchComplete(object sender, SearchTabViewModel.SearchCompleteEventArgs<NonFictionBook> e)
        {
            NonFictionSearchResultsTabViewModel nonFictionSearchResultsTabViewModel =
                new NonFictionSearchResultsTabViewModel(mainModel, CurrentWindowContext, e.SearchQuery, e.SearchResult);
            ShowSearchResults(sender as SearchTabViewModel, nonFictionSearchResultsTabViewModel);
        }

        private void SearchTabFictionSearchComplete(object sender, SearchTabViewModel.SearchCompleteEventArgs<FictionBook> e)
        {
            FictionSearchResultsTabViewModel fictionSearchResultsTabViewModel =
                new FictionSearchResultsTabViewModel(mainModel, CurrentWindowContext, e.SearchQuery, e.SearchResult);
            ShowSearchResults(sender as SearchTabViewModel, fictionSearchResultsTabViewModel);
        }

        private void SearchTabSciMagSearchComplete(object sender, SearchTabViewModel.SearchCompleteEventArgs<SciMagArticle> e)
        {
            SciMagSearchResultsTabViewModel sciMagSearchResultsTabViewModel =
                new SciMagSearchResultsTabViewModel(mainModel, CurrentWindowContext, e.SearchQuery, e.SearchResult);
            ShowSearchResults(sender as SearchTabViewModel, sciMagSearchResultsTabViewModel);
        }

        private void ShowSearchResults(SearchTabViewModel searchTabViewModel, TabViewModel searchResultsTabViewModel)
        {
            if (searchTabViewModel != DefaultSearchTabViewModel)
            {
                searchTabViewModel.NonFictionSearchComplete -= SearchTabNonFictionSearchComplete;
                searchTabViewModel.FictionSearchComplete -= SearchTabFictionSearchComplete;
                searchTabViewModel.SciMagSearchComplete -= SearchTabSciMagSearchComplete;
                TabViewModels.Remove(searchTabViewModel);
            }
            TabViewModels.Add(searchResultsTabViewModel);
            SelectedTabViewModel = searchResultsTabViewModel;
            NotifyPropertyChanged(nameof(IsDefaultSearchTabVisible));
            NotifyPropertyChanged(nameof(AreTabsVisible));
            NotifyPropertyChanged(nameof(IsNewTabButtonVisible));
        }

        private void NewTab()
        {
            if (IsNewTabButtonVisible)
            {
                SearchTabViewModel searchTabViewModel = new SearchTabViewModel(mainModel, CurrentWindowContext);
                searchTabViewModel.NonFictionSearchComplete += SearchTabNonFictionSearchComplete;
                searchTabViewModel.FictionSearchComplete += SearchTabFictionSearchComplete;
                searchTabViewModel.SciMagSearchComplete += SearchTabSciMagSearchComplete;
                TabViewModels.Add(searchTabViewModel);
                SelectedTabViewModel = searchTabViewModel;
                NotifyPropertyChanged(nameof(IsNewTabButtonVisible));
            }
        }

        private void CloseCurrentTab()
        {
            if (TabViewModels.Any())
            {
                CloseTab(SelectedTabViewModel);
            }
        }

        private void CloseTab(TabViewModel tabViewModel)
        {
            if (tabViewModel is SearchTabViewModel searchTabViewModel)
            {
                searchTabViewModel.NonFictionSearchComplete -= SearchTabNonFictionSearchComplete;
                searchTabViewModel.FictionSearchComplete -= SearchTabFictionSearchComplete;
                searchTabViewModel.SciMagSearchComplete -= SearchTabSciMagSearchComplete;
            }
            int removingTabIndex = TabViewModels.IndexOf(tabViewModel);
            TabViewModels.Remove(tabViewModel);
            NotifyPropertyChanged(nameof(IsDefaultSearchTabVisible));
            NotifyPropertyChanged(nameof(AreTabsVisible));
            NotifyPropertyChanged(nameof(IsNewTabButtonVisible));
            if (!TabViewModels.Any())
            {
                SelectedTabViewModel = null;
                DefaultSearchTabViewModel.Refresh(setFocus: true);
            }
            else
            {
                int newSelectedTabIndex = TabViewModels.Count > removingTabIndex ? removingTabIndex : TabViewModels.Count - 1;
                SelectedTabViewModel = TabViewModels[newSelectedTabIndex];
            }
        }

        private void ShowDownloadManager()
        {
            DownloadManagerTabViewModel downloadManagerTabViewModel = TabViewModels.OfType<DownloadManagerTabViewModel>().FirstOrDefault();
            if (downloadManagerTabViewModel == null)
            {
                downloadManagerTabViewModel = new DownloadManagerTabViewModel(mainModel, CurrentWindowContext);
                TabViewModels.Add(downloadManagerTabViewModel);
                SelectedTabViewModel = downloadManagerTabViewModel;
                NotifyPropertyChanged(nameof(IsDefaultSearchTabVisible));
                NotifyPropertyChanged(nameof(AreTabsVisible));
                NotifyPropertyChanged(nameof(IsNewTabButtonVisible));
            }
            else
            {
                SelectedTabViewModel = downloadManagerTabViewModel;
            }
        }

        private void Import()
        {
            OpenFileDialogParameters selectSqlDumpFileDialogParameters = new OpenFileDialogParameters
            {
                DialogTitle = "Выбор SQL-дампа",
                Filter = "Все поддерживаемые файлы|*.sql;*zip;*.rar;*.gz|SQL -дампы (*.sql)|*.sql|Архивы (*.zip, *.rar, *.gz)|*zip;*.rar;*.gz|Все файлы (*.*)|*.*",
                Multiselect = false
            };
            OpenFileDialogResult selectSqlDumpFileDialogResult = WindowManager.ShowOpenFileDialog(selectSqlDumpFileDialogParameters);
            if (selectSqlDumpFileDialogResult.DialogResult)
            {
                ImportWindowViewModel importWindowViewModel = new ImportWindowViewModel(mainModel, selectSqlDumpFileDialogResult.SelectedFilePaths.First());
                IWindowContext importWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.IMPORT_WINDOW, importWindowViewModel, CurrentWindowContext);
                importWindowContext.ShowDialog();
                if (IsDefaultSearchTabVisible)
                {
                    DefaultSearchTabViewModel.Refresh(setFocus: true);
                }
                else
                {
                    foreach (SearchTabViewModel searchTabViewModel in TabViewModels.OfType<SearchTabViewModel>())
                    {
                        searchTabViewModel.Refresh(setFocus: searchTabViewModel == SelectedTabViewModel);
                    }
                }
            }
        }

        private void SettingsMenuItemClick()
        {
            SettingsWindowViewModel settingsWindowViewModel = new SettingsWindowViewModel(mainModel);
            IWindowContext settingsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SETTINGS_WINDOW, settingsWindowViewModel, CurrentWindowContext);
            settingsWindowContext.ShowDialog();
        }

        private void WindowClosed()
        {
            mainModel.AppSettings.MainWindow = new AppSettings.MainWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight,
                Left = WindowLeft,
                Top = WindowTop,
                Maximized = IsWindowMaximized
            };
            mainModel.SaveSettings();
        }
    }
}
