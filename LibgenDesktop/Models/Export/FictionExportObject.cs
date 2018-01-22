using System.Collections.Generic;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Export
{
    internal class FictionExportObject : ExportObject<FictionBook>
    {
        public FictionExportObject(ExportWriter exportWriter)
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
                    "Автор (рус.)",
                    "Серия",
                    "Издатель",
                    "Издание",
                    "Год",
                    "Язык",
                    "Формат",
                    "Страниц",
                    "Версия",
                    "Размер файла",
                    "Добавлено",
                    "Обновлено",
                    "MD5-хэш",
                    "Комментарий",
                    "Libgen ID",
                    "ISBN",
                    "Google Books ID",
                    "ASIN"
                };
            }
        }

        public override void WriteObject(FictionBook book)
        {
            WriteField(book.Id);
            WriteField(book.Title);
            WriteField(book.Authors);
            WriteField(book.RussianAuthor);
            WriteField(book.Series);
            WriteField(book.Publisher);
            WriteField(book.Edition);
            WriteField(book.Year);
            WriteField(book.Language);
            WriteField(book.Format);
            WriteField(book.PagesString);
            WriteField(book.Version);
            WriteField(book.SizeInBytes);
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
