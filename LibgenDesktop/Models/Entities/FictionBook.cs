using System;

namespace LibgenDesktop.Models.Entities
{
    internal class FictionBook : LibgenObject
    {
        public FictionBook()
            : base(LibgenObjectType.FICTION_BOOK)
        {
        }

        public string Md5Hash { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Series { get; set; }
        public string Edition { get; set; }
        public string Language { get; set; }
        public string Year { get; set; }
        public string Publisher { get; set; }
        public string Pages { get; set; }
        public string Identifier { get; set; }
        public string GoogleBookId { get; set; }
        public string Asin { get; set; }
        public string CoverUrl { get; set; }
        public string Format { get; set; }
        public long SizeInBytes { get; set; }
        public string Library { get; set; }
        public string Issue { get; set; }
        public string Locator { get; set; }
        public string Commentary { get; set; }
        public string Generic { get; set; }
        public string Visible { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
