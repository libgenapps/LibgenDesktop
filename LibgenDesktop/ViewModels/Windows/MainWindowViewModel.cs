using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.ViewModels.EventArguments;
using LibgenDesktop.ViewModels.Tabs;
using static LibgenDesktop.Models.Settings.AppSettings;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class MainWindowViewModel : LibgenWindowViewModel
    {
        private MainWindowLocalizator localization;
        private SearchTabViewModel defaultSearchTabViewModel;
        private TabViewModel selectedTabViewModel;
        private bool isDownloadManagerButtonHighlighted;
        private bool isCompletedDownloadCounterVisible;
        private int completedDownloadCount;
        private bool isApplicationUpdateAvailable;

        public MainWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            defaultSearchTabViewModel = null;
            Events = new EventProvider();
            NewTabCommand = new Command(NewTab);
            CloseTabCommand = new Command(param => CloseTab(param as TabViewModel));
            CloseCurrentTabCommand = new Command(CloseCurrentTab);
            ExportCommand = new Command(Export);
            DownloadManagerCommand = new Command(ShowDownloadManager);
            ShowApplicationUpdateCommand = new Command(ShowApplicationUpdate);
            ImportCommand = new Command(Import);
            SynchronizeCommand = new Command(Synchronize);
            SettingsCommand = new Command(SettingsMenuItemClick);
            WindowClosedCommand = new Command(WindowClosed);
            TabViewModels = new ObservableCollection<TabViewModel>();
            Initialize();
            mainModel.ApplicationUpdateCheckCompleted += ApplicationUpdateCheckCompleted;
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
            mainModel.Downloader.DownloaderEvent += DownloaderEvent;
        }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public int WindowLeft { get; set; }
        public int WindowTop { get; set; }
        public bool IsWindowMaximized { get; set; }
        public EventProvider Events { get; }
        public ObservableCollection<TabViewModel> TabViewModels { get; }

        public MainWindowLocalizator Localization
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

        public SearchTabViewModel DefaultSearchTabViewModel
        {
            get
            {
                if (defaultSearchTabViewModel == null)
                {
                    defaultSearchTabViewModel = new SearchTabViewModel(MainModel, CurrentWindowContext);
                }
                return defaultSearchTabViewModel;
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
                SelectedTabChanged();
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

        public bool IsDownloadManagerButtonHighlighted
        {
            get
            {
                return isDownloadManagerButtonHighlighted;
            }
            set
            {
                isDownloadManagerButtonHighlighted = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCompletedDownloadCounterVisible
        {
            get
            {
                return isCompletedDownloadCounterVisible;
            }
            set
            {
                isCompletedDownloadCounterVisible = value;
                NotifyPropertyChanged();
            }
        }

        public int CompletedDownloadCount
        {
            get
            {
                return completedDownloadCount;
            }
            set
            {
                completedDownloadCount = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsApplicationUpdateAvailable
        {
            get
            {
                return isApplicationUpdateAvailable;
            }
            set
            {
                isApplicationUpdateAvailable = value;
                NotifyPropertyChanged();
            }
        }

        public Command NewTabCommand { get; }
        public Command CloseTabCommand { get; }
        public Command CloseCurrentTabCommand { get; }
        public Command ExportCommand { get; }
        public Command DownloadManagerCommand { get; }
        public Command ShowApplicationUpdateCommand { get; }
        public Command ImportCommand { get; }
        public Command SynchronizeCommand { get; }
        public Command SettingsCommand { get; }
        public Command WindowClosedCommand { get; }

        private DownloadManagerTabViewModel DownloadManagerTabViewModel
        {
            get
            {
                return TabViewModels.OfType<DownloadManagerTabViewModel>().FirstOrDefault();
            }
        }

        private void Initialize()
        {
            localization = MainModel.Localization.CurrentLanguage.MainWindow;
            AppSettings appSettings = MainModel.AppSettings;
            MainWindowSettings mainWindowSettings = appSettings.MainWindow;
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
            isDownloadManagerButtonHighlighted = false;
            isCompletedDownloadCounterVisible = false;
            completedDownloadCount = 0;
            isApplicationUpdateAvailable = false;
        }

        private void DefaultSearchTabViewModel_ImportRequested(object sender, EventArgs e)
        {
            Import();
        }

        private void SelectedTabChanged()
        {
            if (SelectedTabViewModel is DownloadManagerTabViewModel)
            {
                IsDownloadManagerButtonHighlighted = false;
                CompletedDownloadCount = 0;
                IsCompletedDownloadCounterVisible = false;
            }
        }

        private void SearchTabNonFictionSearchComplete(object sender, SearchCompleteEventArgs<NonFictionBook> e)
        {
            NonFictionSearchResultsTabViewModel nonFictionSearchResultsTabViewModel =
                new NonFictionSearchResultsTabViewModel(MainModel, CurrentWindowContext, e.SearchQuery, e.SearchResult);
            nonFictionSearchResultsTabViewModel.OpenNonFictionDetailsRequested += OpenNonFictionDetailsRequested;
            ShowSearchResults(sender as SearchTabViewModel, nonFictionSearchResultsTabViewModel);
        }

        private void SearchTabFictionSearchComplete(object sender, SearchCompleteEventArgs<FictionBook> e)
        {
            FictionSearchResultsTabViewModel fictionSearchResultsTabViewModel =
                new FictionSearchResultsTabViewModel(MainModel, CurrentWindowContext, e.SearchQuery, e.SearchResult);
            fictionSearchResultsTabViewModel.OpenFictionDetailsRequested += OpenFictionDetailsRequested;
            ShowSearchResults(sender as SearchTabViewModel, fictionSearchResultsTabViewModel);
        }

        private void SearchTabSciMagSearchComplete(object sender, SearchCompleteEventArgs<SciMagArticle> e)
        {
            SciMagSearchResultsTabViewModel sciMagSearchResultsTabViewModel =
                new SciMagSearchResultsTabViewModel(MainModel, CurrentWindowContext, e.SearchQuery, e.SearchResult);
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
                SearchTabViewModel searchTabViewModel = new SearchTabViewModel(MainModel, CurrentWindowContext);
                searchTabViewModel.NonFictionSearchComplete += SearchTabNonFictionSearchComplete;
                searchTabViewModel.FictionSearchComplete += SearchTabFictionSearchComplete;
                searchTabViewModel.SciMagSearchComplete += SearchTabSciMagSearchComplete;
                TabViewModels.Add(searchTabViewModel);
                SelectedTabViewModel = searchTabViewModel;
                NotifyPropertyChanged(nameof(IsNewTabButtonVisible));
            }
        }

        private void OpenNonFictionDetailsRequested(object sender, OpenNonFictionDetailsEventArgs e)
        {
            Logger.Debug($"Opening non-fiction book with ID = {e.NonFictionBook.Id}, Libgen ID = {e.NonFictionBook.LibgenId}.");
            SearchSettings.DetailsMode openDetailsMode = MainModel.AppSettings.Search.OpenDetailsMode;
            if (openDetailsMode == SearchSettings.DetailsMode.NEW_TAB)
            {
                NonFictionDetailsTabViewModel nonFictionDetailsTabViewModel =
                    new NonFictionDetailsTabViewModel(MainModel, CurrentWindowContext, e.NonFictionBook, isInModalWindow: false);
                nonFictionDetailsTabViewModel.SelectDownloadRequested += SelectDownloadRequested;
                nonFictionDetailsTabViewModel.CloseTabRequested += NonFictionDetailsCloseTabRequested;
                TabViewModels.Add(nonFictionDetailsTabViewModel);
                SelectedTabViewModel = nonFictionDetailsTabViewModel;
            }
            else
            {
                bool modalWindow = openDetailsMode == SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
                NonFictionDetailsWindowViewModel detailsWindowViewModel = new NonFictionDetailsWindowViewModel(MainModel, e.NonFictionBook, modalWindow);
                detailsWindowViewModel.SelectDownloadRequested += SelectDownloadRequested;
                IWindowContext detailsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.NON_FICTION_DETAILS_WINDOW, detailsWindowViewModel, CurrentWindowContext);
                NonFictionDetailsWindowSettings detailsWindowSettings = MainModel.AppSettings.NonFiction.DetailsWindow;
                if (modalWindow)
                {
                    detailsWindowContext.ShowDialog(detailsWindowSettings.Width, detailsWindowSettings.Height);
                    detailsWindowViewModel.SelectDownloadRequested -= SelectDownloadRequested;
                }
                else
                {
                    detailsWindowViewModel.WindowClosed += NonFictionDetailsWindowClosed;
                    detailsWindowContext.Show(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
            }
        }

        private void OpenFictionDetailsRequested(object sender, OpenFictionDetailsEventArgs e)
        {
            Logger.Debug($"Opening fiction book with ID = {e.FictionBook.Id}, Libgen ID = {e.FictionBook.LibgenId}.");
            SearchSettings.DetailsMode openDetailsMode = MainModel.AppSettings.Search.OpenDetailsMode;
            if (openDetailsMode == SearchSettings.DetailsMode.NEW_TAB)
            {
                FictionDetailsTabViewModel fictionDetailsTabViewModel
                    = new FictionDetailsTabViewModel(MainModel, CurrentWindowContext, e.FictionBook, isInModalWindow: false);
                fictionDetailsTabViewModel.SelectDownloadRequested += SelectDownloadRequested;
                fictionDetailsTabViewModel.CloseTabRequested += FictionDetailsCloseTabRequested;
                TabViewModels.Add(fictionDetailsTabViewModel);
                SelectedTabViewModel = fictionDetailsTabViewModel;
            }
            else
            {
                bool modalWindow = openDetailsMode == SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
                FictionDetailsWindowViewModel detailsWindowViewModel = new FictionDetailsWindowViewModel(MainModel, e.FictionBook, modalWindow);
                detailsWindowViewModel.SelectDownloadRequested += SelectDownloadRequested;
                IWindowContext detailsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.FICTION_DETAILS_WINDOW, detailsWindowViewModel, CurrentWindowContext);
                FictionDetailsWindowSettings detailsWindowSettings = MainModel.AppSettings.Fiction.DetailsWindow;
                if (modalWindow)
                {
                    detailsWindowContext.ShowDialog(detailsWindowSettings.Width, detailsWindowSettings.Height);
                    detailsWindowViewModel.SelectDownloadRequested -= SelectDownloadRequested;
                }
                else
                {
                    detailsWindowViewModel.WindowClosed += FictionDetailsWindowClosed;
                    detailsWindowContext.Show(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
            }
        }

        private void OpenSciMagDetailsRequested(object sender, OpenSciMagDetailsEventArgs e)
        {
            Logger.Debug($"Opening article with ID = {e.SciMagArticle.Id}, Libgen ID = {e.SciMagArticle.LibgenId}.");
            SearchSettings.DetailsMode openDetailsMode = MainModel.AppSettings.Search.OpenDetailsMode;
            if (openDetailsMode == SearchSettings.DetailsMode.NEW_TAB)
            {
                SciMagDetailsTabViewModel sciMagDetailsTabViewModel =
                    new SciMagDetailsTabViewModel(MainModel, CurrentWindowContext, e.SciMagArticle, isInModalWindow: false);
                sciMagDetailsTabViewModel.SelectDownloadRequested += SelectDownloadRequested;
                sciMagDetailsTabViewModel.CloseTabRequested += SciMagDetailsCloseTabRequested;
                TabViewModels.Add(sciMagDetailsTabViewModel);
                SelectedTabViewModel = sciMagDetailsTabViewModel;
            }
            else
            {
                bool modalWindow = openDetailsMode == SearchSettings.DetailsMode.NEW_MODAL_WINDOW;
                SciMagDetailsWindowViewModel detailsWindowViewModel = new SciMagDetailsWindowViewModel(MainModel, e.SciMagArticle, modalWindow);
                detailsWindowViewModel.SelectDownloadRequested += SelectDownloadRequested;
                IWindowContext detailsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SCI_MAG_DETAILS_WINDOW, detailsWindowViewModel, CurrentWindowContext);
                SciMagDetailsWindowSettings detailsWindowSettings = MainModel.AppSettings.SciMag.DetailsWindow;
                if (modalWindow)
                {
                    detailsWindowContext.ShowDialog(detailsWindowSettings.Width, detailsWindowSettings.Height);
                    detailsWindowViewModel.SelectDownloadRequested -= SelectDownloadRequested;
                }
                else
                {
                    detailsWindowViewModel.WindowClosed += SciMagDetailsWindowClosed;
                    detailsWindowContext.Show(detailsWindowSettings.Width, detailsWindowSettings.Height);
                }
            }
        }

        private void SelectDownloadRequested(object sender, SelectDownloadEventArgs e)
        {
            if (sender is LibgenWindowViewModel)
            {
                Events.RaiseEvent(ViewModelEvent.RegisteredEventId.BRING_TO_FRONT);
            }
            ShowDownloadManager();
            (SelectedTabViewModel as DownloadManagerTabViewModel).SelectDownload(e.DownloadId);
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

        private void NonFictionDetailsWindowClosed(object sender, EventArgs e)
        {
            NonFictionDetailsWindowViewModel nonFictionDetailsWindowViewModel = sender as NonFictionDetailsWindowViewModel;
            nonFictionDetailsWindowViewModel.SelectDownloadRequested -= SelectDownloadRequested;
            nonFictionDetailsWindowViewModel.WindowClosed -= NonFictionDetailsWindowClosed;
        }

        private void FictionDetailsWindowClosed(object sender, EventArgs e)
        {
            FictionDetailsWindowViewModel fictionDetailsWindowViewModel = sender as FictionDetailsWindowViewModel;
            fictionDetailsWindowViewModel.SelectDownloadRequested -= SelectDownloadRequested;
            fictionDetailsWindowViewModel.WindowClosed -= NonFictionDetailsWindowClosed;
        }

        private void SciMagDetailsWindowClosed(object sender, EventArgs e)
        {
            SciMagDetailsWindowViewModel sciMagDetailsWindowViewModel = sender as SciMagDetailsWindowViewModel;
            sciMagDetailsWindowViewModel.SelectDownloadRequested -= SelectDownloadRequested;
            sciMagDetailsWindowViewModel.WindowClosed -= NonFictionDetailsWindowClosed;
        }

        private void CloseCurrentTab()
        {
            if (TabViewModels.Any())
            {
                CloseTab(SelectedTabViewModel);
            }
        }

        private void Export()
        {
            if (SelectedTabViewModel is SearchResultsTabViewModel searchResultsTabViewModel)
            {
                searchResultsTabViewModel.ShowExportPanel();
            }
        }

        private void CloseTab(TabViewModel tabViewModel)
        {
            tabViewModel.HandleTabClosing();
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
                case FictionSearchResultsTabViewModel fictionSearchResultsTabViewModel:
                    fictionSearchResultsTabViewModel.OpenFictionDetailsRequested -= OpenFictionDetailsRequested;
                    break;
                case SciMagSearchResultsTabViewModel sciMagSearchResultsTabViewModel:
                    sciMagSearchResultsTabViewModel.OpenSciMagDetailsRequested -= OpenSciMagDetailsRequested;
                    break;
                case NonFictionDetailsTabViewModel nonFictionDetailsTabViewModel:
                    nonFictionDetailsTabViewModel.SelectDownloadRequested -= SelectDownloadRequested;
                    nonFictionDetailsTabViewModel.CloseTabRequested -= NonFictionDetailsCloseTabRequested;
                    break;
                case FictionDetailsTabViewModel fictionDetailsTabViewModel:
                    fictionDetailsTabViewModel.SelectDownloadRequested -= SelectDownloadRequested;
                    fictionDetailsTabViewModel.CloseTabRequested -= FictionDetailsCloseTabRequested;
                    break;
                case SciMagDetailsTabViewModel sciMagDetailsTabViewModel:
                    sciMagDetailsTabViewModel.SelectDownloadRequested -= SelectDownloadRequested;
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
            DownloadManagerTabViewModel downloadManagerTabViewModel = DownloadManagerTabViewModel;
            if (downloadManagerTabViewModel == null)
            {
                downloadManagerTabViewModel = new DownloadManagerTabViewModel(MainModel, CurrentWindowContext);
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

        private void ShowApplicationUpdate()
        {
            ApplicationUpdateWindowViewModel applicationUpdateWindowViewModel =
                new ApplicationUpdateWindowViewModel(MainModel, MainModel.LastApplicationUpdateCheckResult);
            applicationUpdateWindowViewModel.ApplicationShutdownRequested += Shutdown;
            IWindowContext applicationUpdateWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.APPLICATION_UPDATE_WINDOW,
                applicationUpdateWindowViewModel, CurrentWindowContext);
            if (applicationUpdateWindowContext.ShowDialog() == true)
            {
                IsApplicationUpdateAvailable = false;
            }
            applicationUpdateWindowViewModel.ApplicationShutdownRequested -= Shutdown;
        }

        private void Import()
        {
            ImportLocalizator importLocalizator = MainModel.Localization.CurrentLanguage.Import;
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(importLocalizator.AllSupportedFiles);
            filterBuilder.Append("|*.sql;*zip;*.rar;*.gz;*.7z|");
            filterBuilder.Append(importLocalizator.SqlDumps);
            filterBuilder.Append(" (*.sql)|*.sql|");
            filterBuilder.Append(importLocalizator.Archives);
            filterBuilder.Append(" (*.zip, *.rar, *.gz, *.7z)|*zip;*.rar;*.gz;*.7z|");
            filterBuilder.Append(importLocalizator.AllFiles);
            filterBuilder.Append(" (*.*)|*.*");
            OpenFileDialogParameters selectSqlDumpFileDialogParameters = new OpenFileDialogParameters
            {
                DialogTitle = importLocalizator.BrowseImportFileDialogTitle,
                Filter = filterBuilder.ToString(),
                Multiselect = false
            };
            OpenFileDialogResult selectSqlDumpFileDialogResult = WindowManager.ShowOpenFileDialog(selectSqlDumpFileDialogParameters);
            if (selectSqlDumpFileDialogResult.DialogResult)
            {
                ImportWindowViewModel importWindowViewModel = new ImportWindowViewModel(MainModel, selectSqlDumpFileDialogResult.SelectedFilePaths.First());
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
            SynchronizationLocalizator synchronizationLocalizator = MainModel.Localization.CurrentLanguage.Synchronization;
            if (MainModel.DatabaseMetadata.NonFictionFirstImportComplete != true)
            {
                ShowMessage(synchronizationLocalizator.ErrorMessageTitle, synchronizationLocalizator.ImportRequired);
                return;
            }
            if (MainModel.AppSettings.Mirrors.NonFictionSynchronizationMirrorName == null)
            {
                ShowMessage(synchronizationLocalizator.ErrorMessageTitle, synchronizationLocalizator.NoSynchronizationMirror);
                return;
            }
            if (MainModel.AppSettings.Network.OfflineMode)
            {
                if (ShowPrompt(synchronizationLocalizator.OfflineModePromptTitle, synchronizationLocalizator.OfflineModePromptText))
                {
                    MainModel.AppSettings.Network.OfflineMode = false;
                    MainModel.SaveSettings();
                }
                else
                {
                    return;
                }
            }
            SynchronizationWindowViewModel synchronizationWindowViewModel = new SynchronizationWindowViewModel(MainModel);
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
            SettingsWindowViewModel settingsWindowViewModel = new SettingsWindowViewModel(MainModel);
            IWindowContext settingsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SETTINGS_WINDOW, settingsWindowViewModel,
                CurrentWindowContext);
            settingsWindowContext.ShowDialog();
        }

        private void ApplicationUpdateCheckCompleted(object sender, EventArgs e)
        {
            IsApplicationUpdateAvailable = MainModel.LastApplicationUpdateCheckResult != null;
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            Localization = MainModel.Localization.CurrentLanguage.MainWindow;
        }

        private void DownloaderEvent(object sender, EventArgs e)
        {
            if (!(SelectedTabViewModel is DownloadManagerTabViewModel))
            {
                switch (e)
                {
                    case DownloadItemAddedEventArgs downloadItemAddedEvent:
                        IsDownloadManagerButtonHighlighted = true;
                        break;
                    case DownloadItemChangedEventArgs downloadItemChangedEvent:
                        DownloadItem changedDownloadItem = downloadItemChangedEvent.ChangedDownloadItem;
                        if (changedDownloadItem.Status == DownloadItemStatus.COMPLETED)
                        {
                            CompletedDownloadCount++;
                            IsCompletedDownloadCounterVisible = true;
                        }
                        break;
                }
            }
        }

        private void Shutdown(object sender, EventArgs e)
        {
            CurrentWindowContext.Close();
        }

        private void WindowClosed()
        {
            DownloadManagerTabViewModel downloadManagerTabViewModel = DownloadManagerTabViewModel;
            if (downloadManagerTabViewModel != null)
            {
                MainModel.AppSettings.DownloadManagerTab = new DownloadManagerTabSettings
                {
                    LogPanelHeight = downloadManagerTabViewModel.LogPanelHeight,
                    ShowDebugLogs = downloadManagerTabViewModel.ShowDebugDownloadLogs
                };
            }
            MainModel.AppSettings.MainWindow = new MainWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight,
                Left = WindowLeft,
                Top = WindowTop,
                Maximized = IsWindowMaximized
            };
            MainModel.SaveSettings();
        }
    }
}
