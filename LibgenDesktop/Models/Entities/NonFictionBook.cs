using System;
using System.Text;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.Models.Entities
{
    internal class NonFictionBook : LibgenObject
    {
        public string Title { get; set; }
        public string VolumeInfo { get; set; }
        public string Series { get; set; }
        public string Periodical { get; set; }
        public string Authors { get; set; }
        public string Year { get; set; }
        public string Edition { get; set; }
        public string Publisher { get; set; }
        public string City { get; set; }
        public string Pages { get; set; }
        public int PagesInFile { get; set; }
        public string Language { get; set; }
        public string Topic { get; set; }
        public string Library { get; set; }
        public string Issue { get; set; }
        public string Identifier { get; set; }
        public string Issn { get; set; }
        public string Asin { get; set; }
        public string Udc { get; set; }
        public string Lbc { get; set; }
        public string Ddc { get; set; }
        public string Lcc { get; set; }
        public string Doi { get; set; }
        public string GoogleBookId { get; set; }
        public string OpenLibraryId { get; set; }
        public string Commentary { get; set; }
        public int Dpi { get; set; }
        public string Color { get; set; }
        public string Cleaned { get; set; }
        public string Orientation { get; set; }
        public string Paginated { get; set; }
        public string Scanned { get; set; }
        public string Bookmarked { get; set; }
        public string Searchable { get; set; }
        public long SizeInBytes { get; set; }
        public string Format { get; set; }
        public string Md5Hash { get; set; }
        public string Generic { get; set; }
        public string Visible { get; set; }
        public string Locator { get; set; }
        public int Local { get; set; }
        public DateTime AddedDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public string CoverUrl { get; set; }
        public string Tags { get; set; }
        public string IdentifierPlain { get; set; }

        public string FileSizeString => Formatters.FileSizeToString(SizeInBytes, false);
        public string FileSizeWithBytesString => Formatters.FileSizeToString(SizeInBytes, true);
        public bool Ocr => Searchable == "1";
        public string SearchableString => StringBooleanToLabelString(Searchable, "да", "нет", "неизвестно");
        public string AddedDateTimeString => AddedDateTime.ToString("dd.MM.yyyy HH:mm:ss");
        public string LastModifiedDateTimeString => LastModifiedDateTime.ToString("dd.MM.yyyy HH:mm:ss");
        public string BookmarkedString => StringBooleanToLabelString(Bookmarked, "есть", "нет", "неизвестно");
        public string ScannedString => StringBooleanToLabelString(Scanned, "да", "нет", "неизвестно");
        public string OrientationString => StringBooleanToLabelString(Orientation, "портретная", "альбомная", "неизвестно");
        public string PaginatedString => StringBooleanToLabelString(Paginated, "да", "нет", "неизвестно");
        public string ColorString => StringBooleanToLabelString(Color, "да", "нет", "неизвестно");
        public string CleanedString => StringBooleanToLabelString(Cleaned, "да", "нет", "неизвестно");

        public string ContentPageCountString
        {
            get
            {
                return !String.IsNullOrWhiteSpace(Pages) ? Pages : "неизвестно";
            }
        }

        public string PagesString
        {
            get
            {
                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.Append(ContentPageCountString);
                resultBuilder.Append(" (содержательная часть) / ");
                resultBuilder.Append(PagesInFile.ToString());
                resultBuilder.Append(" (всего в файле)");
                return resultBuilder.ToString();
            }
        }

        private static string StringBooleanToLabelString(string value, string value1Label, string value0Label, string valueUnknownLabel)
        {
            switch (value)
            {
                case "0":
                    return value0Label;
                case "1":
                    return value1Label;
                default:
                    return valueUnknownLabel;
            }
        }
    }
}
