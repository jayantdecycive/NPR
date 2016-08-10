using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event._ticket.tours;

namespace cfares.domain._event._ticket
{
    [Serializable]
    public class TourTicketCollection : MetaTicketCollection<TourTicket>
    {
 
        public TourTicketCollection() : base()
        { 
        
        }

        public TourTicketCollection(List<TourTicket> Tickets)
            : base(Tickets)
        {
            
        }

        public TourTicketCollection(TourTicketCollection tourTicketCollection):base()
        {
            // TODO: Complete member initialization
            this._Tickets = tourTicketCollection.GetList();
        }
    }

    [Serializable]
    public class TicketCollection : List<Ticket> { 
        
    }

    [Serializable]
    public class MetaTicketCollection<Q>:IMetaTicketCollection<Q> where Q : Ticket,new()
    {
        public MetaTicketCollection() : base()
        { 
        
        }

        public List<Q> GetList() {
            return this._Tickets;
        }

        public MetaTicketCollection(List<Q> Tickets)
            : base()
        {
            this._Tickets = Tickets;
        }

        protected List<Q> _Tickets = new List<Q>();
        public int IndexOf(Q item)
        {
            return _Tickets.IndexOf(item);
        }

        public void Insert(int index, Q item)
        {
            _Tickets.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _Tickets.RemoveAt(index);
        }

        public Q this[int index]
        {
            get
            {
                return _Tickets[index];
            }
            set
            {
                _Tickets[index] = value;
            }
        }

        public void Add(Q item)
        {
            _Tickets.Add(item);
        }

        

        public void Add(int id)
        {
            Q t = new Q();
            t.Id(id.ToString());
            _Tickets.Add(t);
        }

        public void Clear()
        {
            _Tickets.Clear();
        }

        public bool Contains(Q item)
        {
            return _Tickets.Contains(item);
        }

        public void CopyTo(Q[] array, int arrayIndex)
        {
            _Tickets.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get {
                if (_Tickets == null)
                    return 0;
                return _Tickets.Count(); }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(Q item)
        {
            return _Tickets.Remove(item);
        }

        public IEnumerator<Q> GetEnumerator()
        {
            return _Tickets.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Tickets.GetEnumerator();
        }
    }
}
