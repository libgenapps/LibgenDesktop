using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class SetupModePageViewModel : SetupStepViewModel
    {
        private bool isBasicModeSelected;
        private bool isAdvancedModeSelected;

        public SetupModePageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.SETUP_MODE)
        {
            Localization = windowLocalization.SetupModeStep;
            isBasicModeSelected = true;
            isAdvancedModeSelected = false;
        }

        public SetupModeSetupStepLocalizator Localization { get; private set; }

        public bool IsBasicModeSelected
        {
            get
            {
                return isBasicModeSelected;
            }
            set
            {
                isBasicModeSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsAdvancedModeSelected
        {
            get
            {
                return isAdvancedModeSelected;
            }
            set
            {
                isAdvancedModeSelected = value;
                NotifyPropertyChanged();
            }
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SetupWizardStep.LANGUAGE);
        }

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            if (IsBasicModeSelected)
            {
                SharedSetupContext.SelectedSetupMode = SharedSetupContext.SetupMode.BASIC;
                MoveToPage(SetupWizardStep.STEP_LIST);
            }
            else
            {
                SharedSetupContext.SelectedSetupMode = SharedSetupContext.SetupMode.ADVANCED;
                MoveToPage(SetupWizardStep.DATABASE_OPERATION);
            }
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.SetupModeStep;
            NotifyPropertyChanged(nameof(Localization));
        }
    }
}
