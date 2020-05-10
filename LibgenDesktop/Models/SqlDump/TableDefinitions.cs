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
                : base("fiction", TableType.FICTION)
            {
                AddColumn("ID", ColumnType.INT, (book, value) => book.LibgenId = ParseInt(value));
                AddColumn("MD5", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Md5Hash = value);
                AddColumn("Title", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Title = value);
                AddColumn("Author", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Authors = value);
                AddColumn("Series", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Series = value);
                AddColumn("Edition", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Edition = value);
                AddColumn("Language", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Language = value);
                AddColumn("Year", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Year = value);
                AddColumn("Publisher", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Publisher = value);
                AddColumn("Pages", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Pages = value);
                AddColumn("Identifier", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Identifier = value);
                AddColumn("GooglebookID", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.GoogleBookId = value);
                AddColumn("ASIN", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Asin = value);
                AddColumn("Coverurl", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.CoverUrl = value);
                AddColumn("Extension", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Format = value);
                AddColumn("Filesize", ColumnType.INT, (book, value) => book.SizeInBytes = ParseInt(value));
                AddColumn("Library", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Library = value);
                AddColumn("Issue", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Issue = value);
                AddColumn("Locator", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Locator = value);
                AddColumn("Commentary", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Commentary = value);
                AddColumn("Generic", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Generic = value);
                AddColumn("Visible", ColumnType.CHAR_OR_VARCHAR, (book, value) => book.Visible = value);
                AddColumn("TimeAdded", ColumnType.TIMESTAMP, (book, value) => book.AddedDateTime = ParseNullableDateTime(value));
                AddColumn("TimeLastModified", ColumnType.TIMESTAMP, (book, value) => book.LastModifiedDateTime = ParseNullableDateTime(value));
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
