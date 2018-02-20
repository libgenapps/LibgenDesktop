using System;
using System.Collections.Generic;
using System.Text;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class NonFictionDetailsTabLocalizator : DetailsTabLocalizator
    {
        public NonFictionDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Title = FormatHeader(translation => translation?.Title);
            Authors = FormatHeader(translation => translation?.Authors);
            Series = FormatHeader(translation => translation?.Series);
            Publisher = FormatHeader(translation => translation?.Publisher);
            Year = FormatHeader(translation => translation?.Year);
            Language = FormatHeader(translation => translation?.Language);
            FormatLabel = FormatHeader(translation => translation?.Format);
            Isbn = FormatHeader(translation => translation?.Isbn);
            Added = FormatHeader(translation => translation?.Added);
            LastModified = FormatHeader(translation => translation?.LastModified);
            Library = FormatHeader(translation => translation?.Library);
            FileSize = FormatHeader(translation => translation?.FileSize);
            Topics = FormatHeader(translation => translation?.Topics);
            Volume = FormatHeader(translation => translation?.Volume);
            Magazine = FormatHeader(translation => translation?.Magazine);
            City = FormatHeader(translation => translation?.City);
            Edition = FormatHeader(translation => translation?.Edition);
            Pages = FormatHeader(translation => translation?.Pages);
            Tags = FormatHeader(translation => translation?.Tags);
            Md5Hash = FormatHeader(translation => translation?.Md5Hash);
            Comments = FormatHeader(translation => translation?.Comments);
            Identifiers = FormatHeader(translation => translation?.Identifiers);
            LibgenId = FormatHeader(translation => translation?.LibgenId);
            Issn = FormatHeader(translation => translation?.Issn);
            Udc = FormatHeader(translation => translation?.Udc);
            Lbc = FormatHeader(translation => translation?.Lbc);
            Lcc = FormatHeader(translation => translation?.Lcc);
            Ddc = FormatHeader(translation => translation?.Ddc);
            Doi = FormatHeader(translation => translation?.Doi);
            OpenLibraryId = FormatHeader(translation => translation?.OpenLibraryId);
            GoogleBookId = FormatHeader(translation => translation?.GoogleBookId);
            Asin = FormatHeader(translation => translation?.Asin);
            AdditionalAttributes = FormatHeader(translation => translation?.AdditionalAttributes);
            Dpi = FormatHeader(translation => translation?.Dpi);
            Ocr = FormatHeader(translation => translation?.Ocr);
            TableOfContents = FormatHeader(translation => translation?.TableOfContents);
            Scanned = FormatHeader(translation => translation?.Scanned);
            Orientation = FormatHeader(translation => translation?.Orientation);
            Paginated = FormatHeader(translation => translation?.Paginated);
            Colored = FormatHeader(translation => translation?.Colored);
            Cleaned = FormatHeader(translation => translation?.Cleaned);
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
            resultBuilder.Append(Format(translation => translation?.BodyMatterPages));
            resultBuilder.Append(") / ");
            resultBuilder.Append(totalPages.ToString());
            resultBuilder.Append(" (");
            resultBuilder.Append(Format(translation => translation?.TotalPages));
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

        private string Format(Func<Translation.NonFictionDetailsTabTranslation, string> field)
        {
            return Format(translation => field(translation?.NonFictionDetailsTab));
        }

        private string FormatHeader(Func<Translation.NonFictionDetailsTabTranslation, string> field)
        {
            return Format(field) + ":";
        }
    }
}
