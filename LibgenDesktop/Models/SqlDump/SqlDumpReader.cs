using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibgenDesktop.Models.Entities;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Archives.SevenZip;

namespace LibgenDesktop.Models.SqlDump
{
    internal class SqlDumpReader : IDisposable
    {
        internal enum LineCommand
        {
            CREATE_TABLE = 1,
            INSERT,
            OTHER
        }

        internal class ParsedTableDefinition
        {
            public string TableName { get; set; }
            public List<ParsedColumnDefinition> Columns { get; set; }
        }

        internal class ParsedColumnDefinition
        {
            public ParsedColumnDefinition(string columnName, ColumnType columnType)
            {
                ColumnName = columnName;
                ColumnType = columnType;
            }

            public string ColumnName { get; set; }
            public ColumnType ColumnType { get; set; }
        }

        private readonly StreamReader streamReader;
        private readonly ZipArchive zipArchive;
        private readonly RarArchive rarArchive;
        private readonly GZipArchive gZipArchive;
        private readonly SevenZipArchive sevenZipArchive;

        private bool disposed = false;

        public SqlDumpReader(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension.ToLower())
            {
                case ".zip":
                    zipArchive = ZipArchive.Open(filePath);
                    ZipArchiveEntry firstZipArchiveEntry = zipArchive.Entries.First();
                    FileSize = firstZipArchiveEntry.Size;
                    streamReader = new StreamReader(firstZipArchiveEntry.OpenEntryStream());
                    break;
                case ".rar":
                    rarArchive = RarArchive.Open(filePath);
                    RarArchiveEntry firstRarArchiveEntry = rarArchive.Entries.First();
                    FileSize = firstRarArchiveEntry.Size;
                    streamReader = new StreamReader(firstRarArchiveEntry.OpenEntryStream());
                    break;
                case ".gz":
                    gZipArchive = GZipArchive.Open(filePath);
                    GZipArchiveEntry firstGZipArchiveEntry = gZipArchive.Entries.First();
                    FileSize = firstGZipArchiveEntry.Size;
                    streamReader = new StreamReader(firstGZipArchiveEntry.OpenEntryStream());
                    break;
                case ".7z":
                    sevenZipArchive = SevenZipArchive.Open(filePath);
                    SevenZipArchiveEntry firstSevenZipArchiveEntry = sevenZipArchive.Entries.First();
                    FileSize = firstSevenZipArchiveEntry.Size;
                    streamReader = new StreamReader(firstSevenZipArchiveEntry.OpenEntryStream());
                    break;
                default:
                    FileSize = new FileInfo(filePath).Length;
                    streamReader = new StreamReader(filePath);
                    break;
            }
            CurrentFilePosition = 0;
        }

        public long CurrentFilePosition { get; private set; }
        public long FileSize { get; }
        public LineCommand CurrentLineCommand { get; private set; }
        private string CurrentLine { get; set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool ReadLine()
        {
            if (streamReader.EndOfStream)
            {
                CurrentLineCommand = LineCommand.OTHER;
                return false;
            }
            CurrentLine = streamReader.ReadLine();
            CurrentFilePosition = streamReader.BaseStream.Position;
            if (CurrentLine.StartsWith("CREATE TABLE"))
            {
                CurrentLineCommand = LineCommand.CREATE_TABLE;
            }
            else if (CurrentLine.StartsWith("INSERT INTO"))
            {
                CurrentLineCommand = LineCommand.INSERT;
            }
            else
            {
                CurrentLineCommand = LineCommand.OTHER;
            }
            return true;
        }

        public ParsedTableDefinition ParseTableDefinition()
        {
            ParsedTableDefinition result = new ParsedTableDefinition();
            result.TableName = ParseTableName(CurrentLine);
            result.Columns = new List<ParsedColumnDefinition>();
            bool tableParsed = false;
            while (ReadLine())
            {
                string tableLine = CurrentLine.Trim();
                if (tableLine.StartsWith("`"))
                {
                    string[] tableLineParts = tableLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string columnName = tableLineParts[0].Trim('`');
                    string columnTypeString = tableLineParts[1].ToLower();
                    ColumnType? columnType;
                    if (columnTypeString.StartsWith("char") || columnTypeString.StartsWith("varchar"))
                    {
                        columnType = ColumnType.CHAR_OR_VARCHAR;
                    }
                    else if (columnTypeString.StartsWith("int"))
                    {
                        columnType = ColumnType.INT;
                    }
                    else if (columnTypeString.StartsWith("bigint"))
                    {
                        columnType = ColumnType.BIGINT;
                    }
                    else if (columnTypeString.StartsWith("timestamp"))
                    {
                        columnType = ColumnType.TIMESTAMP;
                    }
                    else
                    {
                        columnType = null;
                    }
                    if (columnType.HasValue)
                    {
                        result.Columns.Add(new ParsedColumnDefinition(columnName, columnType.Value));
                    }
                }
                if (tableLine.EndsWith(";"))
                {
                    tableParsed = true;
                    break;
                }
            }
            if (!tableParsed)
            {
                throw new Exception($"Couldn't parse definition of the table \"{result.TableName}\".");
            }
            return result;
        }

        public IEnumerable<T> ParseImportObjects<T>(List<Action<T, string>> objectSetters) where T : new()
        {
            do
            {
                int position = CurrentLine.IndexOf("VALUES (") + 8;
                bool endOfBatchFound = false;
                while (!endOfBatchFound)
                {
                    T newObject = new T();
                    foreach (Action<T, string> objectSetter in objectSetters)
                    {
                        string stringValue = ParseString(CurrentLine, ref position);
                        objectSetter?.Invoke(newObject, stringValue);
                    }
                    yield return newObject;
                    if (CurrentLine[position] == ';')
                    {
                        endOfBatchFound = true;
                    }
                    else
                    {
                        position += 2;
                    }
                }
                ReadLine();
            }
            while (CurrentLineCommand == LineCommand.INSERT);
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
                    if (gZipArchive != null)
                    {
                        gZipArchive.Dispose();
                    }
                    if (sevenZipArchive != null)
                    {
                        sevenZipArchive.Dispose();
                    }
                    streamReader.Dispose();
                }
                disposed = true;
            }
        }

        private string ParseTableName(string line)
        {
            int position = "CREATE TABLE".Length;
            int tableNameStartPosition = 0;
            while (position < line.Length)
            {
                if (line[position] != ' ')
                {
                    if (tableNameStartPosition == 0)
                    {
                        tableNameStartPosition = position;
                    }
                }
                else
                {
                    if (tableNameStartPosition != 0)
                    {
                        string tableName = line.Substring(tableNameStartPosition, position - tableNameStartPosition);
                        int lastDotPosition = tableName.LastIndexOf('.');
                        if (lastDotPosition != -1)
                        {
                            tableName = tableName.Substring(lastDotPosition + 1);
                        }
                        return tableName.Trim('`');
                    }
                }
                position++;
            }
            throw new Exception($"Couldn't parse table name from the line:\r\n{line}");
        }

        private void PopulateBookField(NonFictionBook book, string fieldName, string line, ref int position)
        {
        }

        private string ParseString(string line, ref int position)
        {
            bool openQuote = false;
            bool closingQuote = false;
            if (line[position] == '\'')
            {
                openQuote = true;
                position++;
            }
            int startPosition = position;
            while (position < line.Length)
            {
                if ((line[position] == ',' || line[position] == ')') && !openQuote)
                {
                    break;
                }
                if (line[position] == '\'')
                {
                    if (openQuote)
                    {
                        closingQuote = true;
                        break;
                    }
                    else
                    {
                        throw new Exception($"Matching opening quote was not found for a closing quote at position {startPosition} in the line:\r\n{line}");
                    }
                }
                if (line[position] == '\\' && (line[position + 1] == '\'' || line[position + 1] == '\\'))
                {
                    position++;
                }
                position++;
            }
            if (openQuote && !closingQuote)
            {
                throw new Exception($"Closing quote is missing after position {startPosition} in the line:\r\n{line}");
            }
            else
            {
                string result = line.Substring(startPosition, position - startPosition).Replace(@"\\", @"\").Replace(@"\'", "'");
                if (closingQuote)
                {
                    position += 2;
                }
                else
                {
                    position++;
                }
                return result;
            }
        }
    }
}
