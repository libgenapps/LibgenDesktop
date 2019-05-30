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

        public T LibgenObject { get; }
        public bool ExistsInLibrary => LibgenObject.FileId.HasValue;

        public abstract string FileNameWithoutExtension { get; }
        public abstract string FileExtension { get; }
        public abstract string Md5Hash { get; }

        protected LanguageFormatter Formatter { get; private set; }

        public void UpdateLocalization(LanguageFormatter newFormatter)
        {
            Formatter = newFormatter;
            UpdateLocalizableProperties();
        }

        protected abstract void UpdateLocalizableProperties();
    }
}
