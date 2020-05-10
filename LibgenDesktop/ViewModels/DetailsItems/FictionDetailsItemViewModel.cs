using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Tabs;

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
        public string Md5Hash => Book.Md5Hash;
        public string Title => Book.Title;
        public string Authors => Book.Authors;
        public string Series => Book.Series;
        public string Edition => Book.Edition;
        public string Language => Book.Language;
        public string Year => FictionDetailsTabLocalizator.GetYearString(Book.Year);
        public string Publisher => Book.Publisher;
        public string Pages => localization.GetPagesString(Book.Pages);
        public string Identifier => Book.Identifier;
        public string GoogleBookId => Book.GoogleBookId;
        public string Asin => Book.Asin;
        public string Format => Book.Format;
        public string FileSize => FormatFileSize(Book.SizeInBytes);
        public string Library => Book.Library;
        public string Issue => Book.Issue;
        public string Commentary => Book.Commentary;
        public string AddedDateTime => localization.GetLastModifiedDateTimeString(Book.AddedDateTime);
        public string LastModifiedDateTime => localization.GetLastModifiedDateTimeString(Book.LastModifiedDateTime);
        public string LibgenId => Book.LibgenId.ToString();

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
