using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SciMagExporterLocalizator : ExporterLocalizator
    {
        public SciMagExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Id = Format(translation => translation?.Id);
            Title = Format(translation => translation?.Title);
            Authors = Format(translation => translation?.Authors);
            Magazine = Format(translation => translation?.Magazine);
            Year = Format(translation => translation?.Year);
            Month = Format(translation => translation?.Month);
            Day = Format(translation => translation?.Day);
            Volume = Format(translation => translation?.Volume);
            Issue = Format(translation => translation?.Issue);
            Pages = Format(translation => translation?.Pages);
            FileSize = Format(translation => translation?.FileSize);
            AddedDateTime = Format(translation => translation?.AddedDateTime);
            Md5Hash = Format(translation => translation?.Md5Hash);
            AbstractUrl = Format(translation => translation?.AbstractUrl);
            LibgenId = Format(translation => translation?.LibgenId);
            Doi1 = Format(translation => translation?.Doi1);
            Doi2 = Format(translation => translation?.Doi2);
            Isbn = Format(translation => translation?.Isbn);
            MagazineId = Format(translation => translation?.MagazineId);
            Issnp = Format(translation => translation?.Issnp);
            Issne = Format(translation => translation?.Issne);
            PubmedId = Format(translation => translation?.PubmedId);
            Pmc = Format(translation => translation?.Pmc);
            Pii = Format(translation => translation?.Pii);
            Attribute1 = Format(translation => translation?.Attribute1);
            Attribute2 = Format(translation => translation?.Attribute2);
            Attribute3 = Format(translation => translation?.Attribute3);
            Attribute4 = Format(translation => translation?.Attribute4);
            Attribute5 = Format(translation => translation?.Attribute5);
            Attribute6 = Format(translation => translation?.Attribute6);
        }

        public string Id { get; }
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
        public string LibgenId { get; }
        public string Doi1 { get; }
        public string Doi2 { get; }
        public string Isbn { get; }
        public string MagazineId { get; }
        public string Issnp { get; }
        public string Issne { get; }
        public string PubmedId { get; }
        public string Pmc { get; }
        public string Pii { get; }
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

        private string Format(Func<Translation.SciMagExporterColumnsTranslation, string> field)
        {
            return Format(translation => field(translation?.Exporter?.SciMagColumns));
        }
    }
}
