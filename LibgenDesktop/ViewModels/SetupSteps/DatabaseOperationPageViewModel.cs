using System;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.ViewModels.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class DatabaseOperationPageViewModel : SetupStepViewModel
    {
        private bool isCreateNewDatabaseSelected;
        private bool isOpenExistingDatabaseSelected;

        public DatabaseOperationPageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.DATABASE_OPERATION)
        {
            Localization = windowLocalization.DatabaseOperationStep;
            isCreateNewDatabaseSelected = true;
            isOpenExistingDatabaseSelected = false;
        }

        public DatabaseOperationSetupStepLocalizator Localization { get; private set; }

        public bool IsCreateNewDatabaseSelected
        {
            get
            {
                return isCreateNewDatabaseSelected;
            }
            set
            {
                isCreateNewDatabaseSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsOpenExistingDatabaseSelected
        {
            get
            {
                return isOpenExistingDatabaseSelected;
            }
            set
            {
                isOpenExistingDatabaseSelected = value;
                NotifyPropertyChanged();
            }
        }

        public override void OnBackButtonClick()
        {
            base.OnBackButtonClick();
            MoveToPage(SetupWizardStep.SETUP_MODE);
        }

        public override async void OnNextButtonClick()
        {
            if (IsCreateNewDatabaseSelected)
            {
                SharedSetupContext.SelectedDatabaseOperation = SharedSetupContext.DatabaseOperation.CREATE_DATABASE;
                base.OnNextButtonClick();
                MoveToPage(SetupWizardStep.DOWNLOAD_MODE);
            }
            else
            {
                if (await DatabaseWindowViewModel.OpenDatabase(MainModel, SetupWizardWindowContext))
                {
                    SharedSetupContext.SelectedDatabaseOperation = SharedSetupContext.DatabaseOperation.OPEN_DATABASE;
                    SharedSetupContext.IsDatabaseCreated = false;
                    base.OnNextButtonClick();
                    MoveToPage(SetupWizardStep.CONFIRMATION);
                }
            }
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.DatabaseOperationStep;
            NotifyPropertyChanged(nameof(Localization));
        }
    }
}
