using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.elasticachefabric.dao._base;
using cfacore.shared.domain.media;
//using Enyim.Caching.Configuration;
using System.Net;
//using Enyim.Caching.Memcached;
//using Enyim.Caching;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using cfacore.dao._base;


namespace cfacore.elasticache.dao.shared.media
{
    public class MediaElastiCacheAccess : ElastiCacheAccess<Media>
    {
        public override Media Load(string ID)
        {
            Media m = new Media();
            
            return Load(new Uri(m.UriBase()+ID));
        }

        public override bool Save(Media obj)
        {
            /*MemcachedClientConfiguration config = new MemcachedClientConfiguration();
            config.AddServer(ConnectionString);
            config.Protocol = MemcachedProtocol.Binary;

            MemcachedClient mc = new MemcachedClient(config);
            Uri uri = new Uri(obj.UriBase + obj.Id);
            return mc.Store(StoreMode.Set, uri.ToString(), obj);*/
            return false;
        }

        public MediaElastiCacheAccess(string connection):base(connection) { 
        
        }

       
    }
}
