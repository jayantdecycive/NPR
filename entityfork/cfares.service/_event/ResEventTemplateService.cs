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
    public class ResEventTemplateService : DomainCacheService<ResTemplate>
    {
        private ResEventTemplateXmlAccess XmlDao = null;
        private ICacheAccess<ResTemplate> AfDao = null;

        public ResEventTemplateService(string root, string appFabric)
        {
            XmlDao = new ResEventTemplateXmlAccess(root);
            AfDao = BestAvailableCache(appFabric);
            
        }

        public ResEventTemplateService()
        {
            string dir = System.Configuration.ConfigurationManager.AppSettings["res-template-dir"];
            if (HttpContext.Current != null)
            {
                dir = HttpContext.Current.Server.MapPath(dir);
            }
            else
            {
                dir = System.IO.Path.GetFullPath(dir);
            }
            XmlDao = new ResEventTemplateXmlAccess(dir);
            string connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
        }

        public override bool Save(ResTemplate obj)
        {
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj});
            bool success = XmlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, success = success });
            return success;            
        }

        public override ResTemplate Load(string ID)
        {
            ResTemplate obj = XmlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public override ResTemplate Load(Uri uri)
        {
            ResTemplate obj = XmlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }

        public List<ResTemplate> LoadAll()
        {
            List<ResTemplate> col =  XmlDao.LoadAll();
            this.OnLoadCollection(col);
            return col;
        }

        public override bool Cache(ResTemplate obj)
        {
            bool success = AfDao.Save(obj);
            this.OnCache(new DomainServiceEventArgs { target = obj });
            return success;            
        }

        public override bool CacheAndSave(ResTemplate obj)
        {
            return AfDao.Save(obj) && XmlDao.Save(obj);
        }

        public override ResTemplate DeCacheOrLoad(Uri uri)
        {
            ResTemplate m = AfDao.Load(uri);
            if (m == null)
            {
                m = XmlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

       

        public override ResTemplate DeCacheOrLoad(string id)
        {
            string cacheMode = System.Configuration.ConfigurationManager.AppSettings["cache-mode"];
            if (cacheMode == "app-fabric")
            {
                ResTemplate m = DeCache(id);
                if (m == null)
                {
                    m = XmlDao.Load(id);
                    AfDao.Save(m);
                }
                return m;
            }
            else {
                return XmlDao.Load(id);
            }
        }

        public override ResTemplate DeCache(Uri uri)
        {
            ResTemplate m = new ResTemplate();
            m = AfDao.Load(uri);
            this.OnDeCache(new DomainServiceEventArgs { target = m });
            return m;
        }
        public override ResTemplate DeCache(string Id)
        {
            ResTemplate m = new ResTemplate();
            m = AfDao.Load(new Uri(m.UriBase() + Id));
            this.OnDeCache(new DomainServiceEventArgs { target = m });
            return m;
        }

        public override bool Delete(ResTemplate obj)
        {
            throw new NotImplementedException();
            /*bool success = XmlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj, success = success });
            return success;*/
        }

        public override List<ResTemplate> GetAll()
        {
            throw new NotImplementedException();
        }

        public override ResTemplate[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override bool Forget(ResTemplate obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
