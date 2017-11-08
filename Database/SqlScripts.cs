namespace LibgenDesktop.Database
{
    internal static class SqlScripts
    {
        public const string CREATE_BOOKS_TABLE =
            "CREATE TABLE IF NOT EXISTS books (" +
                "Id INTEGER PRIMARY KEY NOT NULL," +
                "Title TEXT," +
                "VolumeInfo TEXT," +
                "Series TEXT," +
                "Periodical TEXT," +
                "Authors TEXT," +
                "Year TEXT," +
                "Edition TEXT," +
                "Publisher TEXT," +
                "City TEXT," +
                "Pages TEXT," +
                "PagesInFile INTEGER NOT NULL," +
                "Language TEXT," +
                "Topic TEXT," +
                "Library TEXT," +
                "Issue TEXT," +
                "Identifier TEXT," +
                "Issn TEXT," +
                "Asin TEXT," +
                "Udc TEXT," +
                "Lbc TEXT," +
                "Ddc TEXT," +
                "Lcc TEXT," +
                "Doi TEXT," +
                "GoogleBookid TEXT," +
                "OpenLibraryId TEXT," +
                "Commentary TEXT," +
                "Dpi INTEGER," +
                "Color TEXT," +
                "Cleaned TEXT," +
                "Orientation TEXT," +
                "Paginated TEXT," +
                "Scanned TEXT," +
                "Bookmarked TEXT," +
                "Searchable TEXT," +
                "SizeInBytes INTEGER NOT NULL," +
                "Format TEXT," +
                "Md5Hash TEXT," +
                "Generic TEXT," +
                "Visible TEXT," +
                "Locator TEXT," +
                "Local INTEGER," +
                "AddedDateTime TEXT NOT NULL," +
                "LastModifiedDateTime TEXT NOT NULL," +
                "CoverUrl TEXT," +
                "Tags TEXT," +
                "IdentifierPlain TEXT," +
                "LibgenId INTEGER NOT NULL" +
            ")";

        public const string CREATE_BOOKS_FTS_TABLE =
            "CREATE VIRTUAL TABLE IF NOT EXISTS books_fts USING fts5 (Title, Series, Authors, Publisher, IdentifierPlain, " +
            "content=books, content_rowid=Id)";

        public const string COUNT_BOOKS = "SELECT MAX(Id) FROM books LIMIT 1";

        public const string GET_ALL_BOOKS = "SELECT Id,Title,Authors,Series,Year,Publisher,Format,SizeInBytes FROM books ORDER BY Id";

        public const string GET_BOOK_BY_ID = "SELECT Id,Title,VolumeInfo,Series,Periodical,Authors,Year,Edition,Publisher,City," +
            "Pages,PagesInFile,Language,Topic,Library,Issue,Identifier,Issn,Asin,Udc,Lbc,Ddc,Lcc,Doi," +
            "GoogleBookid,OpenLibraryId,Commentary,Dpi,Color,Cleaned,Orientation,Paginated,Scanned,Bookmarked," +
            "Searchable,SizeInBytes,Format,Md5Hash,Generic,Visible,Locator,Local,AddedDateTime," +
            "LastModifiedDateTime,CoverUrl,Tags,IdentifierPlain,LibgenId FROM books WHERE Id = @Id";

        public const string SEARCH_BOOKS = "SELECT Id,Title,Authors,Series,Year,Publisher,Format,SizeInBytes FROM books " +
            "WHERE Id IN (SELECT rowid FROM books_fts WHERE books_fts MATCH @SearchQuery) ORDER BY Id";

        public const string INSERT_BOOK =
            "INSERT INTO books VALUES (@Id,@Title,@VolumeInfo,@Series,@Periodical,@Authors,@Year,@Edition,@Publisher,@City," +
                "@Pages,@PagesInFile,@Language,@Topic,@Library,@Issue,@Identifier,@Issn,@Asin,@Udc,@Lbc,@Ddc,@Lcc,@Doi," +
                "@GoogleBookid,@OpenLibraryId,@Commentary,@Dpi,@Color,@Cleaned,@Orientation,@Paginated,@Scanned,@Bookmarked," +
                "@Searchable,@SizeInBytes,@Format,@Md5Hash,@Generic,@Visible,@Locator,@Local,@AddedDateTime," +
                "@LastModifiedDateTime,@CoverUrl,@Tags,@IdentifierPlain,@LibgenId)";

        public const string INSERT_BOOK_FTS =
            "INSERT INTO books_fts VALUES (@Title,@Series,@Authors,@Publisher,@IdentifierPlain)";
    }
}
