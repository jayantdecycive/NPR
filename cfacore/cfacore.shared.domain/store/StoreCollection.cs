using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.store;

namespace cfacore.shared.domain.store
{
    /*
    public class StoreCollection:IStoreCollection
    {

        public StoreCollection() : base()
        { 
        
        }

        public StoreCollection(List<Store> Stores)
            : base()
        {
            this._Stores = Stores;
        }

        private List<Store> _Stores = new List<Store>();
        public int IndexOf(Store item)
        {
            return _Stores.IndexOf(item);
        }

        public void Insert(int index, Store item)
        {
            _Stores.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _Stores.RemoveAt(index);
        }

        public Store this[int index]
        {
            get
            {
                return _Stores[index];
            }
            set
            {
                _Stores[index] = value;
            }
        }

        public void Add(Store item)
        {
            _Stores.Add(item);
        }

        

        public void Add(int id)
        {
            _Stores.Add(new Store(id.ToString()));
        }

        public void Clear()
        {
            _Stores.Clear();
        }

        public bool Contains(Store item)
        {
            return _Stores.Contains(item);
        }

        public void CopyTo(Store[] array, int arrayIndex)
        {
            _Stores.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Stores.Count(); }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(Store item)
        {
            return _Stores.Remove(item);
        }

        public IEnumerator<Store> GetEnumerator()
        {
            return _Stores.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Stores.GetEnumerator();
        }
    }
     * */
}
