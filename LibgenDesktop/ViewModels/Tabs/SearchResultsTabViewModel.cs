using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.ViewModels.Panels;
using LibgenDesktop.ViewModels.SearchResultItems;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal abstract class SearchResultsTabViewModel<T> : TabViewModel, ISearchResultsTabViewModel where T : LibgenObject
    {
        private readonly LibgenObjectType libgenObjectType;
        private string searchQuery;
        private string lastExecutedSearchQuery;
        private bool isBookmarkSet;
        private bool isExportPanelVisible;
        private bool isSearchProgressPanelVisible;
        private string interruptButtonText;
        private bool isInterruptButtonEnabled;
        private bool isSearchResultsGridVisible;
        private bool isStatusBarVisible;
        private CancellationTokenSource searchCancellationTokenSource;
        private IList selectedRows;

        protected SearchResultsTabViewModel(MainModel mainModel, IWindowContext parentWindowContext, LibgenObjectType libgenObjectType, string searchQuery)
            : base(mainModel, parentWindowContext, searchQuery)
        {
            this.libgenObjectType = libgenObjectType;
            this.searchQuery = searchQuery;
            lastExecutedSearchQuery = searchQuery;
            UpdateBookmarkedState();
            isExportPanelVisible = false;
            isSearchProgressPanelVisible = false;
            isSearchResultsGridVisible = true;
            isStatusBarVisible = true;
            ExportPanelViewModel = new ExportPanelViewModel(mainModel, libgenObjectType, parentWindowContext);
            ExportPanelViewModel.ClosePanel += CloseExportPanel;
            SearchCommand = new Command(Search);
            InterruptSearchCommand = new Command(InterruptSearch);
            ToggleBookmarkCommand = new Command(ToggleBookmark);
            OpenDetailsCommand = new Command(OpenDetails);
            OpenFileCommand = new Command(OpenFile);
            DownloadCommand = new Command(Download);
            ExportCommand = new Command(ShowExportPanel);
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
            Events.RaiseEvent(ViewModelEvent.RegisteredEventId.FOCUS_SEARCH_TEXT_BOX);
        }

        public ExportPanelViewModel ExportPanelViewModel { get; }

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
                if (IsExportPanelVisible)
                {
                    ExportPanelViewModel.UpdateSearchQuery(value);
                }
            }
        }

        public bool IsBookmarkSet
        {
            get
            {
                return isBookmarkSet;
            }
            set
            {
                isBookmarkSet = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsExportPanelVisible
        {
            get
            {
                return isExportPanelVisible;
            }
            set
            {
                isExportPanelVisible = value;
                NotifyPropertyChanged();
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

        public bool IsSearchResultsGridVisible
        {
            get
            {
                return isSearchResultsGridVisible;
            }
            set
            {
                isSearchResultsGridVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsStatusBarVisible
        {
            get
            {
                return isStatusBarVisible;
            }
            set
            {
                isStatusBarVisible = value;
                NotifyPropertyChanged();
            }
        }

        public IList SelectedRows
        {
            get
            {
                return selectedRows;
            }
            set
            {
                selectedRows = value;
                NotifyPropertyChanged(nameof(ShowOpenDetailsMenuItem));
                NotifyPropertyChanged(nameof(ShowOpenFileMenuItem));
                NotifyPropertyChanged(nameof(ShowDownloadMenuItem));
            }
        }

        public bool ShowOpenDetailsMenuItem
        {
            get
            {
                return SelectedRows.Count == 1;
            }
        }

        public bool ShowOpenFileMenuItem
        {
            get
            {
                if (SelectedRows.Count != 1)
                {
                    return false;
                }
                return SelectedItem?.ExistsInLibrary == true;
            }
        }

        public bool ShowDownloadMenuItem
        {
            get
            {
                return (SelectedRows.Count == 1 && SelectedItem?.ExistsInLibrary == false) || SelectedRows.Count > 1;
            }
        }

        public Command SearchCommand { get; }
        public Command InterruptSearchCommand { get; }
        public Command ToggleBookmarkCommand { get; }
        public Command ExportCommand { get; }
        public Command BookDataGridEnterKeyCommand { get; }
        public Command OpenDetailsCommand { get; }
        public Command OpenFileCommand { get; }
        public Command DownloadCommand { get; }

        private SearchResultsTabLocalizator Localization
        {
            get
            {
                return GetLocalization();
            }
        }

        private IEnumerable<SearchResultItemViewModel<T>> SelectedItems
        {
            get
            {
                return SelectedRows.OfType<SearchResultItemViewModel<T>>();
            }
        }

        private SearchResultItemViewModel<T> SelectedItem
        {
            get
            {
                return SelectedItems.FirstOrDefault();
            }
        }

        public void Search(string searchQuery)
        {
            SearchQuery = searchQuery;
            Search();
        }

        public void ShowExportPanel()
        {
            if (IsSearchResultsGridVisible)
            {
                IsSearchResultsGridVisible = false;
                IsStatusBarVisible = false;
                IsExportPanelVisible = true;
                ExportPanelViewModel.ShowPanel(SearchQuery);
            }
        }

        public override void HandleTabClosing()
        {
            MainModel.Localization.LanguageChanged -= LocalizationLanguageChanged;
        }

        protected abstract SearchResultsTabLocalizator GetLocalization();
        protected abstract Task SearchAsync(string searchQuery, CancellationToken cancellationToken);
        protected abstract void OpenDetails(T libgenObject);
        protected abstract string GetDownloadMirrorName();
        protected abstract string GenerateDownloadUrl(Mirrors.MirrorConfiguration mirrorConfiguration, T libgenObject);
        protected abstract string GetDownloadTransformations(Mirrors.MirrorConfiguration mirrorConfiguration);

        protected virtual void UpdateLocalization(Language newLanguage)
        {
            ExportPanelViewModel.UpdateLocalization(newLanguage);
        }

        private async void Search()
        {
            if (!String.IsNullOrWhiteSpace(SearchQuery) && !IsSearchProgressPanelVisible && !IsExportPanelVisible)
            {
                Title = SearchQuery;
                lastExecutedSearchQuery = SearchQuery;
                UpdateBookmarkedState();
                InterruptButtonText = Localization.Interrupt;
                IsInterruptButtonEnabled = true;
                IsSearchProgressPanelVisible = true;
                searchCancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = searchCancellationTokenSource.Token;
                await SearchAsync(SearchQuery, cancellationToken);
                IsSearchProgressPanelVisible = false;
            }
        }

        private void InterruptSearch()
        {
            searchCancellationTokenSource.Cancel();
            InterruptButtonText = Localization.Interrupting;
            IsInterruptButtonEnabled = false;
        }

        private void OpenDetails()
        {
            SearchResultItemViewModel<T> selectedItem = SelectedItem;
            if (selectedItem == null)
            {
                return;
            }
            OpenDetails(selectedItem.LibgenObject);
        }

        private async void OpenFile()
        {
            SearchResultItemViewModel<T> selectedItem = SelectedItem;
            if (selectedItem == null)
            {
                return;
            }
            LibgenObject selectedLibgenObject = selectedItem.LibgenObject;
            if (selectedLibgenObject == null || !selectedLibgenObject.FileId.HasValue)
            {
                return;
            }
            LibraryFile file = await MainModel.LoadFileAsync(selectedLibgenObject.FileId.Value);
            string filePath = file.FilePath;
            if (File.Exists(filePath))
            {
                Process.Start(filePath);
            }
            else
            {
                ShowMessage(Localization.ErrorMessageTitle, Localization.GetFileNotFoundErrorText(filePath));
            }
        }

        private void Download()
        {
            if (MainModel.AppSettings.Network.OfflineMode)
            {
                ShowMessage(Localization.OfflineModeIsOnMessageTitle, Localization.OfflineModeIsOnMessageText);
                return;
            }
            string downloadMirrorName = GetDownloadMirrorName();
            if (downloadMirrorName == null)
            {
                ShowMessage(Localization.ErrorMessageTitle, Localization.NoDownloadMirrorError);
                return;
            }
            List<SearchResultItemViewModel<T>> itemsToDownload = SelectedItems.Where(selectedItem => !selectedItem.ExistsInLibrary).ToList();
            if (itemsToDownload.Count < Constants.LARGE_NUMBER_OF_ITEMS_TO_DOWNLOAD_WARNING_THRESHOLD ||
                ShowPrompt(Localization.LargeNumberOfItemsToDownloadPromptTitle,
                    Localization.GetLargeNumberOfItemsToDownloadPromptText(itemsToDownload.Count)))
            {
                Mirrors.MirrorConfiguration mirror = MainModel.Mirrors[downloadMirrorName];
                if (MainModel.AppSettings.Download.UseDownloadManager)
                {
                    string downloadTransformations = GetDownloadTransformations(mirror);
                    bool restartSessionOnTimeout = mirror.RestartSessionOnTimeout;
                    List<DownloadItemRequest> downloadItemRequests = itemsToDownload.Select(itemToDownload =>
                        new DownloadItemRequest
                        {
                            DownloadPageUrl = GenerateDownloadUrl(mirror, itemToDownload.LibgenObject),
                            FileNameWithoutExtension = itemToDownload.FileNameWithoutExtension,
                            FileExtension = itemToDownload.FileExtension.ToLower(),
                            Md5Hash = itemToDownload.Md5Hash,
                            DownloadTransformations = downloadTransformations,
                            RestartSessionOnTimeout = restartSessionOnTimeout
                        }).ToList();
                    MainModel.Downloader.EnqueueDownloadItems(downloadItemRequests);
                }
                else
                {
                    foreach (SearchResultItemViewModel<T> itemToDownload in itemsToDownload)
                    {
                        Process.Start(GenerateDownloadUrl(mirror, itemToDownload.LibgenObject));
                    }
                }
            }
        }

        private void UpdateBookmarkedState()
        {
            IsBookmarkSet = MainModel.HasBookmark(libgenObjectType, lastExecutedSearchQuery);
        }

        private void ToggleBookmark()
        {
            if (isBookmarkSet)
            {
                MainModel.DeleteBookmark(libgenObjectType, lastExecutedSearchQuery);
            }
            else
            {
                MainModel.AddBookmark(libgenObjectType, lastExecutedSearchQuery, lastExecutedSearchQuery);
            }
            UpdateBookmarkedState();
        }

        private void CloseExportPanel(object sender, EventArgs e)
        {
            IsExportPanelVisible = false;
            IsSearchResultsGridVisible = true;
            IsStatusBarVisible = true;
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            UpdateLocalization(MainModel.Localization.CurrentLanguage);
        }
    }
}
