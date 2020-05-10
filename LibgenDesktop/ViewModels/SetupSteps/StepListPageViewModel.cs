using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class StepListPageViewModel : SetupStepViewModel
    {
        public StepListPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.STEP_LIST)
        {
            Localization = windowLocalization.StepListStep;
        }

        public StepListSetupStepLocalizator Localization { get; private set; }

        public string DownloadingDumpsStepIndex
        {
            get
            {
                return Localization.GetStepString((int)SetupStage.DOWNLOADING_DUMPS);
            }
        }

        public string CreatingDatabaseStepIndex
        {
            get
            {
                return Localization.GetStepString((int)SetupStage.CREATING_DATABASE);
            }
        }

        public string ImportingDumpsStepIndex
        {
            get
            {
                return Localization.GetStepString((int)SetupStage.IMPORTING_DUMPS);
            }
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SetupWizardStep.SETUP_MODE);
        }

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            MoveToPage(SetupWizardStep.DOWNLOAD_MODE);
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.StepListStep;
            NotifyPropertyChanged(nameof(Localization));
            NotifyPropertyChanged(nameof(DownloadingDumpsStepIndex));
            NotifyPropertyChanged(nameof(CreatingDatabaseStepIndex));
            NotifyPropertyChanged(nameof(ImportingDumpsStepIndex));
        }
    }
}
