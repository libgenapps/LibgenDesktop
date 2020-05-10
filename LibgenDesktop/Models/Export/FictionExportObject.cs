using System.Collections.Generic;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Export;

namespace LibgenDesktop.Models.Export
{
    internal class FictionExportObject : ExportObject<FictionBook>
    {
        private readonly FictionExporterLocalizator localization;

        public FictionExportObject(ExportWriter exportWriter, Language currentLanguage)
            : base(exportWriter)
        {
            localization = currentLanguage.FictionExporter;
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
                    localization.Series,
                    localization.Publisher,
                    localization.Edition,
                    localization.Year,
                    localization.Language,
                    localization.FormatHeader,
                    localization.Pages,
                    localization.FileSize,
                    localization.Library,
                    localization.Issue,
                    localization.Added,
                    localization.LastModified,
                    localization.Md5Hash,
                    localization.Comments,
                    localization.LibgenId,
                    localization.Isbn,
                    localization.GoogleBookId,
                    localization.Asin
                };
            }
        }

        public override void WriteObject(FictionBook book)
        {
            WriteField(book.Id);
            WriteField(book.Title);
            WriteField(book.Authors);
            WriteField(book.Series);
            WriteField(book.Publisher);
            WriteField(book.Edition);
            WriteField(localization.GetYearString(book.Year));
            WriteField(book.Language);
            WriteField(book.Format);
            WriteField(localization.GetPagesString(book.Pages));
            WriteField(book.SizeInBytes);
            WriteField(book.Library);
            WriteField(book.Issue);
            WriteField(book.AddedDateTime);
            WriteField(book.LastModifiedDateTime);
            WriteField(book.Md5Hash);
            WriteField(book.Commentary);
            WriteField(book.LibgenId);
            WriteField(book.Identifier);
            WriteField(book.GoogleBookId);
            WriteField(book.Asin);
        }
    }
}
