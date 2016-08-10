using cfares.domain._event;
using cfares.domain._event.resevent;
using cfares.domain.store;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using System;
namespace cfares.repository._event
{
    public interface IResEventRepository : IGenericRepository<IResContext, IResEvent,int>
    {
        IResEvent Find(int key);
        IResEvent FindByHost(string host);
        IResEvent FindById(int id);
        IResEvent FindBySlug(string slug);
        IResEvent FindByUri(Uri uri);
        //System.Linq.IQueryable<IResEvent> GetByActive();
        
        System.Linq.IQueryable<IResEvent> GetByActiveAndStateProvinceCode(string stateProvinceCode);
        IResEvent TempEvent();

        ResSiteUrl[] FindUsedUrls(string[] urls);
        ResSiteUrl[] FindUsedUrls(IResEvent except, string[] urls);

        IResEvent TempEvent(ReservationType resType);
    }
}
