using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.appfabric.dao._base;
using cfacore.domain._base;

using Microsoft.ApplicationServer.Caching;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace cfacore.dao._base
{
    public class AppFabricAccess<T> : CacheAccess<T>, IAppFabricAccess<T> where T : IDomainObject,new()
    {
        protected static DataCacheFactory _factory;
        protected static DataCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFabricAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AppFabricAccess(string connectionString)
        {
            this.ConnectionString = connectionString;
            
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public DataCache Cache{
            get
            {
                if (_cache != null) return _cache;
                Uri appConnection = new Uri(this.ConnectionString);
                // Cache host(s)
                var servers = new List<DataCacheServerEndpoint>(1)
                        {
                            new DataCacheServerEndpoint(appConnection.Host, appConnection.Port)
                        };

                // Configuration
                var configuration =
                    new DataCacheFactoryConfiguration
                    {
                        Servers = servers,
                        LocalCacheProperties =
                            new DataCacheLocalCacheProperties(1000,
                                new TimeSpan(0, 10, 0),
                                DataCacheLocalCacheInvalidationPolicy.TimeoutBased)
                    };

                // Disable tracing to avoid informational / verbose messages on the web page
                DataCacheClientLogManager.ChangeLogLevel(TraceLevel.Off);

                _factory = new DataCacheFactory(configuration);
                _cache = _factory.GetCache(appConnection.PathAndQuery.Trim('/'));

                return _cache;
            }
        }

        
                
      
        

        public override bool Save<Q>(Uri key, Q obj, TimeSpan timeout)
        {
            try
            {

                Cache.Put(key.ToString(), obj, timeout);
                return true;
            }
            catch (Exception Ex)
            {
                Console.Write(Ex.Message);
                return false;
            }
        }

        public override Q Load<Q>(Uri uri)
        {
            return (Q)Cache.Get(uri.ToString());
        }



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
