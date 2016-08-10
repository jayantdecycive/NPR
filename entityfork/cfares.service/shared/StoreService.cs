using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.mysql.dao._base;
using cfacore.appfabric.dao._base;
using cfacore.shared.domain.store;
using cfacore.mysql.dao.shared;
using cfares.appfabric.dao.shared;
using cfacore.dao._base;
using cfacore.shared.domain._base;
using cfares.domain.store;

namespace cfacore.site.controllers.shared
{
    public class StoreService : DomainCacheService<ResStore>
    {
        public StoreMySqlAccess SqlDao = null;
        public ICacheAccess<ResStore> AfDao = null;

        public StoreService(string connectionString, string appFabric)
        {
            SqlDao = new StoreMySqlAccess(connectionString);
            AfDao = BestAvailableCache(appFabric);
        }

        public StoreService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new StoreMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
        }

        public override bool Save(ResStore obj)
        {
            return SqlDao.Save(obj);
        }

        public override ResStore Load(string ID)
        {
            return SqlDao.Load(ID);
        }

        public override ResStore Load(Uri uri)
        {
            return SqlDao.Load(uri);
        }

        public ResStore LoadByMarketableName(string name)
        {
            return SqlDao.LoadByMarketableName(name);
        }



        public override bool Cache(ResStore obj)
        {
            return AfDao.Save(obj);
        }

        public override bool CacheAndSave(ResStore obj)
        {
            return AfDao.Save(obj) && SqlDao.Save(obj);
        }

        public override ResStore DeCacheOrLoad(Uri uri)
        {
            ResStore m = AfDao.Load(uri);
            if (m == null)
            {
                m = SqlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

        public override ResStore DeCacheOrLoad(string id)
        {
            ResStore m = DeCache(id);
            if (m == null)
            {
                m = SqlDao.Load(id);
                AfDao.Save(m);
            }
            return m;
        }

        public override ResStore DeCache(Uri uri)
        {
            return AfDao.Load(uri);
        }
        public override ResStore DeCache(string Id)
        {
            ResStore m = new ResStore();
            return AfDao.Load(new Uri(m.UriBase() + Id));
        }

        public override bool Delete(ResStore obj)
        {
            return SqlDao.Delete(obj);
        }

        public override List<ResStore> GetAll()
        {
            throw new NotImplementedException();
        }

        public override ResStore[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }

        public override bool Forget(ResStore obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
