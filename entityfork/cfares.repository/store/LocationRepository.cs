
#region Imports

using System;
using System.Linq;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.domain.store;

#endregion

namespace cfares.repository.store
{
    public class LocationRepository : GenericRepository<IResContext, ResStore, string>
    {
        public LocationRepository(IResContext context) : base(context) { }

        public override IQueryable<ResStore> PublicQuerySet()
        {
            return base.PublicQuerySet().Where(x=>x.MarketableName!=null);
        }

        public override ResStore FindBySlug(string slug)
        {
            return GetAll().FirstOrDefault(x => x.MarketableName == slug);
        }

        public IQueryable<ResStore> GetByStateProvinceCode( string stateProvinceCode )
        {
			// Case insensitive based on DB collation settings
            return GetAll().Where( o => o.StreetAddress.State == stateProvinceCode );
        }

        public bool HasEmailSubscription(ResStore resStore, domain.user.ResUser resUser)
        {
            var locationSubscription = resStore.PreferredUserSubscriptions.FirstOrDefault(x => x.UserId == resUser.UserId);
            if (locationSubscription == null)
            {
                return false;
            }
            else
            {
                return locationSubscription.ReceiveEmails;
            }
            

        }

        public LocationSubscription AddOrUpdateEmailSubscription(ResStore resStore, int resUser, bool p,bool? favorite=null)
        {
            var locationSubscription = resStore.PreferredUserSubscriptions.FirstOrDefault(x => x.UserId == resUser);
            if (locationSubscription == null)
            {
                

                locationSubscription = new LocationSubscription()
                    {
                        StoreId = resStore.LocationNumber,
                        UserId = resUser
                    };
                //resStore.PreferredUserSubscriptions.Add(locationSubscription);
                Context.LocationSubscriptions.Add(locationSubscription);
            }
            else
            {
                Context.Entry(locationSubscription).State = System.Data.EntityState.Modified;    
            }
            if (favorite != null)
                locationSubscription.Favorite = favorite.Value;
            locationSubscription.ReceiveEmails = p;
            
            //Edit(resStore);
            
            return locationSubscription;
        }

        public LocationSubscription AddOrUpdateEmailSubscription(ResStore resStore, domain.user.ResUser resUser, bool p,bool? favorite=null)
        {
            return AddOrUpdateEmailSubscription(resStore, resUser.UserId, p, favorite);
        }

        public ResStore FindOrRememberByOccurrence(int occurrenceId)
        {
            //todo: utilize a cache or read-only database
            return GetAll("Occurrences").FirstOrDefault(x=>x.Occurrences.Any(o=>o.OccurrenceId==occurrenceId));
        }
    }
}
