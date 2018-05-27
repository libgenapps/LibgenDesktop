using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        private bool areScanButtonsVisible;
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
            ScanNonFictionCommand = new Command(ScanNonFiction);
            ScanFictionCommand = new Command(ScanFiction);
            ScanSciMagCommand = new Command(ScanSciMag);
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

        public bool AreScanButtonsVisible
        {
            get
            {
                return areScanButtonsVisible;
            }
            set
            {
                areScanButtonsVisible = value;
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

        public Command ScanNonFictionCommand { get; }
        public Command ScanFictionCommand { get; }
        public Command ScanSciMagCommand { get; }
        public Command OpenDetailsCommand { get; }
        public Command FoundDataGridEnterKeyCommand { get; }

        public event EventHandler<OpenNonFictionDetailsEventArgs> OpenNonFictionDetailsRequested;
        public event EventHandler<OpenFictionDetailsEventArgs> OpenFictionDetailsRequested;
        public event EventHandler<OpenSciMagDetailsEventArgs> OpenSciMagDetailsRequested;

        private void Initialize()
        {
            areScanButtonsVisible = true;
            isResultsPanelVisible = false;
            isScanLogTabSelected = false;
            scanLogs = new ObservableCollection<string>();
        }

        private void ScanNonFiction()
        {
            Scan<NonFictionBook>(MainModel.ScanNonFictionAsync);
        }

        private void ScanFiction()
        {
            Scan<FictionBook>(MainModel.ScanFictionAsync);
        }

        private void ScanSciMag()
        {
            Scan<SciMagArticle>(MainModel.ScanSciMagAsync);
        }

        private async void Scan<T>(Func<string, Progress<object>, Task> scanFunction) where T : LibgenObject
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
                AreScanButtonsVisible = false;
                IsResultsPanelVisible = true;
                string scanDirectory = selectFolderDialogResult.SelectedFolderPath;
                ScanLogs.Add(Localization.GetScanStartedString(scanDirectory));
                Progress<object> scanProgressHandler = new Progress<object>(HandleScanProgress<T>);
                await scanFunction(scanDirectory, scanProgressHandler);
            }
        }

        private void HandleScanProgress<T>(object progress) where T : LibgenObject
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
                case ScanProgress<T> scanProgress:
                    switch (scanProgress.LibgenObject)
                    {
                        case NonFictionBook nonFictionBook:
                            FoundItems.Add(new NonFictionScanResultItemViewModel(scanProgress.RelativeFilePath, nonFictionBook));
                            break;
                        case FictionBook fictionBook:
                            FoundItems.Add(new FictionScanResultItemViewModel(scanProgress.RelativeFilePath, fictionBook));
                            break;
                        case SciMagArticle sciMagArticle:
                            FoundItems.Add(new SciMagScanResultItemViewModel(scanProgress.RelativeFilePath, sciMagArticle));
                            break;
                    }
                    UpdateResultTabHeaders();
                    break;
                case ScanUnknownProgress scanUnknownProgress:
                    if (scanUnknownProgress.Error)
                    {
                        ScanLogs.Add($"{scanUnknownProgress.RelativeFilePath} — {Localization.Error}.");
                    }
                    else
                    {
                        NotFoundItems.Add(scanUnknownProgress.RelativeFilePath);
                        UpdateResultTabHeaders();
                    }
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
            switch (foundItem)
            {
                case NonFictionScanResultItemViewModel nonFictionFoundItem:
                    OpenNonFictionDetailsRequested?.Invoke(this, new OpenNonFictionDetailsEventArgs(nonFictionFoundItem.LibgenObject));
                    break;
                case FictionScanResultItemViewModel fictionFoundItem:
                    OpenFictionDetailsRequested?.Invoke(this, new OpenFictionDetailsEventArgs(fictionFoundItem.LibgenObject));
                    break;
                case SciMagScanResultItemViewModel sciMagFoundItem:
                    OpenSciMagDetailsRequested?.Invoke(this, new OpenSciMagDetailsEventArgs(sciMagFoundItem.LibgenObject));
                    break;
            }
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            Localization = MainModel.Localization.CurrentLanguage.Library;
            Title = Localization.TabTitle;
            UpdateResultTabHeaders();
        }
    }
}
