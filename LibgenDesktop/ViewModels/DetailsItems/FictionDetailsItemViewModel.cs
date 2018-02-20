using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;

namespace LibgenDesktop.ViewModels.DetailsItems
{
    internal class FictionDetailsItemViewModel : DetailsItemViewModel<FictionBook>
    {
        private FictionDetailsTabLocalizator localization;

        public FictionDetailsItemViewModel(FictionBook libgenObject, Language currentLanguage)
            : base(libgenObject, currentLanguage)
        {
            localization = currentLanguage.FictionDetailsTab;
        }

        public FictionBook Book => LibgenObject;
        public string Title => Book.Title;
        public string Authors => Book.Authors;
        public string RussianAuthor => Book.RussianAuthor;
        public string Series => Book.Series;
        public string Publisher => Book.Publisher;
        public string Edition => Book.Edition;
        public string Year => localization.GetYearString(Book.Year);
        public string Language => Book.Language;
        public string Format => Book.Format;
        public string Pages => localization.GetPagesString(Book.Pages);
        public string Version => Book.Version;
        public string FileSize => FormatFileSize(Book.SizeInBytes);
        public string AddedDateTime => localization.GetAddedDateTimeString(Book.AddedDateTime);
        public string LastModifiedDateTime => FormatDateTime(Book.LastModifiedDateTime);
        public string Md5Hash => Book.Md5Hash;
        public string Commentary => Book.Commentary;
        public string LibgenId => Book.LibgenId.ToString();
        public string Identifier => Book.Identifier;
        public string GoogleBookId => Book.GoogleBookId;
        public string Asin => Book.Asin;

        protected override void UpdateLocalizableProperties()
        {
            localization = CurrentLanguage.FictionDetailsTab;
            NotifyPropertyChanged(nameof(Year));
            NotifyPropertyChanged(nameof(Pages));
            NotifyPropertyChanged(nameof(FileSize));
            NotifyPropertyChanged(nameof(AddedDateTime));
            NotifyPropertyChanged(nameof(LastModifiedDateTime));
        }
    }
}
