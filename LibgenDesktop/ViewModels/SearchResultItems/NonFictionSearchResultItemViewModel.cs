using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;

namespace LibgenDesktop.ViewModels.SearchResultItems
{
    internal class GroupingByIdentifier
    {
        protected Dictionary<string, int> mapUnqiqueID = new Dictionary<string, int>();
        protected int lastId = 1;

        public int GetId(NonFictionBook book)
        {
            if (string.IsNullOrEmpty(book.IdentifierPlain?.Trim()))
                return 0;

            string[] ids = book.IdentifierPlain.Split(',');
            int resId = lastId;
            foreach (var id in ids)
            {
                string id1 = id.Trim();
                int uniqueId;
                if (mapUnqiqueID.TryGetValue(id1, out uniqueId))
                {
                    resId = uniqueId;
                    break;
                }
            }
            if (resId == lastId)
                ++lastId;
            foreach (var id in ids)
            {
                string id1 = id.Trim();
                if (!mapUnqiqueID.ContainsKey(id1))
                    mapUnqiqueID[id1] = resId;
            }
            return resId;
        }
    }

    internal class NonFictionSearchResultItemViewModel : SearchResultItemViewModel<NonFictionBook>
    {
        public NonFictionSearchResultItemViewModel(NonFictionBook book, LanguageFormatter formatter, GroupingByIdentifier grouping)
            : base(book, formatter)
        {
            GroupId = grouping.GetId(book);
        }

        public NonFictionBook Book => LibgenObject;
        public string Title => Book.Title;
        public string Authors => Book.Authors;
        public string Series => Book.Series;
        public string Year => Book.Year;
        public string Identifier => Book.Identifier;
        public int GroupId { get; set; }
        public string Language => Book.Language;
        public string Publisher => Book.Publisher;
        public string Format => Book.Format;
        public string FileSize => Formatter.FileSizeToString(Book.SizeInBytes, false);
        public long SizeInBytes => Book.SizeInBytes;
        public bool Ocr => Book.Searchable == "1";

        public override string FileNameWithoutExtension => $"{Authors} - {Title}";
        public override string FileExtension => Format;
        public override string Md5Hash => Book.Md5Hash;

        public override ObservableCollection<string> GetCopyMenuItems()
        {
            return GetNonEmptyCopyMenuItems(Title, Authors, Series, Year, Language, Publisher, Format, FileSize);
        }

        protected override void UpdateLocalizableProperties()
        {
            NotifyPropertyChanged(nameof(FileSize));
        }
    }
}
