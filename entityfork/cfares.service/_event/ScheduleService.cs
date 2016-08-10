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
using cfares.domain._event.occ;
using cfares.domain._event.resevent;
using cfacore.shared.domain.store;
using cfacore.shared.domain._base;
using cfacore.dao._base;
using cfacore.site.controllers.shared;

namespace cfacore.site.controllers._event
{
    public class ScheduleService : DomainService<Schedule>
    {
        public ScheduleMySqlAccess SqlDao = null;
        
        public ScheduleService(string connectionString, string appFabric)
        {
            SqlDao = new ScheduleMySqlAccess(connectionString);
            Saved += new SaveEventHandler(ApplySlotCapacity);
        }

        public ScheduleService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new ScheduleMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            Saved += new SaveEventHandler(ApplySlotCapacity);
        }

        public override bool Save(Schedule obj)
        {
            bool isNew = false;
            if (!obj.IsBound())
            {                
                isNew = true;
            }
            
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            bool success = SqlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;
            
        }

        
        public void ApplySlotCapacity(object sender, DomainServiceEventArgs e)
        {
            Schedule schedule = e.target as Schedule;
            SlotService slotService = new SlotService();
            if (schedule == null)
                return;
            if (schedule.Slots == null)
                schedule = LoadWithSlots(schedule.Id());
            foreach (Slot slot in schedule.Slots) {
                slot.Capacity = schedule.Capacity;
                slotService.Save(slot);
            }
        }

        public override Schedule Load(string ID)
        {
            Schedule obj = SqlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs{target=obj});
            return obj;
        }

        public Schedule LoadWithSlots(string ID)
        {
            Schedule obj = SqlDao.Load(ID);
            SlotMySqlAccess slotSqlDao = new SlotMySqlAccess(SqlDao.ConnectionString);
            obj.Slots = slotSqlDao.LoadBySchedule(obj).ToList();

            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public override Schedule Load(Uri uri)
        {
            Schedule obj = SqlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }




        public override bool Delete(Schedule obj)
        {
            
            bool success = SqlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj,success=success });
            return success;
        }


        public bool ApplyToSlot(Schedule obj,Slot slot)
        {
            bool success = SqlDao.ApplyToSlot(obj,slot);            
            return success;
        }



        public override List<Schedule> GetAll()
        {

            throw new NotImplementedException();
        }

        public override Schedule[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }



        public Schedule GetScheduleForSlot(Slot slot,int region)
        {
            return SqlDao.GetScheduleWithinTimeRangeAndRegion(slot.Start.LocalDateTime, slot.End.LocalDateTime, region);
        }

        public Schedule CreateScheduleForSlot(Slot slot,int region)
        {

            Schedule schedule = (new Schedule(slot.Start.LocalDateTime, slot.End.LocalDateTime, region));

            OccurrenceService oServ = new OccurrenceService();
            DateTime Start;
            DateTime End;
            if (slot.Occurrence != null)
            {
                Start = oServ.ConvertToTimeZoneContext(slot.Occurrence, slot.Start.LocalDateTime);
                End = oServ.ConvertToTimeZoneContext(slot.Occurrence, slot.End.LocalDateTime);
            }
            else
            {
                Start = oServ.ConvertToTimeZoneContext(slot.Start.LocalDateTime);
                End = oServ.ConvertToTimeZoneContext(slot.End.LocalDateTime);
            }


            schedule.Name = schedule.ImpliedName(Start,End);
            schedule.UrlName = schedule.ImpliedUrlName(Start, End);

            if (Save(schedule))
                return schedule;
            return null;
        }
    }
}
