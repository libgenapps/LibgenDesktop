using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.Models.Database
{
    internal class LocalDatabase : IDisposable
    {
        private SQLiteConnection connection;
        private bool disposed;

        private LocalDatabase(string databaseFullPath)
        {
            DatabaseFullPath = databaseFullPath;
            disposed = false;
            connection = new SQLiteConnection("Data Source = " + databaseFullPath, parseViaFramework: true);
            connection.Open();
            connection.EnableExtensions(true);
            connection.LoadExtension("SQLite.Interop.dll", "sqlite3_fts5_init");
            ExecuteCommands("PRAGMA LOCKING_MODE = EXCLUSIVE", "PRAGMA TEMP_STORE = MEMORY", "PRAGMA JOURNAL_MODE = OFF",
                "PRAGMA SYNCHRONOUS = OFF");
        }

        public string DatabaseFullPath { get; }

        public static LocalDatabase CreateDatabase(string databaseFullPath)
        {
            Logger.Debug($"Creating database file: {databaseFullPath}");
            SQLiteConnection.CreateFile(databaseFullPath);
            LocalDatabase result = new LocalDatabase(databaseFullPath);
            Logger.Debug("Database file has been created successfully.");
            return result;
        }

        public static LocalDatabase OpenDatabase(string databaseFullPath)
        {
            if (!File.Exists(databaseFullPath))
            {
                throw new FileNotFoundException("Database file not found", databaseFullPath);
            }
            Logger.Debug($"Opening database file: {databaseFullPath}");
            LocalDatabase result = new LocalDatabase(databaseFullPath);
            Logger.Debug("Database file has been opened successfully.");
            return result;
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
            return ExecuteIntScalarCommand(SqlScripts.CHECK_IF_METADATA_TABLE_EXIST) == 1;
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
                        string value = ParseNullableDbString(dataReader.GetValue(1));
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

        public void UpdateMetadata(DatabaseMetadata newMetadata)
        {
            DatabaseMetadata oldMetadata = GetMetadata();
            foreach (DatabaseMetadata.FieldDefinition fieldDefinition in DatabaseMetadata.FieldDefinitions.Values)
            {
                if (fieldDefinition.Getter(oldMetadata) != fieldDefinition.Getter(newMetadata))
                {
                    SetMetadataValue(newMetadata, fieldDefinition);
                }
            }
        }

        public void SetMetadataValue(DatabaseMetadata databaseMetadata, DatabaseMetadata.FieldDefinition field)
        {
            bool metadataItemExist;
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.CHECK_IF_METADATA_ITEM_EXIST;
                command.Parameters.AddWithValue("Key", field.FieldName);
                metadataItemExist = ParseIntScalarResult(command.ExecuteScalar()) == 1;
            }
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = metadataItemExist ? SqlScripts.UPDATE_METADATA_ITEM : SqlScripts.INSERT_METADATA_ITEM;
                command.Parameters.AddWithValue("Key", field.FieldName);
                command.Parameters.AddWithValue("Value", field.Getter(databaseMetadata));
                command.ExecuteNonQuery();
            }
        }

        public void CreateNonFictionTables()
        {
            ExecuteCommands(SqlScripts.CREATE_NON_FICTION_TABLE);
            ExecuteCommands(SqlScripts.CREATE_NON_FICTION_FTS_TABLE);
        }

        public void CreateNonFictionMd5HashIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_NON_FICTION_MD5HASH_INDEX);
        }

        public void CreateNonFictionLastModifiedDateTimeIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_NON_FICTION_LASTMODIFIEDDATETIME_INDEX);
        }

        public void CreateNonFictionLibgenIdIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_NON_FICTION_LIBGENID_INDEX);
        }

        public BitArray GetNonFictionLibgenIdsBitArray()
        {
            return GetLibgenIdsBitArray(SqlScripts.GET_ALL_NON_FICTION_LIBGEN_IDS, GetNonFictionMaxLibgenId());
        }

        public int CountNonFictionBooks()
        {
            return ExecuteIntScalarCommand(SqlScripts.COUNT_NON_FICTION);
        }

        public NonFictionBook GetNonFictionBookById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_NON_FICTION_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        NonFictionBook book = ReadNonFictionBook(dataReader);
                        return book;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public NonFictionBook GetNonFictionBookByMd5Hash(string md5Hash)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_NON_FICTION_BY_MD5HASH;
                command.Parameters.AddWithValue("@Md5Hash", md5Hash);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        NonFictionBook book = ReadNonFictionBook(dataReader);
                        return book;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int? GetNonFictionBookIdByLibgenId(int libgenId)
        {
            return GetIdByLibgenId(SqlScripts.GET_NON_FICTION_ID_BY_LIBGENID, libgenId);
        }

        public NonFictionBook GetLastModifiedNonFictionBook()
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_LAST_MODIFIED_NON_FICTION;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        NonFictionBook book = ReadNonFictionBook(dataReader);
                        return book;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int GetNonFictionMaxLibgenId()
        {
            return ExecuteIntScalarCommand(SqlScripts.GET_NON_FICTION_MAX_LIBGEN_ID);
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
                    insertFtsCommand.CommandText = SqlScripts.INSERT_NON_FICTION_FTS_WITHOUT_ID;
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
                        titleFtsParameter.Value = book.Title.SeparateCjkCharactersWithSpaces();
                        seriesFtsParameter.Value = book.Series.SeparateCjkCharactersWithSpaces();
                        authorsFtsParameter.Value = book.Authors.SeparateCjkCharactersWithSpaces();
                        publisherFtsParameter.Value = book.Publisher.SeparateCjkCharactersWithSpaces();
                        identifierPlainFtsParameter.Value = book.IdentifierPlain;
                        insertFtsCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
        }

        public void UpdateNonFictionBooks(List<NonFictionBook> books)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                using (SQLiteCommand deleteFtsCommand = connection.CreateCommand())
                using (SQLiteCommand updateCommand = connection.CreateCommand())
                using (SQLiteCommand insertFtsWithIdCommand = connection.CreateCommand())
                {
                    deleteFtsCommand.CommandText = SqlScripts.DELETE_NON_FICTION_FTS;
                    SQLiteParameter idDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Id", DbType.Int32);
                    SQLiteParameter titleDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter seriesDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter authorsDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter publisherDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierPlainDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@IdentifierPlain", DbType.String);
                    updateCommand.CommandText = SqlScripts.UPDATE_NON_FICTION;
                    SQLiteParameter idParameter = updateCommand.Parameters.Add("@Id", DbType.Int32);
                    SQLiteParameter titleParameter = updateCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter volumeInfoParameter = updateCommand.Parameters.Add("@VolumeInfo", DbType.String);
                    SQLiteParameter seriesParameter = updateCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter periodicalParameter = updateCommand.Parameters.Add("@Periodical", DbType.String);
                    SQLiteParameter authorsParameter = updateCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter yearParameter = updateCommand.Parameters.Add("@Year", DbType.String);
                    SQLiteParameter editionParameter = updateCommand.Parameters.Add("@Edition", DbType.String);
                    SQLiteParameter publisherParameter = updateCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter cityParameter = updateCommand.Parameters.Add("@City", DbType.String);
                    SQLiteParameter pagesParameter = updateCommand.Parameters.Add("@Pages", DbType.String);
                    SQLiteParameter pagesInFileParameter = updateCommand.Parameters.Add("@PagesInFile", DbType.Int32);
                    SQLiteParameter languageParameter = updateCommand.Parameters.Add("@Language", DbType.String);
                    SQLiteParameter topicParameter = updateCommand.Parameters.Add("@Topic", DbType.String);
                    SQLiteParameter libraryParameter = updateCommand.Parameters.Add("@Library", DbType.String);
                    SQLiteParameter issueParameter = updateCommand.Parameters.Add("@Issue", DbType.String);
                    SQLiteParameter identifierParameter = updateCommand.Parameters.Add("@Identifier", DbType.String);
                    SQLiteParameter issnParameter = updateCommand.Parameters.Add("@Issn", DbType.String);
                    SQLiteParameter asinParameter = updateCommand.Parameters.Add("@Asin", DbType.String);
                    SQLiteParameter udcParameter = updateCommand.Parameters.Add("@Udc", DbType.String);
                    SQLiteParameter lbcParameter = updateCommand.Parameters.Add("@Lbc", DbType.String);
                    SQLiteParameter ddcParameter = updateCommand.Parameters.Add("@Ddc", DbType.String);
                    SQLiteParameter lccParameter = updateCommand.Parameters.Add("@Lcc", DbType.String);
                    SQLiteParameter doiParameter = updateCommand.Parameters.Add("@Doi", DbType.String);
                    SQLiteParameter googleBookIdParameter = updateCommand.Parameters.Add("@GoogleBookId", DbType.String);
                    SQLiteParameter openLibraryIdParameter = updateCommand.Parameters.Add("@OpenLibraryId", DbType.String);
                    SQLiteParameter commentaryParameter = updateCommand.Parameters.Add("@Commentary", DbType.String);
                    SQLiteParameter dpiParameter = updateCommand.Parameters.Add("@Dpi", DbType.Int32);
                    SQLiteParameter colorParameter = updateCommand.Parameters.Add("@Color", DbType.String);
                    SQLiteParameter cleanedParameter = updateCommand.Parameters.Add("@Cleaned", DbType.String);
                    SQLiteParameter orientationParameter = updateCommand.Parameters.Add("@Orientation", DbType.String);
                    SQLiteParameter paginatedParameter = updateCommand.Parameters.Add("@Paginated", DbType.String);
                    SQLiteParameter scannedParameter = updateCommand.Parameters.Add("@Scanned", DbType.String);
                    SQLiteParameter bookmarkedParameter = updateCommand.Parameters.Add("@Bookmarked", DbType.String);
                    SQLiteParameter searchableParameter = updateCommand.Parameters.Add("@Searchable", DbType.String);
                    SQLiteParameter sizeInBytesParameter = updateCommand.Parameters.Add("@SizeInBytes", DbType.Int64);
                    SQLiteParameter formatParameter = updateCommand.Parameters.Add("@Format", DbType.String);
                    SQLiteParameter md5HashParameter = updateCommand.Parameters.Add("@Md5Hash", DbType.String);
                    SQLiteParameter genericParameter = updateCommand.Parameters.Add("@Generic", DbType.String);
                    SQLiteParameter visibleParameter = updateCommand.Parameters.Add("@Visible", DbType.String);
                    SQLiteParameter locatorParameter = updateCommand.Parameters.Add("@Locator", DbType.String);
                    SQLiteParameter localParameter = updateCommand.Parameters.Add("@Local", DbType.Int32);
                    SQLiteParameter addedDateTimeParameter = updateCommand.Parameters.Add("@AddedDateTime", DbType.String);
                    SQLiteParameter lastModifiedDateTimeParameter = updateCommand.Parameters.Add("@LastModifiedDateTime", DbType.String);
                    SQLiteParameter coverUrlParameter = updateCommand.Parameters.Add("@CoverUrl", DbType.String);
                    SQLiteParameter tagsParameter = updateCommand.Parameters.Add("@Tags", DbType.String);
                    SQLiteParameter identifierPlainParameter = updateCommand.Parameters.Add("@IdentifierPlain", DbType.String);
                    insertFtsWithIdCommand.CommandText = SqlScripts.INSERT_NON_FICTION_FTS_WITH_ID;
                    SQLiteParameter idFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Id", DbType.Int32);
                    SQLiteParameter titleFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter seriesFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter authorsFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter publisherFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierPlainFtsParameter = insertFtsWithIdCommand.Parameters.Add("@IdentifierPlain", DbType.String);
                    foreach (NonFictionBook book in books)
                    {
                        idDeleteFtsParameter.Value = book.Id;
                        titleDeleteFtsParameter.Value = book.Title.SeparateCjkCharactersWithSpaces();
                        seriesDeleteFtsParameter.Value = book.Series.SeparateCjkCharactersWithSpaces();
                        authorsDeleteFtsParameter.Value = book.Authors.SeparateCjkCharactersWithSpaces();
                        publisherDeleteFtsParameter.Value = book.Publisher.SeparateCjkCharactersWithSpaces();
                        identifierPlainDeleteFtsParameter.Value = book.IdentifierPlain;
                        deleteFtsCommand.ExecuteNonQuery();
                        idParameter.Value = book.Id;
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
                        updateCommand.ExecuteNonQuery();
                        idFtsParameter.Value = book.Id;
                        titleFtsParameter.Value = book.Title.SeparateCjkCharactersWithSpaces();
                        seriesFtsParameter.Value = book.Series.SeparateCjkCharactersWithSpaces();
                        authorsFtsParameter.Value = book.Authors.SeparateCjkCharactersWithSpaces();
                        publisherFtsParameter.Value = book.Publisher.SeparateCjkCharactersWithSpaces();
                        identifierPlainFtsParameter.Value = book.IdentifierPlain;
                        insertFtsWithIdCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
        }

        public List<string> GetNonFictionIndexList()
        {
            return GetIndexList(SqlScripts.GET_NON_FICTION_INDEX_LIST);
        }

        public void CreateFictionTables()
        {
            ExecuteCommands(SqlScripts.CREATE_FICTION_TABLE);
            ExecuteCommands(SqlScripts.CREATE_FICTION_FTS_TABLE);
        }

        public void CreateFictionMd5HashIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_FICTION_MD5HASH_INDEX);
        }

        public void CreateFictionLastModifiedDateTimeIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_FICTION_LASTMODIFIEDDATETIME_INDEX);
        }

        public void CreateFictionLibgenIdIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_FICTION_LIBGENID_INDEX);
        }

        public BitArray GetFictionLibgenIdsBitArray()
        {
            return GetLibgenIdsBitArray(SqlScripts.GET_ALL_FICTION_LIBGEN_IDS, GetFictionMaxLibgenId());
        }

        public int CountFictionBooks()
        {
            return ExecuteIntScalarCommand(SqlScripts.COUNT_FICTION);
        }

        public FictionBook GetFictionBookById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_FICTION_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        FictionBook book = ReadFictionBook(dataReader);
                        return book;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public FictionBook GetFictionBookByMd5Hash(string md5Hash)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_FICTION_BY_MD5HASH;
                command.Parameters.AddWithValue("@Md5Hash", md5Hash);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        FictionBook book = ReadFictionBook(dataReader);
                        return book;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int? GetFictionBookIdByLibgenId(int libgenId)
        {
            return GetIdByLibgenId(SqlScripts.GET_FICTION_ID_BY_LIBGENID, libgenId);
        }

        public FictionBook GetLastModifiedFictionBook()
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_LAST_MODIFIED_FICTION;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        FictionBook book = ReadFictionBook(dataReader);
                        return book;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int GetFictionMaxLibgenId()
        {
            return ExecuteIntScalarCommand(SqlScripts.GET_FICTION_MAX_LIBGEN_ID);
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
                    SQLiteParameter md5HashParameter = insertCommand.Parameters.Add("@Md5Hash", DbType.String);
                    SQLiteParameter titleParameter = insertCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorsParameter = insertCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter seriesParameter = insertCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter editionParameter = insertCommand.Parameters.Add("@Edition", DbType.String);
                    SQLiteParameter languageParameter = insertCommand.Parameters.Add("@Language", DbType.String);
                    SQLiteParameter yearParameter = insertCommand.Parameters.Add("@Year", DbType.String);
                    SQLiteParameter publisherParameter = insertCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter pagesParameter = insertCommand.Parameters.Add("@Pages", DbType.String);
                    SQLiteParameter identifierParameter = insertCommand.Parameters.Add("@Identifier", DbType.String);
                    SQLiteParameter googleBookIdParameter = insertCommand.Parameters.Add("@GoogleBookId", DbType.String);
                    SQLiteParameter asinParameter = insertCommand.Parameters.Add("@Asin", DbType.String);
                    SQLiteParameter coverUrlParameter = insertCommand.Parameters.Add("@CoverUrl", DbType.String);
                    SQLiteParameter formatParameter = insertCommand.Parameters.Add("@Format", DbType.String);
                    SQLiteParameter sizeInBytesParameter = insertCommand.Parameters.Add("@SizeInBytes", DbType.Int64);
                    SQLiteParameter libraryParameter = insertCommand.Parameters.Add("@Library", DbType.String);
                    SQLiteParameter issueParameter = insertCommand.Parameters.Add("@Issue", DbType.String);
                    SQLiteParameter locatorParameter = insertCommand.Parameters.Add("@Locator", DbType.String);
                    SQLiteParameter commentaryParameter = insertCommand.Parameters.Add("@Commentary", DbType.String);
                    SQLiteParameter genericParameter = insertCommand.Parameters.Add("@Generic", DbType.String);
                    SQLiteParameter visibleParameter = insertCommand.Parameters.Add("@Visible", DbType.String);
                    SQLiteParameter addedDateTimeParameter = insertCommand.Parameters.Add("@AddedDateTime", DbType.String);
                    SQLiteParameter lastModifiedDateTimeParameter = insertCommand.Parameters.Add("@LastModifiedDateTime", DbType.String);
                    SQLiteParameter libgenIdParameter = insertCommand.Parameters.Add("@LibgenId", DbType.Int32);
                    insertFtsCommand.CommandText = SqlScripts.INSERT_FICTION_FTS_WITHOUT_ID;
                    SQLiteParameter titleFtsParameter = insertFtsCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorsFtsParameter = insertFtsCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter seriesFtsParameter = insertFtsCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter publisherFtsParameter = insertFtsCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierFtsParameter = insertFtsCommand.Parameters.Add("@Identifier", DbType.String);
                    foreach (FictionBook book in books)
                    {
                        md5HashParameter.Value = book.Md5Hash;
                        titleParameter.Value = book.Title;
                        authorsParameter.Value = book.Authors;
                        seriesParameter.Value = book.Series;
                        editionParameter.Value = book.Edition;
                        languageParameter.Value = book.Language;
                        yearParameter.Value = book.Year;
                        publisherParameter.Value = book.Publisher;
                        pagesParameter.Value = book.Pages;
                        identifierParameter.Value = book.Identifier;
                        googleBookIdParameter.Value = book.GoogleBookId;
                        asinParameter.Value = book.Asin;
                        coverUrlParameter.Value = book.CoverUrl;
                        formatParameter.Value = book.Format;
                        sizeInBytesParameter.Value = book.SizeInBytes;
                        libraryParameter.Value = book.Library;
                        issueParameter.Value = book.Issue;
                        locatorParameter.Value = book.Locator;
                        commentaryParameter.Value = book.Commentary;
                        genericParameter.Value = book.Generic;
                        visibleParameter.Value = book.Visible;
                        addedDateTimeParameter.Value = book.AddedDateTime?.ToString("s");
                        lastModifiedDateTimeParameter.Value = book.LastModifiedDateTime?.ToString("s");
                        libgenIdParameter.Value = book.LibgenId;
                        insertCommand.ExecuteNonQuery();
                        titleFtsParameter.Value = book.Title.SeparateCjkCharactersWithSpaces();
                        authorsFtsParameter.Value = book.Authors.SeparateCjkCharactersWithSpaces();
                        seriesFtsParameter.Value = book.Series.SeparateCjkCharactersWithSpaces();
                        publisherFtsParameter.Value = book.Publisher.SeparateCjkCharactersWithSpaces();
                        identifierFtsParameter.Value = book.Identifier;
                        insertFtsCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
        }

        public void UpdateFictionBooks(List<FictionBook> books)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                using (SQLiteCommand deleteFtsCommand = connection.CreateCommand())
                using (SQLiteCommand updateCommand = connection.CreateCommand())
                using (SQLiteCommand insertFtsWithIdCommand = connection.CreateCommand())
                {
                    deleteFtsCommand.CommandText = SqlScripts.DELETE_FICTION_FTS;
                    SQLiteParameter idDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Id", DbType.Int32);
                    SQLiteParameter titleDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorsDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter seriesDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter publisherDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierDeleteFtsParameter = deleteFtsCommand.Parameters.Add("@Identifier", DbType.String);
                    updateCommand.CommandText = SqlScripts.UPDATE_FICTION;
                    SQLiteParameter idParameter = updateCommand.Parameters.Add("@Id", DbType.Int32);
                    SQLiteParameter md5HashParameter = updateCommand.Parameters.Add("@Md5Hash", DbType.String);
                    SQLiteParameter titleParameter = updateCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorsParameter = updateCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter seriesParameter = updateCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter editionParameter = updateCommand.Parameters.Add("@Edition", DbType.String);
                    SQLiteParameter languageParameter = updateCommand.Parameters.Add("@Language", DbType.String);
                    SQLiteParameter yearParameter = updateCommand.Parameters.Add("@Year", DbType.String);
                    SQLiteParameter publisherParameter = updateCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter pagesParameter = updateCommand.Parameters.Add("@Pages", DbType.String);
                    SQLiteParameter identifierParameter = updateCommand.Parameters.Add("@Identifier", DbType.String);
                    SQLiteParameter googleBookIdParameter = updateCommand.Parameters.Add("@GoogleBookId", DbType.String);
                    SQLiteParameter asinParameter = updateCommand.Parameters.Add("@Asin", DbType.String);
                    SQLiteParameter coverUrlParameter = updateCommand.Parameters.Add("@CoverUrl", DbType.String);
                    SQLiteParameter formatParameter = updateCommand.Parameters.Add("@Format", DbType.String);
                    SQLiteParameter sizeInBytesParameter = updateCommand.Parameters.Add("@SizeInBytes", DbType.Int64);
                    SQLiteParameter libraryParameter = updateCommand.Parameters.Add("@Library", DbType.String);
                    SQLiteParameter issueParameter = updateCommand.Parameters.Add("@Issue", DbType.String);
                    SQLiteParameter locatorParameter = updateCommand.Parameters.Add("@Locator", DbType.String);
                    SQLiteParameter commentaryParameter = updateCommand.Parameters.Add("@Commentary", DbType.String);
                    SQLiteParameter genericParameter = updateCommand.Parameters.Add("@Generic", DbType.String);
                    SQLiteParameter visibleParameter = updateCommand.Parameters.Add("@Visible", DbType.String);
                    SQLiteParameter addedDateTimeParameter = updateCommand.Parameters.Add("@AddedDateTime", DbType.String);
                    SQLiteParameter lastModifiedDateTimeParameter = updateCommand.Parameters.Add("@LastModifiedDateTime", DbType.String);
                    insertFtsWithIdCommand.CommandText = SqlScripts.INSERT_FICTION_FTS_WITH_ID;
                    SQLiteParameter idFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Id", DbType.Int32);
                    SQLiteParameter titleFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter authorsFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter seriesFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter publisherFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierFtsParameter = insertFtsWithIdCommand.Parameters.Add("@Identifier", DbType.String);
                    foreach (FictionBook book in books)
                    {
                        idDeleteFtsParameter.Value = book.Id;
                        titleDeleteFtsParameter.Value = book.Title.SeparateCjkCharactersWithSpaces();
                        authorsDeleteFtsParameter.Value = book.Authors.SeparateCjkCharactersWithSpaces();
                        seriesDeleteFtsParameter.Value = book.Series.SeparateCjkCharactersWithSpaces();
                        publisherDeleteFtsParameter.Value = book.Publisher.SeparateCjkCharactersWithSpaces();
                        identifierDeleteFtsParameter.Value = book.Identifier;
                        deleteFtsCommand.ExecuteNonQuery();
                        idParameter.Value = book.Id;
                        md5HashParameter.Value = book.Md5Hash;
                        titleParameter.Value = book.Title;
                        authorsParameter.Value = book.Authors;
                        seriesParameter.Value = book.Series;
                        editionParameter.Value = book.Edition;
                        languageParameter.Value = book.Language;
                        yearParameter.Value = book.Year;
                        publisherParameter.Value = book.Publisher;
                        pagesParameter.Value = book.Pages;
                        identifierParameter.Value = book.Identifier;
                        googleBookIdParameter.Value = book.GoogleBookId;
                        asinParameter.Value = book.Asin;
                        coverUrlParameter.Value = book.CoverUrl;
                        formatParameter.Value = book.Format;
                        sizeInBytesParameter.Value = book.SizeInBytes;
                        libraryParameter.Value = book.Library;
                        issueParameter.Value = book.Issue;
                        locatorParameter.Value = book.Locator;
                        commentaryParameter.Value = book.Commentary;
                        genericParameter.Value = book.Generic;
                        visibleParameter.Value = book.Visible;
                        addedDateTimeParameter.Value = book.AddedDateTime?.ToString("s");
                        lastModifiedDateTimeParameter.Value = book.LastModifiedDateTime?.ToString("s");
                        updateCommand.ExecuteNonQuery();
                        idFtsParameter.Value = book.Id;
                        titleFtsParameter.Value = book.Title.SeparateCjkCharactersWithSpaces();
                        authorsFtsParameter.Value = book.Authors.SeparateCjkCharactersWithSpaces();
                        seriesFtsParameter.Value = book.Series.SeparateCjkCharactersWithSpaces();
                        publisherFtsParameter.Value = book.Publisher.SeparateCjkCharactersWithSpaces();
                        identifierFtsParameter.Value = book.Identifier;
                        insertFtsWithIdCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
        }

        public List<string> GetFictionIndexList()
        {
            return GetIndexList(SqlScripts.GET_FICTION_INDEX_LIST);
        }

        public void DropFictionTables()
        {
            ExecuteCommands(SqlScripts.DROP_FICTION_FTS_TABLE);
            ExecuteCommands(SqlScripts.DROP_FICTION_TABLE);
        }

        public void CreateSciMagTables()
        {
            ExecuteCommands(SqlScripts.CREATE_SCIMAG_TABLE);
            ExecuteCommands(SqlScripts.CREATE_SCIMAG_FTS_TABLE);
        }

        public void CreateSciMagMd5HashIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_SCIMAG_MD5HASH_INDEX);
        }

        public void CreateSciMagAddedDateTimeIndex()
        {
            ExecuteCommands(SqlScripts.CREATE_SCIMAG_ADDEDDATETIME_INDEX);
        }

        public BitArray GetSciMagLibgenIdsBitArray()
        {
            return GetLibgenIdsBitArray(SqlScripts.GET_ALL_SCIMAG_LIBGEN_IDS, GetSciMagMaxLibgenId());
        }

        public int CountSciMagArticles()
        {
            return ExecuteIntScalarCommand(SqlScripts.COUNT_SCIMAG);
        }

        public SciMagArticle GetSciMagArticleById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_SCIMAG_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        SciMagArticle article = ReadSciMagArticle(dataReader);
                        return article;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public SciMagArticle GetSciMagArticleByMd5Hash(string md5Hash)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_SCIMAG_BY_MD5HASH;
                command.Parameters.AddWithValue("@Md5Hash", md5Hash);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        SciMagArticle article = ReadSciMagArticle(dataReader);
                        return article;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int? GetSciMagArticleIdByLibgenId(int libgenId)
        {
            return GetIdByLibgenId(SqlScripts.GET_SCIMAG_ID_BY_LIBGENID, libgenId);
        }

        public SciMagArticle GetLastAddedSciMagArticle()
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_LAST_ADDED_SCIMAG;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        SciMagArticle article = ReadSciMagArticle(dataReader);
                        return article;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int GetSciMagMaxLibgenId()
        {
            return ExecuteIntScalarCommand(SqlScripts.GET_SCIMAG_MAX_LIBGEN_ID);
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
                    insertFtsCommand.CommandText = SqlScripts.INSERT_SCIMAG_FTS_WITHOUT_ID;
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
                        titleFtsParameter.Value = article.Title.SeparateCjkCharactersWithSpaces();
                        authorsFtsParameter.Value = article.Authors.SeparateCjkCharactersWithSpaces();
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

        public List<string> GetSciMagIndexList()
        {
            return GetIndexList(SqlScripts.GET_SCIMAG_INDEX_LIST);
        }

        public void CreateFilesTable()
        {
            ExecuteCommands(SqlScripts.CREATE_FILES_TABLE);
        }

        public void AddFileIdColumnToNonFictionTable()
        {
            ExecuteCommands(SqlScripts.ALTER_NON_FICTION_ADD_FILE_ID);
        }

        public void AddFileIdColumnToFictionTable()
        {
            ExecuteCommands(SqlScripts.ALTER_FICTION_ADD_FILE_ID);
        }

        public void AddFileIdColumnToSciMagTable()
        {
            ExecuteCommands(SqlScripts.ALTER_SCIMAG_ADD_FILE_ID);
        }

        public LibraryFile GetFileById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_FILE_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        LibraryFile file = ReadFile(dataReader);
                        return file;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void AddFiles(List<LibraryFile> files)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                using (SQLiteCommand insertCommand = connection.CreateCommand())
                using (SQLiteCommand updateNonFictionCommand = connection.CreateCommand())
                using (SQLiteCommand updateFictionCommand = connection.CreateCommand())
                using (SQLiteCommand updateSciMagCommand = connection.CreateCommand())
                using (SQLiteCommand getLastInsertedIdCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = SqlScripts.INSERT_FILE;
                    insertCommand.Parameters.AddWithValue("@Id", null);
                    SQLiteParameter filePathParameter = insertCommand.Parameters.Add("@FilePath", DbType.String);
                    SQLiteParameter archiveEntryParameter = insertCommand.Parameters.Add("@ArchiveEntry", DbType.String);
                    SQLiteParameter objectTypeParameter = insertCommand.Parameters.Add("@ObjectType", DbType.String);
                    SQLiteParameter objectIdParameter = insertCommand.Parameters.Add("@ObjectId", DbType.String);
                    updateNonFictionCommand.CommandText = SqlScripts.UPDATE_NON_FICTION_FILE_ID;
                    SQLiteParameter updateNonFictionFileIdParameter = updateNonFictionCommand.Parameters.Add("@FileId", DbType.Int32);
                    SQLiteParameter updateNonFictionIdParameter = updateNonFictionCommand.Parameters.Add("@Id", DbType.Int32);
                    updateFictionCommand.CommandText = SqlScripts.UPDATE_FICTION_FILE_ID;
                    SQLiteParameter updateFictionFileIdParameter = updateFictionCommand.Parameters.Add("@FileId", DbType.Int32);
                    SQLiteParameter updateFictionIdParameter = updateFictionCommand.Parameters.Add("@Id", DbType.Int32);
                    updateSciMagCommand.CommandText = SqlScripts.UPDATE_SCIMAG_FILE_ID;
                    SQLiteParameter updateSciMagFileIdParameter = updateSciMagCommand.Parameters.Add("@FileId", DbType.Int32);
                    SQLiteParameter updateSciMagIdParameter = updateSciMagCommand.Parameters.Add("@Id", DbType.Int32);
                    getLastInsertedIdCommand.CommandText = SqlScripts.GET_LAST_INSERTED_ID;
                    foreach (LibraryFile file in files)
                    {
                        filePathParameter.Value = file.FilePath;
                        archiveEntryParameter.Value = file.ArchiveEntry;
                        objectTypeParameter.Value = (int)file.ObjectType;
                        objectIdParameter.Value = file.ObjectId;
                        insertCommand.ExecuteNonQuery();
                        file.Id = ParseIntScalarResult(getLastInsertedIdCommand.ExecuteScalar());
                        switch (file.ObjectType)
                        {
                            case LibgenObjectType.NON_FICTION_BOOK:
                                updateNonFictionFileIdParameter.Value = file.Id;
                                updateNonFictionIdParameter.Value = file.ObjectId;
                                updateNonFictionCommand.ExecuteNonQuery();
                                break;
                            case LibgenObjectType.FICTION_BOOK:
                                updateFictionFileIdParameter.Value = file.Id;
                                updateFictionIdParameter.Value = file.ObjectId;
                                updateFictionCommand.ExecuteNonQuery();
                                break;
                            case LibgenObjectType.SCIMAG_ARTICLE:
                                updateSciMagFileIdParameter.Value = file.Id;
                                updateSciMagIdParameter.Value = file.ObjectId;
                                updateSciMagCommand.ExecuteNonQuery();
                                break;
                        }
                    }
                }
                transaction.Commit();
            }
        }

        public string RunCustomSqlQuery(string sqlQuery)
        {
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("[");
            List<string> fieldNames = null;
            bool firstLine = true;
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = sqlQuery;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (fieldNames == null)
                        {
                            fieldNames = new List<string>();
                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                fieldNames.Add(dataReader.GetName(i));
                            }
                        }
                        if (firstLine)
                        {
                            firstLine = false;
                        }
                        else
                        {
                            resultBuilder.Append(",");
                        }
                        resultBuilder.AppendLine();
                        resultBuilder.Append("  { ");
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            if (i > 0)
                            {
                                resultBuilder.Append(", ");
                            }
                            resultBuilder.Append(fieldNames[i]);
                            resultBuilder.Append(" = ");
                            object fieldValue = dataReader.GetValue(i);
                            if (fieldValue == DBNull.Value)
                            {
                                resultBuilder.Append("(null)");
                            }
                            else if (fieldValue is string)
                            {
                                resultBuilder.Append("\"");
                                resultBuilder.Append(fieldValue);
                                resultBuilder.Append("\"");
                            }
                            else
                            {
                                resultBuilder.Append(fieldValue.ToString());
                            }
                        }
                        resultBuilder.Append(" }");
                    }
                }
            }
            if (fieldNames != null)
            {
                resultBuilder.AppendLine();
            }
            resultBuilder.Append("]");
            return resultBuilder.ToString();
        }

        private static string EscapeSearchQuery(string originalSearchQuery)
        {
            return SearchQueryParser.GetEscapedQuery(originalSearchQuery.SeparateCjkCharactersWithSpaces());
        }

        private static string GetSearchCommandWithLimit(string searchCommand, int? resultLimit)
        {
            return searchCommand + (resultLimit.HasValue ? " LIMIT " + resultLimit.Value : String.Empty);
        }

        private static int ParseIntScalarResult(object objectResult)
        {
            return objectResult != DBNull.Value ? (int)(long)objectResult : 0;
        }

        private static int? ParseNullableIntScalarResult(object objectResult)
        {
            return objectResult != DBNull.Value ? (int?)(long)objectResult : null;
        }

        private static string ParseNullableDbString(object input)
        {
            return input != DBNull.Value ? input.ToString() : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime ParseDbDate(string input)
        {
            return DateTime.ParseExact(input, "s", CultureInfo.InvariantCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime? ParseNullableDbDate(object input)
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

        [SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NonFictionBook ReadNonFictionBook(SQLiteDataReader dataReader)
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
            book.FileId = ParseNullableIntScalarResult(dataReader.GetValue(48));
            return book;
        }

        [SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FictionBook ReadFictionBook(SQLiteDataReader dataReader)
        {
            FictionBook book = new FictionBook();
            book.Id = dataReader.GetInt32(0);
            book.Md5Hash = dataReader.GetString(1);
            book.Title = dataReader.GetString(2);
            book.Authors = dataReader.GetString(3);
            book.Series = dataReader.GetString(4);
            book.Edition = dataReader.GetString(5);
            book.Language = dataReader.GetString(6);
            book.Year = dataReader.GetString(7);
            book.Publisher = dataReader.GetString(8);
            book.Pages = dataReader.GetString(9);
            book.Identifier = dataReader.GetString(10);
            book.GoogleBookId = dataReader.GetString(11);
            book.Asin = dataReader.GetString(12);
            book.CoverUrl = dataReader.GetString(13);
            book.Format = dataReader.GetString(14);
            book.SizeInBytes = dataReader.GetInt64(15);
            book.Library = dataReader.GetString(16);
            book.Issue = dataReader.GetString(17);
            book.Locator = dataReader.GetString(18);
            book.Commentary = dataReader.GetString(19);
            book.Generic = dataReader.GetString(20);
            book.Visible = dataReader.GetString(21);
            book.AddedDateTime = ParseNullableDbDate(dataReader.GetValue(22));
            book.LastModifiedDateTime = ParseNullableDbDate(dataReader.GetValue(23));
            book.LibgenId = dataReader.GetInt32(24);
            book.FileId = ParseNullableIntScalarResult(dataReader.GetValue(25));
            return book;
        }

        [SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SciMagArticle ReadSciMagArticle(SQLiteDataReader dataReader)
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
            article.FileId = ParseNullableIntScalarResult(dataReader.GetValue(32));
            return article;
        }

        [SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LibraryFile ReadFile(SQLiteDataReader dataReader)
        {
            LibraryFile file = new LibraryFile();
            file.Id = dataReader.GetInt32(0);
            file.FilePath = dataReader.GetString(1);
            file.ArchiveEntry = ParseNullableDbString(dataReader.GetValue(2));
            file.ObjectType = (LibgenObjectType)dataReader.GetInt32(3);
            file.ObjectId = dataReader.GetInt32(4);
            return file;
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

        private int ExecuteIntScalarCommand(string commandText)
        {
            return ParseIntScalarResult(ExecuteScalarCommand(commandText));
        }

        private object ExecuteScalarCommand(string commandText)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteScalar();
            }
        }

        private List<string> GetIndexList(string getIndexListQuery)
        {
            List<string> result = new List<string>();
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = getIndexListQuery;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(dataReader["name"].ToString());
                    }
                }
            }
            return result;
        }

        private int? GetIdByLibgenId(string query, int libgenId)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.AddWithValue("@LibgenId", libgenId);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        return dataReader.GetInt32(0);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private BitArray GetLibgenIdsBitArray(string query, int maxLibgenId)
        {
            BitArray result = new BitArray(maxLibgenId + 1);
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result[dataReader.GetInt32(0)] = true;
                    }
                }
            }
            return result;
        }
    }
}
