using System;
using System.Collections.ObjectModel;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.ViewModels.EventArguments;
using LibgenDesktop.ViewModels.Library;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal class LibraryTabViewModel : TabViewModel
    {
        private LibraryTabLocalizator localization;
        private bool isScanButtonVisible;
        private bool isResultsPanelVisible;
        private string foundTabHeaderTitle;
        private string notFoundTabHeaderTitle;
        private bool isScanLogTabSelected;
        private ObservableCollection<ScanResultItemViewModel> foundItems;
        private ScanResultItemViewModel selectedFoundItem;
        private ObservableCollection<string> notFoundItems;
        private ObservableCollection<string> scanLogs;

        public LibraryTabViewModel(MainModel mainModel, IWindowContext parentWindowContext)
            : base(mainModel, parentWindowContext, mainModel.Localization.CurrentLanguage.Library.TabTitle)
        {
            Localization = mainModel.Localization.CurrentLanguage.Library;
            ScanCommand = new Command(Scan);
            OpenDetailsCommand = new Command(param => OpenDetails((param as ScanResultItemViewModel)));
            FoundDataGridEnterKeyCommand = new Command(FoundDataGridEnterKeyPressed);
            Initialize();
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
        }

        public LibraryTabLocalizator Localization
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

        public bool IsScanButtonVisible
        {
            get
            {
                return isScanButtonVisible;
            }
            set
            {
                isScanButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsResultsPanelVisible
        {
            get
            {
                return isResultsPanelVisible;
            }
            set
            {
                isResultsPanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string FoundTabHeaderTitle
        {
            get
            {
                return foundTabHeaderTitle;
            }
            set
            {
                foundTabHeaderTitle = value;
                NotifyPropertyChanged();
            }
        }

        public string NotFoundTabHeaderTitle
        {
            get
            {
                return notFoundTabHeaderTitle;
            }
            set
            {
                notFoundTabHeaderTitle = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsScanLogTabSelected
        {
            get
            {
                return isScanLogTabSelected;
            }
            set
            {
                isScanLogTabSelected = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ScanResultItemViewModel> FoundItems
        {
            get
            {
                return foundItems;
            }
            set
            {
                foundItems = value;
                NotifyPropertyChanged();
            }
        }

        public ScanResultItemViewModel SelectedFoundItem
        {
            get
            {
                return selectedFoundItem;
            }
            set
            {
                selectedFoundItem = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> NotFoundItems
        {
            get
            {
                return notFoundItems;
            }
            set
            {
                notFoundItems = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> ScanLogs
        {
            get
            {
                return scanLogs;
            }
            set
            {
                scanLogs = value;
                NotifyPropertyChanged();
            }
        }

        public Command ScanCommand { get; }
        public Command OpenDetailsCommand { get; }
        public Command FoundDataGridEnterKeyCommand { get; }

        public event EventHandler<OpenNonFictionDetailsEventArgs> OpenNonFictionDetailsRequested;

        private void Initialize()
        {
            isScanButtonVisible = true;
            isResultsPanelVisible = false;
            isScanLogTabSelected = false;
            scanLogs = new ObservableCollection<string>();
        }

        private async void Scan()
        {
            SelectFolderDialogParameters selectFolderDialogParameters = new SelectFolderDialogParameters
            {
                DialogTitle = Localization.BrowseDirectoryDialogTitle
            };
            SelectFolderDialogResult selectFolderDialogResult = WindowManager.ShowSelectFolderDialog(selectFolderDialogParameters);
            if (selectFolderDialogResult.DialogResult)
            {
                FoundItems = new ObservableCollection<ScanResultItemViewModel>();
                NotFoundItems = new ObservableCollection<string>();
                UpdateResultTabHeaders();
                IsScanLogTabSelected = true;
                IsScanButtonVisible = false;
                IsResultsPanelVisible = true;
                string scanDirectory = selectFolderDialogResult.SelectedFolderPath;
                ScanLogs.Add(Localization.GetScanStartedString(scanDirectory));
                Progress<object> scanProgressHandler = new Progress<object>(HandleScanProgress);
                await MainModel.ScanAsync(scanDirectory, scanProgressHandler);
            }
        }

        private void HandleScanProgress(object progress)
        {
            switch (progress)
            {
                case GenericProgress genericProgress:
                    switch (genericProgress.ProgressEvent)
                    {
                        case GenericProgress.Event.SCAN_CREATING_INDEXES:
                            ScanLogs.Add(Localization.CreatingIndexes);
                            break;
                    }
                    break;
                case ScanProgress scanProgress:
                    if (scanProgress.Error)
                    {
                        ScanLogs.Add($"{scanProgress.RelativeFilePath} — {Localization.Error}.");
                    }
                    else if (scanProgress.Found)
                    {
                        FoundItems.Add(new ScanResultItemViewModel(LibgenObjectType.NON_FICTION_BOOK, 0, null, scanProgress.RelativeFilePath,
                            scanProgress.Authors, scanProgress.Title, scanProgress.LibgenObject));
                    }
                    else
                    {
                        NotFoundItems.Add(scanProgress.RelativeFilePath);
                    }
                    UpdateResultTabHeaders();
                    break;
                case ScanCompleteProgress scanCompleteProgress:
                    ScanLogs.Add(Localization.GetScanCompleteString(scanCompleteProgress.Found, scanCompleteProgress.NotFound, scanCompleteProgress.Errors));
                    break;
            }
        }

        private void UpdateResultTabHeaders()
        {
            if (FoundItems != null)
            {
                FoundTabHeaderTitle = Localization.GetFoundString(FoundItems.Count);
            }
            if (NotFoundItems != null)
            {
                NotFoundTabHeaderTitle = Localization.GetNotFoundString(NotFoundItems.Count);
            }
        }

        private void FoundDataGridEnterKeyPressed()
        {
            OpenDetails(SelectedFoundItem);
        }

        private void OpenDetails(ScanResultItemViewModel foundItem)
        {
            OpenNonFictionDetailsRequested?.Invoke(this, new OpenNonFictionDetailsEventArgs(foundItem.LibgenObject));
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            Localization = MainModel.Localization.CurrentLanguage.Library;
            Title = Localization.TabTitle;
            UpdateResultTabHeaders();
        }
    }
}
