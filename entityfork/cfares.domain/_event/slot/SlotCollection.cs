using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event.slot.tours;

namespace cfares.domain._event.slot
{
    [Serializable]
    public class TourSlotCollection : SlotCollectionMeta<TourSlot>
    {
        public TourSlotCollection() : base()
        { 
        
        }

        public TourSlotCollection(List<TourSlot> TourSlots)
            : base(TourSlots)
        {

        }
    }

    [Serializable]
    public class SlotCollection : SlotCollectionMeta<Slot>
    {
        public SlotCollection() : base()
        { 
        
        }

        public SlotCollection(List<Slot> Slots)
            : base(Slots)
        {

        }
    }

    public class SlotCollectionMeta<Q> : ISlotCollection<Q> where Q:Slot,new()
    {
        public SlotCollectionMeta() : base()
        { 
        
        }

        public SlotCollectionMeta(List<Q> Slots)
            : base()
        {
            this._Slots = Slots;
        }

        private List<Q> _Slots = new List<Q>();
        public int IndexOf(Q item)
        {
            return _Slots.IndexOf(item);
        }

        public void Insert(int index, Q item)
        {
            _Slots.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _Slots.RemoveAt(index);
        }

        public Q this[int index]
        {
            get
            {
                return _Slots[index];
            }
            set
            {
                _Slots[index] = value;
            }
        }

        public void Add(Q item)
        {
            _Slots.Add(item);
        }

        

        public void Add(int id)
        {
            Q q = new Q();
            q.Id(id.ToString());
            _Slots.Add(q);
        }

        public void Clear()
        {
            _Slots.Clear();
        }

        public bool Contains(Q item)
        {
            return _Slots.Contains(item);
        }

        public void CopyTo(Q[] array, int arrayIndex)
        {
            _Slots.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Slots.Count(); }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(Q item)
        {
            return _Slots.Remove(item);
        }

        public IEnumerator<Q> GetEnumerator()
        {
            return _Slots.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Slots.GetEnumerator();
        }
    }

    
}
