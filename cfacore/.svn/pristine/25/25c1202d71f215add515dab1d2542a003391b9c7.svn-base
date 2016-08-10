using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.elasticachefabric.dao._base;
using cfacore.shared.domain.media;
using cfacore.domain._base;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Enyim.Caching;
using System.Net;

namespace cfacore.dao._base
{
    public class ElastiCacheAccess<T> : CacheAccess<T>, IElastiCacheAccess<T> where T : IDomainObject, new()
    {
        public ElastiCacheAccess(string connectionString)
        {
            this.ConnectionString = connectionString;
            
        }



        protected static MemcachedClientConfiguration _factory;
        protected static MemcachedClient _cache;

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public MemcachedClient Cache
        {
            get
            {
                if (_cache != null) return _cache;

                var fact = new NLogFactory();                
                LogManager.AssignFactory(fact);
                
                // Configuration
                var configuration =
                    new MemcachedClientConfiguration();
                Uri connectionUri = new Uri(this.ConnectionString);
                configuration.AddServer(connectionUri.Host,connectionUri.Port);
                configuration.Protocol = MemcachedProtocol.Text;                
                
                _factory = configuration;
                _cache = new MemcachedClient(_factory);
                
                return _cache;
            }
        }


        /// <summary>
        /// Loads a Domain Object from the specified RootUri.
        /// </summary>
        /// <param name="URI">The URI.</param>
        /// <returns></returns>
        public override Q Load<Q>(Uri URI)
        {

            return Cache.Get<Q>(URI.ToString());
        }

        
        /// <summary>
        /// Saves a Domain Object With a Timeout.
        /// </summary>
        /// <param name="URI">The URI.</param>
        /// <returns></returns>
        public override bool Save<Q>(Uri key, Q obj, TimeSpan timeout)
        {
            try
            {

                return Cache.Store(StoreMode.Set, key.ToString(), obj,timeout);
                
            }
            catch (Exception Ex)
            {
                Console.Write(Ex.Message);                
                return false;
            }

        }

       
        /// <summary>
        /// Removes the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public override bool Delete(Uri uri)
        {
            return Cache.Remove(uri.ToString());
        }

        public override T[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }
    }
}
