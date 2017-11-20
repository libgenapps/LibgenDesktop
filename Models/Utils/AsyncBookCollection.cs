using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Utils
{
    internal class AsyncBookCollection : IReadOnlyList<Book>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        internal class BookEnumerator : IEnumerator<Book>, IEnumerator
        {
            private readonly List<Book> sourceList;
            private readonly int sourceListItemLimit;
            private int currentIndex;

            public BookEnumerator(List<Book> sourceList, int sourceListItemLimit)
            {
                this.sourceList = sourceList;
                this.sourceListItemLimit = sourceListItemLimit;
                currentIndex = -1;
            }

            public Book Current => sourceList[currentIndex];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                currentIndex++;
                System.Diagnostics.Debug.WriteLine("MoveNext " + currentIndex.ToString());
                return currentIndex < sourceListItemLimit;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        private readonly List<Book> internalList;
        private readonly SynchronizationContext synchronizationContext;

        private int reportedBookCount;

        public AsyncBookCollection()
        {
            internalList = new List<Book>();
            synchronizationContext = SynchronizationContext.Current;
            reportedBookCount = 0;
        }

        public Book this[int index] => internalList[index];

        public int Count => reportedBookCount;
        public int AddedBookCount => internalList.Count;

        public bool IsReadOnly => true;

        public bool IsFixedSize => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        object IList.this[int index] { get => this[index]; set => throw new NotImplementedException(); }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetCapacity(int capacity)
        {
            internalList.Capacity = capacity;
        }

        public void AddBook(Book book)
        {
            internalList.Add(book);
        }

        public void AddBooks(IEnumerable<Book> books)
        {
            internalList.AddRange(books);
        }

        public void UpdateReportedBookCount()
        {
            reportedBookCount = AddedBookCount;
            NotifyReset();
        }

        public IEnumerator<Book> GetEnumerator()
        {
            return new BookEnumerator(internalList, reportedBookCount);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BookEnumerator(internalList, reportedBookCount);
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return internalList.Contains((Book)value);
        }

        public void Clear()
        {
            reportedBookCount = 0;
            internalList.Clear();
            NotifyReset();
        }

        public int IndexOf(object value)
        {
            return internalList.IndexOf((Book)value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == synchronizationContext)
            {
                RaiseCollectionChanged(e);
            }
            else
            {
                synchronizationContext.Post(RaiseCollectionChanged, e);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == synchronizationContext)
            {
                RaisePropertyChanged(e);
            }
            else
            {
                synchronizationContext.Post(RaisePropertyChanged, e);
            }
        }

        private void NotifyReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void RaiseCollectionChanged(object param)
        {
            CollectionChanged?.Invoke(this, param as NotifyCollectionChangedEventArgs);
        }

        private void RaisePropertyChanged(object param)
        {
            PropertyChanged?.Invoke(this, param as PropertyChangedEventArgs);
        }
    }
}
