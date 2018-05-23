using System;
using System.Collections.ObjectModel;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.ViewModels.Library;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class LibraryWindowViewModel : LibgenWindowViewModel
    {
        private bool isScanButtonVisible;
        private bool isResultsPanelVisible;
        private string foundTabHeaderTitle;
        private string notFoundTabHeaderTitle;
        private bool isScanLogTabSelected;
        private ObservableCollection<ScanResultItemViewModel> foundItems;
        private ObservableCollection<string> notFoundItems;
        private ObservableCollection<string> scanLogs;

        public LibraryWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            Localization = mainModel.Localization.CurrentLanguage.Library;
            ScanCommand = new Command(Scan);
            Initialize();
        }

        public LibraryWindowLocalizator Localization { get; }

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
                            scanProgress.Authors, scanProgress.Title));
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
            FoundTabHeaderTitle = Localization.GetFoundString(FoundItems.Count);
            NotFoundTabHeaderTitle = Localization.GetNotFoundString(NotFoundItems.Count);
        }
    }
}
