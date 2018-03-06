using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibgenDesktop.Common
{
	public static class SqlLiteDataReaderExtensions
	{
		public static string GetStringExtension(this SQLiteDataReader r, int ord)
		{
			if (!r.IsDBNull(ord))
			{
				return r.GetString(ord);
			}

			return string.Empty;
		}

		public static Int32 GetInt32Extension(this SQLiteDataReader r, int ord)
		{
			if (!r.IsDBNull(ord))
			{
				return r.GetInt32(ord);
			}

			return default(int);
		}
	}
}
