using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Export
{
    internal class FictionExporterLocalizator : ExporterLocalizator<Translation.FictionExporterColumnsTranslation>
    {
        public FictionExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Exporter?.FictionColumns)
        {
            Id = Format(section => section?.Id);
            Title = Format(section => section?.Title);
            Authors = Format(section => section?.Authors);
            Series = Format(section => section?.Series);
            Publisher = Format(section => section?.Publisher);
            Edition = Format(section => section?.Edition);
            Year = Format(section => section?.Year);
            Language = Format(section => section?.Language);
            FormatHeader = Format(section => section?.Format);
            Pages = Format(section => section?.Pages);
            FileSize = Format(section => section?.FileSize);
            Library = Format(section => section?.Library);
            Issue = Format(section => section?.Issue);
            Added = Format(section => section?.Added);
            LastModified = Format(section => section?.LastModified);
            Md5Hash = Format(section => section?.Md5Hash);
            Comments = Format(section => section?.Comments);
            LibgenId = Format(section => section?.LibgenId);
            Isbn = Format(section => section?.Isbn);
            GoogleBookId = Format(section => section?.GoogleBookId);
            Asin = Format(section => section?.Asin);
        }

        public string Id { get; }
        public string Title { get; }
        public string Authors { get; }
        public string Series { get; }
        public string Publisher { get; }
        public string Edition { get; }
        public string Year { get; }
        public string Language { get; }
        public string FormatHeader { get; }
        public string Pages { get; }
        public string FileSize { get; }
        public string Library { get; }
        public string Issue { get; }
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
    }
}
