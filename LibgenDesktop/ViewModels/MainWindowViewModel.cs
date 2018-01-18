using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Views;
using static LibgenDesktop.Models.Settings.AppSettings;

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
            SynchronizeCommand = new Command(Synchronize);
            SettingsCommand = new Command(SettingsMenuItemClick);
            WindowClosedCommand = new Command(WindowClosed);
            DefaultSearchTabViewModel = new SearchTabViewModel(mainModel);
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
        public Command SynchronizeCommand { get; }
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
                new NonFictionSearchResultsTabViewModel(mainModel, e.SearchQuery, e.SearchResult);
            nonFictionSearchResultsTabViewModel.OpenNonFictionDetailsRequested += OpenNonFictionDetailsRequested;
            ShowSearchResults(sender as SearchTabViewModel, nonFictionSearchResultsTabViewModel);
        }

        private void SearchTabFictionSearchComplete(object sender, SearchTabViewModel.SearchCompleteEventArgs<FictionBook> e)
        {
            FictionSearchResultsTabViewModel fictionSearchResultsTabViewModel = new FictionSearchResultsTabViewModel(mainModel, e.SearchQuery, e.SearchResult);
            fictionSearchResultsTabViewModel.OpenFictionDetailsRequested += OpenFictionDetailsRequested;
            ShowSearchResults(sender as SearchTabViewModel, fictionSearchResultsTabViewModel);
        }

        private void SearchTabSciMagSearchComplete(object sender, SearchTabViewModel.SearchCompleteEventArgs<SciMagArticle> e)
        {
            SciMagSearchResultsTabViewModel sciMagSearchResultsTabViewModel = new SciMagSearchResultsTabViewModel(mainModel, e.SearchQuery, e.SearchResult);
            sciMagSearchResultsTabViewModel.OpenSciMagDetailsRequested += OpenSciMagDetailsRequested;
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
                SearchTabViewModel searchTabViewModel = new SearchTabViewModel(mainModel);
                searchTabViewModel.NonFictionSearchComplete += SearchTabNonFictionSearchComplete;
                searchTabViewModel.FictionSearchComplete += SearchTabFictionSearchComplete;
                searchTabViewModel.SciMagSearchComplete += SearchTabSciMagSearchComplete;
                TabViewModels.Add(searchTabViewModel);
                SelectedTabViewModel = searchTabViewModel;
                NotifyPropertyChanged(nameof(IsNewTabButtonVisible));
            }
        }

        private void OpenNonFictionDetailsRequested(object sender, NonFictionSearchResultsTabViewModel.OpenNonFictionDetailsEventArgs e)
        {
            Logger.Debug($"Opening non-fiction book with ID = {e.NonFictionBook.Id}, Libgen ID = {e.NonFictionBook.LibgenId}.");
            SearchSettings.DetailsMode openDetailsMode = mainModel.AppSettings.Search.OpenDetailsMode;
            if (openDetailsMode == SearchSettings.DetailsMode.NEW_TAB)
            {
                NonFictionDetailsTabViewModel nonFictionDetailsTabViewModel =
                    new NonFictionDetailsTabViewModel(mainModel, e.NonFictionBook, isInModalWindow: false);
                nonFictionDetailsTabViewModel.CloseTabRequested += NonFictionDetailsCloseTabRequested;
                TabViewModels.Add(nonFictionDetailsTabViewModel);
                SelectedTabViewModel = nonFictionDetailsTabViewModel;
            }
            else
            {
                bool modalWindow = openDetailsMode == SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
                NonFictionDetailsWindowViewModel detailsWindowViewModel = new NonFictionDetailsWindowViewModel(mainModel, e.NonFictionBook, modalWindow);
                IWindowContext detailsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.NON_FICTION_DETAILS_WINDOW, detailsWindowViewModel, CurrentWindowContext);
                NonFictionDetailsWindowSettings detailsWindowSettings = mainModel.AppSettings.NonFiction.DetailsWindow;
                if (modalWindow)
                {
                    detailsWindowContext.ShowDialog(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
                else
                {
                    detailsWindowContext.Show(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
            }
        }

        private void OpenFictionDetailsRequested(object sender, FictionSearchResultsTabViewModel.OpenFictionDetailsEventArgs e)
        {
            Logger.Debug($"Opening fiction book with ID = {e.FictionBook.Id}, Libgen ID = {e.FictionBook.LibgenId}.");
            SearchSettings.DetailsMode openDetailsMode = mainModel.AppSettings.Search.OpenDetailsMode;
            if (openDetailsMode == SearchSettings.DetailsMode.NEW_TAB)
            {
                FictionDetailsTabViewModel fictionDetailsTabViewModel = new FictionDetailsTabViewModel(mainModel, e.FictionBook, isInModalWindow: false);
                fictionDetailsTabViewModel.CloseTabRequested += FictionDetailsCloseTabRequested;
                TabViewModels.Add(fictionDetailsTabViewModel);
                SelectedTabViewModel = fictionDetailsTabViewModel;
            }
            else
            {
                bool modalWindow = openDetailsMode == SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
                FictionDetailsWindowViewModel detailsWindowViewModel = new FictionDetailsWindowViewModel(mainModel, e.FictionBook, modalWindow);
                IWindowContext detailsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.FICTION_DETAILS_WINDOW, detailsWindowViewModel, CurrentWindowContext);
                FictionDetailsWindowSettings detailsWindowSettings = mainModel.AppSettings.Fiction.DetailsWindow;
                if (modalWindow)
                {
                    detailsWindowContext.ShowDialog(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
                else
                {
                    detailsWindowContext.Show(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
            }
        }

        private void OpenSciMagDetailsRequested(object sender, SciMagSearchResultsTabViewModel.OpenSciMagDetailsEventArgs e)
        {
            Logger.Debug($"Opening article with ID = {e.SciMagArticle.Id}, Libgen ID = {e.SciMagArticle.LibgenId}.");
            SearchSettings.DetailsMode openDetailsMode = mainModel.AppSettings.Search.OpenDetailsMode;
            if (openDetailsMode == SearchSettings.DetailsMode.NEW_TAB)
            {
                SciMagDetailsTabViewModel sciMagDetailsTabViewModel = new SciMagDetailsTabViewModel(mainModel, e.SciMagArticle, isInModalWindow: false);
                sciMagDetailsTabViewModel.CloseTabRequested += SciMagDetailsCloseTabRequested;
                TabViewModels.Add(sciMagDetailsTabViewModel);
                SelectedTabViewModel = sciMagDetailsTabViewModel;
            }
            else
            {
                bool modalWindow = openDetailsMode == SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
                SciMagDetailsWindowViewModel detailsWindowViewModel = new SciMagDetailsWindowViewModel(mainModel, e.SciMagArticle, modalWindow);
                IWindowContext detailsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SCI_MAG_DETAILS_WINDOW, detailsWindowViewModel, CurrentWindowContext);
                SciMagDetailsWindowSettings detailsWindowSettings = mainModel.AppSettings.SciMag.DetailsWindow;
                if (modalWindow)
                {
                    detailsWindowContext.ShowDialog(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
                else
                {
                    detailsWindowContext.Show(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
            }
        }

        private void NonFictionDetailsCloseTabRequested(object sender, EventArgs e)
        {
            CloseTab(sender as NonFictionDetailsTabViewModel);
        }

        private void FictionDetailsCloseTabRequested(object sender, EventArgs e)
        {
            CloseTab(sender as FictionDetailsTabViewModel);
        }

        private void SciMagDetailsCloseTabRequested(object sender, EventArgs e)
        {
            CloseTab(sender as SciMagDetailsTabViewModel);
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
            switch (tabViewModel)
            {
                case SearchTabViewModel searchTabViewModel:
                    searchTabViewModel.NonFictionSearchComplete -= SearchTabNonFictionSearchComplete;
                    searchTabViewModel.FictionSearchComplete -= SearchTabFictionSearchComplete;
                    searchTabViewModel.SciMagSearchComplete -= SearchTabSciMagSearchComplete;
                    break;
                case NonFictionSearchResultsTabViewModel nonFictionSearchResultsTabViewModel:
                    nonFictionSearchResultsTabViewModel.OpenNonFictionDetailsRequested -= OpenNonFictionDetailsRequested;
                    break;
                case NonFictionDetailsTabViewModel nonFictionDetailsTabViewModel:
                    nonFictionDetailsTabViewModel.CloseTabRequested -= NonFictionDetailsCloseTabRequested;
                    break;
                case FictionDetailsTabViewModel fictionDetailsTabViewModel:
                    fictionDetailsTabViewModel.CloseTabRequested -= FictionDetailsCloseTabRequested;
                    break;
                case SciMagDetailsTabViewModel sciMagDetailsTabViewModel:
                    sciMagDetailsTabViewModel.CloseTabRequested -= SciMagDetailsCloseTabRequested;
                    break;
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
                downloadManagerTabViewModel = new DownloadManagerTabViewModel(mainModel);
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
                Filter = "Все поддерживаемые файлы|*.sql;*zip;*.rar;*.gz;*.7z|SQL -дампы (*.sql)|*.sql|Архивы (*.zip, *.rar, *.gz, *.7z)|*zip;*.rar;*.gz;*.7z|Все файлы (*.*)|*.*",
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

        public void Synchronize()
        {
            if (mainModel.NonFictionBookCount == 0)
            {
                MessageBoxWindow.ShowMessage("Ошибка", @"Перед синхронизацией списка нехудожественной литературы необходимо выполнить импорт из дампа базы данных (пункт ""Импорт"" в меню).", CurrentWindowContext);
                return;
            }
            if (mainModel.AppSettings.Mirrors.NonFictionSynchronizationMirrorName == null)
            {
                MessageBoxWindow.ShowMessage("Ошибка", @"Не выбрано зеркало для синхронизации списка нехудожественной литературы.", CurrentWindowContext);
                return;
            }
            if (mainModel.AppSettings.Network.OfflineMode)
            {
                if (MessageBoxWindow.ShowPrompt("Автономный режим", "Синхронизация невозможна при включенном автономном режиме. Выключить автономный режим?", CurrentWindowContext))
                {
                    mainModel.AppSettings.Network.OfflineMode = false;
                    mainModel.SaveSettings();
                }
                else
                {
                    return;
                }
            }
            SynchronizationWindowViewModel synchronizationWindowViewModel = new SynchronizationWindowViewModel(mainModel);
            IWindowContext synchronizationWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SYNCHRONIZATION_WINDOW, synchronizationWindowViewModel, CurrentWindowContext);
            synchronizationWindowContext.ShowDialog();
            if (IsDefaultSearchTabVisible)
            {
                DefaultSearchTabViewModel.SetFocus();
            }
            else
            {
                if (SelectedTabViewModel is SearchTabViewModel searchTabViewModel)
                {
                    searchTabViewModel.SetFocus();
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
