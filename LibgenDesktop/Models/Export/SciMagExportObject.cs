using System.Collections.Generic;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Export
{
    internal class SciMagExportObject : ExportObject<SciMagArticle>
    {
        public SciMagExportObject(ExportWriter exportWriter)
            : base(exportWriter)
        {
        }

        public override IEnumerable<string> FieldList
        {
            get
            {
                return new[]
                {
                    "ID",
                    "Наименование",
                    "Авторы",
                    "Журнал",
                    "Год",
                    "Месяц",
                    "День",
                    "Том",
                    "Выпуск",
                    "Страницы",
                    "Размер файла",
                    "Добавлено",
                    "MD5-хэш",
                    "Abstract URL",
                    "Libgen ID",
                    "DOI 1",
                    "DOI 2",
                    "ISBN",
                    "ID журнала",
                    "ISSN (p)",
                    "ISSN (e)",
                    "Pubmed ID",
                    "PMC",
                    "PII",
                    "Атрибут 1",
                    "Атрибут 2",
                    "Атрибут 3",
                    "Атрибут 4",
                    "Атрибут 5",
                    "Атрибут 6"
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
            WriteField(article.PagesString);
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
