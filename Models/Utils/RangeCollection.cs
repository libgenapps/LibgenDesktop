using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibgenDesktop.Models.Utils
{
    internal class RangeCollection : IList
    {
        internal class RangeCollectionEnumerator : IEnumerator
        {
            private readonly IList sourceList;
            private readonly int sourceListItemLimit;
            private int currentIndex;

            public RangeCollectionEnumerator(IList sourceList, int sourceListItemLimit)
            {
                this.sourceList = sourceList;
                this.sourceListItemLimit = sourceListItemLimit;
                currentIndex = -1;
            }

            public object Current => sourceList[currentIndex];

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

        private readonly IList fullCollection;
        private readonly int itemLimit;

        public RangeCollection(IList fullCollection, int itemLimit)
        {
            this.fullCollection = fullCollection;
            this.itemLimit = itemLimit;
        }

        public object this[int index]
        {
            get => fullCollection[index];
            set => throw new NotImplementedException();
        }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public int Count => itemLimit;

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return new RangeCollectionEnumerator(fullCollection, itemLimit);
        }

        public int IndexOf(object value)
        {
            return fullCollection.IndexOf(value);
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
    }
}
