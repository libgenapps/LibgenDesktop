using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Database
{
    internal class LocalDatabase : IDisposable
    {
        private SQLiteConnection connection;
        private bool disposed;

        private LocalDatabase(string databaseFilePath)
        {
            disposed = false;
            connection = new SQLiteConnection("Data Source = " + databaseFilePath);
            connection.Open();
            connection.EnableExtensions(true);
            connection.LoadExtension("SQLite.Interop.dll", "sqlite3_fts5_init");
            ExecuteCommands("PRAGMA LOCKING_MODE = EXCLUSIVE", "PRAGMA TEMP_STORE = MEMORY", "PRAGMA JOURNAL_MODE = OFF",
                "PRAGMA SYNCHRONOUS = OFF");
        }

        public static LocalDatabase CreateDatabase(string databaseFilePath)
        {
            SQLiteConnection.CreateFile(databaseFilePath);
            return new LocalDatabase(databaseFilePath);
        }

        public static LocalDatabase OpenDatabase(string databaseFilePath)
        {
            if (!File.Exists(databaseFilePath))
            {
                throw new FileNotFoundException("Database file not found", databaseFilePath);
            }
            return new LocalDatabase(databaseFilePath);
        }

        public void Dispose()
        {
            if (!disposed && connection != null)
            {
                connection.Dispose();
                connection = null;
            }
            disposed = true;
        }

        public void CreateMetadataTable()
        {
            ExecuteCommands(SqlScripts.CREATE_METADATA_TABLE);
        }

        public bool CheckIfMetadataExists()
        {
            return ExecuteScalarCommand(SqlScripts.CHECK_IF_METADATA_TABLE_EXIST) == 1;
        }

        public DatabaseMetadata GetMetadata()
        {
            DatabaseMetadata result = new DatabaseMetadata();
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_ALL_METADATA_ITEMS;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        string key = dataReader.GetString(0);
                        string value = dataReader.GetString(1);
                        if (DatabaseMetadata.FieldDefinitions.TryGetValue(key.ToLower(), out DatabaseMetadata.FieldDefinition field))
                        {
                            field.Setter(result, value);
                        }
                    }
                }
            }
            return result;
        }

        public void AddMetadata(DatabaseMetadata databaseMetadata)
        {
            foreach (DatabaseMetadata.FieldDefinition fieldDefinition in DatabaseMetadata.FieldDefinitions.Values)
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = SqlScripts.INSERT_METADATA_ITEM;
                    command.Parameters.AddWithValue("Key", fieldDefinition.FieldName);
                    command.Parameters.AddWithValue("Value", fieldDefinition.Getter(databaseMetadata));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateNonFictionTables()
        {
            ExecuteCommands(SqlScripts.CREATE_NON_FICTION_TABLE);
            ExecuteCommands(SqlScripts.CREATE_NON_FICTION_FTS_TABLE);
        }

        public int CountNonFictionBooks()
        {
            return ExecuteScalarCommand(SqlScripts.COUNT_NON_FICTION);
        }

        public NonFictionBook GetNonFictionBookById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_NON_FICTION_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();
                    NonFictionBook book = ReadNonFictionBook(dataReader);
                    return book;
                }
            }
        }

        public IEnumerable<NonFictionBook> SearchNonFictionBooks(string searchQuery, int? resultLimit)
        {
            searchQuery = EscapeSearchQuery(searchQuery);
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = GetSearchCommandWithLimit(SqlScripts.SEARCH_NON_FICTION, resultLimit);
                command.Parameters.AddWithValue("@SearchQuery", searchQuery);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        NonFictionBook book = ReadNonFictionBook(dataReader);
                        yield return book;
                    }
                }
            }
        }

        public void AddNonFictionBooks(List<NonFictionBook> books)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                using (SQLiteCommand insertCommand = connection.CreateCommand())
                using (SQLiteCommand insertFtsCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = SqlScripts.INSERT_NON_FICTION;
                    insertCommand.Parameters.AddWithValue("@Id", null);
                    SQLiteParameter titleParameter = insertCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter volumeInfoParameter = insertCommand.Parameters.Add("@VolumeInfo", DbType.String);
                    SQLiteParameter seriesParameter = insertCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter periodicalParameter = insertCommand.Parameters.Add("@Periodical", DbType.String);
                    SQLiteParameter authorsParameter = insertCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter yearParameter = insertCommand.Parameters.Add("@Year", DbType.String);
                    SQLiteParameter editionParameter = insertCommand.Parameters.Add("@Edition", DbType.String);
                    SQLiteParameter publisherParameter = insertCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter cityParameter = insertCommand.Parameters.Add("@City", DbType.String);
                    SQLiteParameter pagesParameter = insertCommand.Parameters.Add("@Pages", DbType.String);
                    SQLiteParameter pagesInFileParameter = insertCommand.Parameters.Add("@PagesInFile", DbType.Int32);
                    SQLiteParameter languageParameter = insertCommand.Parameters.Add("@Language", DbType.String);
                    SQLiteParameter topicParameter = insertCommand.Parameters.Add("@Topic", DbType.String);
                    SQLiteParameter libraryParameter = insertCommand.Parameters.Add("@Library", DbType.String);
                    SQLiteParameter issueParameter = insertCommand.Parameters.Add("@Issue", DbType.String);
                    SQLiteParameter identifierParameter = insertCommand.Parameters.Add("@Identifier", DbType.String);
                    SQLiteParameter issnParameter = insertCommand.Parameters.Add("@Issn", DbType.String);
                    SQLiteParameter asinParameter = insertCommand.Parameters.Add("@Asin", DbType.String);
                    SQLiteParameter udcParameter = insertCommand.Parameters.Add("@Udc", DbType.String);
                    SQLiteParameter lbcParameter = insertCommand.Parameters.Add("@Lbc", DbType.String);
                    SQLiteParameter ddcParameter = insertCommand.Parameters.Add("@Ddc", DbType.String);
                    SQLiteParameter lccParameter = insertCommand.Parameters.Add("@Lcc", DbType.String);
                    SQLiteParameter doiParameter = insertCommand.Parameters.Add("@Doi", DbType.String);
                    SQLiteParameter googleBookIdParameter = insertCommand.Parameters.Add("@GoogleBookId", DbType.String);
                    SQLiteParameter openLibraryIdParameter = insertCommand.Parameters.Add("@OpenLibraryId", DbType.String);
                    SQLiteParameter commentaryParameter = insertCommand.Parameters.Add("@Commentary", DbType.String);
                    SQLiteParameter dpiParameter = insertCommand.Parameters.Add("@Dpi", DbType.Int32);
                    SQLiteParameter colorParameter = insertCommand.Parameters.Add("@Color", DbType.String);
                    SQLiteParameter cleanedParameter = insertCommand.Parameters.Add("@Cleaned", DbType.String);
                    SQLiteParameter orientationParameter = insertCommand.Parameters.Add("@Orientation", DbType.String);
                    SQLiteParameter paginatedParameter = insertCommand.Parameters.Add("@Paginated", DbType.String);
                    SQLiteParameter scannedParameter = insertCommand.Parameters.Add("@Scanned", DbType.String);
                    SQLiteParameter bookmarkedParameter = insertCommand.Parameters.Add("@Bookmarked", DbType.String);
                    SQLiteParameter searchableParameter = insertCommand.Parameters.Add("@Searchable", DbType.String);
                    SQLiteParameter sizeInBytesParameter = insertCommand.Parameters.Add("@SizeInBytes", DbType.Int64);
                    SQLiteParameter formatParameter = insertCommand.Parameters.Add("@Format", DbType.String);
                    SQLiteParameter md5HashParameter = insertCommand.Parameters.Add("@Md5Hash", DbType.String);
                    SQLiteParameter genericParameter = insertCommand.Parameters.Add("@Generic", DbType.String);
                    SQLiteParameter visibleParameter = insertCommand.Parameters.Add("@Visible", DbType.String);
                    SQLiteParameter locatorParameter = insertCommand.Parameters.Add("@Locator", DbType.String);
                    SQLiteParameter localParameter = insertCommand.Parameters.Add("@Local", DbType.Int32);
                    SQLiteParameter addedDateTimeParameter = insertCommand.Parameters.Add("@AddedDateTime", DbType.String);
                    SQLiteParameter lastModifiedDateTimeParameter = insertCommand.Parameters.Add("@LastModifiedDateTime", DbType.String);
                    SQLiteParameter coverUrlParameter = insertCommand.Parameters.Add("@CoverUrl", DbType.String);
                    SQLiteParameter tagsParameter = insertCommand.Parameters.Add("@Tags", DbType.String);
                    SQLiteParameter identifierPlainParameter = insertCommand.Parameters.Add("@IdentifierPlain", DbType.String);
                    SQLiteParameter libgenIdParameter = insertCommand.Parameters.Add("@LibgenId", DbType.Int32);
                    insertFtsCommand.CommandText = SqlScripts.INSERT_NON_FICTION_FTS;
                    SQLiteParameter titleFtsParameter = insertFtsCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter seriesFtsParameter = insertFtsCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter authorsFtsParameter = insertFtsCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter publisherFtsParameter = insertFtsCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierPlainFtsParameter = insertFtsCommand.Parameters.Add("@IdentifierPlain", DbType.String);
                    foreach (NonFictionBook book in books)
                    {
                        titleParameter.Value = book.Title;
                        volumeInfoParameter.Value = book.VolumeInfo;
                        seriesParameter.Value = book.Series;
                        periodicalParameter.Value = book.Periodical;
                        authorsParameter.Value = book.Authors;
                        yearParameter.Value = book.Year;
                        editionParameter.Value = book.Edition;
                        publisherParameter.Value = book.Publisher;
                        cityParameter.Value = book.City;
                        pagesParameter.Value = book.Pages;
                        pagesInFileParameter.Value = book.PagesInFile;
                        languageParameter.Value = book.Language;
                        topicParameter.Value = book.Topic;
                        libraryParameter.Value = book.Library;
                        issueParameter.Value = book.Issue;
                        identifierParameter.Value = book.Identifier;
                        issnParameter.Value = book.Issn;
                        asinParameter.Value = book.Asin;
                        udcParameter.Value = book.Udc;
                        lbcParameter.Value = book.Lbc;
                        ddcParameter.Value = book.Ddc;
                        lccParameter.Value = book.Lcc;
                        doiParameter.Value = book.Doi;
                        googleBookIdParameter.Value = book.GoogleBookId;
                        openLibraryIdParameter.Value = book.OpenLibraryId;
                        commentaryParameter.Value = book.Commentary;
                        dpiParameter.Value = book.Dpi;
                        colorParameter.Value = book.Color;
                        cleanedParameter.Value = book.Cleaned;
                        orientationParameter.Value = book.Orientation;
                        paginatedParameter.Value = book.Paginated;
                        scannedParameter.Value = book.Scanned;
                        bookmarkedParameter.Value = book.Bookmarked;
                        searchableParameter.Value = book.Searchable;
                        sizeInBytesParameter.Value = book.SizeInBytes;
                        formatParameter.Value = book.Format;
                        md5HashParameter.Value = book.Md5Hash;
                        genericParameter.Value = book.Generic;
                        visibleParameter.Value = book.Visible;
                        locatorParameter.Value = book.Locator;
                        localParameter.Value = book.Local;
                        addedDateTimeParameter.Value = book.AddedDateTime.ToString("s");
                        lastModifiedDateTimeParameter.Value = book.LastModifiedDateTime.ToString("s");
                        coverUrlParameter.Value = book.CoverUrl;
                        tagsParameter.Value = book.Tags;
                        identifierPlainParameter.Value = book.IdentifierPlain;
                        libgenIdParameter.Value = book.LibgenId;
                        insertCommand.ExecuteNonQuery();
                        titleFtsParameter.Value = book.Title;
                        seriesFtsParameter.Value = book.Series;
                        authorsFtsParameter.Value = book.Authors;
                        publisherFtsParameter.Value = book.Publisher;
                        identifierPlainFtsParameter.Value = book.IdentifierPlain;
                        insertFtsCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
        }

        public void CreateFictionTables()
        {
            ExecuteCommands(SqlScripts.CREATE_FICTION_TABLE);
            ExecuteCommands(SqlScripts.CREATE_FICTION_FTS_TABLE);
        }

        public int CountFictionBooks()
        {
            return ExecuteScalarCommand(SqlScripts.COUNT_FICTION);
        }

        public FictionBook GetFictionBookById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_FICTION_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();
                    FictionBook book = ReadFictionBook(dataReader);
                    return book;
                }
            }
        }

        public IEnumerable<FictionBook> SearchFictionBooks(string searchQuery, int? resultLimit)
        {
            searchQuery = EscapeSearchQuery(searchQuery);
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = GetSearchCommandWithLimit(SqlScripts.SEARCH_FICTION, resultLimit);
                command.Parameters.AddWithValue("@SearchQuery", searchQuery);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        FictionBook book = ReadFictionBook(dataReader);
                        yield return book;
                    }
                }
            }
        }

        public void AddFictionBooks(List<FictionBook> books)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                using (SQLiteCommand insertCommand = connection.CreateCommand())
                using (SQLiteCommand insertFtsCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = SqlScripts.INSERT_FICTION;
                    insertCommand.Parameters.AddWithValue("@Id", null);
                    SQLiteParameter authorFamily1Parameter = insertCommand.Parameters.Add("@AuthorFamily1", DbType.String);
                    SQLiteParameter authorName1Parameter = insertCommand.Parameters.Add("@AuthorName1", DbType.String);
                    SQLiteParameter authorSurname1Parameter = insertCommand.Parameters.Add("@AuthorSurname1", DbType.String);
                    SQLiteParameter role1Parameter = insertCommand.Parameters.Add("@Role1", DbType.String);
                    SQLiteParameter pseudonim1Parameter = insertCommand.Parameters.Add("@Pseudonim1", DbType.String);
                    SQLiteParameter authorFamily2Parameter = insertCommand.Parameters.Add("@AuthorFamily2", DbType.String);
                    SQLiteParameter authorName2Parameter = insertCommand.Parameters.Add("@AuthorName2", DbType.String);
                    SQLiteParameter authorSurname2Parameter = insertCommand.Parameters.Add("@AuthorSurname2", DbType.String);
                    SQLiteParameter role2Parameter = insertCommand.Parameters.Add("@Role2", DbType.String);
                    SQLiteParameter pseudonim2Parameter = insertCommand.Parameters.Add("@Pseudonim2", DbType.String);
                    SQLiteParameter authorFamily3Parameter = insertCommand.Parameters.Add("@AuthorFamily3", DbType.String);
                    SQLiteParameter authorName3Parameter = insertCommand.Parameters.Add("@AuthorName3", DbType.String);
                    SQLiteParameter authorSurname3Parameter = insertCommand.Parameters.Add("@AuthorSurname3", DbType.String);
                    SQLiteParameter role3Parameter = insertCommand.Parameters.Add("@Role3", DbType.String);
                    SQLiteParameter pseudonim3Parameter = insertCommand.Parameters.Add("@Pseudonim3", DbType.String);
                    SQLiteParameter authorFamily4Parameter = insertCommand.Parameters.Add("@AuthorFamily4", DbType.String);
                    SQLiteParameter authorName4Parameter = insertCommand.Parameters.Add("@AuthorName4", DbType.String);
                    SQLiteParameter authorSurname4Parameter = insertCommand.Parameters.Add("@AuthorSurname4", DbType.String);
                    SQLiteParameter role4Parameter = insertCommand.Parameters.Add("@Role4", DbType.String);
                    SQLiteParameter pseudonim4Parameter = insertCommand.Parameters.Add("@Pseudonim4", DbType.String);
                    SQLiteParameter series1Parameter = insertCommand.Parameters.Add("@Series1", DbType.String);
                    SQLiteParameter series2Parameter = insertCommand.Parameters.Add("@Series2", DbType.String);
                    SQLiteParameter series3Parameter = insertCommand.Parameters.Add("@Series3", DbType.String);
                    SQLiteParameter series4Parameter = insertCommand.Parameters.Add("@Series4", DbType.String);
                    SQLiteParameter titleParameter = insertCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter formatParameter = insertCommand.Parameters.Add("@Format", DbType.String);
                    SQLiteParameter versionParameter = insertCommand.Parameters.Add("@Version", DbType.String);
                    SQLiteParameter sizeInBytesParameter = insertCommand.Parameters.Add("@SizeInBytes", DbType.Int64);
                    SQLiteParameter md5HashParameter = insertCommand.Parameters.Add("@Md5Hash", DbType.String);
                    SQLiteParameter pathParameter = insertCommand.Parameters.Add("@Path", DbType.String);
                    SQLiteParameter languageParameter = insertCommand.Parameters.Add("@Language", DbType.String);
                    SQLiteParameter pagesParameter = insertCommand.Parameters.Add("@Pages", DbType.String);
                    SQLiteParameter identifierParameter = insertCommand.Parameters.Add("@Identifier", DbType.String);
                    SQLiteParameter yearParameter = insertCommand.Parameters.Add("@Year", DbType.String);
                    SQLiteParameter publisherParameter = insertCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter editionParameter = insertCommand.Parameters.Add("@Edition", DbType.String);
                    SQLiteParameter commentaryParameter = insertCommand.Parameters.Add("@Commentary", DbType.String);
                    SQLiteParameter addedDateTimeParameter = insertCommand.Parameters.Add("@AddedDateTime", DbType.String);
                    SQLiteParameter lastModifiedDateTimeParameter = insertCommand.Parameters.Add("@LastModifiedDateTime", DbType.String);
                    SQLiteParameter russianAuthorFamilyParameter = insertCommand.Parameters.Add("@RussianAuthorFamily", DbType.String);
                    SQLiteParameter russianAuthorNameParameter = insertCommand.Parameters.Add("@RussianAuthorName", DbType.String);
                    SQLiteParameter russianAuthorSurnameParameter = insertCommand.Parameters.Add("@RussianAuthorSurname", DbType.String);
                    SQLiteParameter coverParameter = insertCommand.Parameters.Add("@Cover", DbType.String);
                    SQLiteParameter googleBookIdParameter = insertCommand.Parameters.Add("@GoogleBookId", DbType.String);
                    SQLiteParameter asinParameter = insertCommand.Parameters.Add("@Asin", DbType.String);
                    SQLiteParameter authorHashParameter = insertCommand.Parameters.Add("@AuthorHash", DbType.String);
                    SQLiteParameter titleHashParameter = insertCommand.Parameters.Add("@TitleHash", DbType.String);
                    SQLiteParameter visibleParameter = insertCommand.Parameters.Add("@Visible", DbType.String);
                    SQLiteParameter libgenIdParameter = insertCommand.Parameters.Add("@LibgenId", DbType.Int32);
                    insertFtsCommand.CommandText = SqlScripts.INSERT_FICTION_FTS;
                    SQLiteParameter titleFtsParameter = insertFtsCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorFamily1FtsParameter = insertFtsCommand.Parameters.Add("@AuthorFamily1", DbType.String);
                    SQLiteParameter authorName1FtsParameter = insertFtsCommand.Parameters.Add("@AuthorName1", DbType.String);
                    SQLiteParameter authorSurname1FtsParameter = insertFtsCommand.Parameters.Add("@AuthorSurname1", DbType.String);
                    SQLiteParameter pseudonim1FtsParameter = insertFtsCommand.Parameters.Add("@Pseudonim1", DbType.String);
                    SQLiteParameter authorFamily2FtsParameter = insertFtsCommand.Parameters.Add("@AuthorFamily2", DbType.String);
                    SQLiteParameter authorName2FtsParameter = insertFtsCommand.Parameters.Add("@AuthorName2", DbType.String);
                    SQLiteParameter authorSurname2FtsParameter = insertFtsCommand.Parameters.Add("@AuthorSurname2", DbType.String);
                    SQLiteParameter pseudonim2FtsParameter = insertFtsCommand.Parameters.Add("@Pseudonim2", DbType.String);
                    SQLiteParameter authorFamily3FtsParameter = insertFtsCommand.Parameters.Add("@AuthorFamily3", DbType.String);
                    SQLiteParameter authorName3FtsParameter = insertFtsCommand.Parameters.Add("@AuthorName3", DbType.String);
                    SQLiteParameter authorSurname3FtsParameter = insertFtsCommand.Parameters.Add("@AuthorSurname3", DbType.String);
                    SQLiteParameter pseudonim3FtsParameter = insertFtsCommand.Parameters.Add("@Pseudonim3", DbType.String);
                    SQLiteParameter authorFamily4FtsParameter = insertFtsCommand.Parameters.Add("@AuthorFamily4", DbType.String);
                    SQLiteParameter authorName4FtsParameter = insertFtsCommand.Parameters.Add("@AuthorName4", DbType.String);
                    SQLiteParameter authorSurname4FtsParameter = insertFtsCommand.Parameters.Add("@AuthorSurname4", DbType.String);
                    SQLiteParameter pseudonim4FtsParameter = insertFtsCommand.Parameters.Add("@Pseudonim4", DbType.String);
                    SQLiteParameter russianAuthorFamilyFtsParameter = insertFtsCommand.Parameters.Add("@RussianAuthorFamily", DbType.String);
                    SQLiteParameter russianAuthorNameFtsParameter = insertFtsCommand.Parameters.Add("@RussianAuthorName", DbType.String);
                    SQLiteParameter russianAuthorSurnameFtsParameter = insertFtsCommand.Parameters.Add("@RussianAuthorSurname", DbType.String);
                    SQLiteParameter series1FtsParameter = insertFtsCommand.Parameters.Add("@Series1", DbType.String);
                    SQLiteParameter series2FtsParameter = insertFtsCommand.Parameters.Add("@Series2", DbType.String);
                    SQLiteParameter series3FtsParameter = insertFtsCommand.Parameters.Add("@Series3", DbType.String);
                    SQLiteParameter series4FtsParameter = insertFtsCommand.Parameters.Add("@Series4", DbType.String);
                    SQLiteParameter publisherFtsParameter = insertFtsCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierFtsParameter = insertFtsCommand.Parameters.Add("@Identifier", DbType.String);
                    foreach (FictionBook book in books)
                    {
                        authorFamily1Parameter.Value = book.AuthorFamily1;
                        authorName1Parameter.Value = book.AuthorName1;
                        authorSurname1Parameter.Value = book.AuthorSurname1;
                        role1Parameter.Value = book.Role1;
                        pseudonim1Parameter.Value = book.Pseudonim1;
                        authorFamily2Parameter.Value = book.AuthorFamily2;
                        authorName2Parameter.Value = book.AuthorName2;
                        authorSurname2Parameter.Value = book.AuthorSurname2;
                        role2Parameter.Value = book.Role2;
                        pseudonim2Parameter.Value = book.Pseudonim2;
                        authorFamily3Parameter.Value = book.AuthorFamily3;
                        authorName3Parameter.Value = book.AuthorName3;
                        authorSurname3Parameter.Value = book.AuthorSurname3;
                        role3Parameter.Value = book.Role3;
                        pseudonim3Parameter.Value = book.Pseudonim3;
                        authorFamily4Parameter.Value = book.AuthorFamily4;
                        authorName4Parameter.Value = book.AuthorName4;
                        authorSurname4Parameter.Value = book.AuthorSurname4;
                        role4Parameter.Value = book.Role4;
                        pseudonim4Parameter.Value = book.Pseudonim4;
                        series1Parameter.Value = book.Series1;
                        series2Parameter.Value = book.Series2;
                        series3Parameter.Value = book.Series3;
                        series4Parameter.Value = book.Series4;
                        titleParameter.Value = book.Title;
                        formatParameter.Value = book.Format;
                        versionParameter.Value = book.Version;
                        sizeInBytesParameter.Value = book.SizeInBytes;
                        md5HashParameter.Value = book.Md5Hash;
                        pathParameter.Value = book.Path;
                        languageParameter.Value = book.Language;
                        pagesParameter.Value = book.Pages;
                        identifierParameter.Value = book.Identifier;
                        yearParameter.Value = book.Year;
                        publisherParameter.Value = book.Publisher;
                        editionParameter.Value = book.Edition;
                        commentaryParameter.Value = book.Commentary;
                        addedDateTimeParameter.Value = book.AddedDateTime?.ToString("s");
                        lastModifiedDateTimeParameter.Value = book.LastModifiedDateTime.ToString("s");
                        russianAuthorFamilyParameter.Value = book.RussianAuthorFamily;
                        russianAuthorNameParameter.Value = book.RussianAuthorName;
                        russianAuthorSurnameParameter.Value = book.RussianAuthorSurname;
                        coverParameter.Value = book.Cover;
                        googleBookIdParameter.Value = book.GoogleBookId;
                        asinParameter.Value = book.Asin;
                        authorHashParameter.Value = book.AuthorHash;
                        titleHashParameter.Value = book.TitleHash;
                        visibleParameter.Value = book.Visible;
                        libgenIdParameter.Value = book.LibgenId;
                        insertCommand.ExecuteNonQuery();
                        titleFtsParameter.Value = book.Title;
                        authorFamily1FtsParameter.Value = book.AuthorFamily1;
                        authorName1FtsParameter.Value = book.AuthorName1;
                        authorSurname1FtsParameter.Value = book.AuthorSurname1;
                        pseudonim1FtsParameter.Value = book.Pseudonim1;
                        authorFamily2FtsParameter.Value = book.AuthorFamily2;
                        authorName2FtsParameter.Value = book.AuthorName2;
                        authorSurname2FtsParameter.Value = book.AuthorSurname2;
                        pseudonim2FtsParameter.Value = book.Pseudonim2;
                        authorFamily3FtsParameter.Value = book.AuthorFamily3;
                        authorName3FtsParameter.Value = book.AuthorName3;
                        authorSurname3FtsParameter.Value = book.AuthorSurname3;
                        pseudonim3FtsParameter.Value = book.Pseudonim3;
                        authorFamily4FtsParameter.Value = book.AuthorFamily4;
                        authorName4FtsParameter.Value = book.AuthorName4;
                        authorSurname4FtsParameter.Value = book.AuthorSurname4;
                        pseudonim4FtsParameter.Value = book.Pseudonim4;
                        russianAuthorFamilyFtsParameter.Value = book.RussianAuthorFamily;
                        russianAuthorNameFtsParameter.Value = book.RussianAuthorName;
                        russianAuthorSurnameFtsParameter.Value = book.RussianAuthorSurname;
                        series1FtsParameter.Value = book.Series1;
                        series2FtsParameter.Value = book.Series2;
                        series3FtsParameter.Value = book.Series3;
                        series4FtsParameter.Value = book.Series4;
                        publisherFtsParameter.Value = book.Publisher;
                        identifierFtsParameter.Value = book.Identifier;
                        insertFtsCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
        }

        public void CreateSciMagTables()
        {
            ExecuteCommands(SqlScripts.CREATE_SCIMAG_TABLE);
            ExecuteCommands(SqlScripts.CREATE_SCIMAG_FTS_TABLE);
        }

        public int CountSciMagArticles()
        {
            return ExecuteScalarCommand(SqlScripts.COUNT_SCIMAG);
        }

        public SciMagArticle GetSciMagArticleById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_SCIMAG_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();
                    SciMagArticle article = ReadSciMagArticle(dataReader);
                    return article;
                }
            }
        }

        public IEnumerable<SciMagArticle> SearchSciMagArticles(string searchQuery, int? resultLimit)
        {
            searchQuery = EscapeSearchQuery(searchQuery);
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = GetSearchCommandWithLimit(SqlScripts.SEARCH_SCIMAG, resultLimit);
                command.Parameters.AddWithValue("@SearchQuery", searchQuery);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        SciMagArticle article = ReadSciMagArticle(dataReader);
                        yield return article;
                    }
                }
            }
        }

        public void AddSciMagArticles(List<SciMagArticle> articles)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                using (SQLiteCommand insertCommand = connection.CreateCommand())
                using (SQLiteCommand insertFtsCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = SqlScripts.INSERT_SCIMAG;
                    insertCommand.Parameters.AddWithValue("@Id", null);
                    SQLiteParameter doiParameter = insertCommand.Parameters.Add("@Doi", DbType.String);
                    SQLiteParameter doi2Parameter = insertCommand.Parameters.Add("@Doi2", DbType.String);
                    SQLiteParameter titleParameter = insertCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorsParameter = insertCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter yearParameter = insertCommand.Parameters.Add("@Year", DbType.String);
                    SQLiteParameter monthParameter = insertCommand.Parameters.Add("@Month", DbType.String);
                    SQLiteParameter dayParameter = insertCommand.Parameters.Add("@Day", DbType.String);
                    SQLiteParameter volumeParameter = insertCommand.Parameters.Add("@Volume", DbType.String);
                    SQLiteParameter issueParameter = insertCommand.Parameters.Add("@Issue", DbType.String);
                    SQLiteParameter firstPageParameter = insertCommand.Parameters.Add("@FirstPage", DbType.String);
                    SQLiteParameter lastPageParameter = insertCommand.Parameters.Add("@LastPage", DbType.String);
                    SQLiteParameter journalParameter = insertCommand.Parameters.Add("@Journal", DbType.String);
                    SQLiteParameter isbnParameter = insertCommand.Parameters.Add("@Isbn", DbType.String);
                    SQLiteParameter issnpParameter = insertCommand.Parameters.Add("@Issnp", DbType.String);
                    SQLiteParameter issneParameter = insertCommand.Parameters.Add("@Issne", DbType.String);
                    SQLiteParameter md5HashParameter = insertCommand.Parameters.Add("@Md5Hash", DbType.String);
                    SQLiteParameter sizeInBytesParameter = insertCommand.Parameters.Add("@SizeInBytes", DbType.Int64);
                    SQLiteParameter addedDateTimeParameter = insertCommand.Parameters.Add("@AddedDateTime", DbType.String);
                    SQLiteParameter journalIdParameter = insertCommand.Parameters.Add("@JournalId", DbType.String);
                    SQLiteParameter abstractUrlParameter = insertCommand.Parameters.Add("@AbstractUrl", DbType.String);
                    SQLiteParameter attribute1Parameter = insertCommand.Parameters.Add("@Attribute1", DbType.String);
                    SQLiteParameter attribute2Parameter = insertCommand.Parameters.Add("@Attribute2", DbType.String);
                    SQLiteParameter attribute3Parameter = insertCommand.Parameters.Add("@Attribute3", DbType.String);
                    SQLiteParameter attribute4Parameter = insertCommand.Parameters.Add("@Attribute4", DbType.String);
                    SQLiteParameter attribute5Parameter = insertCommand.Parameters.Add("@Attribute5", DbType.String);
                    SQLiteParameter attribute6Parameter = insertCommand.Parameters.Add("@Attribute6", DbType.String);
                    SQLiteParameter visibleParameter = insertCommand.Parameters.Add("@Visible", DbType.String);
                    SQLiteParameter pubmedIdParameter = insertCommand.Parameters.Add("@PubmedId", DbType.String);
                    SQLiteParameter pmcParameter = insertCommand.Parameters.Add("@Pmc", DbType.String);
                    SQLiteParameter piiParameter = insertCommand.Parameters.Add("@Pii", DbType.String);
                    SQLiteParameter libgenIdParameter = insertCommand.Parameters.Add("@LibgenId", DbType.Int32);
                    insertFtsCommand.CommandText = SqlScripts.INSERT_SCIMAG_FTS;
                    SQLiteParameter titleFtsParameter = insertFtsCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorsFtsParameter = insertFtsCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter doiFtsParameter = insertFtsCommand.Parameters.Add("@Doi", DbType.String);
                    SQLiteParameter doi2FtsParameter = insertFtsCommand.Parameters.Add("@Doi2", DbType.String);
                    SQLiteParameter pubmedIdFtsParameter = insertFtsCommand.Parameters.Add("@PubmedId", DbType.String);
                    SQLiteParameter journalFtsParameter = insertFtsCommand.Parameters.Add("@Journal", DbType.String);
                    SQLiteParameter issnpFtsParameter = insertFtsCommand.Parameters.Add("@Issnp", DbType.String);
                    SQLiteParameter issneFtsParameter = insertFtsCommand.Parameters.Add("@Issne", DbType.String);
                    foreach (SciMagArticle article in articles)
                    {
                        doiParameter.Value = article.Doi;
                        doi2Parameter.Value = article.Doi2;
                        titleParameter.Value = article.Title;
                        authorsParameter.Value = article.Authors;
                        yearParameter.Value = article.Year;
                        monthParameter.Value = article.Month;
                        dayParameter.Value = article.Day;
                        volumeParameter.Value = article.Volume;
                        issueParameter.Value = article.Issue;
                        firstPageParameter.Value = article.FirstPage;
                        lastPageParameter.Value = article.LastPage;
                        journalParameter.Value = article.Journal;
                        isbnParameter.Value = article.Isbn;
                        issnpParameter.Value = article.Issnp;
                        issneParameter.Value = article.Issne;
                        md5HashParameter.Value = article.Md5Hash;
                        sizeInBytesParameter.Value = article.SizeInBytes;
                        addedDateTimeParameter.Value = article.AddedDateTime?.ToString("s");
                        journalIdParameter.Value = article.JournalId;
                        abstractUrlParameter.Value = article.AbstractUrl;
                        attribute1Parameter.Value = article.Attribute1;
                        attribute2Parameter.Value = article.Attribute2;
                        attribute3Parameter.Value = article.Attribute3;
                        attribute4Parameter.Value = article.Attribute4;
                        attribute5Parameter.Value = article.Attribute5;
                        attribute6Parameter.Value = article.Attribute6;
                        visibleParameter.Value = article.Visible;
                        pubmedIdParameter.Value = article.PubmedId;
                        pmcParameter.Value = article.Pmc;
                        piiParameter.Value = article.Pii;
                        libgenIdParameter.Value = article.LibgenId;
                        insertCommand.ExecuteNonQuery();
                        titleFtsParameter.Value = article.Title;
                        authorsFtsParameter.Value = article.Authors;
                        doiFtsParameter.Value = article.Doi;
                        doi2FtsParameter.Value = article.Doi2;
                        pubmedIdFtsParameter.Value = article.PubmedId;
                        journalFtsParameter.Value = article.Journal;
                        issnpFtsParameter.Value = article.Issnp;
                        issneFtsParameter.Value = article.Issne;
                        insertFtsCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NonFictionBook ReadNonFictionBook(SQLiteDataReader dataReader)
        {
            NonFictionBook book = new NonFictionBook();
            book.Id = dataReader.GetInt32(0);
            book.Title = dataReader.GetString(1);
            book.VolumeInfo = dataReader.GetString(2);
            book.Series = dataReader.GetString(3);
            book.Periodical = dataReader.GetString(4);
            book.Authors = dataReader.GetString(5);
            book.Year = dataReader.GetString(6);
            book.Edition = dataReader.GetString(7);
            book.Publisher = dataReader.GetString(8);
            book.City = dataReader.GetString(9);
            book.Pages = dataReader.GetString(10);
            book.PagesInFile = dataReader.GetInt32(11);
            book.Language = dataReader.GetString(12);
            book.Topic = dataReader.GetString(13);
            book.Library = dataReader.GetString(14);
            book.Issue = dataReader.GetString(15);
            book.Identifier = dataReader.GetString(16);
            book.Issn = dataReader.GetString(17);
            book.Asin = dataReader.GetString(18);
            book.Udc = dataReader.GetString(19);
            book.Lbc = dataReader.GetString(20);
            book.Ddc = dataReader.GetString(21);
            book.Lcc = dataReader.GetString(22);
            book.Doi = dataReader.GetString(23);
            book.GoogleBookId = dataReader.GetString(24);
            book.OpenLibraryId = dataReader.GetString(25);
            book.Commentary = dataReader.GetString(26);
            book.Dpi = dataReader.GetInt32(27);
            book.Color = dataReader.GetString(28);
            book.Cleaned = dataReader.GetString(29);
            book.Orientation = dataReader.GetString(30);
            book.Paginated = dataReader.GetString(31);
            book.Scanned = dataReader.GetString(32);
            book.Bookmarked = dataReader.GetString(33);
            book.Searchable = dataReader.GetString(34);
            book.SizeInBytes = dataReader.GetInt64(35);
            book.Format = dataReader.GetString(36);
            book.Md5Hash = dataReader.GetString(37);
            book.Generic = dataReader.GetString(38);
            book.Visible = dataReader.GetString(39);
            book.Locator = dataReader.GetString(40);
            book.Local = dataReader.GetInt32(41);
            book.AddedDateTime = ParseDbDate(dataReader.GetString(42));
            book.LastModifiedDateTime = ParseDbDate(dataReader.GetString(43));
            book.CoverUrl = dataReader.GetString(44);
            book.Tags = dataReader.GetString(45);
            book.IdentifierPlain = dataReader.GetString(46);
            book.LibgenId = dataReader.GetInt32(47);
            return book;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FictionBook ReadFictionBook(SQLiteDataReader dataReader)
        {
            FictionBook book = new FictionBook();
            book.Id = dataReader.GetInt32(0);
            book.AuthorFamily1 = dataReader.GetString(1);
            book.AuthorName1 = dataReader.GetString(2);
            book.AuthorSurname1 = dataReader.GetString(3);
            book.Role1 = dataReader.GetString(4);
            book.Pseudonim1 = dataReader.GetString(5);
            book.AuthorFamily2 = dataReader.GetString(6);
            book.AuthorName2 = dataReader.GetString(7);
            book.AuthorSurname2 = dataReader.GetString(8);
            book.Role2 = dataReader.GetString(9);
            book.Pseudonim2 = dataReader.GetString(10);
            book.AuthorFamily3 = dataReader.GetString(11);
            book.AuthorName3 = dataReader.GetString(12);
            book.AuthorSurname3 = dataReader.GetString(13);
            book.Role3 = dataReader.GetString(14);
            book.Pseudonim3 = dataReader.GetString(15);
            book.AuthorFamily4 = dataReader.GetString(16);
            book.AuthorName4 = dataReader.GetString(17);
            book.AuthorSurname4 = dataReader.GetString(18);
            book.Role4 = dataReader.GetString(19);
            book.Pseudonim4 = dataReader.GetString(20);
            book.Series1 = dataReader.GetString(21);
            book.Series2 = dataReader.GetString(22);
            book.Series3 = dataReader.GetString(23);
            book.Series4 = dataReader.GetString(24);
            book.Title = dataReader.GetString(25);
            book.Format = dataReader.GetString(26);
            book.Version = dataReader.GetString(27);
            book.SizeInBytes = dataReader.GetInt64(28);
            book.Md5Hash = dataReader.GetString(29);
            book.Path = dataReader.GetString(30);
            book.Language = dataReader.GetString(31);
            book.Pages = dataReader.GetString(32);
            book.Identifier = dataReader.GetString(33);
            book.Year = dataReader.GetString(34);
            book.Publisher = dataReader.GetString(35);
            book.Edition = dataReader.GetString(36);
            book.Commentary = dataReader.GetString(37);
            book.AddedDateTime = ParseNullableDbDate(dataReader.GetValue(38));
            book.LastModifiedDateTime = ParseDbDate(dataReader.GetString(39));
            book.RussianAuthorFamily = dataReader.GetString(40);
            book.RussianAuthorName = dataReader.GetString(41);
            book.RussianAuthorSurname = dataReader.GetString(42);
            book.Cover = dataReader.GetString(43);
            book.GoogleBookId = dataReader.GetString(44);
            book.Asin = dataReader.GetString(45);
            book.AuthorHash = dataReader.GetString(46);
            book.TitleHash = dataReader.GetString(47);
            book.Visible = dataReader.GetString(48);
            book.LibgenId = dataReader.GetInt32(49);
            return book;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SciMagArticle ReadSciMagArticle(SQLiteDataReader dataReader)
        {
            SciMagArticle article = new SciMagArticle();
            article.Id = dataReader.GetInt32(0);
            article.Doi = dataReader.GetString(1);
            article.Doi2 = dataReader.GetString(2);
            article.Title = dataReader.GetString(3);
            article.Authors = dataReader.GetString(4);
            article.Year = dataReader.GetString(5);
            article.Month = dataReader.GetString(6);
            article.Day = dataReader.GetString(7);
            article.Volume = dataReader.GetString(8);
            article.Issue = dataReader.GetString(9);
            article.FirstPage = dataReader.GetString(10);
            article.LastPage = dataReader.GetString(11);
            article.Journal = dataReader.GetString(12);
            article.Isbn = dataReader.GetString(13);
            article.Issnp = dataReader.GetString(14);
            article.Issne = dataReader.GetString(15);
            article.Md5Hash = dataReader.GetString(16);
            article.SizeInBytes = dataReader.GetInt64(17);
            article.AddedDateTime = ParseNullableDbDate(dataReader.GetValue(18));
            article.JournalId = dataReader.GetString(19);
            article.AbstractUrl = dataReader.GetString(20);
            article.Attribute1 = dataReader.GetString(21);
            article.Attribute2 = dataReader.GetString(22);
            article.Attribute3 = dataReader.GetString(23);
            article.Attribute4 = dataReader.GetString(24);
            article.Attribute5 = dataReader.GetString(25);
            article.Attribute6 = dataReader.GetString(26);
            article.Visible = dataReader.GetString(27);
            article.PubmedId = dataReader.GetString(28);
            article.Pmc = dataReader.GetString(29);
            article.Pii = dataReader.GetString(30);
            article.LibgenId = dataReader.GetInt32(31);
            return article;
        }

        private string EscapeSearchQuery(string originalSearchQuery)
        {
            return SearchQueryParser.GetEscapedQuery(originalSearchQuery);
        }

        private string GetSearchCommandWithLimit(string searchCommand, int? resultLimit)
        {
            return searchCommand + (resultLimit.HasValue ? " LIMIT " + resultLimit.Value : String.Empty);
        }

        private void ExecuteCommands(params string[] commands)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                foreach (string commandText in commands)
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private int ExecuteScalarCommand(string commandText)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                object objectResult = command.ExecuteScalar();
                return objectResult != DBNull.Value ? (int)(long)objectResult : 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime ParseDbDate(string input)
        {
            return DateTime.ParseExact(input, "s", CultureInfo.InvariantCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime? ParseNullableDbDate(object input)
        {
            if (input is string inputString)
            {
                if (String.IsNullOrEmpty(inputString))
                {
                    return null;
                }
                return ParseDbDate(inputString);
            }
            return null;
        }
    }
}
