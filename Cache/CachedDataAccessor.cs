using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using LibgenDesktop.Database;

namespace LibgenDesktop.Cache
{
    internal class CachedDataAccessor : IVirtualListDataSource
    {
        private readonly List<Book> cachedBooks;

        public CachedDataAccessor(List<Book> cachedBooks)
        {
            this.cachedBooks = cachedBooks;
        }

        public event EventHandler SortingRequested;

        public void PrepareCache(int first, int last)
        {
        }

        public object GetNthObject(int n)
        {
            if (cachedBooks.Count > n)
            {
                return cachedBooks[n];
            }
            else
            {
                return null;
            }
        }

        public int GetObjectCount()
        {
            return cachedBooks.Count;
        }

        public int GetObjectIndex(object model)
        {
            return -1;
        }

        public void AddObjects(ICollection modelObjects)
        {
            throw new NotImplementedException();
        }

        public void InsertObjects(int index, ICollection modelObjects)
        {
            throw new NotImplementedException();
        }

        public void RemoveObjects(ICollection modelObjects)
        {
            throw new NotImplementedException();
        }

        public int SearchText(string value, int first, int last, OLVColumn column)
        {
            throw new NotImplementedException();
        }

        public void SetObjects(IEnumerable collection)
        {
        }

        public void Sort(OLVColumn column, SortOrder order)
        {
        }

        public void UpdateObject(int index, object modelObject)
        {
            throw new NotImplementedException();
        }

        private void RaiseSortingRequested()
        {

        }
    }
}
