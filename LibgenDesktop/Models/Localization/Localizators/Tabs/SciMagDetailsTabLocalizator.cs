using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class SciMagDetailsTabLocalizator : DetailsTabLocalizator<Translation.SciMagDetailsTabTranslation>
    {
        public SciMagDetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SciMagDetailsTab)
        {
            Title = FormatHeader(section => section?.Title);
            Authors = FormatHeader(section => section?.Authors);
            Magazine = FormatHeader(section => section?.Magazine);
            Year = FormatHeader(section => section?.Year);
            Month = FormatHeader(section => section?.Month);
            Day = FormatHeader(section => section?.Day);
            Volume = FormatHeader(section => section?.Volume);
            Issue = FormatHeader(section => section?.Issue);
            Pages = FormatHeader(section => section?.Pages);
            FileSize = FormatHeader(section => section?.FileSize);
            AddedDateTime = FormatHeader(section => section?.AddedDateTime);
            Md5Hash = FormatHeader(section => section?.Md5Hash);
            AbstractUrl = FormatHeader(section => section?.AbstractUrl);
            Identifiers = FormatHeader(section => section?.Identifiers);
            LibgenId = FormatHeader(section => section?.LibgenId);
            Doi = FormatHeader(section => section?.Doi);
            Isbn = FormatHeader(section => section?.Isbn);
            MagazineId = FormatHeader(section => section?.MagazineId);
            Issnp = FormatHeader(section => section?.Issnp);
            Issne = FormatHeader(section => section?.Issne);
            PubmedId = FormatHeader(section => section?.PubmedId);
            Pmc = FormatHeader(section => section?.Pmc);
            Pii = FormatHeader(section => section?.Pii);
            AdditionalAttributes = FormatHeader(section => section?.AdditionalAttributes);
            Attribute1 = FormatHeader(section => section?.Attribute1);
            Attribute2 = FormatHeader(section => section?.Attribute2);
            Attribute3 = FormatHeader(section => section?.Attribute3);
            Attribute4 = FormatHeader(section => section?.Attribute4);
            Attribute5 = FormatHeader(section => section?.Attribute5);
            Attribute6 = FormatHeader(section => section?.Attribute6);
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
    }
}
