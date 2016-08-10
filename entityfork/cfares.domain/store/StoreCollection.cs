using cfares.domain.store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.shared.domain.store
{
    
    public class StoreCollection
    {

        public StoreCollection() : base()
        { 
        
        }

        public StoreCollection(List<ResStore> Stores)
            : base()
        {
            this._Stores = Stores;
        }

        private List<ResStore> _Stores = new List<ResStore>();
        public int IndexOf(ResStore item)
        {
            return _Stores.IndexOf(item);
        }

        public void Insert(int index, ResStore item)
        {
            _Stores.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _Stores.RemoveAt(index);
        }

        public ResStore this[int index]
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

        public void Add(ResStore item)
        {
            _Stores.Add(item);
        }

        

        public void Add(int id)
        {
            _Stores.Add(new ResStore(id.ToString()));
        }

        public void Clear()
        {
            _Stores.Clear();
        }

        public bool Contains(ResStore item)
        {
            return _Stores.Contains(item);
        }

        public void CopyTo(ResStore[] array, int arrayIndex)
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

        public bool Remove(ResStore item)
        {
            return _Stores.Remove(item);
        }

        public IEnumerator<ResStore> GetEnumerator()
        {
            return _Stores.GetEnumerator();
        }

        
    }

}
