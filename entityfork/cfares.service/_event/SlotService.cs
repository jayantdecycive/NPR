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
using cfares.domain._event.slot.tours;
using System.IO;
using cfares.domain._event.slot;
using cfares.domain.user;
using cfacore.mysql.dao.res;
using cfacore.shared.domain._base;
using cfacore.dao._base;
using cfacore.site.controllers._event;
using cfares.domain._event._ticket.tours;
using cfacore.shared.domain.user;
using System.Collections;

namespace cfacore.site.controllers.shared
{
    public class SlotService : DomainCacheService<Slot>
    {
        public SlotMySqlAccess SqlDao = null;
        public ICacheAccess<Slot> AfDao = null;

        public SlotService(string connectionString, string appFabric)
        {
            SqlDao = new SlotMySqlAccess(connectionString);
            AfDao = BestAvailableCache(appFabric);
            Saved += new SaveEventHandler(RegisterSchedule);
        }

        public SlotService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new SlotMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
            Saved += new SaveEventHandler(RegisterSchedule);
        }

        public string ToiVisitorCSV(string slotId) {
            return ToiVisitorCSV(slotId,"^",true,false);
        }

        public string ToiVisitorCSV(string slotId, string delimiter, bool includeHeaders)
        {
            return ToiVisitorCSV(slotId, delimiter, includeHeaders, false);
        }

        public string ToiVisitorCSV(string slotId, string delimiter,bool includeHeaders, bool isShort) {
            string data = "";

            TourSlot slot = LoadTourWithTicketsAndOwners(slotId);

            OccurrenceService converter = new OccurrenceService();

            List<string> rows = new List<string>();

            List<string> headercols = new List<string>();
                        
            headercols.Add("Visitor First Name");
            headercols.Add("Visitor Last Name");            
            headercols.Add("Company Name");
            if (isShort)
            {
                headercols.Add("Purpose");
                headercols.Add("Instructions Upon Arrival");
                headercols.Add("Unique Employee ID");
                headercols.Add("Scheduled Time In");
                headercols.Add("Scheduled Time Out");
            }

            if(includeHeaders)
                rows.Add(string.Join(delimiter, headercols.ToArray()));

            foreach (TourTicket t in slot.Tickets)
            {
                foreach (Name guestName in t.AllGuestNames)
                {
                    Hashtable dataTable = new Hashtable();
                    foreach (string headercol in headercols) {
                        dataTable.Add(headercol,"");
                    }

                    dataTable["Visitor First Name"] = (guestName.First);
                    dataTable["Visitor Last Name"] = (guestName.Last);

                    dataTable["Company Name"] = (t.GroupName);
                    if (isShort)
                    {
                        dataTable["Purpose"] = "Home Office Tours";
                        dataTable["Instructions Upon Arrival"] = "";

                        dataTable["Unique Employee ID"] = "6342012";

                        dataTable["Scheduled Time In"] = slot.Start.LocalDateTime.ToString();
                        dataTable["Scheduled Time Out"] = slot.End.LocalDateTime.ToString();
                    }
                    List<string> values = new List<string>();

                    foreach (string headercol in headercols)
                    {
                        if (dataTable[headercol]!=null)
                            values.Add(dataTable[headercol].ToString());
                        else
                            values.Add(string.Empty);
                    }

                    rows.Add(string.Join(delimiter, values.ToArray()));
                }
            }

            data = string.Join(Environment.NewLine, rows.ToArray());

            return data;
        } 

        const int DEFAULT_REGION = 1;

        public void RegisterSchedule(object sender, DomainServiceEventArgs e)
        {

            Slot slot = e.target as Slot;
            if (!e.success || slot == null || !slot.IsScheduled || (slot.Schedule!=null&&slot.ScheduleId!=null))
                return;

            ScheduleService scheduleService = new ScheduleService();
            Schedule matchingSchedule = scheduleService.GetScheduleForSlot(slot, DEFAULT_REGION);

            try
            {
                if (matchingSchedule == null || !matchingSchedule.IsBound())
                    matchingSchedule = scheduleService.CreateScheduleForSlot(slot,DEFAULT_REGION);
            
                if (matchingSchedule.ScheduleId == slot.ScheduleId)
                    return;

                slot.Id( matchingSchedule.Id());
            
                Save(slot, true);

            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

        public override bool Save(Slot obj) {
            return Save(obj,false);
        }

        public bool Save(Slot obj, bool silent)
        {
            bool isNew = !obj.IsBound();
            if(!silent)
                this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            bool success = SqlDao.Save(obj);
            if(!silent)
                this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;            
        }

        public bool Save(TourSlot obj)
        {
            return Save(obj,false);
        }

        public bool Save(TourSlot obj,bool silent)
        {
            bool isNew = !obj.IsBound();
            if (!silent)
                this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            bool success = SqlDao.Save(obj);
            if(!silent)
                this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;
        }


        /// <summary>
        /// Loads the specified SlotId.
        /// </summary>
        /// <param name="SlotId">The SlotId.</param>
        /// <test class="cfares.Tests.SlotServiceTest">Test reference compares expected domain objects.</test>
        /// <returns></returns>
        public override Slot Load(string ID)
        {
            Slot obj = SqlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public TourSlot LoadTour(string ID)
        {
            TourSlot obj = SqlDao.LoadTour(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public TourSlot LoadTourWithTickets(string ID)
        {
            TourSlot obj = LoadTour(ID);
            TicketService t = new TicketService();
            //obj.Tickets = t.LoadToursBySlot(ID);
            return obj;
        }

        public TourSlot LoadTourWithTicketsAndOwners(string ID)
        {
            TourSlot obj = LoadTour(ID);
            TicketService t = new TicketService();
            //obj.Tickets = t.LoadToursAndOwnersBySlot(ID);
            return obj;
        }

        public TourSlot LoadTourWithCameos(string ID)
        {
            TourSlot slot = LoadTour(ID);
            if (slot != null)
            {
                ResUserMySqlAccess userDao = new ResUserMySqlAccess(SqlDao.ConnectionString);
                slot.Cameos = userDao.GetTypedSlotCameos(ID).ToList();
            }
            //slot.Guide = (ResAdmin)userDao.Load(slot.Id());
            return slot;
        }

        public TourSlot LoadTourWithCameos(TourSlot slot)
        {
            
            ResUserMySqlAccess userDao = new ResUserMySqlAccess(SqlDao.ConnectionString);

            if(slot!=null)
                slot.Cameos = userDao.GetTypedSlotCameos(slot.Id()).ToList();
            
            //slot.Guide = (ResAdmin)userDao.Load(slot.Id());
            return slot;
        }

        public TourSlot LoadTourWithGuideOccurrenceAndEvent(string ID)
        {
            return SqlDao.LoadTourWithGuideOccurrenceAndEvent(ID);
        }

        public TourSlot LoadTour(Uri URI) {            
            TourSlot obj = SqlDao.LoadTour(Path.GetFileName(URI.LocalPath));
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public override Slot Load(Uri uri)
        {
            Slot obj = SqlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;            
        }

        public TourSlot LoadTourSlotWithCount(string id)
        {

            TourSlot obj = SqlDao.LoadTourSlotWithCapacity(id);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public SlotCollection DeCacheOrLoadByEventTypeWithDateRange(string EventType, DateTime Start, DateTime End)
        {
            return SqlDao.GetSlotsByEventTypeWithDateRange(EventType, Start, End);
        }


        public SlotCollection LoadByApproximateDateRange(DateTime hour, string eventType)
        {
            return SqlDao.LoadByApproximateDateRange(hour, eventType);
        }

        public TourSlotCollection DeCacheOrLoadTourByEventTypeWithDateRange(string EventType, DateTime Start, DateTime End)
        {
            var col= SqlDao.GetTourSlotsByEventTypeWithDateRange(EventType, Start, End);
            return col;

        }
         
        public override bool Cache(Slot obj)
        {
            bool success = AfDao.Save(obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;            
        }

        public bool Cache(string key, Slot obj)
        {
            bool success = AfDao.Save(key,obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;
        }

        public override bool CacheAndSave(Slot obj)
        {
            return AfDao.Save(obj) && SqlDao.Save(obj);
        }

        public override Slot DeCacheOrLoad(Uri uri)
        {
            Slot m = AfDao.Load(uri);
            if (m == null)
            {
                m = SqlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

        public override Slot DeCacheOrLoad(string id)
        {
            Slot m = DeCache(id);
            if (m == null)
            {
                m = SqlDao.Load(id);
                AfDao.Save(m);
            }
            return m;
        }

        public override Slot[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override List<Slot> GetAll()
        {

            return SqlDao.LoadAll();
        }

        public override Slot DeCache(Uri uri)
        {
            Slot obj = AfDao.Load(uri);
            this.OnDeCache(new DomainServiceEventArgs { target = obj });
            return obj;
        }
        public override Slot DeCache(string Id)
        {
            Slot m = new Slot();
            return DeCache(new Uri(m.UriBase() + Id));
        }

        public override bool Delete(Slot obj)
        {
            bool success = SqlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;
        }

        public bool SaveCameosForTourSlot(string id, string[] UserIds, int[] CameoTypes)
        {
            if (UserIds.Length == 0)
                return true;
            ResUserMySqlAccess userSqlDao = new ResUserMySqlAccess(SqlDao.ConnectionString);
            return userSqlDao.SaveCameosForTourSlot(id, UserIds, CameoTypes);
        }

        public bool RemoveCameosForTourSlot(string id, string[] UserIds, int[] CameoTypes)
        {
            if (UserIds.Length == 0)
                return true;
            ResUserMySqlAccess userSqlDao = new ResUserMySqlAccess(SqlDao.ConnectionString);
            return userSqlDao.RemoveCameosForTourSlot(id, UserIds, CameoTypes);
        }

        public override bool Forget(Slot obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
