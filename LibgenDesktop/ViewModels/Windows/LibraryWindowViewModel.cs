using System;
using System.Collections.ObjectModel;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.ProgressArgs;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class LibraryWindowViewModel : LibgenWindowViewModel
    {
        private bool isScanButtonVisible;
        private bool isLogPanelVisible;
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

        public bool IsLogPanelVisible
        {
            get
            {
                return isLogPanelVisible;
            }
            set
            {
                isLogPanelVisible = value;
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
            isLogPanelVisible = false;
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
                IsScanButtonVisible = false;
                IsLogPanelVisible = true;
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
                    if (scanProgress.Found)
                    {
                        ScanLogs.Add($"{scanProgress.RelativeFilePath}: {scanProgress.Authors} — {scanProgress.Title}");
                    }
                    else
                    {
                        ScanLogs.Add($"{scanProgress.RelativeFilePath}: {Localization.NotFound}.");
                    }
                    break;
                case ScanCompleteProgress scanCompleteProgress:
                    ScanLogs.Add(Localization.GetScanCompleteString(scanCompleteProgress.Found, scanCompleteProgress.NotFound));
                    break;
            }
        }
    }
}
