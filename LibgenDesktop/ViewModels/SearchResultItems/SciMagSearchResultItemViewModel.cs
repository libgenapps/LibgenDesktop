using System.Collections.ObjectModel;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.SearchResultItems
{
    internal class SciMagSearchResultItemViewModel : SearchResultItemViewModel<SciMagArticle>
    {
        public SciMagSearchResultItemViewModel(SciMagArticle article, LanguageFormatter formatter)
            : base(article, formatter)
        {
        }

        public SciMagArticle Article => LibgenObject;
        public string Title => Article.Title;
        public string Authors => Article.Authors;
        public string Journal => Article.Journal;
        public string Year => Article.Year;
        public string FileSize => Formatter.FileSizeToString(Article.SizeInBytes, false);
        public long SizeInBytes => Article.SizeInBytes;
        public string Doi => Article.DoiString;

        public override string FileNameWithoutExtension => $"{Authors} - {Title}";
        public override string FileExtension => "pdf";
        public override string Md5Hash => Article.Md5Hash;

        public override ObservableCollection<string> GetCopyMenuItems()
        {
            return GetNonEmptyCopyMenuItems(Title, Authors, Journal, Year, FileSize, Doi);
        }

        protected override void UpdateLocalizableProperties()
        {
            NotifyPropertyChanged(nameof(FileSize));
        }
    }
}
