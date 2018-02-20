using System.Collections.Generic;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;

namespace LibgenDesktop.Models.Export
{
    internal class NonFictionExportObject : ExportObject<NonFictionBook>
    {
        private NonFictionExporterLocalizator localization;

        public NonFictionExportObject(ExportWriter exportWriter, Language currentLanguage)
            : base(exportWriter)
        {
            localization = currentLanguage.NonFictionExporter;
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
                    localization.Year,
                    localization.Language,
                    localization.FormatHeader,
                    localization.Isbn,
                    localization.Added,
                    localization.LastModified,
                    localization.Library,
                    localization.FileSize,
                    localization.Topics,
                    localization.Volume,
                    localization.Magazine,
                    localization.City,
                    localization.Edition,
                    localization.BodyMatterPages,
                    localization.TotalPages,
                    localization.Tags,
                    localization.Md5Hash,
                    localization.Comments,
                    localization.LibgenId,
                    localization.Issn,
                    localization.Udc,
                    localization.Lbc,
                    localization.Lcc,
                    localization.Ddc,
                    localization.Doi,
                    localization.OpenLibraryId,
                    localization.GoogleBookId,
                    localization.Asin,
                    localization.Dpi,
                    localization.Ocr,
                    localization.TableOfContents,
                    localization.Scanned,
                    localization.Orientation,
                    localization.Paginated,
                    localization.Colored,
                    localization.Cleaned
                };
            }
        }

        public override void WriteObject(NonFictionBook book)
        {
            WriteField(book.Id);
            WriteField(book.Title);
            WriteField(book.Authors);
            WriteField(book.Series);
            WriteField(book.Publisher);
            WriteField(book.Year);
            WriteField(book.Language);
            WriteField(book.Format);
            WriteField(book.Identifier);
            WriteField(book.AddedDateTime);
            WriteField(book.LastModifiedDateTime);
            WriteField(book.Library);
            WriteField(book.SizeInBytes);
            WriteField(book.Topic);
            WriteField(book.VolumeInfo);
            WriteField(book.Periodical);
            WriteField(book.City);
            WriteField(book.Edition);
            WriteField(localization.GetBodyMatterPageCountString(book.Pages));
            WriteField(book.PagesInFile);
            WriteField(book.Tags);
            WriteField(book.Md5Hash);
            WriteField(book.Commentary);
            WriteField(book.LibgenId);
            WriteField(book.Issn);
            WriteField(book.Udc);
            WriteField(book.Lbc);
            WriteField(book.Lcc);
            WriteField(book.Ddc);
            WriteField(book.Doi);
            WriteField(book.OpenLibraryId);
            WriteField(book.GoogleBookId);
            WriteField(book.Asin);
            WriteField(book.Dpi);
            WriteField(localization.GetOcrString(book.Searchable));
            WriteField(localization.GetBookmarkedString(book.Bookmarked));
            WriteField(localization.GetScannedString(book.Scanned));
            WriteField(localization.GetOrientationString(book.Orientation));
            WriteField(localization.GetPaginatedString(book.Paginated));
            WriteField(localization.GetColorString(book.Color));
            WriteField(localization.GetCleanedString(book.Cleaned));
        }
    }
}
