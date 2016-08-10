using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.domain._event.resevent
{
    /*public class ResEventCollection : IResEventCollection
    {
        public ResEventCollection() : base()
        { 
        
        }

        public ResEventCollection(List<ResEvent> ResEvents)
            : base()
        {
            this._ResEvents = ResEvents;
        }

        private List<ResEvent> _ResEvents = new List<ResEvent>();
        public int IndexOf(ResEvent item)
        {
            return _ResEvents.IndexOf(item);
        }

        public void Insert(int index, ResEvent item)
        {
            _ResEvents.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _ResEvents.RemoveAt(index);
        }

        public ResEvent this[int index]
        {
            get
            {
                return _ResEvents[index];
            }
            set
            {
                _ResEvents[index] = value;
            }
        }

        public void Add(ResEvent item)
        {
            _ResEvents.Add(item);
        }

        

        public void Add(int id)
        {
            _ResEvents.Add(new ResEvent(id.ToString()));
        }

        public void Clear()
        {
            _ResEvents.Clear();
        }

        public bool Contains(ResEvent item)
        {
            return _ResEvents.Contains(item);
        }

        public void CopyTo(ResEvent[] array, int arrayIndex)
        {
            _ResEvents.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _ResEvents.Count(); }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(ResEvent item)
        {
            return _ResEvents.Remove(item);
        }

        public IEnumerator<ResEvent> GetEnumerator()
        {
            return _ResEvents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if(_ResEvents!=null)
                return _ResEvents.GetEnumerator();
            return null;
        }
    }*/
}
