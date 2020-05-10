using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.ViewModels.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class DownloadModePageViewModel : SetupStepViewModel
    {
        private readonly SetupWizardProxySettingsWindowViewModel setupWizardProxySettingsWindowViewModel;
        private bool isHeaderVisible;
        private bool isDownloadManagerModeSelected;
        private bool isBrowserModeSelected;
        private bool areProxyServerControlsEnabled;
        private bool isProxyServerEnabled;
        private bool isProxyServerSettingsButtonEnabled;

        public DownloadModePageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.DOWNLOAD_MODE)
        {
            setupWizardProxySettingsWindowViewModel = new SetupWizardProxySettingsWindowViewModel(mainModel);
            Localization = windowLocalization.DownloadModeStep;
            isHeaderVisible = true;
            isDownloadManagerModeSelected = true;
            isBrowserModeSelected = false;
            areProxyServerControlsEnabled = true;
            isProxyServerEnabled = MainModel.AppSettings.Network.UseProxy;
            isProxyServerSettingsButtonEnabled = isProxyServerEnabled;
            ProxyServerSettingsCommand = new Command(ProxyServerSettingsClick);
        }

        public DownloadModeSetupStepLocalizator Localization { get; private set; }

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

        public bool IsDownloadManagerModeSelected
        {
            get
            {
                return isDownloadManagerModeSelected;
            }
            set
            {
                isDownloadManagerModeSelected = value;
                AreProxyServerControlsEnabled = value;
                IsProxyServerSettingsButtonEnabled = value && isProxyServerEnabled;
                NotifyPropertyChanged();
            }
        }

        public bool IsBrowserModeSelected
        {
            get
            {
                return isBrowserModeSelected;
            }
            set
            {
                isBrowserModeSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool AreProxyServerControlsEnabled
        {
            get
            {
                return areProxyServerControlsEnabled;
            }
            set
            {
                areProxyServerControlsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProxyServerEnabled
        {
            get
            {
                return isProxyServerEnabled;
            }
            set
            {
                isProxyServerEnabled = value;
                IsProxyServerSettingsButtonEnabled = value && isDownloadManagerModeSelected;
                NotifyPropertyChanged();
            }
        }

        public bool IsProxyServerSettingsButtonEnabled
        {
            get
            {
                return isProxyServerSettingsButtonEnabled;
            }
            set
            {
                isProxyServerSettingsButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public Command ProxyServerSettingsCommand { get; }

        public override void OnPageEnter()
        {
            IsHeaderVisible = SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC;
            IsProxyServerEnabled = MainModel.AppSettings.Network.UseProxy;
            base.OnPageEnter();
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MainModel.AppSettings.Network.UseProxy = IsProxyServerEnabled;
            MainModel.SaveSettings();
            MoveToPage(SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.BASIC ?
                SetupWizardStep.STEP_LIST : SetupWizardStep.DATABASE_OPERATION);
        }

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            MainModel.AppSettings.Network.UseProxy = IsProxyServerEnabled;
            MainModel.SaveSettings();
            MainModel.ReconfigureSettingsDependencies();
            if (IsDownloadManagerModeSelected)
            {
                SharedSetupContext.SelectedDownloadMode = SharedSetupContext.DownloadMode.DOWNLOAD_MANAGER;
                MoveToPage(SetupWizardStep.DOWNLOAD_DUMP_INFO);
            }
            else
            {
                SharedSetupContext.SelectedDownloadMode = SharedSetupContext.DownloadMode.MANUAL;
                MoveToPage(SetupWizardStep.COLLECTIONS);
            }
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.DownloadModeStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(Header));
        }

        private void ProxyServerSettingsClick()
        {
            IWindowContext setupWizardProxySettingsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SETUP_WIZARD_PROXY_SETTINGS_WINDOW,
                setupWizardProxySettingsWindowViewModel, SetupWizardWindowContext);
            setupWizardProxySettingsWindowViewModel.PopulateFieldsFromAppSettings();
            setupWizardProxySettingsWindowViewModel.SetFocus();
            setupWizardProxySettingsWindowContext.ShowDialog();
        }
    }
}
