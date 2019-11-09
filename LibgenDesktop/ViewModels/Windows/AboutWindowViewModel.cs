using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators;
using LibgenDesktop.Models.Update;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class AboutWindowViewModel : LibgenWindowViewModel
    {
        internal class TranslatorViewModel
        {
            public TranslatorViewModel(string language, string translator)
            {
                Language = language;
                Translator = translator;
            }

            public string Language { get; }
            public string Translator { get; }
        }

        private string versionText;
        private ObservableCollection<TranslatorViewModel> translators;
        private string checkForUpdatesButtonCaption;
        private bool isCheckForUpdatesButtonEnabled;
        private bool isCheckForUpdatesButtonVisible;
        private string disabledCheckForUpdatesButtonTooltip;
        private bool isLatestVersionMessageVisible;
        private bool isUpdatePanelVisible;
        private string newVersionMessage;

        public AboutWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            Localization = mainModel.Localization.CurrentLanguage.About;
            ApplicationUpdateRequested = false;
            UpdateCheckResult = null;
            versionText = Localization.GetVersionString(Constants.CURRENT_GITHUB_RELEASE_NAME, Constants.CURRENT_GITHUB_RELEASE_DATE);
            translators = new ObservableCollection<TranslatorViewModel>(mainModel.Localization.Languages.Select(language =>
                new TranslatorViewModel(language.DisplayName, language.TranslatorName)).OrderBy(translator => translator.Language));
            checkForUpdatesButtonCaption = Localization.CheckForUpdates;
            isCheckForUpdatesButtonEnabled = !mainModel.AppSettings.Network.OfflineMode;
            if (!isCheckForUpdatesButtonEnabled)
            {
                DisabledCheckForUpdatesButtonTooltip = Localization.OfflineModeIsOnTooltip;
            }
            isCheckForUpdatesButtonVisible = true;
            isLatestVersionMessageVisible = false;
            isUpdatePanelVisible = false;
            CheckForUpdatesCommand = new Command(CheckForUpdates);
            UpdateCommand = new Command(Update);
            CloseWindowCommand = new Command(CloseWindow);
        }

        public AboutWindowLocalizator Localization { get; }
        public bool ApplicationUpdateRequested { get; private set; }
        public Updater.UpdateCheckResult UpdateCheckResult { get; private set; }

        public string VersionText
        {
            get
            {
                return versionText;
            }
            set
            {
                versionText = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<TranslatorViewModel> Translators
        {
            get
            {
                return translators;
            }
            set
            {
                translators = value;
                NotifyPropertyChanged();
            }
        }

        public string CheckForUpdatesButtonCaption
        {
            get
            {
                return checkForUpdatesButtonCaption;
            }
            set
            {
                checkForUpdatesButtonCaption = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCheckForUpdatesButtonEnabled
        {
            get
            {
                return isCheckForUpdatesButtonEnabled;
            }
            set
            {
                isCheckForUpdatesButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCheckForUpdatesButtonVisible
        {
            get
            {
                return isCheckForUpdatesButtonVisible;
            }
            set
            {
                isCheckForUpdatesButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string DisabledCheckForUpdatesButtonTooltip
        {
            get
            {
                return disabledCheckForUpdatesButtonTooltip;
            }
            set
            {
                disabledCheckForUpdatesButtonTooltip = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsLatestVersionMessageVisible
        {
            get
            {
                return isLatestVersionMessageVisible;
            }
            set
            {
                isLatestVersionMessageVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsUpdatePanelVisible
        {
            get
            {
                return isUpdatePanelVisible;
            }
            set
            {
                isUpdatePanelVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string NewVersionMessage
        {
            get
            {
                return newVersionMessage;
            }
            set
            {
                newVersionMessage = value;
                NotifyPropertyChanged();
            }
        }

        public Command CheckForUpdatesCommand { get; }
        public Command UpdateCommand { get; }
        public Command CloseWindowCommand { get; }

        private async void CheckForUpdates()
        {
            IsCheckForUpdatesButtonEnabled = false;
            CheckForUpdatesButtonCaption = Localization.CheckingUpdates;
            try
            {
                UpdateCheckResult = await MainModel.CheckForApplicationUpdateAsync();
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception, CurrentWindowContext);
                IsCheckForUpdatesButtonEnabled = true;
                CheckForUpdatesButtonCaption = Localization.CheckForUpdates;
                return;
            }
            IsCheckForUpdatesButtonVisible = false;
            if (UpdateCheckResult == null)
            {
                IsLatestVersionMessageVisible = true;
            }
            else
            {
                NewVersionMessage = Localization.GetNewVersionAvailableString(UpdateCheckResult.NewReleaseName, UpdateCheckResult.PublishedAt);
                IsUpdatePanelVisible = true;
            }
        }

        private void Update()
        {
            ApplicationUpdateRequested = true;
            CurrentWindowContext.CloseDialog(true);
        }

        private void CloseWindow()
        {
            CurrentWindowContext.CloseDialog(true);
        }
    }
}
