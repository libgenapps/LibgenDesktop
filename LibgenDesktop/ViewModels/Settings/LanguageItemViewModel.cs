using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.Settings
{
    internal class LanguageItemViewModel : ViewModel
    {
        public LanguageItemViewModel(Language language)
        {
            Language = language;
        }

        public Language Language { get; }

        public string DisplayName => Language.DisplayName;

        public string PercentTranslated => Language.Settings.GetGeneralPercentTranslated(Language.PercentTranslated);

        public bool IsFullyTranslated => Language.PercentTranslated == 100;
    }
}
