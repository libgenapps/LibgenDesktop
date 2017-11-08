using System;
using System.Collections.Generic;
using LibgenDesktop.Database;
using LibgenDesktop.Import;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Cache
{
    internal class DataCache
    {
        private readonly LocalDatabase localDatabase;
        private List<Book> cachedBooks;
        private List<Book> searchResults;
        private CachedDataAccessor cachedBooksDataAccessor;
        private CachedDataAccessor searchResultsDataAccessor;

        public DataCache(LocalDatabase localDatabase)
        {
            this.localDatabase = localDatabase;
            cachedBooks = new List<Book>();
            cachedBooksDataAccessor = new CachedDataAccessor(cachedBooks);
            searchResults = new List<Book>();
            searchResultsDataAccessor = new CachedDataAccessor(searchResults);
            DataAccessor = cachedBooksDataAccessor;
            IsInAllBooksMode = true;
        }

        public CachedDataAccessor DataAccessor { get; private set; }
        public bool IsInAllBooksMode { get; private set; }

        public ProgressOperation CreateLoadBooksOperation()
        {
            searchResults.Clear();
            DataAccessor = cachedBooksDataAccessor;
            return new LoadBooksOperation(localDatabase, cachedBooks);
        }

        public ProgressOperation CreateSearchBooksOperation(string searchQuery)
        {
            searchResults.Clear();
            searchQuery = searchQuery.Trim();
            if (searchQuery == String.Empty)
            {
                DataAccessor = cachedBooksDataAccessor;
                IsInAllBooksMode = true;
                return null;
            }
            else
            {
                DataAccessor = searchResultsDataAccessor;
                IsInAllBooksMode = false;
                return new SearchBooksOperation(localDatabase, searchResults, searchQuery);
            }
        }

        public ProgressOperation CreateImportSqlDumpOperation(string sqlDumpFilePath)
        {
            cachedBooks.Clear();
            IsInAllBooksMode = true;
            return new ImportSqlDumpOperation(localDatabase, sqlDumpFilePath, cachedBooks);
        }
    }
}
