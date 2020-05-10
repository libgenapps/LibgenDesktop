using System;
using System.Collections.ObjectModel;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.SearchResultItems
{
    internal abstract class SearchResultItemViewModel<T> : ViewModel where T : LibgenObject
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

        public abstract ObservableCollection<string> GetCopyMenuItems();

        public void UpdateLocalization(LanguageFormatter newFormatter)
        {
            Formatter = newFormatter;
            UpdateLocalizableProperties();
        }

        protected abstract void UpdateLocalizableProperties();

        protected ObservableCollection<string> GetNonEmptyCopyMenuItems(params string[] copyMenuItems)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            foreach (string copyMenuItem in copyMenuItems)
            {
                if (!String.IsNullOrWhiteSpace(copyMenuItem))
                {
                    result.Add(copyMenuItem);
                }
            }
            return result;
        }
    }
}
