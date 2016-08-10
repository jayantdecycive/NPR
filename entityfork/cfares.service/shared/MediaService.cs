using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.mysql.dao._base;
using cfacore.appfabric.dao._base;
using cfacore.shared.domain.media;
using cfacore.mysql.dao.shared;
using cfares.appfabric.dao.shared;
using System.Drawing;
using System.Net;
using cfares.domain.user;
using cfacore.dao._base;
using cfacore.shared.domain._base;

namespace cfacore.site.controllers.shared.media
{
    public class MediaService : DomainCacheService<Media>
    {
        public IMySqlAccess<Media> SqlDao = null;
        public ICacheAccess<Media> AfDao = null;

        public MediaService(string connectionString, string appFabric)
        {
            SqlDao = new MediaMySqlAccess(connectionString);
            AfDao = BestAvailableCache(appFabric);
        }

        public MediaService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new MediaMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            AfDao = BestAvailableCache(connectionString);
        }

        public Media LoadFromBitmap(Bitmap bitmap) {
            Media media = new Media();
            media.Height = bitmap.Height;
            media.Width = bitmap.Width;
            return media;
        }

        public Media LoadFromUri(Uri uri,ResUser owner, string name, string description)
        {
            WebRequest wr = WebRequest.Create(uri);
            WebResponse webresponse = wr.GetResponse();
            System.IO.Stream response = webresponse.GetResponseStream();
            /*             
             * TODO: Allow New Media Types
             * */
            Bitmap remoteImage = (Bitmap)Bitmap.FromStream(response);
            Media loadedMedia = LoadFromBitmap(remoteImage);
            loadedMedia.MediaType=(MediaType.Image);
            loadedMedia.MediaUri = uri;
            if (owner == null) {
                owner = ResUser.System;
                loadedMedia.IsSystem = true;
            }
            loadedMedia.Owner = owner;
            loadedMedia.Name = name;
            loadedMedia.Description = description;
            loadedMedia.CreatedDate = DateTime.Now;
            int ContentLength;
            if (int.TryParse(webresponse.Headers.Get("Content-Length"), out ContentLength))
            {
                loadedMedia.FileSize = ContentLength;
            }
            return loadedMedia;
        }

        public Media LoadFromUri(Uri uri, string name, string description)
        {            
            return LoadFromUri(uri,null,name,description);
        }

        public Media LoadFromUri(Uri uri)
        {
            string name = string.Format("Media Found by system at: {0}",uri.ToString());
            return LoadFromUri(uri, null, name ,string.Format("{0} Found on: {1}",name,DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()) );
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
            {
                m = SqlDao.Load(uri);
                AfDao.Save(m);
            }
            return m;
        }

        public override Media DeCacheOrLoad(string id)
        {
            Media m = DeCache(id);
            if (m == null)
            {
                m = SqlDao.Load(id);
                AfDao.Save(m);
            }
            return m;
        }

        public override Media DeCache(Uri uri)
        {
            return AfDao.Load(uri);
        }
        public override Media DeCache(string Id)
        {
            Media m = new Media();
            return AfDao.Load(new Uri(m.UriBase() + Id));
        }

        public override bool Delete(Media obj)
        {
            return SqlDao.Delete(obj);
        }

        public override List<Media> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Media[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override bool Forget(Media obj)
        {
            bool s = AfDao.Delete(obj);
            this.OnForget(new DomainServiceEventArgs { target = obj });
            return s;
        }
    }
}
