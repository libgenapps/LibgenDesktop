namespace LibgenDesktop.Models.Database
{
    internal static class SqlScripts
    {
        public const string CHECK_IF_METADATA_TABLE_EXIST = @"SELECT COUNT(name) FROM sqlite_master WHERE type=""table"" AND name=""metadata""";

        public const string CREATE_METADATA_TABLE =
            "CREATE TABLE IF NOT EXISTS metadata (" +
                "Key TEXT PRIMARY KEY NOT NULL," +
                "Value TEXT" +
            ")";

        public const string GET_ALL_METADATA_ITEMS = "SELECT Key,Value FROM metadata";

        public const string INSERT_METADATA_ITEM = "INSERT INTO metadata VALUES (@Key,@Value)";

        public const string UPDATE_METADATA_ITEM = "UPDATE metadata SET Value = @Value WHERE Key = @Key";

        public const string CREATE_NON_FICTION_TABLE =
            "CREATE TABLE IF NOT EXISTS non_fiction (" +
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
                "GoogleBookId TEXT," +
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

        public const string CREATE_NON_FICTION_FTS_TABLE =
            "CREATE VIRTUAL TABLE IF NOT EXISTS non_fiction_fts USING fts5 (Title, Series, Authors, Publisher, IdentifierPlain, " +
                "content=non_fiction, content_rowid=Id)";

        public const string COUNT_NON_FICTION = "SELECT MAX(Id) FROM non_fiction LIMIT 1";

        public const string GET_NON_FICTION_BY_ID = "SELECT * FROM non_fiction WHERE Id = @Id";

        public const string SEARCH_NON_FICTION = "SELECT * FROM non_fiction " +
            "WHERE Id IN (SELECT rowid FROM non_fiction_fts WHERE non_fiction_fts MATCH @SearchQuery) ORDER BY Id";

        public const string INSERT_NON_FICTION =
            "INSERT INTO non_fiction VALUES (@Id,@Title,@VolumeInfo,@Series,@Periodical,@Authors,@Year,@Edition,@Publisher,@City," +
                "@Pages,@PagesInFile,@Language,@Topic,@Library,@Issue,@Identifier,@Issn,@Asin,@Udc,@Lbc,@Ddc,@Lcc,@Doi," +
                "@GoogleBookId,@OpenLibraryId,@Commentary,@Dpi,@Color,@Cleaned,@Orientation,@Paginated,@Scanned,@Bookmarked," +
                "@Searchable,@SizeInBytes,@Format,@Md5Hash,@Generic,@Visible,@Locator,@Local,@AddedDateTime," +
                "@LastModifiedDateTime,@CoverUrl,@Tags,@IdentifierPlain,@LibgenId)";

        public const string INSERT_NON_FICTION_FTS =
            "INSERT INTO non_fiction_fts VALUES (@Title,@Series,@Authors,@Publisher,@IdentifierPlain)";

        public const string CREATE_FICTION_TABLE =
            "CREATE TABLE IF NOT EXISTS fiction (" +
                "Id INTEGER PRIMARY KEY NOT NULL," +
                "AuthorFamily1 TEXT," +
                "AuthorName1 TEXT," +
                "AuthorSurname1 TEXT," +
                "Role1 TEXT," +
                "Pseudonim1 TEXT," +
                "AuthorFamily2 TEXT," +
                "AuthorName2 TEXT," +
                "AuthorSurname2 TEXT," +
                "Role2 TEXT," +
                "Pseudonim2 TEXT," +
                "AuthorFamily3 TEXT," +
                "AuthorName3 TEXT," +
                "AuthorSurname3 TEXT," +
                "Role3 TEXT," +
                "Pseudonim3 TEXT," +
                "AuthorFamily4 TEXT," +
                "AuthorName4 TEXT," +
                "AuthorSurname4 TEXT," +
                "Role4 TEXT," +
                "Pseudonim4 TEXT," +
                "Series1 TEXT," +
                "Series2 TEXT," +
                "Series3 TEXT," +
                "Series4 TEXT," +
                "Title TEXT," +
                "Format TEXT," +
                "Version TEXT," +
                "SizeInBytes INTEGER NOT NULL," +
                "Md5Hash TEXT," +
                "Path TEXT," +
                "Language TEXT," +
                "Pages TEXT," +
                "Identifier TEXT," +
                "Year TEXT," +
                "Publisher TEXT," +
                "Edition TEXT," +
                "Commentary TEXT," +
                "AddedDateTime TEXT," +
                "LastModifiedDateTime TEXT NOT NULL," +
                "RussianAuthorFamily TEXT," +
                "RussianAuthorName TEXT," +
                "RussianAuthorSurname TEXT," +
                "Cover TEXT," +
                "GoogleBookId TEXT," +
                "Asin TEXT," +
                "AuthorHash TEXT," +
                "TitleHash TEXT," +
                "Visible TEXT," +
                "LibgenId INTEGER NOT NULL" +
            ")";

        public const string CREATE_FICTION_FTS_TABLE =
            "CREATE VIRTUAL TABLE IF NOT EXISTS fiction_fts USING fts5 (Title, AuthorFamily1, AuthorName1, AuthorSurname1, Pseudonim1, " +
                "AuthorFamily2, AuthorName2, AuthorSurname2, Pseudonim2, AuthorFamily3, AuthorName3, AuthorSurname3, Pseudonim3, " +
                "AuthorFamily4, AuthorName4, AuthorSurname4, Pseudonim4, RussianAuthorFamily, RussianAuthorName, RussianAuthorSurname, " +
                "Series1, Series2, Series3, Series4, Publisher, Identifier, content=fiction, content_rowid=Id)";

        public const string COUNT_FICTION = "SELECT MAX(Id) FROM fiction LIMIT 1";

        public const string GET_FICTION_BY_ID = "SELECT * FROM fiction WHERE Id = @Id";

        public const string SEARCH_FICTION = "SELECT * FROM fiction WHERE Id IN (SELECT rowid FROM fiction_fts WHERE fiction_fts MATCH @SearchQuery) ORDER BY Id";

        public const string INSERT_FICTION =
            "INSERT INTO fiction VALUES (@Id,@AuthorFamily1,@AuthorName1,@AuthorSurname1,@Role1,@Pseudonim1,@AuthorFamily2,@AuthorName2,@AuthorSurname2,@Role2,@Pseudonim2," +
                "@AuthorFamily3,@AuthorName3,@AuthorSurname3,@Role3,@Pseudonim3,@AuthorFamily4,@AuthorName4,@AuthorSurname4,@Role4,@Pseudonim4,@Series1,@Series2,@Series3,@Series4," +
                "@Title,@Format,@Version,@SizeInBytes,@Md5Hash,@Path,@Language,@Pages,@Identifier,@Year,@Publisher,@Edition,@Commentary,@AddedDateTime,@LastModifiedDateTime," +
                "@RussianAuthorFamily,@RussianAuthorName,@RussianAuthorSurname,@Cover,@GoogleBookId,@Asin,@AuthorHash,@TitleHash,@Visible,@LibgenId)";

        public const string INSERT_FICTION_FTS =
            "INSERT INTO fiction_fts VALUES (@Title,@AuthorFamily1,@AuthorName1,@AuthorSurname1,@Pseudonim1," +
                "@AuthorFamily2, @AuthorName2, @AuthorSurname2, @Pseudonim2, @AuthorFamily3, @AuthorName3, @AuthorSurname3, @Pseudonim3, " +
                "@AuthorFamily4, @AuthorName4, @AuthorSurname4, @Pseudonim4, @RussianAuthorFamily, @RussianAuthorName, @RussianAuthorSurname, " +
                "@Series1, @Series2, @Series3, @Series4, @Publisher, @Identifier)";

        public const string CREATE_SCIMAG_TABLE =
            "CREATE TABLE IF NOT EXISTS scimag (" +
                "Id INTEGER PRIMARY KEY NOT NULL," +
                "Doi TEXT," +
                "Doi2 TEXT," +
                "Title TEXT," +
                "Authors TEXT," +
                "Year TEXT," +
                "Month TEXT," +
                "Day TEXT," +
                "Volume TEXT," +
                "Issue TEXT," +
                "FirstPage TEXT," +
                "LastPage TEXT," +
                "Journal TEXT," +
                "Isbn TEXT," +
                "Issnp TEXT," +
                "Issne TEXT," +
                "Md5Hash TEXT," +
                "SizeInBytes INTEGER NOT NULL," +
                "AddedDateTime TEXT," +
                "JournalId TEXT," +
                "AbstractUrl TEXT," +
                "Attribute1 TEXT," +
                "Attribute2 TEXT," +
                "Attribute3 TEXT," +
                "Attribute4 TEXT," +
                "Attribute5 TEXT," +
                "Attribute6 TEXT," +
                "Visible TEXT," +
                "PubmedId TEXT," +
                "Pmc TEXT," +
                "Pii TEXT," +
                "LibgenId INTEGER NOT NULL" +
            ")";

        public const string CREATE_SCIMAG_FTS_TABLE =
            "CREATE VIRTUAL TABLE IF NOT EXISTS scimag_fts USING fts5 (Title, Authors, Doi, Doi2, PubmedId, Journal, Issnp, Issne, content=scimag, content_rowid=Id)";

        public const string COUNT_SCIMAG = "SELECT MAX(Id) FROM scimag LIMIT 1";

        public const string GET_SCIMAG_BY_ID = "SELECT * FROM scimag WHERE Id = @Id";

        public const string SEARCH_SCIMAG = "SELECT * FROM scimag WHERE Id IN (SELECT rowid FROM scimag_fts WHERE scimag_fts MATCH @SearchQuery) ORDER BY Id";

        public const string INSERT_SCIMAG =
            "INSERT INTO scimag VALUES (@Id,@Doi,@Doi2,@Title,@Authors,@Year,@Month,@Day,@Volume,@Issue,@FirstPage,@LastPage,@Journal,@Isbn,@Issnp,@Issne," +
                "@Md5Hash,@SizeInBytes,@AddedDateTime,@JournalId,@AbstractUrl,@Attribute1,@Attribute2,@Attribute3,@Attribute4,@Attribute5,@Attribute6," +
                "@Visible,@PubmedId,@Pmc,@Pii,@LibgenId)";

        public const string INSERT_SCIMAG_FTS =
            "INSERT INTO scimag_fts VALUES (@Title,@Authors,@Doi,@Doi2,@PubmedId,@Journal,@Issnp,@Issne)";
    }
}
