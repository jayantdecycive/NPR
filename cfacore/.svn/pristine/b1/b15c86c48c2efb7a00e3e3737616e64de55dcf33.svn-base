using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.shared.domain._base;
using cfacore.dao._base;

namespace cfacore.site.controllers._base
{
    

    public abstract class DomainCacheService<T> : DomainService<T>, IDomainCacheService<T> where T : IDomainObject, new()
    {
        public abstract bool Cache(T obj);
        public virtual bool Cache(string key, T obj) {
            obj.Id(key);
            return Cache(obj);
        }
        public abstract bool CacheAndSave(T obj);

        public abstract T DeCacheOrLoad(Uri uri);
        public abstract T DeCacheOrLoad(string ID);
        public abstract T DeCache(Uri uri);
        public abstract T DeCache(string ID);

        public abstract bool Forget(T obj);
        public virtual bool Forget(string key, T obj) {
            obj.Id(key);
            return Forget(obj);
        }
        

        
        public event DeCacheEventHandler DeCached;        
        public event CacheEventHandler Cached;
        public event ForgetEventHandler Forgotten;

        public virtual ICacheAccess<T> BestAvailableCache(string ConnectionString)
        {
            ICacheAccess<T> c;
            #if (DEBUG)
                c = new MemoryCacheAccess<T>(ConnectionString);    
            #else            
                c = new ElastiCacheAccess<T>(ConnectionString);            
            #endif
            
            //c = new ElastiCacheAccess<T>(ConnectionString);            
            return c;
        }

        public void OnCache(DomainServiceEventArgs e)
        {
            if (Cached != null)
                Cached(this,e);
        }

        public void OnForget(DomainServiceEventArgs e)
        {
            if (Forgotten != null)
                Forgotten(this, e);
        }

        public void OnDeCache(DomainServiceEventArgs e)
        {
            if (DeCached != null)
                DeCached(this, e);
        }
    }
}
