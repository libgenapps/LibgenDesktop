using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class FictionDetailsTabLocalizator : DetailsTabLocalizator
    {
        public FictionDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Title = FormatHeader(translation => translation?.Title);
            Authors = FormatHeader(translation => translation?.Authors);
            RussianAuthor = FormatHeader(translation => translation?.RussianAuthor);
            Series = FormatHeader(translation => translation?.Series);
            Publisher = FormatHeader(translation => translation?.Publisher);
            Edition = FormatHeader(translation => translation?.Edition);
            Year = FormatHeader(translation => translation?.Year);
            Language = FormatHeader(translation => translation?.Language);
            FormatLabel = FormatHeader(translation => translation?.Format);
            Pages = FormatHeader(translation => translation?.Pages);
            Version = FormatHeader(translation => translation?.Version);
            FileSize = FormatHeader(translation => translation?.FileSize);
            Added = FormatHeader(translation => translation?.Added);
            LastModified = FormatHeader(translation => translation?.LastModified);
            Md5Hash = FormatHeader(translation => translation?.Md5Hash);
            Comments = FormatHeader(translation => translation?.Comments);
            Identifiers = FormatHeader(translation => translation?.Identifiers);
            LibgenId = FormatHeader(translation => translation?.LibgenId);
            Isbn = FormatHeader(translation => translation?.Isbn);
            GoogleBookId = FormatHeader(translation => translation?.GoogleBookId);
            Asin = FormatHeader(translation => translation?.Asin);
        }

        public string Title { get; }
        public string Authors { get; }
        public string RussianAuthor { get; }
        public string Series { get; }
        public string Publisher { get; }
        public string Edition { get; }
        public string Year { get; }
        public string Language { get; }
        public string FormatLabel { get; }
        public string Pages { get; }
        public string Version { get; }
        public string FileSize { get; }
        public string Added { get; }
        public string LastModified { get; }
        public string Md5Hash { get; }
        public string Comments { get; }
        public string Identifiers { get; }
        public string LibgenId { get; }
        public string Isbn { get; }
        public string GoogleBookId { get; }
        public string Asin { get; }

        public string GetYearString(string value) => value != "0" ? value : String.Empty;
        public string GetPagesString(string value) => value != "0" ? value : Unknown;
        public string GetAddedDateTimeString(DateTime? value) => value.HasValue ? Formatter.ToFormattedDateTimeString(value.Value) : Unknown;

        private string Format(Func<Translation.FictionDetailsTabTranslation, string> field)
        {
            return Format(translation => field(translation?.FictionDetailsTab));
        }

        private string FormatHeader(Func<Translation.FictionDetailsTabTranslation, string> field)
        {
            return Format(field) + ":";
        }
    }
}
