
#region Imports

using System.Data.Entity.Infrastructure;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using System;
using System.Linq;

#endregion

namespace cfares.repository._event
{
    public class ReservationTypeRepository : GenericRepository<IResContext, ReservationType, string>,
    IGenericRepository<IResContext,ReservationType,string>
    {
        public ReservationTypeRepository(IResContext context) : base(context) { }
        public ReservationTypeRepository() {}

        public override IQueryable<ReservationType> PublicQuerySet()
        {
            return GetAll();
        }

        public override ReservationType FindBySlug(string slug)
        {
            return Find(x => x.ReservationTypeId==slug);
        }

        public ReservationType FindByUri( Uri uri ) {
			return FindByUrl( FormatUrlAndPath( uri ) );
        }

		public static string FormatUrlAndPath( Uri uri )
		{
			return string.Format("{0}{1}{2}{3}{4}",
				uri.Scheme,
				Uri.SchemeDelimiter,
				uri.Host,
				uri.Port == 80 ? string.Empty : ":" + uri.Port,
				uri.AbsolutePath );
		}

        public ReservationType FindByHost( string host ) {
			// Case insensitive based on DB collation settings
            return GetAll( "SiteUrls" ).Where( o => o.SiteUrls.Any( u => host == u.Url ) )
				.OrderByDescending( i => i.SiteUrls.Max( ii => ii.Url.Length ) )
				.FirstOrDefault();
        }

        public ReservationType FindByUrl( string url ) {
			// Case insensitive based on DB collation settings
            return GetAll( "SiteUrls" ).Where( o => o.SiteUrls.Any( u => url.StartsWith(u.Url) ) )
				.OrderByDescending( i => i.SiteUrls.Max( ii => ii.Url.Length ) )
				.FirstOrDefault();
        }

        public IReservationType FindByEvent(int id)
        {
            return GetAll().FirstOrDefault(x => x.EventList.Any(e=>e.ResEventId==id));
        }

        public override void OnSave(object sender, SavedEventArgs<ReservationType> args)
        {
            //mdrake: need to kill removed site urls
            var deadResSiteUrls = Context.SiteUrls.Where(x => (x.ResEventId == null && x.ResEventTypeId == null) || x.Url == string.Empty || x.Url == null);
            foreach (var deadResSiteUrl in deadResSiteUrls)
            {
                Context.SiteUrls.Remove(deadResSiteUrl);
            }
            base.OnSave(sender, args);
        }

        public IReservationType FindByOccurrence(int id)
        {
            return GetAll().FirstOrDefault(x => x.EventList.Any(e => e.Occurrences.Any(o=>o.OccurrenceId==id)));
        }
    }
}
