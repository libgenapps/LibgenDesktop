using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class FictionExporterLocalizator : ExporterLocalizator
    {
        public FictionExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Id = Format(translation => translation?.Id);
            Title = Format(translation => translation?.Title);
            Authors = Format(translation => translation?.Authors);
            RussianAuthor = Format(translation => translation?.RussianAuthor);
            Series = Format(translation => translation?.Series);
            Publisher = Format(translation => translation?.Publisher);
            Edition = Format(translation => translation?.Edition);
            Year = Format(translation => translation?.Year);
            Language = Format(translation => translation?.Language);
            FormatHeader = Format(translation => translation?.Format);
            Pages = Format(translation => translation?.Pages);
            Version = Format(translation => translation?.Version);
            FileSize = Format(translation => translation?.FileSize);
            Added = Format(translation => translation?.Added);
            LastModified = Format(translation => translation?.LastModified);
            Md5Hash = Format(translation => translation?.Md5Hash);
            Comments = Format(translation => translation?.Comments);
            LibgenId = Format(translation => translation?.LibgenId);
            Isbn = Format(translation => translation?.Isbn);
            GoogleBookId = Format(translation => translation?.GoogleBookId);
            Asin = Format(translation => translation?.Asin);
        }

        public string Id { get; }
        public string Title { get; }
        public string Authors { get; }
        public string RussianAuthor { get; }
        public string Series { get; }
        public string Publisher { get; }
        public string Edition { get; }
        public string Year { get; }
        public string Language { get; }
        public string FormatHeader { get; }
        public string Pages { get; }
        public string Version { get; }
        public string FileSize { get; }
        public string Added { get; }
        public string LastModified { get; }
        public string Md5Hash { get; }
        public string Comments { get; }
        public string LibgenId { get; }
        public string Isbn { get; }
        public string GoogleBookId { get; }
        public string Asin { get; }

        public string GetYearString(string value) => value != "0" ? value : String.Empty;
        public string GetPagesString(string value) => !String.IsNullOrWhiteSpace(value) ? value : Unknown;

        private string Format(Func<Translation.FictionExporterColumnsTranslation, string> field)
        {
            return Format(translation => field(translation?.Exporter?.FictionColumns));
        }
    }
}
