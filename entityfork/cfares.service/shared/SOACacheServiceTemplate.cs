using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.mysql.dao._base;
using cfacore.appfabric.dao._base;

namespace cfacore.site.controllers.shared.$safeitemname$
{
    public class $safeitemname$Service : DomainCacheService<$safeitemname$>
    {
        public IMySqlAccess<$safeitemname$> SqlDao = null;
        public IAppFabricAccess<$safeitemname$> AfDao = null;

        public $safeitemname$Service(string connectionString, string appFabric)
        {
            SqlDao = new $safeitemname$MySqlAccess(connectionString);
            AfDao = new $safeitemname$AppFabricAccess(appFabric);
        }

        public $safeitemname$Service()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cfa-res-mysql"].ConnectionString;
            SqlDao = new $safeitemname$MySqlAccess(connectionString);
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cfa-app-fabric"].ConnectionString;
            AfDao = new $safeitemname$AppFabricAccess(connectionString);
        }

        public override bool Save($safeitemname$ obj)
        {
            return SqlDao.Save(obj);
        }

        public override $safeitemname$ Load(string ID)
        {
            return SqlDao.Load(ID);
        }

        public override $safeitemname$ Load(Uri uri)
        {
            return SqlDao.Load(uri);
        }



        public override bool Cache($safeitemname$ obj)
        {
            return AfDao.Save(obj);
        }

        public override bool CacheAndSave($safeitemname$ obj)
        {
            return AfDao.Save(obj) && SqlDao.Save(obj);
        }

        public override $safeitemname$ DeCacheOrLoad(Uri uri)
        {
            $safeitemname$ m = AfDao.Load(uri);
            if (m == null){
                m = SqlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

        public override $safeitemname$ DeCacheOrLoad(string id)
        {
            $safeitemname$ m = DeCache(id);
            if (m == null){
                m = SqlDao.Load(id);
                AfDao.Save(m);
            }
            return m;
        }

        public override $safeitemname$ DeCache(Uri uri)
        {
            return AfDao.Load(uri);
        }
        public override $safeitemname$ DeCache(string Id)
        {
            $safeitemname$ m = new $safeitemname$();
            return AfDao.Load(new Uri(m.UriBase + Id));
        }
    }
}
