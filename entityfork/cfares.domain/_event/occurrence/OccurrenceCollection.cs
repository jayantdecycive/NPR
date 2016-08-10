using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event.slot;

namespace cfares.domain._event.occ
{
    [Serializable]
    public class OccurrenceCollection : IOccurrenceCollection
    {
        public OccurrenceCollection() : base()
        { 
        
        }

        public OccurrenceCollection(List<Occurrence> Occurrences)
            : base()
        {
            this._Occurrences = Occurrences;
        }

        private List<Occurrence> _Occurrences = new List<Occurrence>();
        public int IndexOf(Occurrence item)
        {
            return _Occurrences.IndexOf(item);
        }

        public void Insert(int index, Occurrence item)
        {
            _Occurrences.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _Occurrences.RemoveAt(index);
        }

        public Occurrence this[int index]
        {
            get
            {
                return _Occurrences[index];
            }
            set
            {
                _Occurrences[index] = value;
            }
        }

        public void Add(Occurrence item)
        {
            _Occurrences.Add(item);
        }

        public void Add(string item)
        {
            _Occurrences.Add(Occurrence.Parse(item));
        }

        public void Add(int id)
        {
            _Occurrences.Add(new Occurrence(id.ToString()));
        }

        public void Clear()
        {
            _Occurrences.Clear();
        }

        public bool Contains(Occurrence item)
        {
            return _Occurrences.Contains(item);
        }

        public void CopyTo(Occurrence[] array, int arrayIndex)
        {
            _Occurrences.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get {                 
                return _Occurrences.Count(); 
            }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(Occurrence item)
        {
            return _Occurrences.Remove(item);
        }

        public IEnumerator<Occurrence> GetEnumerator()
        {
            return _Occurrences.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Occurrences.GetEnumerator();
        }

        public SlotCollection Slots {
            get {
                IEnumerable<Slot> slots = new SlotCollection();
                if(this._Occurrences!=null)
                foreach(Occurrence oc in this._Occurrences){
                    slots = slots.Union(oc.SlotsList);
                }
                return new SlotCollection(slots.ToList<Slot>());
            }
        }
    }
}
