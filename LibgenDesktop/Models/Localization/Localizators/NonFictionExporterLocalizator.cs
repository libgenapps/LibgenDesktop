using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class NonFictionExporterLocalizator : ExporterLocalizator
    {
        public NonFictionExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Id = Format(translation => translation?.Id);
            Title = Format(translation => translation?.Title);
            Authors = Format(translation => translation?.Authors);
            Series = Format(translation => translation?.Series);
            Publisher = Format(translation => translation?.Publisher);
            Year = Format(translation => translation?.Year);
            Language = Format(translation => translation?.Language);
            FormatHeader = Format(translation => translation?.Format);
            Isbn = Format(translation => translation?.Isbn);
            Added = Format(translation => translation?.Added);
            LastModified = Format(translation => translation?.LastModified);
            Library = Format(translation => translation?.Library);
            FileSize = Format(translation => translation?.FileSize);
            Topics = Format(translation => translation?.Topics);
            Volume = Format(translation => translation?.Volume);
            Magazine = Format(translation => translation?.Magazine);
            City = Format(translation => translation?.City);
            Edition = Format(translation => translation?.Edition);
            BodyMatterPages = Format(translation => translation?.BodyMatterPages);
            TotalPages = Format(translation => translation?.TotalPages);
            Tags = Format(translation => translation?.Tags);
            Md5Hash = Format(translation => translation?.Md5Hash);
            Comments = Format(translation => translation?.Comments);
            LibgenId = Format(translation => translation?.LibgenId);
            Issn = Format(translation => translation?.Issn);
            Udc = Format(translation => translation?.Udc);
            Lbc = Format(translation => translation?.Lbc);
            Lcc = Format(translation => translation?.Lcc);
            Ddc = Format(translation => translation?.Ddc);
            Doi = Format(translation => translation?.Doi);
            OpenLibraryId = Format(translation => translation?.OpenLibraryId);
            GoogleBookId = Format(translation => translation?.GoogleBookId);
            Asin = Format(translation => translation?.Asin);
            Dpi = Format(translation => translation?.Dpi);
            Ocr = Format(translation => translation?.Ocr);
            TableOfContents = Format(translation => translation?.TableOfContents);
            Scanned = Format(translation => translation?.Scanned);
            Orientation = Format(translation => translation?.Orientation);
            Paginated = Format(translation => translation?.Paginated);
            Colored = Format(translation => translation?.Colored);
            Cleaned = Format(translation => translation?.Cleaned);
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

        private string Format(Func<Translation.NonFictionExporterColumnsTranslation, string> field)
        {
            return Format(translation => field(translation?.Exporter?.NonFictionColumns));
        }
    }
}
