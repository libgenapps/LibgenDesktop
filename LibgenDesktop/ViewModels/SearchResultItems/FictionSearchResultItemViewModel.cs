using System;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.SearchResultItems
{
    internal class FictionSearchResultItemViewModel : SearchResultItemViewModel<FictionBook>
    {
        public FictionSearchResultItemViewModel(FictionBook book, LanguageFormatter formatter)
            : base(book, formatter)
        {
        }

        public FictionBook Book => LibgenObject;
        public string Title => Book.Title;
        public string Authors => Book.Authors;
        public string Series => Book.Series;
        public string Year => Book.Year != "0" ? Book.Year : String.Empty;
        public string Publisher => Book.Publisher;
        public string Format => Book.Format;
        public string FileSize => Formatter.FileSizeToString(Book.SizeInBytes, false);
        public long SizeInBytes => Book.SizeInBytes;

        protected override void UpdateLocalizableProperties()
        {
            NotifyPropertyChanged(nameof(FileSize));
        }
    }
}
