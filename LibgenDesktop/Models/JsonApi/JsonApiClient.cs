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
            string url = $"{jsonApiUrl}?fields={FIELD_LIST}&timenewer={lastModifiedDateTime:yyyy-MM-dd HH:mm:ss}&idnewer={lastLibgenId}&mode=newer";
            Logger.Debug($"Sending a request to {url}");
            HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
            Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
            Logger.Debug("Response headers:", response.Headers.ToString().TrimEnd(), response.Content.Headers.ToString().TrimEnd());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"JSON API returned {(int)response.StatusCode} {response.StatusCode}.");
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

        private static DateTime ParseDateTime(string input)
        {
            if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return DateTime.UtcNow;
            }
            return result;
        }

        private NonFictionBook ConvertToNonFictionBook(JsonApiNonFictionBook jsonApiNonFictionBook)
        {
            return new NonFictionBook
            {
                Title = jsonApiNonFictionBook.Title ?? String.Empty,
                VolumeInfo = jsonApiNonFictionBook.VolumeInfo ?? String.Empty,
                Series = jsonApiNonFictionBook.Series ?? String.Empty,
                Periodical = jsonApiNonFictionBook.Periodical ?? String.Empty,
                Authors = jsonApiNonFictionBook.Authors ?? String.Empty,
                Year = jsonApiNonFictionBook.Year ?? String.Empty,
                Edition = jsonApiNonFictionBook.Edition ?? String.Empty,
                Publisher = jsonApiNonFictionBook.Publisher ?? String.Empty,
                City = jsonApiNonFictionBook.City ?? String.Empty,
                Pages = jsonApiNonFictionBook.Pages ?? String.Empty,
                PagesInFile = jsonApiNonFictionBook.PagesInFile,
                Language = jsonApiNonFictionBook.Language ?? String.Empty,
                Topic = jsonApiNonFictionBook.Topic ?? String.Empty,
                Library = jsonApiNonFictionBook.Library ?? String.Empty,
                Issue = jsonApiNonFictionBook.Issue ?? String.Empty,
                Identifier = jsonApiNonFictionBook.Identifier ?? String.Empty,
                Issn = jsonApiNonFictionBook.Issn ?? String.Empty,
                Asin = jsonApiNonFictionBook.Asin ?? String.Empty,
                Udc = jsonApiNonFictionBook.Udc ?? String.Empty,
                Lbc = jsonApiNonFictionBook.Lbc ?? String.Empty,
                Ddc = jsonApiNonFictionBook.Ddc ?? String.Empty,
                Lcc = jsonApiNonFictionBook.Lcc ?? String.Empty,
                Doi = jsonApiNonFictionBook.Doi ?? String.Empty,
                GoogleBookId = jsonApiNonFictionBook.GoogleBookId ?? String.Empty,
                OpenLibraryId = jsonApiNonFictionBook.OpenLibraryId ?? String.Empty,
                Commentary = jsonApiNonFictionBook.Commentary ?? String.Empty,
                Dpi = jsonApiNonFictionBook.Dpi,
                Color = jsonApiNonFictionBook.Color ?? String.Empty,
                Cleaned = jsonApiNonFictionBook.Cleaned ?? String.Empty,
                Orientation = jsonApiNonFictionBook.Orientation ?? String.Empty,
                Paginated = jsonApiNonFictionBook.Paginated ?? String.Empty,
                Scanned = jsonApiNonFictionBook.Scanned ?? String.Empty,
                Bookmarked = jsonApiNonFictionBook.Bookmarked ?? String.Empty,
                Searchable = jsonApiNonFictionBook.Searchable ?? String.Empty,
                SizeInBytes = jsonApiNonFictionBook.SizeInBytes,
                Format = jsonApiNonFictionBook.Format ?? String.Empty,
                Md5Hash = jsonApiNonFictionBook.Md5Hash ?? String.Empty,
                Generic = jsonApiNonFictionBook.Generic ?? String.Empty,
                Visible = jsonApiNonFictionBook.Visible ?? String.Empty,
                Locator = jsonApiNonFictionBook.Locator ?? String.Empty,
                Local = jsonApiNonFictionBook.Local,
                AddedDateTime = ParseDateTime(jsonApiNonFictionBook.AddedDateTime),
                LastModifiedDateTime = ParseDateTime(jsonApiNonFictionBook.LastModifiedDateTime),
                CoverUrl = jsonApiNonFictionBook.CoverUrl,
                Tags = jsonApiNonFictionBook.Tags,
                IdentifierPlain = jsonApiNonFictionBook.IdentifierPlain,
                LibgenId = jsonApiNonFictionBook.LibgenId
            };
        }
    }
}
