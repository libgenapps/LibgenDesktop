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

        public const string CHECK_IF_METADATA_ITEM_EXIST = "SELECT COUNT(*) FROM metadata WHERE Key = @Key";

        public const string GET_METADATA_ITEM = "SELECT Value FROM metadata WHERE Key = @Key";

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
                "LibgenId INTEGER NOT NULL," +
                "FileId INTEGER," +
                "FOREIGN KEY (FileId) REFERENCES files(Id)" +
            ")";

        public const string CREATE_NON_FICTION_FTS_TABLE =
            "CREATE VIRTUAL TABLE IF NOT EXISTS non_fiction_fts USING fts5 (Title, Series, Authors, Publisher, IdentifierPlain, " +
                "content=non_fiction, content_rowid=Id)";

        public const string GET_ALL_NON_FICTION_LIBGEN_IDS = "SELECT LibgenId FROM non_fiction";

        public const string COUNT_NON_FICTION = "SELECT MAX(Id) FROM non_fiction LIMIT 1";

        public const string GET_NON_FICTION_BY_ID = "SELECT * FROM non_fiction WHERE Id = @Id";

        public const string GET_NON_FICTION_BY_MD5HASH = "SELECT * FROM non_fiction WHERE Md5Hash = @Md5Hash COLLATE NOCASE";

        public const string GET_NON_FICTION_ID_BY_LIBGENID = "SELECT Id FROM non_fiction WHERE LibgenId = @LibgenId LIMIT 1";

        public const string GET_LAST_MODIFIED_NON_FICTION =
            "SELECT * FROM non_fiction WHERE LastModifiedDateTime = (SELECT MAX(LastModifiedDateTime) FROM non_fiction) ORDER BY LibgenId DESC LIMIT 1";

        public const string GET_NON_FICTION_MAX_LIBGEN_ID = "SELECT MAX(LibgenId) FROM non_fiction LIMIT 1";

        public const string SEARCH_NON_FICTION = "SELECT * FROM non_fiction " +
            "WHERE Id IN (SELECT rowid FROM non_fiction_fts WHERE non_fiction_fts MATCH @SearchQuery) ORDER BY Id";

        public const string INSERT_NON_FICTION =
            "INSERT INTO non_fiction VALUES (@Id,@Title,@VolumeInfo,@Series,@Periodical,@Authors,@Year,@Edition,@Publisher,@City," +
                "@Pages,@PagesInFile,@Language,@Topic,@Library,@Issue,@Identifier,@Issn,@Asin,@Udc,@Lbc,@Ddc,@Lcc,@Doi," +
                "@GoogleBookId,@OpenLibraryId,@Commentary,@Dpi,@Color,@Cleaned,@Orientation,@Paginated,@Scanned,@Bookmarked," +
                "@Searchable,@SizeInBytes,@Format,@Md5Hash,@Generic,@Visible,@Locator,@Local,@AddedDateTime," +
                "@LastModifiedDateTime,@CoverUrl,@Tags,@IdentifierPlain,@LibgenId,NULL)";

        public const string UPDATE_NON_FICTION =
            "UPDATE non_fiction SET " +
                "Title=@Title," +
                "VolumeInfo=@VolumeInfo," +
                "Series=@Series," +
                "Periodical=@Periodical," +
                "Authors=@Authors," +
                "Year=@Year," +
                "Edition=@Edition," +
                "Publisher=@Publisher," +
                "City=@City," +
                "Pages=@Pages," +
                "PagesInFile=@PagesInFile," +
                "Language=@Language," +
                "Topic=@Topic," +
                "Library=@Library," +
                "Issue=@Issue," +
                "Identifier=@Identifier," +
                "Issn=@Issn," +
                "Asin=@Asin," +
                "Udc=@Udc," +
                "Lbc=@Lbc," +
                "Ddc=@Ddc," +
                "Lcc=@Lcc," +
                "Doi=@Doi," +
                "GoogleBookId=@GoogleBookId," +
                "OpenLibraryId=@OpenLibraryId," +
                "Commentary=@Commentary," +
                "Dpi=@Dpi," +
                "Color=@Color," +
                "Cleaned=@Cleaned," +
                "Orientation=@Orientation," +
                "Paginated=@Paginated," +
                "Scanned=@Scanned," +
                "Bookmarked=@Bookmarked," +
                "Searchable=@Searchable," +
                "SizeInBytes=@SizeInBytes," +
                "Format=@Format," +
                "Md5Hash=@Md5Hash," +
                "Generic=@Generic," +
                "Visible=@Visible," +
                "Locator=@Locator," +
                "Local=@Local," +
                "AddedDateTime=@AddedDateTime," +
                "LastModifiedDateTime=@LastModifiedDateTime," +
                "CoverUrl=@CoverUrl," +
                "Tags=@Tags," +
                "IdentifierPlain=@IdentifierPlain " +
                "WHERE Id=@Id";

        public const string INSERT_NON_FICTION_FTS_WITHOUT_ID =
            "INSERT INTO non_fiction_fts VALUES (@Title,@Series,@Authors,@Publisher,@IdentifierPlain)";

        public const string INSERT_NON_FICTION_FTS_WITH_ID = "INSERT INTO non_fiction_fts (rowid,Title,Series,Authors,Publisher,IdentifierPlain) " +
            "VALUES (@Id,@Title,@Series,@Authors,@Publisher,@IdentifierPlain)";

        public const string DELETE_NON_FICTION_FTS = "INSERT INTO non_fiction_fts (non_fiction_fts,rowid,Title,Series,Authors,Publisher,IdentifierPlain) " +
            "VALUES ('delete',@Id,@Title,@Series,@Authors,@Publisher,@IdentifierPlain)";

        public const string GET_NON_FICTION_INDEX_LIST = "PRAGMA index_list(non_fiction)";

        public const string NON_FICTION_INDEX_PREFIX = "IX_non_fiction_";

        public const string CREATE_NON_FICTION_MD5HASH_INDEX = "CREATE UNIQUE INDEX " + NON_FICTION_INDEX_PREFIX +
            "Md5Hash ON non_fiction (Md5Hash COLLATE NOCASE)";

        public const string CREATE_NON_FICTION_LASTMODIFIEDDATETIME_INDEX = "CREATE INDEX " + NON_FICTION_INDEX_PREFIX +
            "LastModifiedDateTime ON non_fiction (LastModifiedDateTime DESC)";

        public const string CREATE_NON_FICTION_LIBGENID_INDEX = "CREATE UNIQUE INDEX " + NON_FICTION_INDEX_PREFIX + "LibgenId ON non_fiction (LibgenId ASC)";

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
                "LibgenId INTEGER NOT NULL," +
                "FileId INTEGER," +
                "FOREIGN KEY (FileId) REFERENCES files(Id)" +
            ")";

        public const string CREATE_FICTION_FTS_TABLE =
            "CREATE VIRTUAL TABLE IF NOT EXISTS fiction_fts USING fts5 (Title, AuthorFamily1, AuthorName1, AuthorSurname1, Pseudonim1, " +
                "AuthorFamily2, AuthorName2, AuthorSurname2, Pseudonim2, AuthorFamily3, AuthorName3, AuthorSurname3, Pseudonim3, " +
                "AuthorFamily4, AuthorName4, AuthorSurname4, Pseudonim4, RussianAuthorFamily, RussianAuthorName, RussianAuthorSurname, " +
                "Series1, Series2, Series3, Series4, Publisher, Identifier, content=fiction, content_rowid=Id)";

        public const string GET_ALL_FICTION_LIBGEN_IDS = "SELECT LibgenId FROM fiction";

        public const string COUNT_FICTION = "SELECT MAX(Id) FROM fiction LIMIT 1";

        public const string GET_FICTION_BY_ID = "SELECT * FROM fiction WHERE Id = @Id";

        public const string GET_FICTION_BY_MD5HASH = "SELECT * FROM fiction WHERE Md5Hash = @Md5Hash COLLATE NOCASE";

        public const string GET_FICTION_ID_BY_LIBGENID = "SELECT Id FROM fiction WHERE LibgenId = @LibgenId LIMIT 1";

        public const string GET_LAST_MODIFIED_FICTION = 
            "SELECT * FROM fiction WHERE LastModifiedDateTime = (SELECT MAX(LastModifiedDateTime) FROM fiction) ORDER BY LibgenId DESC LIMIT 1";

        public const string GET_FICTION_MAX_LIBGEN_ID = "SELECT MAX(LibgenId) FROM fiction LIMIT 1";

        public const string SEARCH_FICTION = "SELECT * FROM fiction WHERE Id IN (SELECT rowid FROM fiction_fts WHERE fiction_fts MATCH @SearchQuery) ORDER BY Id";

        public const string INSERT_FICTION =
            "INSERT INTO fiction VALUES (@Id,@AuthorFamily1,@AuthorName1,@AuthorSurname1,@Role1,@Pseudonim1,@AuthorFamily2,@AuthorName2,@AuthorSurname2,@Role2,@Pseudonim2," +
                "@AuthorFamily3,@AuthorName3,@AuthorSurname3,@Role3,@Pseudonim3,@AuthorFamily4,@AuthorName4,@AuthorSurname4,@Role4,@Pseudonim4,@Series1,@Series2,@Series3,@Series4," +
                "@Title,@Format,@Version,@SizeInBytes,@Md5Hash,@Path,@Language,@Pages,@Identifier,@Year,@Publisher,@Edition,@Commentary,@AddedDateTime,@LastModifiedDateTime," +
                "@RussianAuthorFamily,@RussianAuthorName,@RussianAuthorSurname,@Cover,@GoogleBookId,@Asin,@AuthorHash,@TitleHash,@Visible,@LibgenId,NULL)";

        public const string UPDATE_FICTION =
            "UPDATE fiction SET " +
                "AuthorFamily1=@AuthorFamily1," +
                "AuthorName1=@AuthorName1," +
                "AuthorSurname1=@AuthorSurname1," +
                "Role1=@Role1," +
                "Pseudonim1=@Pseudonim1," +
                "AuthorFamily2=@AuthorFamily2," +
                "AuthorName2=@AuthorName2," +
                "AuthorSurname2=@AuthorSurname2," +
                "Role2=@Role2," +
                "Pseudonim2=@Pseudonim2," +
                "AuthorFamily3=@AuthorFamily3," +
                "AuthorName3=@AuthorName3," +
                "AuthorSurname3=@AuthorSurname3," +
                "Role3=@Role3," +
                "Pseudonim3=@Pseudonim3," +
                "AuthorFamily4=@AuthorFamily4," +
                "AuthorName4=@AuthorName4," +
                "AuthorSurname4=@AuthorSurname4," +
                "Role4=@Role4," +
                "Pseudonim4=@Pseudonim4," +
                "Series1=@Series1," +
                "Series2=@Series2," +
                "Series3=@Series3," +
                "Series4=@Series4," +
                "Title=@Title," +
                "Format=@Format," +
                "Version=@Version," +
                "SizeInBytes=@SizeInBytes," +
                "Md5Hash=@Md5Hash," +
                "Path=@Path," +
                "Language=@Language," +
                "Pages=@Pages," +
                "Identifier=@Identifier," +
                "Year=@Year," +
                "Publisher=@Publisher," +
                "Edition=@Edition," +
                "Commentary=@Commentary," +
                "AddedDateTime=@AddedDateTime," +
                "LastModifiedDateTime=@LastModifiedDateTime," +
                "RussianAuthorFamily=@RussianAuthorFamily," +
                "RussianAuthorName=@RussianAuthorName," +
                "RussianAuthorSurname=@RussianAuthorSurname," +
                "Cover=@Cover," +
                "GoogleBookId=@GoogleBookId," +
                "Asin=@Asin," +
                "AuthorHash=@AuthorHash," +
                "TitleHash=@TitleHash," +
                "Visible=@Visible " +
                "WHERE Id=@Id";

        public const string INSERT_FICTION_FTS_WITHOUT_ID =
            "INSERT INTO fiction_fts VALUES (@Title,@AuthorFamily1,@AuthorName1,@AuthorSurname1,@Pseudonim1," +
                "@AuthorFamily2,@AuthorName2,@AuthorSurname2,@Pseudonim2,@AuthorFamily3,@AuthorName3,@AuthorSurname3,@Pseudonim3," +
                "@AuthorFamily4,@AuthorName4,@AuthorSurname4,@Pseudonim4,@RussianAuthorFamily,@RussianAuthorName,@RussianAuthorSurname," +
                "@Series1,@Series2,@Series3,@Series4,@Publisher,@Identifier)";

        public const string INSERT_FICTION_FTS_WITH_ID =
            "INSERT INTO fiction_fts (rowid,Title,AuthorFamily1,AuthorName1,AuthorSurname1,Pseudonim1," +
                "AuthorFamily2,AuthorName2,AuthorSurname2,Pseudonim2,AuthorFamily3,AuthorName3,AuthorSurname3,Pseudonim3," +
                "AuthorFamily4,AuthorName4,AuthorSurname4,Pseudonim4,RussianAuthorFamily,RussianAuthorName,RussianAuthorSurname," +
                "Series1,Series2,Series3,Series4,Publisher,Identifier) VALUES (@Id,@Title,@AuthorFamily1,@AuthorName1,@AuthorSurname1,@Pseudonim1," +
                "@AuthorFamily2,@AuthorName2,@AuthorSurname2,@Pseudonim2,@AuthorFamily3,@AuthorName3,@AuthorSurname3,@Pseudonim3," +
                "@AuthorFamily4,@AuthorName4,@AuthorSurname4,@Pseudonim4,@RussianAuthorFamily,@RussianAuthorName,@RussianAuthorSurname," +
                "@Series1,@Series2,@Series3,@Series4,@Publisher,@Identifier)";

        public const string DELETE_FICTION_FTS =
            "INSERT INTO fiction_fts (fiction_fts,rowid,Title,AuthorFamily1,AuthorName1,AuthorSurname1,Pseudonim1," +
                "AuthorFamily2,AuthorName2,AuthorSurname2,Pseudonim2,AuthorFamily3,AuthorName3,AuthorSurname3,Pseudonim3," +
                "AuthorFamily4,AuthorName4,AuthorSurname4,Pseudonim4,RussianAuthorFamily,RussianAuthorName,RussianAuthorSurname," +
                "Series1,Series2,Series3,Series4,Publisher,Identifier) "+
                "VALUES ('delete',@Id,@Title,@AuthorFamily1,@AuthorName1,@AuthorSurname1,@Pseudonim1," +
                "@AuthorFamily2,@AuthorName2,@AuthorSurname2,@Pseudonim2,@AuthorFamily3,@AuthorName3,@AuthorSurname3,@Pseudonim3," +
                "@AuthorFamily4,@AuthorName4,@AuthorSurname4,@Pseudonim4,@RussianAuthorFamily,@RussianAuthorName,@RussianAuthorSurname," +
                "@Series1,@Series2,@Series3,@Series4,@Publisher,@Identifier)";

        public const string GET_FICTION_INDEX_LIST = "PRAGMA index_list(fiction)";

        public const string FICTION_INDEX_PREFIX = "IX_fiction_";

        public const string CREATE_FICTION_MD5HASH_INDEX = "CREATE UNIQUE INDEX " + FICTION_INDEX_PREFIX + "Md5Hash ON fiction (Md5Hash COLLATE NOCASE)";

        public const string CREATE_FICTION_LASTMODIFIEDDATETIME_INDEX = "CREATE INDEX " + FICTION_INDEX_PREFIX +
            "LastModifiedDateTime ON fiction (LastModifiedDateTime DESC)";

        public const string CREATE_FICTION_LIBGENID_INDEX = "CREATE UNIQUE INDEX " + FICTION_INDEX_PREFIX + "LibgenId ON fiction (LibgenId ASC)";

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
                "LibgenId INTEGER NOT NULL," +
                "FileId INTEGER," +
                "FOREIGN KEY (FileId) REFERENCES files(Id)" +
            ")";

        public const string CREATE_SCIMAG_FTS_TABLE = "CREATE VIRTUAL TABLE IF NOT EXISTS scimag_fts USING fts5 "+
            "(Title, Authors, Doi, Doi2, PubmedId, Journal, Issnp, Issne, content=scimag, content_rowid=Id)";

        public const string GET_ALL_SCIMAG_LIBGEN_IDS = "SELECT LibgenId FROM scimag";

        public const string COUNT_SCIMAG = "SELECT MAX(Id) FROM scimag LIMIT 1";

        public const string GET_SCIMAG_BY_ID = "SELECT * FROM scimag WHERE Id = @Id";

        public const string GET_SCIMAG_BY_MD5HASH = "SELECT * FROM scimag WHERE Md5Hash = @Md5Hash COLLATE NOCASE LIMIT 1";

        public const string GET_SCIMAG_ID_BY_LIBGENID = "SELECT Id FROM scimag WHERE LibgenId = @LibgenId LIMIT 1";

        public const string GET_LAST_ADDED_SCIMAG =
            "SELECT * FROM scimag WHERE AddedDateTime = (SELECT MAX(AddedDateTime) FROM scimag) ORDER BY LibgenId DESC LIMIT 1";

        public const string GET_SCIMAG_MAX_LIBGEN_ID = "SELECT MAX(LibgenId) FROM scimag LIMIT 1";

        public const string SEARCH_SCIMAG = "SELECT * FROM scimag WHERE Id IN (SELECT rowid FROM scimag_fts WHERE scimag_fts MATCH @SearchQuery) ORDER BY Id";

        public const string INSERT_SCIMAG =
            "INSERT INTO scimag VALUES (@Id,@Doi,@Doi2,@Title,@Authors,@Year,@Month,@Day,@Volume,@Issue,@FirstPage,@LastPage,@Journal,@Isbn,@Issnp,@Issne," +
                "@Md5Hash,@SizeInBytes,@AddedDateTime,@JournalId,@AbstractUrl,@Attribute1,@Attribute2,@Attribute3,@Attribute4,@Attribute5,@Attribute6," +
                "@Visible,@PubmedId,@Pmc,@Pii,@LibgenId,NULL)";

        public const string INSERT_SCIMAG_FTS_WITHOUT_ID = "INSERT INTO scimag_fts VALUES (@Title,@Authors,@Doi,@Doi2,@PubmedId,@Journal,@Issnp,@Issne)";

        public const string GET_SCIMAG_INDEX_LIST = "PRAGMA index_list(scimag)";

        public const string SCIMAG_INDEX_PREFIX = "IX_scimag_";

        public const string CREATE_SCIMAG_MD5HASH_INDEX = "CREATE INDEX " + SCIMAG_INDEX_PREFIX + "Md5Hash ON scimag (Md5Hash COLLATE NOCASE)";

        public const string CREATE_SCIMAG_ADDEDDATETIME_INDEX = "CREATE INDEX " + SCIMAG_INDEX_PREFIX + "AddedDateTime ON scimag (AddedDateTime DESC)";

        public const string CREATE_FILES_TABLE =
            "CREATE TABLE IF NOT EXISTS files (" +
                "Id INTEGER PRIMARY KEY NOT NULL," +
                "FilePath TEXT NOT NULL," +
                "ArchiveEntry TEXT," +
                "ObjectType INTEGER NOT NULL," +
                "ObjectId INTEGER NOT NULL" +
            ")";

        public const string ALTER_NON_FICTION_ADD_FILE_ID = "ALTER TABLE non_fiction ADD COLUMN FileId INTEGER REFERENCES files(Id)";

        public const string ALTER_FICTION_ADD_FILE_ID = "ALTER TABLE fiction ADD COLUMN FileId INTEGER REFERENCES files(Id)";

        public const string ALTER_SCIMAG_ADD_FILE_ID = "ALTER TABLE scimag ADD COLUMN FileId INTEGER REFERENCES files(Id)";

        public const string GET_FILE_BY_ID = "SELECT * FROM files WHERE Id = @Id";

        public const string INSERT_FILE = "INSERT INTO files VALUES (@Id,@FilePath,@ArchiveEntry,@ObjectType,@ObjectId)";

        public const string UPDATE_FILE =
            "UPDATE files SET " +
                "FilePath=@FilePath," +
                "ArchiveEntry=@ArchiveEntry," +
                "ObjectType=@ObjectType," +
                "ObjectId=@ObjectId " +
                "WHERE Id=@Id";

        public const string UPDATE_NON_FICTION_FILE_ID = "UPDATE non_fiction SET FileId=@FileId WHERE Id=@Id";

        public const string UPDATE_FICTION_FILE_ID = "UPDATE fiction SET FileId=@FileId WHERE Id=@Id";

        public const string UPDATE_SCIMAG_FILE_ID = "UPDATE scimag SET FileId=@FileId WHERE Id=@Id";

        public const string GET_LAST_INSERTED_ID = "SELECT last_insert_rowid()";
    }
}
