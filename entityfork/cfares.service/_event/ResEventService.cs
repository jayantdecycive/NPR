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
using cfares.domain.store;

namespace cfacore.site.controllers._event
{
    public class ResEventService : DomainCacheService<ResEvent>
    {
        public ResEventMySqlAccess SqlDao = null;
        public OccurrenceMySqlAccess OccurrenceSqlDao = null;
        public ICacheAccess<ResEvent> AfDao = null;

        public ResEventService(string connectionString, string appFabric)
        {
            SqlDao = new ResEventMySqlAccess(connectionString);
            AfDao = BestAvailableCache(appFabric);
            OccurrenceSqlDao = new OccurrenceMySqlAccess(connectionString);
        }

        public ResEventService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString; 
            SqlDao = new ResEventMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
        }

        public override bool Save(ResEvent obj)
        {
            bool isNew = false;
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            if (!obj.IsBound() && string.IsNullOrEmpty(obj.ProtoOccurrence.Id()))
            {
                SqlDao.Save(obj);

                var occ = new Occurrence();
                occ.ResEvent = obj;
                occ.Store = new ResStore("0");
                occ.RegistrationAvailability = (obj.GetRegistrationAvailability());
                occ.SlotRange = obj.GetRegistrationAvailability();
                occ.Status = OccurrenceStatus.ProtoType;
                OccurrenceService oServ = new OccurrenceService();
                oServ.Save((Occurrence)occ);
                isNew = true;
            }
            else { 
                OccurrenceService oServ = new OccurrenceService();

                var ProtoOccurrence = oServ.Load(obj.ProtoOccurrence.Id());
                ProtoOccurrence.RegistrationAvailability = obj.GetRegistrationAvailability();
                ProtoOccurrence.SlotRange = obj.GetRegistrationAvailability();
                oServ.Save((Occurrence)obj.ProtoOccurrence);
                isNew = false;
            }
            bool success = SqlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;
            
        }

        public override ResEvent Load(string ID)
        {
            ResEvent obj = SqlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs{target=obj});
            return obj;
        }

        public override ResEvent Load(Uri uri)
        {
            ResEvent obj =  SqlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }



        public override bool Cache(ResEvent obj)
        {
            bool success = AfDao.Save(obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;
            
        }

        public override bool CacheAndSave(ResEvent obj)
        {
            return AfDao.Save(obj) && SqlDao.Save(obj);
        }

        public override ResEvent DeCacheOrLoad(Uri uri)
        {
            ResEvent m = AfDao.Load(uri);
            if (m == null)
            {
                m = SqlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

        public OccurrenceCollection LoadOccurrences(string resEventId) {
            OccurrenceCollection occurrences = OccurrenceSqlDao.LoadByResEvent(resEventId);            
            return occurrences;
        }

        public void LoadOccurrences(ResEvent resEvent)
        {
            OccurrenceCollection occurrences = OccurrenceSqlDao.LoadByResEvent(resEvent.Id());
            resEvent.Occurrences = occurrences;
        }

        public override ResEvent DeCacheOrLoad(string id)
        {
            ResEvent m = DeCache(id);
            if (m == null)
            {
                m = SqlDao.Load(id);
                AfDao.Save(m);
            }
            return m;
        }

        public override ResEvent DeCache(Uri uri)
        {
            ResEvent obj = AfDao.Load(uri);
            this.OnDeCache(new DomainServiceEventArgs { target=obj});
            return obj;
            
        }
        public override ResEvent DeCache(string Id)
        {
            ResEvent m = new ResEvent();
            m= AfDao.Load(new Uri(m.UriBase() + Id));
            this.OnDeCache(new DomainServiceEventArgs { target = m });
            return m;
        }

        public override bool Delete(ResEvent obj)
        {
            
            bool success = SqlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj,success=success });
            return success;
        }

        public ResEvent DeCacheOrLoadByName(string subdomain)
        {
            ResEvent m = DeCacheByName(subdomain);
            if (m == null)
            {
                m = SqlDao.LoadByUrlName(subdomain);
                AfDao.Save(m.UriBase(m.Urls),m);
            }
            return m;
        }

        private ResEvent DeCacheByName(string subdomain)
        {
            ResEvent stooge = new ResEvent();
            ResEvent obj = AfDao.Load(stooge.UriBase(subdomain));
            this.OnDeCache(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public string GetLatestYearMonthForEventType(string id)
        {
            return SqlDao.GetLatestYearMonthForEventType(id);
        }

        public override List<ResEvent> GetAll()
        {

            throw new NotImplementedException();
        }

        public override ResEvent[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }

        public override bool Forget(ResEvent obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
