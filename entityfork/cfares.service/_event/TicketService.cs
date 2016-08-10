using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.mysql.dao._base;
using cfacore.appfabric.dao._base;
using cfares.domain._event;
using cfacore.mysql.dao._event;
using cfares.appfabric.dao._event;
using cfacore.queue.dao._base;
using cfares.queue.dao._event;
using System.IO;
using cfares.domain._event._ticket.tours;
using cfares.domain._event._ticket;
using cfacore.shared.domain._base;
using cfacore.dao._base;
using cfacore.site.controllers.shared;

namespace cfacore.site.controllers._event
{
    public class TicketService : DomainQueueService<Ticket>
    {
        public TicketMySqlAccess SqlDao = null;
        public ICacheAccess<Ticket> AfDao = null;
        public IQueueAccess<Ticket> QueueDao = null;

        public TicketService(string connectionString, string appFabric, string queue)
        {
            SqlDao = new TicketMySqlAccess(connectionString);
            AfDao = BestAvailableCache(appFabric);
            QueueDao = new TicketQueueAccess(queue);

            BeforeSaved+=new SaveEventHandler(CapacityCheck);
        }

        public TicketService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new TicketMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);

            connectionString = cfares.service.ConfigManager.QueueConnectionString;
            QueueDao = new TicketQueueAccess(connectionString);
            BeforeSaved += new SaveEventHandler(CapacityCheck);
        }

        public override bool Save(Ticket obj)
        {
            bool isNew = !obj.IsBound();
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            bool success = SqlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;      
        }

        public bool Save(TourTicket obj)
        {            
            bool isNew = !obj.IsBound();
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            bool success = SqlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;      
        }

        public void CapacityCheck(object sender, DomainServiceEventArgs e)
        {
            ITicket ticket;
            
            int slotAvailable = 0;
            int ticketSize = 0;
            SlotService slotService = new SlotService(this.SqlDao.ConnectionString,this.AfDao.ConnectionString);

            
            if (e.target is TourTicket)
            {
                TourTicket tourTicket = e.target as TourTicket;
                
                ticketSize = tourTicket.GuestCount;

                if (tourTicket.Slot == null || !tourTicket.Slot.IsBound() || tourTicket.Slot.TicketsAvailable == 0)
                {
                    tourTicket.Slot = slotService.LoadTourSlotWithCount(tourTicket.Slot.Id());
                }

                

                slotAvailable = tourTicket.Slot.TicketsAvailable;
                if (tourTicket.IsBound())
                {
                    TourTicket currentTicket = LoadTour(tourTicket.Id());
                    if(currentTicket!=null)
                        slotAvailable += currentTicket.GuestCount;
                }
                ticket = tourTicket as Ticket;
                
            }
            else {
                ticket = e.target as Ticket;
                ticketSize = 1;

                if (ticket.Slot == null || !ticket.Slot.IsBound() || ticket.Slot.TicketsAvailable == 0)
                {
                    ticket.Slot = slotService.LoadTourSlotWithCount(ticket.Slot.Id());
                }
                slotAvailable = ticket.Slot.TicketsAvailable;
                if (ticket.IsBound())
                    slotAvailable += 1;
            }

            if (ticketSize > slotAvailable) {
                throw new Exception(string.Format("There is not enough room in the slot for this Ticket. Please keep your guest count below {0}.",slotAvailable));
            }

            
        }

        public TicketCollection LoadToursByGroupName(string GroupName)
        {
            return SqlDao.LoadToursByGroupName(GroupName);
        }

        public override Ticket Load(Uri uri)
        {
            Ticket obj = SqlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public override Ticket Load(string ID)
        {
            Ticket obj = SqlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public TourTicket LoadTour(string ID)
        {
            TourTicket obj = SqlDao.LoadTour(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public TourTicketCollection LoadToursBySlot(string ID)
        {
            TourTicketCollection tickets = SqlDao.LoadToursBySlot(ID);
            
            return tickets;
        }

        

        public TourTicketCollection LoadToursByOwner(string ID)
        {
            TourTicketCollection tickets = SqlDao.LoadToursByOwner(ID);

            return tickets;
        }

        public TourTicketCollection LoadToursAndOwnersBySlot(string ID)
        {
            TourTicketCollection tickets = SqlDao.LoadToursAndOwnersBySlot(ID);

            return tickets;
        }

        public TicketCollection LoadByUser(string UserID)
        {
            TicketCollection obj = SqlDao.LoadByUser(UserID);
            this.OnLoadCollection(obj);
            return obj;
        }

        public TourTicket LoadTour(Uri uri)
        {
            return LoadTour(Path.GetFileName(uri.LocalPath));
        }



        public override bool Cache(Ticket obj)
        {
            bool success = AfDao.Save(obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;    
        }

        public override bool CacheAndSave(Ticket obj)
        {
            return AfDao.Save(obj) && SqlDao.Save(obj);
        }

        public override Ticket DeCacheOrLoad(Uri uri)
        {
            Ticket m = AfDao.Load(uri);
            if (m == null)
            {
                m = SqlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

        public override Ticket DeCacheOrLoad(string id)
        {
            Ticket m = DeCache(id);
            if (m == null)
            {
                m = SqlDao.Load(id);
                AfDao.Save(m);
            }
            return m;
        }

        public override Ticket DeCache(Uri uri)
        {
            Ticket obj = AfDao.Load(uri);
            this.OnDeCache(new DomainServiceEventArgs { target = obj });
            return obj;
        }
        public override Ticket DeCache(string Id)
        {
            Ticket m = new Ticket();
            return DeCache(new Uri(m.UriBase() + Id));
        }

        public override bool Queue(Ticket obj,string action)
        {
            bool success= QueueDao.Save(obj, action);
            this.OnQueue(new DomainServiceEventArgs { target=obj});
            return success;
        }

        public override Ticket DeQueue(Uri uri)
        {
            Ticket obj = QueueDao.Load(uri);
            this.OnDeQueue(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public override bool QueueAndSave(Ticket obj, string action)
        {
            QueueDao.Save(obj, action);
            return SqlDao.Save(obj);
        }

        public override Ticket DeQueueOrLoad(Uri uri)
        {
            Ticket t = QueueDao.Load(uri);
            if (t == null || !t.IsBound()) {
                t = SqlDao.Load(uri);
            }
            return t;

        }

        public override bool Delete(Ticket obj)
        {
            bool success = SqlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;
        }

        public TourTicket LoadTourWithOwner(string id)
        {
            TourTicket obj = SqlDao.LoadTourWithOwner(id);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public TourTicket LoadTourWithOwnerAndSlot(string id)
        {
            TourTicket obj = SqlDao.LoadTourWithOwnerAndSlot(id);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public void Cache(string cid, TourTicket tourTicket)
        {
            AfDao.Save<TourTicket>(cid,tourTicket);
            this.OnCache(new DomainServiceEventArgs { target = tourTicket });
        }

        public TourTicket DeCacheTour(string cid)
        {
            TourTicket myTicket = AfDao.Load<TourTicket>(cid);
            this.OnDeCache(new DomainServiceEventArgs { target = myTicket });
            return myTicket;
        }

        public bool Forget(TourTicket tourTicket)
        {
            bool s = AfDao.Delete(tourTicket);
            this.OnForget(new DomainServiceEventArgs { target = tourTicket });
            return s;
        }

        public override List<Ticket> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Ticket[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override bool Forget(Ticket obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj});
            return s;
        }
    }
}
