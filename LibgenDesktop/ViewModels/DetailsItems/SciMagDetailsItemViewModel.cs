using System;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;

namespace LibgenDesktop.ViewModels.DetailsItems
{
    internal class SciMagDetailsItemViewModel : DetailsItemViewModel<SciMagArticle>
    {
        private SciMagDetailsTabLocalizator localization;

        public SciMagDetailsItemViewModel(SciMagArticle libgenObject, Language currentLanguage)
            : base(libgenObject, currentLanguage)
        {
            localization = currentLanguage.SciMagDetailsTab;
        }

        public SciMagArticle Article => LibgenObject;
        public string Title => Article.Title;
        public string Authors => Article.Authors;
        public string Journal => Article.Journal;
        public string Year => Article.Year;
        public string Month => Article.Month;
        public string Day => Article.Day;
        public string Volume => Article.Volume;
        public string Issue => Article.Issue;
        public string Pages => localization.GetPagesString(Article.FirstPage, Article.LastPage);
        public string FileSize => FormatFileSize(Article.SizeInBytes);
        public string AddedDateTime => localization.GetAddedDateTimeString(Article.AddedDateTime);
        public string Md5Hash => Article.Md5Hash;
        public string AbstractUrl => Article.AbstractUrl;
        public string LibgenId => Article.LibgenId.ToString();
        public string Doi => Article.DoiString;
        public string Isbn => Article.Isbn;
        public string JournalId => Article.JournalId;
        public string Issnp => Article.Issnp;
        public string Issne => Article.Issne;
        public string PubmedId => Article.PubmedId;
        public string Pmc => Article.Pmc;
        public string Pii => Article.Pii;
        public string Attribute1 => Article.Attribute1;
        public string Attribute2 => Article.Attribute2;
        public string Attribute3 => Article.Attribute3;
        public string Attribute4 => Article.Attribute4;
        public string Attribute5 => Article.Attribute5;
        public string Attribute6 => Article.Attribute6;

        protected override void UpdateLocalizableProperties()
        {
            localization = CurrentLanguage.SciMagDetailsTab;
            NotifyPropertyChanged(nameof(Pages));
            NotifyPropertyChanged(nameof(FileSize));
            NotifyPropertyChanged(nameof(AddedDateTime));
        }
    }
}
