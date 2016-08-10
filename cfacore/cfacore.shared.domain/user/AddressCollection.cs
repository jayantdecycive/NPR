using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;

namespace cfacore.shared.domain.user
{
    public class AddressCollection : IAddressCollection
    {
        public AddressCollection() : base()
        { 
        
        }

        public AddressCollection(List<Address> Addresss)
            : base()
        {
            this._Addresss = Addresss;
        }

        private List<Address> _Addresss = new List<Address>();
        public int IndexOf(Address item)
        {
            return _Addresss.IndexOf(item);
        }

        public void Insert(int index, Address item)
        {
            _Addresss.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _Addresss.RemoveAt(index);
        }

        public Address this[int index]
        {
            get
            {
                return _Addresss[index];
            }
            set
            {
                _Addresss[index] = value;
            }
        }

        public void Add(Address item)
        {
            _Addresss.Add(item);
        }

        

        public void Add(int id)
        {
            _Addresss.Add(new Address(id.ToString()));
        }

        public void Clear()
        {
            _Addresss.Clear();
        }

        public bool Contains(Address item)
        {
            return _Addresss.Contains(item);
        }

        public void CopyTo(Address[] array, int arrayIndex)
        {
            _Addresss.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Addresss.Count(); }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(Address item)
        {
            return _Addresss.Remove(item);
        }

        public IEnumerator<Address> GetEnumerator()
        {
            return _Addresss.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Addresss.GetEnumerator();
        }
    }
}
