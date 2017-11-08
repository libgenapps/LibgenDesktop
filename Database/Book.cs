using System;

namespace LibgenDesktop.Database
{
    internal class Book
    {
        internal class BookExtendedProperties
        {
            public string VolumeInfo { get; set; }
            public string Periodical { get; set; }
            public string Edition { get; set; }
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
            public string GoogleBookid { get; set; }
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
            public int LibgenId { get; set; }
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Series { get; set; }
        public string Authors { get; set; }
        public string Year { get; set; }
        public string Publisher { get; set; }
        public long SizeInBytes { get; set; }
        public string Format { get; set; }
        public BookExtendedProperties ExtendedProperties { get; set; }
    }
}
