using System.Data.Entity.Validation;
using System.Linq.Expressions;
using cfacore.domain.store;
using cfacore.shared.service.Locations;
using cfares.domain._event;
using cfares.domain._event.occ;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cfares.repository._event
{
    public class OccurrenceRepository : OccurrenceRepository<Occurrence> {
        public OccurrenceRepository(IResContext context) : base(context) { }

        public static string DefaultEntityIncludes {
            get { return "Store"; }
        }

        public static IOccurrenceRepository Get(IOccurrence evt, IResContext context)
        {
            var repoType = typeof(OccurrenceRepository<Occurrence>);
            var newRepoType = repoType.GetGenericTypeDefinition().MakeGenericType(evt.GetType());
            return Activator.CreateInstance(newRepoType, new object[] { context }) as IOccurrenceRepository;
        }


        public IOccurrence FindOrRemember(int occurrenceId)
        {
            return Find(occurrenceId);
        }
    }

    public static class OccurrenceQueryableExtentions
    {
        public static IQueryable<IOccurrence> GetActive(this IQueryable<IOccurrence> query)
        {
            return query.Where(x=>x.Status==OccurrenceStatus.Live||x.Status==OccurrenceStatus.Hidden);
        }

        public static IQueryable<IOccurrence> GetAccessible(this IQueryable<IOccurrence> query)
        {
            return query.Where(x => x.Status != OccurrenceStatus.Deactivated);
        }

        public static IQueryable<IOccurrence> GetVisible(this IQueryable<IOccurrence> query)
        {
            return query.Where(x => x.Status == OccurrenceStatus.Live);
        }

        public static IQueryable<OccurenceLocationResult> GetByLocationSearchTermAndRadiusAndLocations(this IQueryable<IOccurrence> query,
            string locationSearchTerm, int radiusInMiles)
        {
            // TODO - SH - Geocode / reverse geocode city+state combinations
            // .. Currently only supporting zip code based searches
            IEnumerable<LocationResult> locations = new LocationAPIService()
                .GetByPostalCodeAndLocations(locationSearchTerm, radiusInMiles, query.Select(x=>x.Store));

            return query.GetLocationResultsByActiveAndLocations(locations).OrderBy(o => o.Distance);
        }

        public static IQueryable<OccurenceLocationResult> GetLocationResultsByActiveAndLocations(
            this IQueryable<IOccurrence> query,IEnumerable<LocationResult> locations)
        {
            // For join safety below
        
            // Get location numbers
            List<string> ids = query.Select(l => l.StoreId).ToList();


            // Case insensitive based on DB collation settings
            return query
                .Join(locations, o => o.StoreId, r=> r.LocationNumber,
                    (o, r) => new
                    {
                        Occurrence = o,
                        LocationResult = r,
                    })
                .Select(i => new OccurenceLocationResult(i.Occurrence, i.LocationResult.Distance, i.LocationResult.DistanceUOM));
        }
    }

    public interface IOccurrenceRepository
    {
        IQueryable<IOccurrence> PublicQuerySet();
        IQueryable<IOccurrence> GetActive();
        IQueryable<IOccurrence> GetActive( string with );
        IQueryable<IOccurrence> GetActive( string[] with );
        void DeleteByEvent( IResEvent resEvent );

        IEnumerable<OccurenceLocationResult> GetByLocationSearchTermAndRadius( 
            string locationSearchTerm, int radiusInMiles );

        IEnumerable<OccurenceLocationResult> GetByLocationSearchTermAndRadiusAndLocations( 
            string locationSearchTerm, int radiusInMiles, IEnumerable<IStore> locationsFilter );

        IEnumerable<OccurenceLocationResult> GetLocationResultsByActiveAndLocations( 
            IEnumerable<LocationResult> locationResults );

        IEnumerable<IOccurrence> GetByActiveAndLocations(IEnumerable<IStore> locations);
        IOccurrence FindBySlug(string slug);
        void Detatch(IOccurrence existing);
        IResContext Context { get; set; }
        IOccurrence Find(Int32 key);
        IQueryable<IOccurrence> UserQuerySet(ResUser user);
        IOccurrence Find(Expression<Func<IOccurrence, bool>> keySelector, string include);
        IOccurrence Find(Expression<Func<IOccurrence, bool>> keySelector, string[] include);
        IQueryable<IOccurrence> GetAll();
        IQueryable<IOccurrence> GetAll(string with);
        IQueryable<IOccurrence> GetAll(string[] with);
        IQueryable<IOccurrence> Get(Expression<Func<IOccurrence, bool>> predicate);
        IQueryable<IOccurrence> Get(Expression<Func<IOccurrence, bool>> predicate,string with);
        IQueryable<IOccurrence> Get(Expression<Func<IOccurrence, bool>> predicate, string[] with);
        IOccurrence Find(Expression<Func<IOccurrence, bool>> predicate);
        void Add(IOccurrence entity);
        void Delete(IOccurrence entity);
        bool Save(IOccurrence entity);
        void Edit(IOccurrence entity);
        IEnumerable<DbEntityValidationResult> Commit();
        IEnumerable<DbEntityValidationResult> Commit(bool robust);
    }

    public class OccurrenceRepository<TOccurrence> : GenericRepository<IResContext, TOccurrence, int,IOccurrence>, IOccurrenceRepository where TOccurrence:class,IOccurrence,new()
    {

    

        public OccurrenceRepository(IResContext context) : base(context) { }

        public override IQueryable<IOccurrence> PublicQuerySet()
        {
            return GetAll().Where(x => x.Status == OccurrenceStatus.Live && x.ResEvent.Status == ResEventStatus.Live);
        }

		public IQueryable<IOccurrence> GetActive() {
			return GetActive( new string[]{} );
        }
		public IQueryable<IOccurrence> GetActive( string with ) {
			return GetActive( new[]{with} );
        }
		public IQueryable<IOccurrence> GetActive( string[] with ) {
			return GetAll( with )
				.Where( e => e.Status == OccurrenceStatus.Live );
        }

		public void DeleteByEvent( IResEvent resEvent ) {
			// Slots deleted by means of cascade
			foreach( Occurrence o in resEvent.Occurrences.ToList() )
				Delete( o );
        }

		public IEnumerable<OccurenceLocationResult> GetByLocationSearchTermAndRadius( 
			string locationSearchTerm, int radiusInMiles )
		{
			// TODO - SH - Geocode / reverse geocode city+state combinations
			// .. Currently only supporting zip code based searches
			IEnumerable<LocationResult> locations = new LocationAPIService()
				.GetByPostalCode( locationSearchTerm, radiusInMiles );

			return GetLocationResultsByActiveAndLocations( locations ).OrderBy( o => o.Distance );
		}

		public IEnumerable<OccurenceLocationResult> GetByLocationSearchTermAndRadiusAndLocations( 
			string locationSearchTerm, int radiusInMiles, IEnumerable<IStore> locationsFilter )
		{
			// TODO - SH - Geocode / reverse geocode city+state combinations
			// .. Currently only supporting zip code based searches
			IEnumerable<LocationResult> locations = new LocationAPIService()
				.GetByPostalCodeAndLocations( locationSearchTerm, radiusInMiles, locationsFilter );

			return GetLocationResultsByActiveAndLocations( locations ).OrderBy( o => o.Distance );
		}

        public IEnumerable<OccurenceLocationResult> GetLocationResultsByActiveAndLocations( 
			IEnumerable<LocationResult> locationResults )
        {
			// For join safety below
	        IEnumerable<LocationResult> locationResultsList = 
				locationResults as IList<LocationResult> ?? locationResults.ToList();

			// Get location numbers
	        List<string> ids = locationResultsList.Select(l => l.LocationNumber).ToList();
            ResEventRepository eventRepository = new ResEventRepository(Context);

			// Case insensitive based on DB collation settings
			return eventRepository.GetActive()
				.Join( GetActive(), e => e.ResEventId, o => o.ResEvent.ResEventId,
					(e, o) => new {
						Event = e,
						Occurence = o,
					})
				.Where( i => ids.Contains( i.Occurence.Store.LocationNumber ) )
				.ToList() // Only parameterless constructors and initializers are supported in LINQ to Entities
				.Join(locationResultsList, m => m.Occurence.Store.LocationNumber, l => l.LocationNumber,
					(m, l) => new {
						m.Event,
						m.Occurence,
						l.Distance,
						l.DistanceUOM
					})
				.Select( i => new OccurenceLocationResult( i.Occurence, i.Distance, i.DistanceUOM ) );
        }

        public IEnumerable<IOccurrence> GetByActiveAndLocations(IEnumerable<IStore> locations)
        {
            // Get location number list
            List<string> ids = locations.Select(l => l.LocationNumber).ToList();
            var resRepo = new ResEventRepository(Context);

			// Case insensitive based on DB collation settings
            return resRepo.GetActive()
                .Join(Context.Occurrences, e => e.ResEventId, o => o.ResEventId,
                    (e, o) => new
                    {
                        Event = e,
                        Occurence = o,
                    })
                .Where(i => ids.Contains(i.Occurence.Store.LocationNumber))
                .ToList() // Only parameterless constructors and initializers are supported in LINQ to Entities
                .Select(i => new TOccurrence{ResEvent=(ResEvent)i.Event, Store=i.Occurence.Store});
            //TODO: Watch out for typing error here
        }

        public override IOccurrence FindBySlug(string slug)
        {
            throw new NotImplementedException();
        }
    }
}
