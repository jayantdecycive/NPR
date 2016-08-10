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
using cfacore.shared.domain._base;
using cfacore.dao._base;

namespace cfacore.site.controllers._event
{
    public class OccurrenceService : DomainCacheService<Occurrence>
    {
        public OccurrenceMySqlAccess SqlDao = null;
        public ICacheAccess<Occurrence> CacheDao = null;

        public OccurrenceService(string connectionString, string appFabric)
        {
            SqlDao = new OccurrenceMySqlAccess(connectionString);
            CacheDao = BestAvailableCache(appFabric);
        }

        public OccurrenceService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString; 
            SqlDao = new OccurrenceMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString; 
            CacheDao = BestAvailableCache(connectionString);
        }

        public override bool Save(Occurrence obj)
        {
            bool isNew = !obj.IsBound();
            bool success = SqlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;            
        }

        public override Occurrence Load(string ID)
        {
            Occurrence obj = SqlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public bool LoadSlots(Occurrence obj)
        {
            SlotMySqlAccess slotSqlDao = new SlotMySqlAccess(SqlDao.ConnectionString);
            obj.SlotsList = slotSqlDao.LoadByOccurrence(obj);
            return obj.SlotsList.Count > 0;
        }

        public override Occurrence Load(Uri uri)
        {
            Occurrence obj = SqlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;            
        }



        public override bool Cache(Occurrence obj)
        {
            bool success = CacheDao.Save(obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;
        }

        public override bool CacheAndSave(Occurrence obj)
        {
            return CacheDao.Save(obj) && SqlDao.Save(obj);
        }

        public override Occurrence DeCacheOrLoad(Uri uri)
        {
            Occurrence m = CacheDao.Load(uri);
            if (m == null)
            {
                m = SqlDao.Load(uri);
                CacheDao.Save(m);
            }
            return m;
        }

        public override Occurrence DeCacheOrLoad(string id)
        {
            Occurrence m = DeCache(id);
            if (m == null)
            {
                m = SqlDao.Load(id);
                CacheDao.Save(m);
            }
            return m;
        }

        public override Occurrence DeCache(Uri uri)
        {
            Occurrence obj = CacheDao.Load(uri);
            this.OnDeCache(new DomainServiceEventArgs { target = obj });
            return obj;
        }
        public override Occurrence DeCache(string Id)
        {
            Occurrence m = new Occurrence();

            Occurrence obj = CacheDao.Load(new Uri(m.UriBase() + Id));
            this.OnDeCache(new DomainServiceEventArgs { target = obj });
            return obj;
            
        }

        public override bool Delete(Occurrence obj)
        {
            bool success = SqlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;
        }

        public TimeSpan ApplyDaylightSavings(string id) {
            
            TimeZoneInfo baseZone=TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            if(!baseZone.IsDaylightSavingTime(DateTime.Now)){
                return baseZone.BaseUtcOffset;
            }else{
                try{
                    return TimeZoneInfo.FindSystemTimeZoneById(baseZone.DaylightName).BaseUtcOffset;
                }catch(TimeZoneNotFoundException ex){
                    Console.Write(ex.Message);
                    return baseZone.BaseUtcOffset.Add(new TimeSpan(0,1,0,0,0));
                    //return baseZone.BaseUtcOffset.Add(new TimeSpan(0,0,0,0,0));
                }
            }
            
        }

        public TimeZoneInfo GMTOffset {
            get {
                if (_GMTOffset==null)
                    _GMTOffset = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                return _GMTOffset;
            }
            set {
                _GMTOffset = value;
            }
        }

        private TimeZoneInfo _GMTOffset;
        public TimeZoneInfo TimeZoneContext()
        {
            return GMTOffset;
        }
        public TimeZoneInfo TimeZoneContext(Occurrence Occurrence)
        {
            if(Occurrence.Store!=null&&Occurrence.Store.IsBound())
                GMTOffset = TimeZoneInfo.FindSystemTimeZoneById(Occurrence.Store.GMTOffset);
            return GMTOffset;
        }
        public DateTime ConvertToTimeZoneContext(Occurrence Occurrence,DateTime Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext(Occurrence);
            return Convert.Add(tzi.GetUtcOffset(Convert));
        }

        public DateTime ConvertToTimeZoneContext(DateTime Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext();
            return Convert.Add(tzi.GetUtcOffset(Convert));
        }


        public DateRange ConvertToTimeZoneContext(DateRange Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext();
            return Convert.Add(tzi.GetUtcOffset(Convert.Start));
            
        }

        public DateTime ConvertFromTimeZoneContext(Occurrence Occurrence, DateTime Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext(Occurrence);
            return Convert.Add(-tzi.GetUtcOffset(Convert));
        }

        public DateTime ConvertFromTimeZoneContext(DateTime Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext();
            return Convert.Add(-tzi.GetUtcOffset(Convert));
        }


        public DateRange ConvertFromTimeZoneContext(DateRange Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext();
            return Convert.Add(-tzi.GetUtcOffset(Convert.Start));

        }

        public override List<Occurrence> GetAll()
        {

            throw new NotImplementedException();
        }

        public override Occurrence[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }

        public override bool Forget(Occurrence obj)
        {
            bool s = CacheDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
