using System;
using System.Collections.ObjectModel;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.ViewModels.Settings;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal class LanguagePageViewModel : SetupStepViewModel
    {
        private ObservableCollection<LanguageItemViewModel> languagesList;
        private LanguageItemViewModel selectedLanguage;

        public LanguagePageViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy,
            SetupWizardWindowLocalizator windowLocalization, SharedSetupContext sharedSetupContext)
            : base(mainModel, setupWizardWindowContextProxy, windowLocalization, sharedSetupContext, SetupWizardStep.LANGUAGE)
        {
            Localization = windowLocalization.LanguageStep;
            languagesList = new ObservableCollection<LanguageItemViewModel>(MainModel.Localization.Languages.Select(language =>
                new LanguageItemViewModel(language)));
            selectedLanguage = languagesList.First(languageItem => languageItem.Language == MainModel.Localization.CurrentLanguage);
        }

        public LanguageSetupStepLocalizator Localization { get; private set; }

        public ObservableCollection<LanguageItemViewModel> LanguagesList
        {
            get
            {
                return languagesList;
            }
            set
            {
                languagesList = value;
                NotifyPropertyChanged();
            }
        }

        public LanguageItemViewModel SelectedLanguage
        {
            get
            {
                return selectedLanguage;
            }
            set
            {
                selectedLanguage = value;
                NotifyPropertyChanged();
                MainModel.Localization.SwitchLanguage(value.Language.Name);
            }
        }

        public override bool IsBackButtonVisible => false;

        public override void OnNextButtonClick()
        {
            base.OnNextButtonClick();
            MainModel.AppSettings.General.Language = SelectedLanguage.Language.Name;
            MainModel.SaveSettings();
            MoveToPage(SetupWizardStep.SETUP_MODE);
        }

        protected override void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization)
        {
            Localization = windowLocalization.LanguageStep;
            NotifyPropertyChanged(nameof(Localization));
        }
    }
}
