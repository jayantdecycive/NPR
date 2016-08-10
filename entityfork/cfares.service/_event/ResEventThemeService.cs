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
using cfacore.dao._base;
using cfacore.shared.domain._base;

namespace cfacore.site.controllers._event
{
    public class ResEventThemeService : DomainCacheService<ResEventTheme>
    {
        private ResEventThemeXmlAccess XmlDao = null;
        private ICacheAccess<ResEventTheme> AfDao = null;

        public ResEventThemeService(string root, string appFabric)
        {
            XmlDao = new ResEventThemeXmlAccess(root);
            AfDao = BestAvailableCache(appFabric);
            
        }

        public ResEventThemeService()
        {
            string dir = System.Configuration.ConfigurationManager.AppSettings["res-theme-dir"];
            if(HttpContext.Current!=null)
                dir = HttpContext.Current.Server.MapPath(dir);
            else
                dir = System.IO.Path.GetFullPath(dir);
            XmlDao = new ResEventThemeXmlAccess(dir);
            string connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
        }

        public override bool Save(ResEventTheme obj)
        {
            return XmlDao.Save(obj);
        }

        public override ResEventTheme Load(string ID)
        {
            return XmlDao.Load(ID);
        }

        public override ResEventTheme Load(Uri uri)
        {
            return XmlDao.Load(uri);
        }

        public List<ResEventTheme> LoadAll()
        {
            return XmlDao.LoadAll();
        }

        public override bool Cache(ResEventTheme obj)
        {
            return AfDao.Save(obj);
        }

        public override bool CacheAndSave(ResEventTheme obj)
        {
            return AfDao.Save(obj) && XmlDao.Save(obj);
        }

        public override ResEventTheme DeCacheOrLoad(Uri uri)
        {
            ResEventTheme m = AfDao.Load(uri);
            if (m == null)
            {
                m = XmlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

       

        public override ResEventTheme DeCacheOrLoad(string id)
        {
            //HACK
            string cacheMode = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("cache-mode");
            if (cacheMode == "app-fabric")
            {
                ResEventTheme m = DeCache(id);
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

        public override ResEventTheme DeCache(Uri uri)
        {
            return AfDao.Load(uri);
        }
        public override ResEventTheme DeCache(string Id)
        {
            ResEventTheme m = new ResEventTheme();
            return AfDao.Load(new Uri(m.UriBase() + Id));
        }

        public override bool Delete(ResEventTheme obj)
        {
            throw new NotImplementedException();
        }

        public override List<ResEventTheme> GetAll()
        {
            throw new NotImplementedException();
        }

        public override ResEventTheme[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override bool Forget(ResEventTheme obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
