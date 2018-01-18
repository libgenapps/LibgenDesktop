using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Entities;
using Newtonsoft.Json;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models.JsonApi
{
    internal class JsonApiClient
    {
        private const string FIELD_LIST = "id,title,volumeinfo,series,periodical,author,year,edition,publisher,city,pages,pagesinfile,language,topic," +
            "library,issue,identifier,issn,asin,udc,lbc,ddc,lcc,doi,googlebookid,openlibraryid,commentary,dpi,color,cleaned,orientation,paginated," +
            "scanned,bookmarked,searchable,filesize,extension,md5,generic,visible,locator,local,timeadded,timelastmodified,coverurl,tags,identifierwodash";

        private readonly HttpClient httpClient;
        private readonly string jsonApiUrl;
        private DateTime lastModifiedDateTime;
        private int lastLibgenId;

        public JsonApiClient(HttpClient httpClient, string jsonApiUrl, DateTime lastModifiedDateTime, int lastLibgenId)
        {
            this.httpClient = httpClient;
            this.jsonApiUrl = jsonApiUrl;
            this.lastModifiedDateTime = lastModifiedDateTime;
            this.lastLibgenId = lastLibgenId;
        }

        public async Task<List<NonFictionBook>> DownloadNextBatchAsync(CancellationToken cancellationToken)
        {
            string url = $"{jsonApiUrl}?fields={FIELD_LIST}&timenewer={lastModifiedDateTime.ToString("yyyy-MM-dd HH:mm:ss")}&idnewer={lastLibgenId}&mode=newer";
            Logger.Debug($"Sending a request to {url}");
            HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
            Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
            Logger.Debug("Response headers:", response.Headers.ToString().TrimEnd());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"JSON API returned {response.StatusCode}.");
            }
            string responseContent = await response.Content.ReadAsStringAsync();
            Logger.Debug("Response content:", responseContent);
            List<JsonApiNonFictionBook> books;
            try
            {
                books = JsonConvert.DeserializeObject<List<JsonApiNonFictionBook>>(responseContent);
            }
            catch (Exception exception)
            {
                throw new Exception("Server response is not a valid JSON string.", exception);
            }
            Logger.Debug($"{books.Count} books have been parsed from the server response.");
            List<NonFictionBook> result = books.Select(ConvertToNonFictionBook).ToList();
            if (result.Any())
            {
                lastModifiedDateTime = result.Last().LastModifiedDateTime;
                lastLibgenId = result.Last().LibgenId;
            }
            return result;
        }

        private NonFictionBook ConvertToNonFictionBook(JsonApiNonFictionBook jsonApiNonFictionBook)
        {
            return new NonFictionBook
            {
                Title = jsonApiNonFictionBook.Title,
                VolumeInfo = jsonApiNonFictionBook.VolumeInfo,
                Series = jsonApiNonFictionBook.Series,
                Periodical = jsonApiNonFictionBook.Periodical,
                Authors = jsonApiNonFictionBook.Authors,
                Year = jsonApiNonFictionBook.Year,
                Edition = jsonApiNonFictionBook.Edition,
                Publisher = jsonApiNonFictionBook.Publisher,
                City = jsonApiNonFictionBook.City,
                Pages = jsonApiNonFictionBook.Pages,
                PagesInFile = jsonApiNonFictionBook.PagesInFile,
                Language = jsonApiNonFictionBook.Language,
                Topic = jsonApiNonFictionBook.Topic,
                Library = jsonApiNonFictionBook.Library,
                Issue = jsonApiNonFictionBook.Issue,
                Identifier = jsonApiNonFictionBook.Identifier,
                Issn = jsonApiNonFictionBook.Issn,
                Asin = jsonApiNonFictionBook.Asin,
                Udc = jsonApiNonFictionBook.Udc,
                Lbc = jsonApiNonFictionBook.Lbc,
                Ddc = jsonApiNonFictionBook.Ddc,
                Lcc = jsonApiNonFictionBook.Lcc,
                Doi = jsonApiNonFictionBook.Doi,
                GoogleBookId = jsonApiNonFictionBook.GoogleBookId,
                OpenLibraryId = jsonApiNonFictionBook.OpenLibraryId,
                Commentary = jsonApiNonFictionBook.Commentary,
                Dpi = jsonApiNonFictionBook.Dpi,
                Color = jsonApiNonFictionBook.Color,
                Cleaned = jsonApiNonFictionBook.Cleaned,
                Orientation = jsonApiNonFictionBook.Orientation,
                Paginated = jsonApiNonFictionBook.Paginated,
                Scanned = jsonApiNonFictionBook.Scanned,
                Bookmarked = jsonApiNonFictionBook.Bookmarked,
                Searchable = jsonApiNonFictionBook.Searchable,
                SizeInBytes = jsonApiNonFictionBook.SizeInBytes,
                Format = jsonApiNonFictionBook.Format,
                Md5Hash = jsonApiNonFictionBook.Md5Hash,
                Generic = jsonApiNonFictionBook.Generic,
                Visible = jsonApiNonFictionBook.Visible,
                Locator = jsonApiNonFictionBook.Locator,
                Local = jsonApiNonFictionBook.Local,
                AddedDateTime = DateTime.ParseExact(jsonApiNonFictionBook.AddedDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                LastModifiedDateTime = DateTime.ParseExact(jsonApiNonFictionBook.LastModifiedDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                CoverUrl = jsonApiNonFictionBook.CoverUrl,
                Tags = jsonApiNonFictionBook.Tags,
                IdentifierPlain = jsonApiNonFictionBook.IdentifierPlain,
                LibgenId = jsonApiNonFictionBook.LibgenId
            };
        }
    }
}
