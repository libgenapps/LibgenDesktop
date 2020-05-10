using System.Collections.Generic;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Export;

namespace LibgenDesktop.Models.Export
{
    internal class SciMagExportObject : ExportObject<SciMagArticle>
    {
        private readonly SciMagExporterLocalizator localization;

        public SciMagExportObject(ExportWriter exportWriter, Language currentLanguage)
            : base(exportWriter)
        {
            localization = currentLanguage.SciMagExporter;
        }

        public override IEnumerable<string> FieldList
        {
            get
            {
                return new[]
                {
                    localization.Id,
                    localization.Title,
                    localization.Authors,
                    localization.Magazine,
                    localization.Year,
                    localization.Month,
                    localization.Day,
                    localization.Volume,
                    localization.Issue,
                    localization.Pages,
                    localization.FileSize,
                    localization.AddedDateTime,
                    localization.Md5Hash,
                    localization.AbstractUrl,
                    localization.LibgenId,
                    localization.Doi1,
                    localization.Doi2,
                    localization.Isbn,
                    localization.MagazineId,
                    localization.Issnp,
                    localization.Issne,
                    localization.PubmedId,
                    localization.Pmc,
                    localization.Pii,
                    localization.Attribute1,
                    localization.Attribute2,
                    localization.Attribute3,
                    localization.Attribute4,
                    localization.Attribute5,
                    localization.Attribute6
                };
            }
        }

        public override void WriteObject(SciMagArticle article)
        {
            WriteField(article.Id);
            WriteField(article.Title);
            WriteField(article.Authors);
            WriteField(article.Journal);
            WriteField(article.Year);
            WriteField(article.Month);
            WriteField(article.Day);
            WriteField(article.Volume);
            WriteField(article.Issue);
            WriteField(localization.GetPagesString(article.FirstPage, article.LastPage));
            WriteField(article.SizeInBytes);
            WriteField(article.AddedDateTime);
            WriteField(article.Md5Hash);
            WriteField(article.AbstractUrl);
            WriteField(article.LibgenId);
            WriteField(article.Doi);
            WriteField(article.Doi2);
            WriteField(article.Isbn);
            WriteField(article.JournalId);
            WriteField(article.Issnp);
            WriteField(article.Issne);
            WriteField(article.PubmedId);
            WriteField(article.Pmc);
            WriteField(article.Pii);
            WriteField(article.Attribute1);
            WriteField(article.Attribute2);
            WriteField(article.Attribute3);
            WriteField(article.Attribute4);
            WriteField(article.Attribute5);
            WriteField(article.Attribute6);
        }
    }
}
