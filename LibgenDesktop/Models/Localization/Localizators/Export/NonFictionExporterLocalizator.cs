using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Export
{
    internal class NonFictionExporterLocalizator : ExporterLocalizator<Translation.NonFictionExporterColumnsTranslation>
    {
        public NonFictionExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Exporter?.NonFictionColumns)
        {
            Id = Format(section => section?.Id);
            Title = Format(section => section?.Title);
            Authors = Format(section => section?.Authors);
            Series = Format(section => section?.Series);
            Publisher = Format(section => section?.Publisher);
            Year = Format(section => section?.Year);
            Language = Format(section => section?.Language);
            FormatHeader = Format(section => section?.Format);
            Isbn = Format(section => section?.Isbn);
            Added = Format(section => section?.Added);
            LastModified = Format(section => section?.LastModified);
            Library = Format(section => section?.Library);
            FileSize = Format(section => section?.FileSize);
            Topics = Format(section => section?.Topics);
            Volume = Format(section => section?.Volume);
            Magazine = Format(section => section?.Magazine);
            City = Format(section => section?.City);
            Edition = Format(section => section?.Edition);
            BodyMatterPages = Format(section => section?.BodyMatterPages);
            TotalPages = Format(section => section?.TotalPages);
            Tags = Format(section => section?.Tags);
            Md5Hash = Format(section => section?.Md5Hash);
            Comments = Format(section => section?.Comments);
            LibgenId = Format(section => section?.LibgenId);
            Issn = Format(section => section?.Issn);
            Udc = Format(section => section?.Udc);
            Lbc = Format(section => section?.Lbc);
            Lcc = Format(section => section?.Lcc);
            Ddc = Format(section => section?.Ddc);
            Doi = Format(section => section?.Doi);
            OpenLibraryId = Format(section => section?.OpenLibraryId);
            GoogleBookId = Format(section => section?.GoogleBookId);
            Asin = Format(section => section?.Asin);
            Dpi = Format(section => section?.Dpi);
            Ocr = Format(section => section?.Ocr);
            TableOfContents = Format(section => section?.TableOfContents);
            Scanned = Format(section => section?.Scanned);
            Orientation = Format(section => section?.Orientation);
            Paginated = Format(section => section?.Paginated);
            Colored = Format(section => section?.Colored);
            Cleaned = Format(section => section?.Cleaned);
        }

        public string Id { get; }
        public string Title { get; }
        public string Authors { get; }
        public string Series { get; }
        public string Publisher { get; }
        public string Year { get; }
        public string Language { get; }
        public string FormatHeader { get; }
        public string Isbn { get; }
        public string Added { get; }
        public string LastModified { get; }
        public string Library { get; }
        public string FileSize { get; }
        public string Topics { get; }
        public string Volume { get; }
        public string Magazine { get; }
        public string City { get; }
        public string Edition { get; }
        public string BodyMatterPages { get; }
        public string TotalPages { get; }
        public string Tags { get; }
        public string Md5Hash { get; }
        public string Comments { get; }
        public string LibgenId { get; }
        public string Issn { get; }
        public string Udc { get; }
        public string Lbc { get; }
        public string Lcc { get; }
        public string Ddc { get; }
        public string Doi { get; }
        public string OpenLibraryId { get; }
        public string GoogleBookId { get; }
        public string Asin { get; }
        public string Dpi { get; }
        public string Ocr { get; }
        public string TableOfContents { get; }
        public string Scanned { get; }
        public string Orientation { get; }
        public string Paginated { get; }
        public string Colored { get; }
        public string Cleaned { get; }

        public string GetBodyMatterPageCountString(string value) => !String.IsNullOrWhiteSpace(value) ? value : Unknown;

        public string GetOcrString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetBookmarkedString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetScannedString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetOrientationString(string value) => StringBooleanToOrientationString(value);

        public string GetPaginatedString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetColorString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetCleanedString(string value) => StringBooleanToYesNoUnknownString(value);
    }
}
