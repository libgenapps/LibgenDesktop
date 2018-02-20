using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SciMagDetailsTabLocalizator : DetailsTabLocalizator
    {
        public SciMagDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Title = FormatHeader(translation => translation?.Title);
            Authors = FormatHeader(translation => translation?.Authors);
            Magazine = FormatHeader(translation => translation?.Magazine);
            Year = FormatHeader(translation => translation?.Year);
            Month = FormatHeader(translation => translation?.Month);
            Day = FormatHeader(translation => translation?.Day);
            Volume = FormatHeader(translation => translation?.Volume);
            Issue = FormatHeader(translation => translation?.Issue);
            Pages = FormatHeader(translation => translation?.Pages);
            FileSize = FormatHeader(translation => translation?.FileSize);
            AddedDateTime = FormatHeader(translation => translation?.AddedDateTime);
            Md5Hash = FormatHeader(translation => translation?.Md5Hash);
            AbstractUrl = FormatHeader(translation => translation?.AbstractUrl);
            Identifiers = FormatHeader(translation => translation?.Identifiers);
            LibgenId = FormatHeader(translation => translation?.LibgenId);
            Doi = FormatHeader(translation => translation?.Doi);
            Isbn = FormatHeader(translation => translation?.Isbn);
            MagazineId = FormatHeader(translation => translation?.MagazineId);
            Issnp = FormatHeader(translation => translation?.Issnp);
            Issne = FormatHeader(translation => translation?.Issne);
            PubmedId = FormatHeader(translation => translation?.PubmedId);
            Pmc = FormatHeader(translation => translation?.Pmc);
            Pii = FormatHeader(translation => translation?.Pii);
            AdditionalAttributes = FormatHeader(translation => translation?.AdditionalAttributes);
            Attribute1 = FormatHeader(translation => translation?.Attribute1);
            Attribute2 = FormatHeader(translation => translation?.Attribute2);
            Attribute3 = FormatHeader(translation => translation?.Attribute3);
            Attribute4 = FormatHeader(translation => translation?.Attribute4);
            Attribute5 = FormatHeader(translation => translation?.Attribute5);
            Attribute6 = FormatHeader(translation => translation?.Attribute6);
        }

        public string Title { get; }
        public string Authors { get; }
        public string Magazine { get; }
        public string Year { get; }
        public string Month { get; }
        public string Day { get; }
        public string Volume { get; }
        public string Issue { get; }
        public string Pages { get; }
        public string FileSize { get; }
        public string AddedDateTime { get; }
        public string Md5Hash { get; }
        public string AbstractUrl { get; }
        public string Identifiers { get; }
        public string LibgenId { get; }
        public string Doi { get; }
        public string Isbn { get; }
        public string MagazineId { get; }
        public string Issnp { get; }
        public string Issne { get; }
        public string PubmedId { get; }
        public string Pmc { get; }
        public string Pii { get; }
        public string AdditionalAttributes { get; }
        public string Attribute1 { get; }
        public string Attribute2 { get; }
        public string Attribute3 { get; }
        public string Attribute4 { get; }
        public string Attribute5 { get; }
        public string Attribute6 { get; }

        public string GetPagesString(string firstPage, string lastPage)
        {
            if ((!String.IsNullOrWhiteSpace(firstPage) && firstPage != "0") || (!String.IsNullOrWhiteSpace(lastPage) && lastPage != "0"))
            {
                firstPage = firstPage != "0" ? firstPage.Trim() + " " : String.Empty;
                lastPage = lastPage != "0" ? " " + lastPage.Trim() : String.Empty;
                return firstPage + "–" + lastPage;
            }
            else
            {
                return Unknown;
            }
        }

        public string GetAddedDateTimeString(DateTime? value) => value.HasValue ? Formatter.ToFormattedDateTimeString(value.Value) : Unknown;

        private string Format(Func<Translation.SciMagDetailsTabTranslation, string> field)
        {
            return Format(translation => field(translation?.SciMagDetailsTab));
        }

        private string FormatHeader(Func<Translation.SciMagDetailsTabTranslation, string> field)
        {
            return Format(field) + ":";
        }
    }
}
