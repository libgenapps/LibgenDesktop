using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.SearchResultItems
{
    internal abstract class SearchResultItemViewModel<T> : ViewModel where T: LibgenObject
    {
        public SearchResultItemViewModel(T libgenObject, LanguageFormatter formatter)
        {
            LibgenObject = libgenObject;
            Formatter = formatter;
        }

        public bool ExistsInLibrary => LibgenObject.FileId.HasValue;

        protected T LibgenObject { get; }
        protected LanguageFormatter Formatter { get; private set; }

        public void UpdateLocalization(LanguageFormatter newFormatter)
        {
            Formatter = newFormatter;
            UpdateLocalizableProperties();
        }

        protected abstract void UpdateLocalizableProperties();
    }
}
