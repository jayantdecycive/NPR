using System;
using cfacore.domain._base;
using cfacore.shared.domain._base;
namespace cfacore.site.controllers._base
{
    public delegate void CacheEventHandler(object sender, DomainServiceEventArgs e);
    public delegate void ForgetEventHandler(object sender, DomainServiceEventArgs e);
    public delegate void DeCacheEventHandler(object sender, DomainServiceEventArgs e);

    interface IDomainCacheService<IDomainObject>
    {
        bool Cache(IDomainObject obj);
        bool CacheAndSave(IDomainObject obj);
        
        IDomainObject DeCache(Uri uri);
        IDomainObject DeCacheOrLoad(Uri uri);

        void OnCache(DomainServiceEventArgs e);
        void OnDeCache(DomainServiceEventArgs e);
        void OnForget(DomainServiceEventArgs e);

        event DeCacheEventHandler DeCached;
        event CacheEventHandler Cached;
        event ForgetEventHandler Forgotten;
    }
}
