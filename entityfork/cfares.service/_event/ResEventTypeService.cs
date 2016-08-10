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
using cfares.file.dao._event;
using System.Web;
using cfacore.shared.domain._base;
using cfacore.dao._base;

namespace cfacore.site.controllers._event
{
    public class ResEventTypeService : DomainCacheService<ReservationType>
    {
        public ResEventTypeXmlAccess XmlDao = null;
        public ICacheAccess<ReservationType> AfDao = null;

        public ResEventTypeService(string root, string appFabric)
        {
            XmlDao = new ResEventTypeXmlAccess(root);
            AfDao = BestAvailableCache(appFabric);
            
        }

        public ResEventTypeService()
        {
            string dir = System.Configuration.ConfigurationManager.AppSettings["res-type-dir"];
            if (HttpContext.Current != null)
                dir = HttpContext.Current.Server.MapPath(dir);
            else
                dir = System.IO.Path.GetFullPath(dir);
            XmlDao = new ResEventTypeXmlAccess(dir);
            string connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
        }

        public override bool Save(ReservationType obj)
        {
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = false });
            bool success = XmlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, success = success });
            return success;        
        }

        public override ReservationType Load(string ID)
        {

            ReservationType obj = XmlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public override ReservationType Load(Uri uri)
        {
            ReservationType obj = XmlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public List<ReservationType> LoadAll()
        {
            List<ReservationType> col = XmlDao.LoadAll();
            this.OnLoadCollection(col);
            return col;
        }

        public List<ReservationType> LoadAll(string dirpath)
        {
            List<ReservationType> col = XmlDao.LoadAll(dirpath);
            this.OnLoadCollection(col);
            return col;
        }



        public override bool Cache(ReservationType obj)
        {
            bool success = AfDao.Save(obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;           
        }

        public override bool CacheAndSave(ReservationType obj)
        {
            return AfDao.Save(obj) && XmlDao.Save(obj);
        }

        public override ReservationType DeCacheOrLoad(Uri uri)
        {
            ReservationType m = AfDao.Load(uri);
            if (m == null)
            {
                m = XmlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }



        public override ReservationType DeCacheOrLoad(string id)
        {
            ReservationType m = DeCache(id);
            if (m == null)
            {
                m = XmlDao.Load(id);
                AfDao.Save(m);
            }

            return m;
        }

        public override ReservationType DeCache(Uri uri)
        {
            ReservationType m = new ReservationType();
            m = AfDao.Load(uri);
            this.OnDeCache(new DomainServiceEventArgs { target = m });
            return m;
        }
        public override ReservationType DeCache(string Id)
        {
            ReservationType m = new ReservationType();
            m = AfDao.Load(new Uri(m.UriBase() + Id));
            this.OnDeCache(new DomainServiceEventArgs { target = m });
            return m;
        }

        public override bool Delete(ReservationType obj)
        {
            throw new NotImplementedException();
            /*bool success = XmlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;*/
        }

        public override List<ReservationType> GetAll()
        {
            throw new NotImplementedException();
        }

        public override ReservationType[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override bool Forget(ReservationType obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
