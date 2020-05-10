using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class FictionDetailsTabLocalizator : DetailsTabLocalizator<Translation.FictionDetailsTabTranslation>
    {
        public FictionDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.FictionDetailsTab)
        {
            Title = FormatHeader(section => section?.Title);
            Authors = FormatHeader(section => section?.Authors);
            Series = FormatHeader(section => section?.Series);
            Publisher = FormatHeader(section => section?.Publisher);
            Edition = FormatHeader(section => section?.Edition);
            Year = FormatHeader(section => section?.Year);
            Language = FormatHeader(section => section?.Language);
            FormatLabel = FormatHeader(section => section?.Format);
            Pages = FormatHeader(section => section?.Pages);
            FileSize = FormatHeader(section => section?.FileSize);
            Library = FormatHeader(section => section?.Library);
            Issue = FormatHeader(section => section?.Issue);
            Added = FormatHeader(section => section?.Added);
            LastModified = FormatHeader(section => section?.LastModified);
            Md5Hash = FormatHeader(section => section?.Md5Hash);
            Comments = FormatHeader(section => section?.Comments);
            Identifiers = FormatHeader(section => section?.Identifiers);
            LibgenId = FormatHeader(section => section?.LibgenId);
            Isbn = FormatHeader(section => section?.Isbn);
            GoogleBookId = FormatHeader(section => section?.GoogleBookId);
            Asin = FormatHeader(section => section?.Asin);
        }

        public string Title { get; }
        public string Authors { get; }
        public string Series { get; }
        public string Publisher { get; }
        public string Edition { get; }
        public string Year { get; }
        public string Language { get; }
        public string FormatLabel { get; }
        public string Pages { get; }
        public string FileSize { get; }
        public string Library { get; }
        public string Issue { get; }
        public string Added { get; }
        public string LastModified { get; }
        public string Md5Hash { get; }
        public string Comments { get; }
        public string Identifiers { get; }
        public string LibgenId { get; }
        public string Isbn { get; }
        public string GoogleBookId { get; }
        public string Asin { get; }

        public static string GetYearString(string value) => value != "0" ? value : String.Empty;

        public string GetPagesString(string value) => value != "0" ? value : Unknown;

        public string GetLastModifiedDateTimeString(DateTime? value) => value.HasValue ? Formatter.ToFormattedDateTimeString(value.Value) : Unknown;
    }
}
