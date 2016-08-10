using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using cfacore.domain._base;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace cfacore.dao._base
{
    public class MemoryCacheAccess<T> : CacheAccess<T>, IMemoryCacheAccess<T> where T : IDomainObject, new()
    {
        protected static CacheItemPolicy _factory;
        protected static ObjectCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFabricAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MemoryCacheAccess(string connectionString)
        {
            this.ConnectionString = connectionString;

        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public ObjectCache Cache
        {
            get
            {
                if (_cache != null) return _cache;
                Uri appConnection = new Uri(this.ConnectionString);
                // Cache host(s)
                
                // Configuration
                var configuration =
                    new CacheItemPolicy();

                // Disable tracing to avoid informational / verbose messages on the web page
                

                _factory = configuration;
                _cache = MemoryCache.Default;                

                return _cache;
            }
        }


        /// <summary>
        /// Loads a Domain Object from the specified RootUri.
        /// </summary>
        /// <param name="URI">The URI.</param>
        /// <returns></returns>        
        public override Q Load<Q>(Uri uri)
        {
            Q q = (Q)Cache.Get(uri.ToString());
            return q;
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
                CacheItemPolicy pol = new CacheItemPolicy();
                pol.SlidingExpiration = timeout;
                Cache.Set(key.ToString(), obj, pol);
                return true;
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
            return Cache.Remove(uri.ToString())!=null;
        }

        public override T[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }
    }
}
