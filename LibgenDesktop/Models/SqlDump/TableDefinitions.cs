using System.Collections.Generic;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.SqlDump
{
    internal static class TableDefinitions
    {
        internal class NonFictionTableDefinition : TableDefinition<NonFictionBook>
        {
            public NonFictionTableDefinition()
                : base("updated", TableType.NON_FICTION)
            {
                AddColumn("ID", ColumnType.INT, (book, value) => book.LibgenId = ParseInt(value));
                AddColumn("Title", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Title = value);
                AddColumn("VolumeInfo", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.VolumeInfo = value);
                AddColumn("Series", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Series = value);
                AddColumn("Periodical", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Periodical = value);
                AddColumn("Author", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Authors = value);
                AddColumn("Year", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Year = value);
                AddColumn("Edition", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Edition = value);
                AddColumn("Publisher", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Publisher = value);
                AddColumn("City", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.City = value);
                AddColumn("Pages", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Pages = value);
                AddColumn("PagesInFile", ColumnType.INT, (book, value) => book.PagesInFile = ParseInt(value));
                AddColumn("Language", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Language = value);
                AddColumn("Topic", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Topic = value);
                AddColumn("Library", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Library = value);
                AddColumn("Issue", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Issue = value);
                AddColumn("Identifier", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Identifier = value);
                AddColumn("ISSN", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Issn = value);
                AddColumn("ASIN", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Asin = value);
                AddColumn("UDC", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Udc = value);
                AddColumn("LBC", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Lbc = value);
                AddColumn("DDC", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Ddc = value);
                AddColumn("LCC", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Lcc = value);
                AddColumn("Doi", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Doi = value);
                AddColumn("Googlebookid", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.GoogleBookId = value);
                AddColumn("OpenLibraryID", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.OpenLibraryId = value);
                AddColumn("Commentary", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Commentary = value);
                AddColumn("DPI", ColumnType.INT, (book, value) => book.Dpi = ParseInt(value));
                AddColumn("Color", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Color = value);
                AddColumn("Cleaned", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Cleaned = value);
                AddColumn("Orientation", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Orientation = value);
                AddColumn("Paginated", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Paginated = value);
                AddColumn("Scanned", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Scanned = value);
                AddColumn("Bookmarked", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Bookmarked = value);
                AddColumn("Searchable", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Searchable = value);
                AddColumn("Filesize", ColumnType.BIGINT, (book, value) => book.SizeInBytes = ParseLong(value));
                AddColumn("Extension", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Format = value);
                AddColumn("MD5", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Md5Hash = value);
                AddColumn("Generic", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Generic = value);
                AddColumn("Visible", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Visible = value);
                AddColumn("Locator", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Locator = value);
                AddColumn("Local", ColumnType.INT, (book, value) => book.Local = ParseInt(value));
                AddColumn("TimeAdded", ColumnType.TIMESTAMP, (book, value) => book.AddedDateTime = ParseDateTime(value));
                AddColumn("TimeLastModified", ColumnType.TIMESTAMP, (book, value) => book.LastModifiedDateTime = ParseDateTime(value));
                AddColumn("Coverurl", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.CoverUrl = value);
                AddColumn("Tags", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Tags = value);
                AddColumn("IdentifierWODash", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.IdentifierPlain = value);
            }
        }

        internal class FictionTableDefinition : TableDefinition<FictionBook>
        {
            public FictionTableDefinition()
                : base("main", TableType.FICTION)
            {
                AddColumn("ID", ColumnType.INT, (book, value) => book.LibgenId = ParseInt(value));
                AddColumn("AuthorFamily1", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorFamily1 = value);
                AddColumn("AuthorName1", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorName1 = value);
                AddColumn("AuthorSurname1", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorSurname1 = value);
                AddColumn("Role1", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Role1 = value);
                AddColumn("Pseudonim1", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Pseudonim1 = value);
                AddColumn("AuthorFamily2", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorFamily2 = value);
                AddColumn("AuthorName2", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorName2 = value);
                AddColumn("AuthorSurname2", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorSurname2 = value);
                AddColumn("Role2", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Role2 = value);
                AddColumn("Pseudonim2", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Pseudonim2 = value);
                AddColumn("AuthorFamily3", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorFamily3 = value);
                AddColumn("AuthorName3", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorName3 = value);
                AddColumn("AuthorSurname3", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorSurname3 = value);
                AddColumn("Role3", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Role3 = value);
                AddColumn("Pseudonim3", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Pseudonim3 = value);
                AddColumn("AuthorFamily4", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorFamily4 = value);
                AddColumn("AuthorName4", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorName4 = value);
                AddColumn("AuthorSurname4", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorSurname4 = value);
                AddColumn("Role4", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Role4 = value);
                AddColumn("Pseudonim4", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Pseudonim4 = value);
                AddColumn("Series1", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Series1 = value);
                AddColumn("Series2", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Series2 = value);
                AddColumn("Series3", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Series3 = value);
                AddColumn("Series4", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Series4 = value);
                AddColumn("Title", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Title = value);
                AddColumn("Extension", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Format = value);
                AddColumn("Version", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Version = value);
                AddColumn("Filesize", ColumnType.INT, (book, value) => book.SizeInBytes = ParseLong(value));
                AddColumn("MD5", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Md5Hash = value);
                AddColumn("Path", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Path = value);
                AddColumn("Language", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Language = value);
                AddColumn("Pages", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Pages = value);
                AddColumn("Identifier", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Identifier = value);
                AddColumn("Year", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Year = value);
                AddColumn("Publisher", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Publisher = value);
                AddColumn("Edition", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Edition = value);
                AddColumn("Commentary", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Commentary = value);
                AddColumn("TimeAdded", ColumnType.TIMESTAMP, (book, value) => book.AddedDateTime = ParseNullableDateTime(value));
                AddColumn("TimeLastModified", ColumnType.TIMESTAMP, (book, value) => book.LastModifiedDateTime = ParseDateTime(value));
                AddColumn("RussianAuthorFamily", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.RussianAuthorFamily = value);
                AddColumn("RussianAuthorName", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.RussianAuthorName = value);
                AddColumn("RussianAuthorSurname", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.RussianAuthorSurname = value);
                AddColumn("Cover", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Cover = value);
                AddColumn("GooglebookID", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.GoogleBookId = value);
                AddColumn("ASIN", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Asin = value);
                AddColumn("AuthorHash", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.AuthorHash = value);
                AddColumn("TitleHash", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.TitleHash = value);
                AddColumn("Visible", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Visible = value);
            }
        }

        internal class SciMagTableDefinition : TableDefinition<SciMagArticle>
        {
            public SciMagTableDefinition()
                : base("scimag", TableType.SCI_MAG)
            {
                AddColumn("ID", ColumnType.INT, (article, value) => article.LibgenId = ParseInt(value));
                AddColumn("DOI", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Doi = value);
                AddColumn("DOI2", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Doi2 = value);
                AddColumn("Title", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Title = value);
                AddColumn("Author", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Authors = value);
                AddColumn("Year", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Year = value);
                AddColumn("Month", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Month = value);
                AddColumn("Day", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Day = value);
                AddColumn("Volume", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Volume = value);
                AddColumn("Issue", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Issue = value);
                AddColumn("First_page", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.FirstPage = value);
                AddColumn("Last_page", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.LastPage = value);
                AddColumn("Journal", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Journal = value);
                AddColumn("ISBN", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Isbn = value);
                AddColumn("ISSNP", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Issnp = value);
                AddColumn("ISSNE", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Issne = value);
                AddColumn("MD5", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Md5Hash = value);
                AddColumn("Filesize", ColumnType.INT, (article, value) => article.SizeInBytes = ParseLong(value));
                AddColumn("TimeAdded", ColumnType.TIMESTAMP, (article, value) => article.AddedDateTime = ParseNullableDateTime(value));
                AddColumn("JOURNALID", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.JournalId = value);
                AddColumn("AbstractURL", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.AbstractUrl = value);
                AddColumn("Attribute1", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Attribute1 = value);
                AddColumn("Attribute2", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Attribute2 = value);
                AddColumn("Attribute3", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Attribute3 = value);
                AddColumn("Attribute4", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Attribute4 = value);
                AddColumn("Attribute5", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Attribute5 = value);
                AddColumn("Attribute6", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Attribute6 = value);
                AddColumn("visible", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Visible = value);
                AddColumn("PubmedID", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.PubmedId = value);
                AddColumn("PMC", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Pmc = value);
                AddColumn("PII", ColumnType.CHAR_OR_VARCHAR, (article, value) => article.Pii = value);
            }
        }

        static TableDefinitions()
        {
            NonFiction = new NonFictionTableDefinition();
            Fiction = new FictionTableDefinition();
            SciMag = new SciMagTableDefinition();
            AllTables = new Dictionary<string, TableDefinition>
            {
                { NonFiction.TableName, NonFiction },
                { Fiction.TableName, Fiction },
                { SciMag.TableName, SciMag }
            };
        }

        public static Dictionary<string, TableDefinition> AllTables { get; }
        public static TableDefinition<NonFictionBook> NonFiction { get; }
        public static TableDefinition<FictionBook> Fiction { get; }
        public static TableDefinition<SciMagArticle> SciMag { get; }
    }
}
