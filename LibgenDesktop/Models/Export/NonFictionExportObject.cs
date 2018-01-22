using System.Collections.Generic;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Export
{
    internal class NonFictionExportObject : ExportObject<NonFictionBook>
    {
        public NonFictionExportObject(ExportWriter exportWriter)
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
                    "Серия",
                    "Издатель",
                    "Год",
                    "Язык",
                    "Формат",
                    "ISBN",
                    "Добавлено",
                    "Обновлено",
                    "Библиотека",
                    "Размер файла",
                    "Темы",
                    "Том",
                    "Журнал",
                    "Город",
                    "Издание",
                    "Страниц (содержательная часть)",
                    "Страниц (всего в файле)",
                    "Теги",
                    "MD5-хэш",
                    "Комментарий",
                    "Libgen ID",
                    "ISSN",
                    "UDC",
                    "LBC",
                    "LCC",
                    "DDC",
                    "DOI",
                    "OpenLibraryID",
                    "GoogleID",
                    "ASIN",
                    "DPI",
                    "OCR",
                    "Оглавление",
                    "Отсканирована",
                    "Ориентация",
                    "Постраничная",
                    "Цветная",
                    "Вычищенная"
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
            WriteField(book.ContentPageCountString);
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
            WriteField(book.SearchableString);
            WriteField(book.BookmarkedString);
            WriteField(book.ScannedString);
            WriteField(book.OrientationString);
            WriteField(book.PaginatedString);
            WriteField(book.ColorString);
            WriteField(book.CleanedString);
        }
    }
}
