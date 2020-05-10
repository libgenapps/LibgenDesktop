using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class ConfirmationPageViewModel : SetupStepViewModel
    {
        private bool isOnlineModeSelected;
        private bool isDownloadManagerModeSelected;
        private bool isBrowserModeSelected;

        public ConfirmationPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.CONFIRMATION)
        {
            Localization = windowLocalization.ConfirmationStep;
            isOnlineModeSelected = true;
            isDownloadManagerModeSelected = true;
            isBrowserModeSelected = false;
            ToggleOnlineModeCommand = new Command(ToggleOnlineMode);
        }

        public ConfirmationSetupStepLocalizator Localization { get; private set; }

        public bool IsOnlineModeSelected
        {
            get
            {
                return isOnlineModeSelected;
            }
            set
            {
                isOnlineModeSelected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsDownloadingEnabled));
            }
        }

        public bool IsDownloadingEnabled
        {
            get
            {
                return IsOnlineModeSelected;
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

        public override bool IsNextButtonVisible => false;

        public override bool IsFinishButtonVisible => true;

        public Command ToggleOnlineModeCommand { get; }

        public override void OnPageEnter()
        {
            base.OnPageEnter();
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            if (SharedSetupContext.SelectedSetupMode == SharedSetupContext.SetupMode.ADVANCED &&
                SharedSetupContext.SelectedDatabaseOperation == SharedSetupContext.DatabaseOperation.OPEN_DATABASE)
            {
                MoveToPage(SetupWizardStep.DATABASE_OPERATION);
            }
            else
            {
                MoveToPage(SetupWizardStep.IMPORT_DUMPS);
            }
        }

        public override void OnFinishButtonClick()
        {
            base.OnFinishButtonClick();
            MainModel.AppSettings.Network.OfflineMode = !IsOnlineModeSelected;
            MainModel.AppSettings.Download.UseDownloadManager = IsDownloadManagerModeSelected;
            MainModel.SaveSettings();
            MainModel.ReconfigureSettingsDependencies();
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.ConfirmationStep;
            NotifyPropertyChanged(nameof(Localization));
        }

        private void ToggleOnlineMode()
        {
            IsOnlineModeSelected = !IsOnlineModeSelected;
        }
    }
}
