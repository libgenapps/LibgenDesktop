using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Export
{
    internal class SciMagExporterLocalizator : ExporterLocalizator<Translation.SciMagExporterColumnsTranslation>
    {
        public SciMagExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Exporter?.SciMagColumns)
        {
            Id = Format(section => section?.Id);
            Title = Format(section => section?.Title);
            Authors = Format(section => section?.Authors);
            Magazine = Format(section => section?.Magazine);
            Year = Format(section => section?.Year);
            Month = Format(section => section?.Month);
            Day = Format(section => section?.Day);
            Volume = Format(section => section?.Volume);
            Issue = Format(section => section?.Issue);
            Pages = Format(section => section?.Pages);
            FileSize = Format(section => section?.FileSize);
            AddedDateTime = Format(section => section?.AddedDateTime);
            Md5Hash = Format(section => section?.Md5Hash);
            AbstractUrl = Format(section => section?.AbstractUrl);
            LibgenId = Format(section => section?.LibgenId);
            Doi1 = Format(section => section?.Doi1);
            Doi2 = Format(section => section?.Doi2);
            Isbn = Format(section => section?.Isbn);
            MagazineId = Format(section => section?.MagazineId);
            Issnp = Format(section => section?.Issnp);
            Issne = Format(section => section?.Issne);
            PubmedId = Format(section => section?.PubmedId);
            Pmc = Format(section => section?.Pmc);
            Pii = Format(section => section?.Pii);
            Attribute1 = Format(section => section?.Attribute1);
            Attribute2 = Format(section => section?.Attribute2);
            Attribute3 = Format(section => section?.Attribute3);
            Attribute4 = Format(section => section?.Attribute4);
            Attribute5 = Format(section => section?.Attribute5);
            Attribute6 = Format(section => section?.Attribute6);
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
    }
}
