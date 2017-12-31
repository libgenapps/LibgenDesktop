using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Utils
{
    internal class AsyncBookCollection : IReadOnlyList<NonFictionBook>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        internal class BookEnumerator : IEnumerator<NonFictionBook>, IEnumerator
        {
            private readonly List<NonFictionBook> sourceList;
            private readonly int sourceListItemLimit;
            private int currentIndex;

            public BookEnumerator(List<NonFictionBook> sourceList, int sourceListItemLimit)
            {
                this.sourceList = sourceList;
                this.sourceListItemLimit = sourceListItemLimit;
                currentIndex = -1;
            }

            public NonFictionBook Current => sourceList[currentIndex];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                currentIndex++;
                return currentIndex < sourceListItemLimit;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        private readonly List<NonFictionBook> internalList;
        private readonly SynchronizationContext synchronizationContext;

        private int reportedBookCount;

        public AsyncBookCollection()
        {
            internalList = new List<NonFictionBook>();
            synchronizationContext = SynchronizationContext.Current;
            reportedBookCount = 0;
        }

        public NonFictionBook this[int index] => internalList[index];

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

        public void AddBook(NonFictionBook book)
        {
            internalList.Add(book);
        }

        public void AddBooks(IEnumerable<NonFictionBook> books)
        {
            internalList.AddRange(books);
        }

        public void UpdateReportedBookCount()
        {
            reportedBookCount = AddedBookCount;
            NotifyReset();
        }

        public IEnumerator<NonFictionBook> GetEnumerator()
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
            return internalList.Contains((NonFictionBook)value);
        }

        public void Clear()
        {
            reportedBookCount = 0;
            internalList.Clear();
            NotifyReset();
        }

        public int IndexOf(object value)
        {
            return internalList.IndexOf((NonFictionBook)value);
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
