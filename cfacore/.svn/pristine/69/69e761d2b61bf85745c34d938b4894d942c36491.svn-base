using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.shared.domain.media;
using cfacore.mysql.dao._base;
using cfacore.mysql.dao.shared.media;

using cfacore.appfabric.dao.shared.media;
using cfacore.appfabric.dao._base;

namespace cfacore.site.controllers.shared.media
{
    public class MediaService: DomainCacheService<Media>
    {
        public IMySqlAccess<Media> SqlDao = null;
        public IAppFabricAccess<Media> AfDao = null;

        public MediaService(string connectionString, string appFabric)
        {
            SqlDao = new MediaMySqlAccess(connectionString);
            AfDao = new MediaAppFabricAccess(appFabric);
        }

        public MediaService()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cfa-id-mysql"].ConnectionString;
            SqlDao = new MediaMySqlAccess(connectionString);
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cfa-app-fabric"].ConnectionString;
            AfDao = new MediaAppFabricAccess(connectionString);
        }

        public override bool Save(Media obj)
        {
            return SqlDao.Save(obj);
        }

        public override Media Load(string ID)
        {
            return SqlDao.Load(ID);
        }

        public override Media Load(Uri uri)
        {
            return SqlDao.Load(uri);
        }



        public override bool Cache(Media obj)
        {
            return AfDao.Save(obj);
        }

        public override bool CacheAndSave(Media obj)
        {
            return AfDao.Save(obj) && SqlDao.Save(obj);
        }

        public override Media DeCacheOrLoad(Uri uri)
        {
            Media m = AfDao.Load(uri);
            if (m == null)
                m = SqlDao.Load(uri);
            return m;
        }

        public override Media DeCacheOrLoad(string id)
        {
            Media m = DeCache(id);
            if (m == null)
                m = SqlDao.Load(id);
            return m;
        }

        public override Media DeCache(Uri uri)
        {
            return AfDao.Load(uri);
        }
        public override Media DeCache(string Id)
        {
            Media m = new Media();
            return AfDao.Load(new Uri(m.UriBase()+Id));
        }

        public override bool Delete(Media obj)
        {
            throw new NotImplementedException();
        }

        public override bool Forget(Media obj)
        {
            throw new NotImplementedException();
        }

        public override Media[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override List<Media> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
