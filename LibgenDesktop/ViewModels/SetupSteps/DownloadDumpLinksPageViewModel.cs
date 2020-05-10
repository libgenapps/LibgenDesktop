using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.Models.Settings;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class DownloadDumpLinksPageViewModel : SetupStepViewModel
    {
        private ObservableCollection<FileItemViewModel> fileList;

        internal class FileItemViewModel : ViewModel
        {
            private readonly SharedSetupContext.Collection collection;
            private readonly Mirrors.DatabaseDumpManualDownloadConfiguration databaseDumpManualDownloadConfiguration;
            private DownloadDumpLinksSetupStepLocalizator localization;

            public FileItemViewModel(SharedSetupContext.Collection collection, DownloadDumpLinksSetupStepLocalizator localization,
                Mirrors.DatabaseDumpManualDownloadConfiguration databaseDumpManualDownloadConfiguration)
            {
                this.collection = collection;
                this.localization = localization;
                this.databaseDumpManualDownloadConfiguration = databaseDumpManualDownloadConfiguration;
            }

            public string CollectionName
            {
                get
                {
                    switch (collection.Identifier)
                    {
                        case SharedSetupContext.CollectionIdentifier.NON_FICTION:
                            return localization.NonFictionDumpName;
                        case SharedSetupContext.CollectionIdentifier.FICTION:
                            return localization.FictionDumpName;
                        case SharedSetupContext.CollectionIdentifier.SCIMAG:
                            return localization.SciMagArticlesDumpName;
                        default:
                            throw new Exception($"Unexpected collection identifier: {collection.Identifier}.");
                    }
                }
            }

            public string FileName
            {
                get
                {
                    string localizedDateTemplate = GetLocalizedDateTemplate(databaseDumpManualDownloadConfiguration.DateTemplate, localization);
                    string fileNameTemplate;
                    switch (collection.Identifier)
                    {
                        case SharedSetupContext.CollectionIdentifier.NON_FICTION:
                            fileNameTemplate = databaseDumpManualDownloadConfiguration.NonFictionFileNameTemplate;
                            break;
                        case SharedSetupContext.CollectionIdentifier.FICTION:
                            fileNameTemplate = databaseDumpManualDownloadConfiguration.FictionFileNameTemplate;
                            break;
                        case SharedSetupContext.CollectionIdentifier.SCIMAG:
                            fileNameTemplate = databaseDumpManualDownloadConfiguration.SciMagFileNameTemplate;
                            break;
                        default:
                            throw new Exception($"Unexpected collection identifier: {collection.Identifier}.");
                    }
                    return localization.GetFileNameString(fileNameTemplate.Replace("{date}", localizedDateTemplate));
                }
            }

            public void UpdateLocalization(DownloadDumpLinksSetupStepLocalizator localization)
            {
                this.localization = localization;
                NotifyPropertyChanged(nameof(CollectionName));
                NotifyPropertyChanged(nameof(FileName));
            }
        }

        private readonly Mirrors.DatabaseDumpManualDownloadConfiguration databaseDumpManualDownloadConfiguration;
        private bool isHeaderVisible;

        public DownloadDumpLinksPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.DOWNLOAD_DUMP_LINKS)
        {
            Localization = windowLocalization.DownloadDumpLinksStep;
            isHeaderVisible = true;
            fileList = new ObservableCollection<FileItemViewModel>();
            Mirrors.MirrorConfiguration databaseDumpMirror = MainModel.Mirrors[DEFAULT_DATABASE_DUMP_MIRROR_NAME];
            DatabaseDumpListPageUrl = databaseDumpMirror.DatabaseDumpPageUrl;
            databaseDumpManualDownloadConfiguration = databaseDumpMirror.DatabaseDumpManualDownload;
        }

        public DownloadDumpLinksSetupStepLocalizator Localization { get; private set; }

        public string Header
        {
            get
            {
                return GetHeaderString(SetupStage.DOWNLOADING_DUMPS);
            }
        }

        public bool IsHeaderVisible
        {
            get
            {
                return isHeaderVisible;
            }
            set
            {
                isHeaderVisible = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<FileItemViewModel> FileList
        {
            get
            {
                return fileList;
            }
            set
            {
                fileList = value;
                NotifyPropertyChanged();
            }
        }

        public string DatabaseDumpListPageUrl { get; }

        public string MostRecentDateNote
        {
            get
            {
                return Localization.GetMostRecentDateNoteString(GetLocalizedDateTemplate(databaseDumpManualDownloadConfiguration.DateTemplate, Localization));
            }
        }

        public override void OnPageEnter()
        {
            base.OnPageEnter();
            IsHeaderVisible = SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC;
            FileList = new ObservableCollection<FileItemViewModel>(SharedSetupContext.Collections.Where(collection => collection.IsSelected).
                Select(collection => new FileItemViewModel(collection, Localization, databaseDumpManualDownloadConfiguration)));
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SetupWizardStep.COLLECTIONS);
        }

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            MoveToPage(SetupWizardStep.CREATE_DATABASE);
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.DownloadDumpLinksStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(Header));
            NotifyPropertyChanged(nameof(MostRecentDateNote));
        }

        private static string GetLocalizedDateTemplate(string inputDateTemplate, DownloadDumpLinksSetupStepLocalizator localization)
        {
            return inputDateTemplate.ToLowerInvariant().Replace("yyyy", localization.YYYY).Replace("mm", localization.MM).Replace("dd", localization.DD);
        }
    }
}
