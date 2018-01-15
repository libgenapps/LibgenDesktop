using System;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Settings;

namespace LibgenDesktop.Models.Utils
{
    internal static class UrlGenerator
    {
        public static string GetNonFictionDownloadUrl(Mirrors.MirrorConfiguration mirror, NonFictionBook book)
        {
            return Replace(mirror.NonFictionDownloadUrl, book.LibgenId, book.Md5Hash, book.CoverUrl, String.Empty);
        }

        public static string GetNonFictionCoverUrl(Mirrors.MirrorConfiguration mirror, NonFictionBook book)
        {
            return Replace(mirror.NonFictionCoverUrl, book.LibgenId, book.Md5Hash, book.CoverUrl, String.Empty);
        }

        public static string GetFictionDownloadUrl(Mirrors.MirrorConfiguration mirror, FictionBook book)
        {
            return Replace(mirror.FictionDownloadUrl, book.LibgenId, book.Md5Hash, String.Empty, String.Empty);
        }

        public static string GetFictionCoverUrl(Mirrors.MirrorConfiguration mirror, FictionBook book)
        {
            return Replace(mirror.FictionCoverUrl, book.LibgenId, book.Md5Hash, String.Empty, String.Empty);
        }

        public static string GetSciMagDownloadUrl(Mirrors.MirrorConfiguration mirror, SciMagArticle article)
        {
            return Replace(mirror.SciMagDownloadUrl, article.LibgenId, String.Empty, String.Empty, article.Doi);
        }

        private static string Replace(string template, int id, string md5, string coverUrl, string doi)
        {
            if (String.IsNullOrWhiteSpace(template))
            {
                return null;
            }
            string thousandBucket = (id / 1000 * 1000).ToString();
            return template.Replace("{id}", id.ToString()).Replace("{md5}", md5).Replace("{cover-url}", coverUrl).Replace("{doi}", doi).
                Replace("{thousand-bucket}", thousandBucket);
        }
    }
}
