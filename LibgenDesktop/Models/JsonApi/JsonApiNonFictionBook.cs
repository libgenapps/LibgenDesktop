using Newtonsoft.Json;

namespace LibgenDesktop.Models.JsonApi
{
    internal class JsonApiNonFictionBook
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("volumeinfo")]
        public string VolumeInfo { get; set; }

        [JsonProperty("series")]
        public string Series { get; set; }

        [JsonProperty("periodical")]
        public string Periodical { get; set; }

        [JsonProperty("author")]
        public string Authors { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("edition")]
        public string Edition { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("pages")]
        public string Pages { get; set; }

        [JsonProperty("pagesinfile")]
        public int PagesInFile { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("library")]
        public string Library { get; set; }

        [JsonProperty("issue")]
        public string Issue { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("issn")]
        public string Issn { get; set; }

        [JsonProperty("asin")]
        public string Asin { get; set; }

        [JsonProperty("udc")]
        public string Udc { get; set; }

        [JsonProperty("lbc")]
        public string Lbc { get; set; }

        [JsonProperty("ddc")]
        public string Ddc { get; set; }

        [JsonProperty("lcc")]
        public string Lcc { get; set; }

        [JsonProperty("doi")]
        public string Doi { get; set; }

        [JsonProperty("googlebookid")]
        public string GoogleBookId { get; set; }

        [JsonProperty("openlibraryid")]
        public string OpenLibraryId { get; set; }

        [JsonProperty("commentary")]
        public string Commentary { get; set; }

        [JsonProperty("dpi")]
        public int Dpi { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("cleaned")]
        public string Cleaned { get; set; }

        [JsonProperty("orientation")]
        public string Orientation { get; set; }

        [JsonProperty("paginated")]
        public string Paginated { get; set; }

        [JsonProperty("scanned")]
        public string Scanned { get; set; }

        [JsonProperty("bookmarked")]
        public string Bookmarked { get; set; }

        [JsonProperty("searchable")]
        public string Searchable { get; set; }

        [JsonProperty("filesize")]
        public long SizeInBytes { get; set; }

        [JsonProperty("extension")]
        public string Format { get; set; }

        [JsonProperty("md5")]
        public string Md5Hash { get; set; }

        [JsonProperty("generic")]
        public string Generic { get; set; }

        [JsonProperty("visible")]
        public string Visible { get; set; }

        [JsonProperty("locator")]
        public string Locator { get; set; }

        [JsonProperty("local")]
        public int Local { get; set; }

        [JsonProperty("timeadded")]
        public string AddedDateTime { get; set; }

        [JsonProperty("timelastmodified")]
        public string LastModifiedDateTime { get; set; }

        [JsonProperty("coverurl")]
        public string CoverUrl { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("identifierwodash")]
        public string IdentifierPlain { get; set; }

        [JsonProperty("id")]
        public int LibgenId { get; set; }
    }
}
