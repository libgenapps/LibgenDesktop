using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.Models.Utils
{
    internal static class UrlGenerator
    {
        private static Regex regex;
        private static Dictionary<string, Func<NonFictionBook, string>> nonFictionTransformations;
        private static Dictionary<string, Func<FictionBook, string>> fictionTransformations;
        private static Dictionary<string, Func<SciMagArticle, string>> sciMagTransformations;

        static UrlGenerator()
        {
            regex = new Regex("{([^}]+)}");
            nonFictionTransformations = new Dictionary<string, Func<NonFictionBook, string>>
            {
                { "id", book => book.LibgenId.ToString() },
                { "title", book => book.Title },
                { "volume", book => book.VolumeInfo },
                { "series", book => book.Series },
                { "periodical", book => book.Periodical },
                { "authors", book => book.Authors },
                { "year", book => book.Year },
                { "edition", book => book.Edition },
                { "publisher", book => book.Publisher },
                { "city", book => book.City },
                { "pages", book => book.Pages },
                { "pages-in-file", book => book.PagesInFile.ToString() },
                { "language", book => book.Language },
                { "topic", book => book.Topic },
                { "library", book => book.Library },
                { "issue", book => book.Issue },
                { "isbn", book => book.Identifier },
                { "issn", book => book.Issn },
                { "asin", book => book.Asin },
                { "udc", book => book.Udc },
                { "lbc", book => book.Lbc },
                { "ddc", book => book.Ddc },
                { "lcc", book => book.Lcc },
                { "doi", book => book.Doi },
                { "google-book-id", book => book.GoogleBookId },
                { "open-library-id", book => book.OpenLibraryId },
                { "commentary", book => book.Commentary },
                { "dpi", book => book.Dpi.ToString() },
                { "color", book => book.Color },
                { "cleaned", book => book.Cleaned },
                { "orientation", book => book.Orientation },
                { "paginated", book => book.Paginated },
                { "scanned", book => book.Scanned },
                { "bookmarked", book => book.Bookmarked },
                { "searchable", book => book.Searchable },
                { "size", book => book.SizeInBytes.ToString() },
                { "ext", book => book.Format },
                { "md5", book => book.Md5Hash },
                { "generic", book => book.Generic },
                { "visible", book => book.Visible },
                { "locator", book => book.Locator },
                { "local", book => book.Local.ToString() },
                { "added", book => book.AddedDateTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { "last-modified", book => book.LastModifiedDateTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { "cover-url", book => book.CoverUrl },
                { "tags", book => book.Tags },
                { "isbn-plain", book => book.IdentifierPlain },
                { "thousand-index", book => (book.LibgenId / 1000).ToString("D4") },
                { "thousand-bucket", book => (book.LibgenId / 1000 * 1000).ToString() }
            };
            fictionTransformations = new Dictionary<string, Func<FictionBook, string>>
            {
                { "id", book => book.LibgenId.ToString() },
                { "title", book => book.Title },
                { "authors", book => book.Authors },
                { "series", book => book.Series },
                { "russian-author", book => book.RussianAuthor },
                { "author1-last-name", book => book.AuthorFamily1 },
                { "author1-first-name", book => book.AuthorName1 },
                { "author1-surname", book => book.AuthorSurname1 },
                { "author1-role", book => book.Role1 },
                { "author1-pseudonim", book => book.Pseudonim1 },
                { "author2-last-name", book => book.AuthorFamily2 },
                { "author2-first-name", book => book.AuthorName2 },
                { "author2-surname", book => book.AuthorSurname2 },
                { "author2-role", book => book.Role2 },
                { "author2-pseudonim", book => book.Pseudonim2 },
                { "author3-last-name", book => book.AuthorFamily3 },
                { "author3-first-name", book => book.AuthorName3 },
                { "author3-surname", book => book.AuthorSurname3 },
                { "author3-role", book => book.Role3 },
                { "author3-pseudonim", book => book.Pseudonim3 },
                { "author4-last-name", book => book.AuthorFamily4 },
                { "author4-first-name", book => book.AuthorName4 },
                { "author4-surname", book => book.AuthorSurname4 },
                { "author4-role", book => book.Role4 },
                { "author4-pseudonim", book => book.Pseudonim4 },
                { "series1", book => book.Series1 },
                { "series2", book => book.Series2 },
                { "series3", book => book.Series3 },
                { "series4", book => book.Series4 },
                { "ext", book => book.Format },
                { "version", book => book.Version },
                { "size", book => book.SizeInBytes.ToString() },
                { "md5", book => book.Md5Hash },
                { "path", book => book.Path },
                { "language", book => book.Language },
                { "pages", book => book.Pages },
                { "isbn", book => book.Identifier },
                { "year", book => book.Year },
                { "publisher", book => book.Publisher },
                { "edition", book => book.Edition },
                { "commentary", book => book.Commentary },
                { "added", book => book.AddedDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? String.Empty },
                { "last-modified", book => book.LastModifiedDateTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { "russian-author-last-name", book => book.RussianAuthorFamily },
                { "russian-author-first-name", book => book.RussianAuthorName },
                { "russian-author-surname", book => book.RussianAuthorSurname },
                { "cover", book => book.Cover },
                { "google-book-id", book => book.GoogleBookId },
                { "asin", book => book.Asin },
                { "author-hash", book => book.AuthorHash },
                { "title-hash", book => book.TitleHash },
                { "visible", book => book.Visible },
                { "thousand-index", book => (book.LibgenId / 1000).ToString("D4") },
                { "thousand-bucket", book => (book.LibgenId / 1000 * 1000).ToString() }
            };
            sciMagTransformations = new Dictionary<string, Func<SciMagArticle, string>>
            {
                { "id", article => article.LibgenId.ToString() },
                { "title", article => article.Title },
                { "authors", article => article.Authors },
                { "doi", article => article.Doi },
                { "doi2", article => article.Doi2 },
                { "year", article => article.Year },
                { "month", article => article.Month },
                { "day", article => article.Day },
                { "volume", article => article.Volume },
                { "issue", article => article.Issue },
                { "first-page", article => article.FirstPage },
                { "last-page", article => article.LastPage },
                { "journal", article => article.Journal },
                { "isbn", article => article.Isbn },
                { "issnp", article => article.Issnp },
                { "issne", article => article.Issne },
                { "md5", article => article.Md5Hash },
                { "size", article => article.SizeInBytes.ToString() },
                { "added", article => article.AddedDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? String.Empty },
                { "journal-id", article => article.JournalId },
                { "abstract-url", article => article.AbstractUrl },
                { "attribute1", article => article.Attribute1 },
                { "attribute2", article => article.Attribute2 },
                { "attribute3", article => article.Attribute3 },
                { "attribute4", article => article.Attribute4 },
                { "attribute5", article => article.Attribute5 },
                { "attribute6", article => article.Attribute6 },
                { "visible", article => article.Visible },
                { "pubmed-id", article => article.PubmedId },
                { "pmc", article => article.Pmc },
                { "pii", article => article.Pii },
                { "thousand-index", book => (book.LibgenId / 1000).ToString("D4") },
                { "thousand-bucket", book => (book.LibgenId / 1000 * 1000).ToString() }
            };
        }

        public static string GetNonFictionDownloadUrl(Mirrors.MirrorConfiguration mirror, NonFictionBook book)
        {
            return Replace(mirror.NonFictionDownloadUrl, nonFictionTransformations, book);
        }

        public static string GetNonFictionCoverUrl(Mirrors.MirrorConfiguration mirror, NonFictionBook book)
        {
            return Replace(mirror.NonFictionCoverUrl, nonFictionTransformations, book);
        }

        public static string GetFictionDownloadUrl(Mirrors.MirrorConfiguration mirror, FictionBook book)
        {
            return Replace(mirror.FictionDownloadUrl, fictionTransformations, book);
        }

        public static string GetFictionCoverUrl(Mirrors.MirrorConfiguration mirror, FictionBook book)
        {
            return Replace(mirror.FictionCoverUrl, fictionTransformations, book);
        }

        public static string GetSciMagDownloadUrl(Mirrors.MirrorConfiguration mirror, SciMagArticle article)
        {
            return Replace(mirror.SciMagDownloadUrl, sciMagTransformations, article);
        }

        private static string Replace<T>(string template, Dictionary<string, Func<T, string>> transformations, T objectValues)
        {
            if (String.IsNullOrWhiteSpace(template))
            {
                return null;
            }
            string result = regex.Replace(template, match =>
            {
                string transformationKey = match.Groups[1].Value.ToLower();
                bool toUpperCase = false;
                bool toLowerCase = false;
                if (transformationKey.EndsWith(":u"))
                {
                    toUpperCase = true;
                }
                else if (transformationKey.EndsWith(":l"))
                {
                    toLowerCase = true;
                }
                if (toUpperCase || toLowerCase)
                {
                    transformationKey = transformationKey.Substring(0, transformationKey.Length - 2);
                }
                string replaceWith;
                if (transformations.TryGetValue(transformationKey, out Func<T, string> transformation))
                {
                    replaceWith = transformation(objectValues);
                    if (toUpperCase)
                    {
                        replaceWith = replaceWith.ToUpper();
                    }
                    else if (toLowerCase)
                    {
                        replaceWith = replaceWith.ToLower();
                    }
                }
                else
                {
                    replaceWith = match.ToString();
                }
                return replaceWith;
            });
            return result;
        }
    }
}
