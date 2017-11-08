using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace LibgenDesktop.Database
{
    internal class LocalDatabase
    {
        public const int INSERT_TRANSACTION_BATCH = 500;
        private const string DEFAULT_DATABASE_FILE_NAME = "libgen.db";

        private readonly SQLiteConnection connection;

        public LocalDatabase()
        {
            string filePath = DEFAULT_DATABASE_FILE_NAME;
            if (!File.Exists(filePath))
            {
                SQLiteConnection.CreateFile(filePath);
            }
            connection = new SQLiteConnection("Data Source = " + filePath);
            connection.Open();
            connection.EnableExtensions(true);
            connection.LoadExtension("SQLite.Interop.dll", "sqlite3_fts5_init");
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.CREATE_BOOKS_TABLE;
                command.ExecuteNonQuery();
            }
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.CREATE_BOOKS_FTS_TABLE;
                command.ExecuteNonQuery();
            }
            ExecuteCommands("PRAGMA LOCKING_MODE = EXCLUSIVE");
        }

        public int CountBooks()
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.COUNT_BOOKS;
                object objectResult = command.ExecuteScalar();
                return objectResult != DBNull.Value ? (int)(long)objectResult : 0;
            }
        }

        public IEnumerable<Book> GetAllBooks()
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_ALL_BOOKS;
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Book book = new Book();
                        book.Id = dataReader.GetInt32(0);
                        book.Title = dataReader.GetString(1);
                        book.Authors = dataReader.GetString(2);
                        book.Series = dataReader.GetString(3);
                        book.Year = dataReader.GetString(4);
                        book.Publisher = dataReader.GetString(5);
                        book.Format = dataReader.GetString(6);
                        book.SizeInBytes = dataReader.GetInt64(7);
                        yield return book;
                    }
                }
            }
        }

        public Book GetBookById(int id)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.GET_BOOK_BY_ID;
                command.Parameters.AddWithValue("@Id", id);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();
                    Book book = new Book();
                    book.ExtendedProperties = new Book.BookExtendedProperties();
                    book.Id = dataReader.GetInt32(0);
                    book.Title = dataReader.GetString(1);
                    book.ExtendedProperties.VolumeInfo = dataReader.GetString(2);
                    book.Series = dataReader.GetString(3);
                    book.ExtendedProperties.Periodical = dataReader.GetString(4);
                    book.Authors = dataReader.GetString(5);
                    book.Year = dataReader.GetString(6);
                    book.ExtendedProperties.Edition = dataReader.GetString(7);
                    book.Publisher = dataReader.GetString(8);
                    book.ExtendedProperties.City = dataReader.GetString(9);
                    book.ExtendedProperties.Pages = dataReader.GetString(10);
                    book.ExtendedProperties.PagesInFile = dataReader.GetInt32(11);
                    book.ExtendedProperties.Language = dataReader.GetString(12);
                    book.ExtendedProperties.Topic = dataReader.GetString(13);
                    book.ExtendedProperties.Library = dataReader.GetString(14);
                    book.ExtendedProperties.Issue = dataReader.GetString(15);
                    book.ExtendedProperties.Identifier = dataReader.GetString(16);
                    book.ExtendedProperties.Issn = dataReader.GetString(17);
                    book.ExtendedProperties.Asin = dataReader.GetString(18);
                    book.ExtendedProperties.Udc = dataReader.GetString(19);
                    book.ExtendedProperties.Lbc = dataReader.GetString(20);
                    book.ExtendedProperties.Ddc = dataReader.GetString(21);
                    book.ExtendedProperties.Lcc = dataReader.GetString(22);
                    book.ExtendedProperties.Doi = dataReader.GetString(23);
                    book.ExtendedProperties.GoogleBookid = dataReader.GetString(24);
                    book.ExtendedProperties.OpenLibraryId = dataReader.GetString(25);
                    book.ExtendedProperties.Commentary = dataReader.GetString(26);
                    book.ExtendedProperties.Dpi = dataReader.GetInt32(27);
                    book.ExtendedProperties.Color = dataReader.GetString(28);
                    book.ExtendedProperties.Cleaned = dataReader.GetString(29);
                    book.ExtendedProperties.Orientation = dataReader.GetString(30);
                    book.ExtendedProperties.Paginated = dataReader.GetString(31);
                    book.ExtendedProperties.Scanned = dataReader.GetString(32);
                    book.ExtendedProperties.Bookmarked = dataReader.GetString(33);
                    book.ExtendedProperties.Searchable = dataReader.GetString(34);
                    book.SizeInBytes = dataReader.GetInt64(35);
                    book.Format = dataReader.GetString(36);
                    book.ExtendedProperties.Md5Hash = dataReader.GetString(37);
                    book.ExtendedProperties.Generic = dataReader.GetString(38);
                    book.ExtendedProperties.Visible = dataReader.GetString(39);
                    book.ExtendedProperties.Locator = dataReader.GetString(40);
                    book.ExtendedProperties.Local = dataReader.GetInt32(41);
                    book.ExtendedProperties.AddedDateTime = DateTime.ParseExact(dataReader.GetString(42), "s", CultureInfo.InvariantCulture);
                    book.ExtendedProperties.LastModifiedDateTime = DateTime.ParseExact(dataReader.GetString(43), "s", CultureInfo.InvariantCulture);
                    book.ExtendedProperties.CoverUrl = dataReader.GetString(44);
                    book.ExtendedProperties.Tags = dataReader.GetString(45);
                    book.ExtendedProperties.IdentifierPlain = dataReader.GetString(46);
                    book.ExtendedProperties.LibgenId = dataReader.GetInt32(47);
                    return book;
                }
            }
        }

        public IEnumerable<Book> SearchBooks(string searchQuery)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = SqlScripts.SEARCH_BOOKS;
                command.Parameters.AddWithValue("@SearchQuery", searchQuery);
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Book book = new Book();
                        book.Id = dataReader.GetInt32(0);
                        book.Title = dataReader.GetString(1);
                        book.Authors = dataReader.GetString(2);
                        book.Series = dataReader.GetString(3);
                        book.Year = dataReader.GetString(4);
                        book.Publisher = dataReader.GetString(5);
                        book.Format = dataReader.GetString(6);
                        book.SizeInBytes = dataReader.GetInt64(7);
                        yield return book;
                    }
                }
            }
        }

        public void AddBooks(List<Book> books)
        {
            ExecuteCommands("PRAGMA TEMP_STORE = MEMORY", "PRAGMA JOURNAL_MODE = OFF",
                "PRAGMA SYNCHRONOUS = OFF");
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                using (SQLiteCommand insertCommand = connection.CreateCommand())
                using (SQLiteCommand insertFtsCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = SqlScripts.INSERT_BOOK;
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
                    SQLiteParameter googleBookidParameter = insertCommand.Parameters.Add("@GoogleBookid", DbType.String);
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
                    insertFtsCommand.CommandText = SqlScripts.INSERT_BOOK_FTS;
                    SQLiteParameter titleFtsParameter = insertFtsCommand.Parameters.Add("@Title", DbType.String);
                    SQLiteParameter seriesFtsParameter = insertFtsCommand.Parameters.Add("@Series", DbType.String);
                    SQLiteParameter authorsFtsParameter = insertFtsCommand.Parameters.Add("@Authors", DbType.String);
                    SQLiteParameter publisherFtsParameter = insertFtsCommand.Parameters.Add("@Publisher", DbType.String);
                    SQLiteParameter identifierPlainFtsParameter = insertFtsCommand.Parameters.Add("@IdentifierPlain", DbType.String);
                    foreach (Book book in books)
                    {
                        titleParameter.Value = book.Title;
                        volumeInfoParameter.Value = book.ExtendedProperties.VolumeInfo;
                        seriesParameter.Value = book.Series;
                        periodicalParameter.Value = book.ExtendedProperties.Periodical;
                        authorsParameter.Value = book.Authors;
                        yearParameter.Value = book.Year;
                        editionParameter.Value = book.ExtendedProperties.Edition;
                        publisherParameter.Value = book.Publisher;
                        cityParameter.Value = book.ExtendedProperties.City;
                        pagesParameter.Value = book.ExtendedProperties.Pages;
                        pagesInFileParameter.Value = book.ExtendedProperties.PagesInFile;
                        languageParameter.Value = book.ExtendedProperties.Language;
                        topicParameter.Value = book.ExtendedProperties.Topic;
                        libraryParameter.Value = book.ExtendedProperties.Library;
                        issueParameter.Value = book.ExtendedProperties.Issue;
                        identifierParameter.Value = book.ExtendedProperties.Identifier;
                        issnParameter.Value = book.ExtendedProperties.Issn;
                        asinParameter.Value = book.ExtendedProperties.Asin;
                        udcParameter.Value = book.ExtendedProperties.Udc;
                        lbcParameter.Value = book.ExtendedProperties.Lbc;
                        ddcParameter.Value = book.ExtendedProperties.Ddc;
                        lccParameter.Value = book.ExtendedProperties.Lcc;
                        doiParameter.Value = book.ExtendedProperties.Doi;
                        googleBookidParameter.Value = book.ExtendedProperties.GoogleBookid;
                        openLibraryIdParameter.Value = book.ExtendedProperties.OpenLibraryId;
                        commentaryParameter.Value = book.ExtendedProperties.Commentary;
                        dpiParameter.Value = book.ExtendedProperties.Dpi;
                        colorParameter.Value = book.ExtendedProperties.Color;
                        cleanedParameter.Value = book.ExtendedProperties.Cleaned;
                        orientationParameter.Value = book.ExtendedProperties.Orientation;
                        paginatedParameter.Value = book.ExtendedProperties.Paginated;
                        scannedParameter.Value = book.ExtendedProperties.Scanned;
                        bookmarkedParameter.Value = book.ExtendedProperties.Bookmarked;
                        searchableParameter.Value = book.ExtendedProperties.Searchable;
                        sizeInBytesParameter.Value = book.SizeInBytes;
                        formatParameter.Value = book.Format;
                        md5HashParameter.Value = book.ExtendedProperties.Md5Hash;
                        genericParameter.Value = book.ExtendedProperties.Generic;
                        visibleParameter.Value = book.ExtendedProperties.Visible;
                        locatorParameter.Value = book.ExtendedProperties.Locator;
                        localParameter.Value = book.ExtendedProperties.Local;
                        addedDateTimeParameter.Value = book.ExtendedProperties.AddedDateTime.ToString("s");
                        lastModifiedDateTimeParameter.Value = book.ExtendedProperties.LastModifiedDateTime.ToString("s");
                        coverUrlParameter.Value = book.ExtendedProperties.CoverUrl;
                        tagsParameter.Value = book.ExtendedProperties.Tags;
                        identifierPlainParameter.Value = book.ExtendedProperties.IdentifierPlain;
                        libgenIdParameter.Value = book.ExtendedProperties.LibgenId;
                        insertCommand.ExecuteNonQuery();
                        titleFtsParameter.Value = book.Title;
                        seriesFtsParameter.Value = book.Series;
                        authorsFtsParameter.Value = book.Authors;
                        publisherFtsParameter.Value = book.Publisher;
                        identifierPlainFtsParameter.Value = book.ExtendedProperties.IdentifierPlain;
                        insertFtsCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            ExecuteCommands("PRAGMA TEMP_STORE = DEFAULT", "PRAGMA JOURNAL_MODE = DELETE",
                "PRAGMA SYNCHRONOUS = FULL");
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
    }
}
