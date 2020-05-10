using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Tabs;

namespace LibgenDesktop.ViewModels.DetailsItems
{
    internal class NonFictionDetailsItemViewModel : DetailsItemViewModel<NonFictionBook>
    {
        private NonFictionDetailsTabLocalizator localization;

        public NonFictionDetailsItemViewModel(NonFictionBook book, Language currentLanguage)
            : base(book, currentLanguage)
        {
            localization = currentLanguage.NonFictionDetailsTab;
        }

        public NonFictionBook Book => LibgenObject;
        public string Title => Book.Title;
        public string Authors => Book.Authors;
        public string Series => Book.Series;
        public string Publisher => Book.Publisher;
        public string Year => Book.Year;
        public string Language => Book.Language;
        public string Format => Book.Format;
        public string Identifier => Book.Identifier;
        public string AddedDateTime => FormatDateTime(Book.AddedDateTime);
        public string LastModifiedDateTime => FormatDateTime(Book.LastModifiedDateTime);
        public string Library => Book.Library;
        public string FileSize => FormatFileSize(Book.SizeInBytes);
        public string Topic => Book.Topic;
        public string Volume => Book.VolumeInfo;
        public string Periodical => Book.Periodical;
        public string City => Book.City;
        public string Edition => Book.Edition;
        public string Pages => localization.GetPagesText(Book.Pages, Book.PagesInFile);
        public string Tags => Book.Tags;
        public string Md5Hash => Book.Md5Hash;
        public string Commentary => Book.Commentary;
        public string LibgenId => Book.LibgenId.ToString();
        public string Issn => Book.Issn;
        public string Udc => Book.Udc;
        public string Lbc => Book.Lbc;
        public string Lcc => Book.Lcc;
        public string Ddc => Book.Ddc;
        public string Doi => Book.Doi;
        public string OpenLibraryId => Book.OpenLibraryId;
        public string GoogleBookId => Book.GoogleBookId;
        public string Asin => Book.Asin;
        public string Dpi => Book.Dpi.ToString();
        public string Ocr => localization.GetOcrString(Book.Searchable);
        public string Bookmarked => localization.GetBookmarkedString(Book.Bookmarked);
        public string Scanned => localization.GetScannedString(Book.Scanned);
        public string Orientation => localization.GetOrientationString(Book.Orientation);
        public string Paginated => localization.GetPaginatedString(Book.Paginated);
        public string Color => localization.GetColorString(Book.Color);
        public string Cleaned => localization.GetCleanedString(Book.Cleaned);

        protected override void UpdateLocalizableProperties()
        {
            localization = CurrentLanguage.NonFictionDetailsTab;
            NotifyPropertyChanged(nameof(AddedDateTime));
            NotifyPropertyChanged(nameof(LastModifiedDateTime));
            NotifyPropertyChanged(nameof(FileSize));
            NotifyPropertyChanged(nameof(Pages));
            NotifyPropertyChanged(nameof(Ocr));
            NotifyPropertyChanged(nameof(Bookmarked));
            NotifyPropertyChanged(nameof(Scanned));
            NotifyPropertyChanged(nameof(Orientation));
            NotifyPropertyChanged(nameof(Paginated));
            NotifyPropertyChanged(nameof(Color));
            NotifyPropertyChanged(nameof(Cleaned));
        }
    }
}
