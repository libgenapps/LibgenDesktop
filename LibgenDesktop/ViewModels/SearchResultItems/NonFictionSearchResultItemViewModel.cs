using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.SearchResultItems
{
    internal class NonFictionSearchResultItemViewModel : SearchResultItemViewModel<NonFictionBook>
    {
        public NonFictionSearchResultItemViewModel(NonFictionBook book, LanguageFormatter formatter)
            : base(book, formatter)
        {
        }

        public NonFictionBook Book => LibgenObject;
        public string Title => Book.Title;
        public string Authors => Book.Authors;
        public string Series => Book.Series;
        public string Year => Book.Year;
        public string Publisher => Book.Publisher;
        public string Format => Book.Format;
        public string FileSize => Formatter.FileSizeToString(Book.SizeInBytes, false);
        public long SizeInBytes => Book.SizeInBytes;
        public bool Ocr => Book.Searchable == "1";

        public override string FileNameWithoutExtension => $"{Authors} - {Title}";
        public override string FileExtension => Format;
        public override string Md5Hash => Book.Md5Hash;

        protected override void UpdateLocalizableProperties()
        {
            NotifyPropertyChanged(nameof(FileSize));
        }
    }
}
