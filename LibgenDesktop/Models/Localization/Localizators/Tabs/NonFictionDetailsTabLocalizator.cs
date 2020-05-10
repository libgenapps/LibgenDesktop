using System;
using System.Collections.Generic;
using System.Text;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class NonFictionDetailsTabLocalizator : DetailsTabLocalizator<Translation.NonFictionDetailsTabTranslation>
    {
        public NonFictionDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.NonFictionDetailsTab)
        {
            Title = FormatHeader(section => section?.Title);
            Authors = FormatHeader(section => section?.Authors);
            Series = FormatHeader(section => section?.Series);
            Publisher = FormatHeader(section => section?.Publisher);
            Year = FormatHeader(section => section?.Year);
            Language = FormatHeader(section => section?.Language);
            FormatLabel = FormatHeader(section => section?.Format);
            Isbn = FormatHeader(section => section?.Isbn);
            Added = FormatHeader(section => section?.Added);
            LastModified = FormatHeader(section => section?.LastModified);
            Library = FormatHeader(section => section?.Library);
            FileSize = FormatHeader(section => section?.FileSize);
            Topics = FormatHeader(section => section?.Topics);
            Volume = FormatHeader(section => section?.Volume);
            Magazine = FormatHeader(section => section?.Magazine);
            City = FormatHeader(section => section?.City);
            Edition = FormatHeader(section => section?.Edition);
            Pages = FormatHeader(section => section?.Pages);
            Tags = FormatHeader(section => section?.Tags);
            Md5Hash = FormatHeader(section => section?.Md5Hash);
            Comments = FormatHeader(section => section?.Comments);
            Identifiers = FormatHeader(section => section?.Identifiers);
            LibgenId = FormatHeader(section => section?.LibgenId);
            Issn = FormatHeader(section => section?.Issn);
            Udc = FormatHeader(section => section?.Udc);
            Lbc = FormatHeader(section => section?.Lbc);
            Lcc = FormatHeader(section => section?.Lcc);
            Ddc = FormatHeader(section => section?.Ddc);
            Doi = FormatHeader(section => section?.Doi);
            OpenLibraryId = FormatHeader(section => section?.OpenLibraryId);
            GoogleBookId = FormatHeader(section => section?.GoogleBookId);
            Asin = FormatHeader(section => section?.Asin);
            AdditionalAttributes = FormatHeader(section => section?.AdditionalAttributes);
            Dpi = FormatHeader(section => section?.Dpi);
            Ocr = FormatHeader(section => section?.Ocr);
            TableOfContents = FormatHeader(section => section?.TableOfContents);
            Scanned = FormatHeader(section => section?.Scanned);
            Orientation = FormatHeader(section => section?.Orientation);
            Paginated = FormatHeader(section => section?.Paginated);
            Colored = FormatHeader(section => section?.Colored);
            Cleaned = FormatHeader(section => section?.Cleaned);
        }

        public string Title { get; }
        public string Authors { get; }
        public string Series { get; }
        public string Publisher { get; }
        public string Year { get; }
        public string Language { get; }
        public string FormatLabel { get; }
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
        public string Pages { get; }
        public string Tags { get; }
        public string Md5Hash { get; }
        public string Comments { get; }
        public string Identifiers { get; }
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
        public string AdditionalAttributes { get; }
        public string Dpi { get; }
        public string Ocr { get; }
        public string TableOfContents { get; }
        public string Scanned { get; }
        public string Orientation { get; }
        public string Paginated { get; }
        public string Colored { get; }
        public string Cleaned { get; }

        public string GetPagesText(string bodyMatterPages, int totalPages)
        {
            StringBuilder resultBuilder = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(bodyMatterPages))
            {
                resultBuilder.Append(bodyMatterPages);
            }
            else
            {
                resultBuilder.Append(Unknown);
            }
            resultBuilder.Append(" (");
            resultBuilder.Append(Format(section => section?.BodyMatterPages));
            resultBuilder.Append(") / ");
            resultBuilder.Append(Formatter.ToFormattedString(totalPages));
            resultBuilder.Append(" (");
            resultBuilder.Append(Format(section => section?.TotalPages));
            resultBuilder.Append(")");
            return resultBuilder.ToString();
        }

        public string GetOcrString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetBookmarkedString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetScannedString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetOrientationString(string value) => StringBooleanToOrientationString(value);

        public string GetPaginatedString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetColorString(string value) => StringBooleanToYesNoUnknownString(value);

        public string GetCleanedString(string value) => StringBooleanToYesNoUnknownString(value);
    }
}
