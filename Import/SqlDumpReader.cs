using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LibgenDesktop.Database;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;

namespace LibgenDesktop.Import
{
    internal class SqlDumpReader : IDisposable
    {
        public class ReadRowsProgressEventArgs : EventArgs
        {
            public long CurrentPosition { get; set; }
            public long TotalLength { get; set; }
            public int RowsParsed { get; set; }
        }

        private readonly long fileSize;
        private readonly StreamReader streamReader;
        private readonly ZipArchive zipArchive;
        private readonly RarArchive rarArchive;

        private bool disposed = false;
        private long currentFilePosition;

        public SqlDumpReader(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension.ToLower())
            {
                case ".zip":
                    zipArchive = ZipArchive.Open(filePath);
                    ZipArchiveEntry firstZipArchiveEntry = zipArchive.Entries.First();
                    fileSize = firstZipArchiveEntry.Size;
                    streamReader = new StreamReader(firstZipArchiveEntry.OpenEntryStream());
                    break;
                case ".rar":
                    rarArchive = RarArchive.Open(filePath);
                    RarArchiveEntry firstRarArchiveEntry = rarArchive.Entries.First();
                    fileSize = firstRarArchiveEntry.Size;
                    streamReader = new StreamReader(firstRarArchiveEntry.OpenEntryStream());
                    break;
                default:
                    fileSize = new FileInfo(filePath).Length;
                    streamReader = new StreamReader(filePath);
                    break;
            }
        }

        public event EventHandler<ReadRowsProgressEventArgs> ReadRowsProgress;

        public IEnumerable<Book> ReadRows()
        {
            currentFilePosition = 0;
            int rowsParsed = 0;
            bool updateTableParsed = false;
            RaiseReadRowsProgressEvent(0);
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                currentFilePosition += line.Length + 1;
                if (line.StartsWith("CREATE TABLE"))
                {
                    string tableName = ParseTableName(line);
                    if (tableName == "updated")
                    {
                        while (!streamReader.EndOfStream)
                        {
                            line = streamReader.ReadLine();
                            currentFilePosition += line.Length + 1;
                            if (line.StartsWith("CREATE TABLE"))
                            {
                                break;
                            }
                            if (line.StartsWith("INSERT INTO `updated`"))
                            {
                                if (line.StartsWith("INSERT INTO `updated` (`ID`, `Title`, `VolumeInfo`, `Series`, `Periodical`, `Author`, `Year`, `Edition`, `Publisher`, `City`, `Pages`, `PagesInFile`, `Language`, `Topic`, `Library`, `Issue`, `Identifier`, `ISSN`, `ASIN`, `UDC`, `LBC`, `DDC`, `LCC`, `Doi`, `Googlebookid`, `OpenLibraryID`, `Commentary`, `DPI`, `Color`, `Cleaned`, `Orientation`, `Paginated`, `Scanned`, `Bookmarked`, `Searchable`, `Filesize`, `Extension`, `MD5`, `Generic`, `Visible`, `Locator`, `Local`, `TimeAdded`, `TimeLastModified`, `Coverurl`, `Tags`, `IdentifierWODash`) VALUES ("))
                                {
                                    int position = line.IndexOf("VALUES (") + 8;
                                    bool endOfBatchFound = false;
                                    while (!endOfBatchFound)
                                    {
                                        rowsParsed++;
                                        Book book = new Book();
                                        book.Id = rowsParsed;
                                        book.ExtendedProperties = new Book.BookExtendedProperties();
                                        book.ExtendedProperties.LibgenId = ParseInt32(line, ref position);
                                        book.Title = ParseString(line, ref position);
                                        book.ExtendedProperties.VolumeInfo = ParseString(line, ref position);
                                        book.Series = ParseString(line, ref position);
                                        book.ExtendedProperties.Periodical = ParseString(line, ref position);
                                        book.Authors = ParseString(line, ref position);
                                        book.Year = ParseString(line, ref position);
                                        book.ExtendedProperties.Edition = ParseString(line, ref position);
                                        book.Publisher = ParseString(line, ref position);
                                        book.ExtendedProperties.City = ParseString(line, ref position);
                                        book.ExtendedProperties.Pages = ParseString(line, ref position);
                                        book.ExtendedProperties.PagesInFile = ParseInt32(line, ref position);
                                        book.ExtendedProperties.Language = ParseString(line, ref position);
                                        book.ExtendedProperties.Topic = ParseString(line, ref position);
                                        book.ExtendedProperties.Library = ParseString(line, ref position);
                                        book.ExtendedProperties.Issue = ParseString(line, ref position);
                                        book.ExtendedProperties.Identifier = ParseString(line, ref position);
                                        book.ExtendedProperties.Issn = ParseString(line, ref position);
                                        book.ExtendedProperties.Asin = ParseString(line, ref position);
                                        book.ExtendedProperties.Udc = ParseString(line, ref position);
                                        book.ExtendedProperties.Lbc = ParseString(line, ref position);
                                        book.ExtendedProperties.Ddc = ParseString(line, ref position);
                                        book.ExtendedProperties.Lcc = ParseString(line, ref position);
                                        book.ExtendedProperties.Doi = ParseString(line, ref position);
                                        book.ExtendedProperties.GoogleBookid = ParseString(line, ref position);
                                        book.ExtendedProperties.OpenLibraryId = ParseString(line, ref position);
                                        book.ExtendedProperties.Commentary = ParseString(line, ref position);
                                        book.ExtendedProperties.Dpi = ParseInt32(line, ref position);
                                        book.ExtendedProperties.Color = ParseString(line, ref position);
                                        book.ExtendedProperties.Cleaned = ParseString(line, ref position);
                                        book.ExtendedProperties.Orientation = ParseString(line, ref position);
                                        book.ExtendedProperties.Paginated = ParseString(line, ref position);
                                        book.ExtendedProperties.Scanned = ParseString(line, ref position);
                                        book.ExtendedProperties.Bookmarked = ParseString(line, ref position);
                                        book.Searchable = ParseString(line, ref position);
                                        book.SizeInBytes = ParseInt64(line, ref position);
                                        book.Format = ParseString(line, ref position);
                                        book.ExtendedProperties.Md5Hash = ParseString(line, ref position);
                                        book.ExtendedProperties.Generic = ParseString(line, ref position);
                                        book.ExtendedProperties.Visible = ParseString(line, ref position);
                                        book.ExtendedProperties.Locator = ParseString(line, ref position);
                                        book.ExtendedProperties.Local = ParseInt32(line, ref position);
                                        book.ExtendedProperties.AddedDateTime = ParseDateTime(line, ref position);
                                        book.ExtendedProperties.LastModifiedDateTime = ParseDateTime(line, ref position);
                                        book.ExtendedProperties.CoverUrl = ParseString(line, ref position);
                                        book.ExtendedProperties.Tags = ParseString(line, ref position);
                                        book.ExtendedProperties.IdentifierPlain = ParseString(line, ref position);
                                        yield return book;
                                        if (line[position] == ';')
                                        {
                                            endOfBatchFound = true;
                                        }
                                        else
                                        {
                                            position += 2;
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception($"Unexpected set of columns in the line:\r\n{line}");
                                }
                            }
                            RaiseReadRowsProgressEvent(rowsParsed);
                        }
                        updateTableParsed = true;
                    }
                }
                else
                {
                    RaiseReadRowsProgressEvent(rowsParsed);
                }
            }
            if (!updateTableParsed)
            {
                throw new Exception("Couldn't find \"updated\" table rows in the dump file.");
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (zipArchive != null)
                    {
                        zipArchive.Dispose();
                    }
                    if (rarArchive != null)
                    {
                        rarArchive.Dispose();
                    }
                    streamReader.Dispose();
                }
                disposed = true;
            }
        }

        protected virtual void RaiseReadRowsProgressEvent(int rowsParsed)
        {
            ReadRowsProgress?.Invoke(this, new ReadRowsProgressEventArgs
            {
                CurrentPosition = streamReader.BaseStream.Position,
                TotalLength = fileSize,
                RowsParsed = rowsParsed
            });
        }

        private string ParseTableName(string line)
        {
            int position = 0;
            int tableNameStartPosition = 0;
            while (position < line.Length)
            {
                if (line[position] == '`')
                {
                    if (tableNameStartPosition == 0)
                    {
                        tableNameStartPosition = position + 1;
                    }
                    else
                    {
                        return line.Substring(tableNameStartPosition, position - tableNameStartPosition);
                    }
                }
                position++;
            }
            throw new Exception($"Couldn't parse table name from the line:\r\n{line}");
        }

        private int ParseInt32(string line, ref int position)
        {
            int startPosition = position;
            while (Char.IsDigit(line[position]))
            {
                position++;
            }
            int result = Int32.Parse(line.Substring(startPosition, position - startPosition));
            position++;
            return result;
        }

        private long ParseInt64(string line, ref int position)
        {
            int startPosition = position;
            while (Char.IsDigit(line[position]))
            {
                position++;
            }
            long result = Int64.Parse(line.Substring(startPosition, position - startPosition));
            position++;
            return result;
        }

        private string ParseString(string line, ref int position)
        {
            position++;
            int startPosition = position;
            while (position < line.Length && line[position] != '\'')
            {
                if (line[position] == '\\' && line[position + 1] == '\'')
                {
                    position++;
                }
                position++;
            }
            if (position == line.Length)
            {
                throw new Exception($"Closing quote is missing after position {startPosition} in the line:\r\n{line}");
            }
            else
            {
                string result = line.Substring(startPosition, position - startPosition).Replace("\\'", "'");
                position += 2;
                return result;
            }
        }

        private DateTime ParseDateTime(string line, ref int position)
        {
            return DateTime.ParseExact(ParseString(line, ref position), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
