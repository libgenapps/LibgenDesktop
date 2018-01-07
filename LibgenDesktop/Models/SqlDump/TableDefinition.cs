using System;
using System.Collections.Generic;
using System.Globalization;

namespace LibgenDesktop.Models.SqlDump
{
    internal abstract class TableDefinition
    {
        public TableDefinition(string tableName, TableType tableType)
        {
            TableName = tableName;
            TableType = tableType;
            Columns = new Dictionary<string, ColumnDefinition>();
        }

        public string TableName { get; }
        public TableType TableType { get; }
        public Dictionary<string, ColumnDefinition> Columns { get; }

        protected void AddColumn(string columnName, ColumnType columnType)
        {
            Columns.Add(columnName.ToLower(), new ColumnDefinition(columnName, columnType));
        }

        protected int ParseInt(string value)
        {
            return Int32.Parse(value);
        }

        protected long ParseLong(string value)
        {
            return Int64.Parse(value);
        }

        protected DateTime ParseDateTime(string value)
        {
            return DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        protected DateTime? ParseNullableDateTime(string value)
        {
            if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }
    }

    internal abstract class TableDefinition<T> : TableDefinition
    {
        public TableDefinition(string tableName, TableType tableType)
            : base(tableName, tableType)
        {
            ColumnSetters = new Dictionary<string, Action<T, string>>();
        }

        public Dictionary<string, Action<T, string>> ColumnSetters { get; }

        public void AddColumn(string columnName, ColumnType columnType, Action<T, string> setter)
        {
            columnName = columnName.ToLower();
            AddColumn(columnName, columnType);
            ColumnSetters.Add(columnName, setter);
        }

        public List<Action<T, string>> GetSortedColumnSetters(IEnumerable<string> columnNames)
        {
            List<Action<T, string>> result = new List<Action<T, string>>();
            foreach (string columnName in columnNames)
            {
                if (ColumnSetters.TryGetValue(columnName.ToLower(), out Action<T, string> columnSetter))
                {
                    result.Add(columnSetter);
                }
                else
                {
                    result.Add(null);
                }
            }
            return result;
        }
    }
}
