
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cfacore.shared.service.Locations;
using cfares.domain._event;
using cfares.domain._event.occ;
using cfares.domain.store;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.store;

namespace cfares.Tests.RepositoryTests
{
	[TestClass]
	public class OccurrenceRepositoryTest
	{
		[TestMethod]
		public void GetActiveValid()
		{
			List<IOccurrence> results = new OccurrenceRepository(new CfaResContext()).GetActive().ToList();
			Assert.IsTrue( results.Any() );
		}

		[TestMethod]
		public void GetActiveLazyLoadValid()
		{
			List<IOccurrence> results = new OccurrenceRepository(new CfaResContext()).GetActive().ToList();
			Assert.IsTrue( results.Any() );

			IList<Slot> sl = results.First().SlotsList;
			Assert.IsTrue( sl.Count > 0 );
		}

		//[TestMethod]
		//public void GetActiveJoinedLazyLoadValid()
		//{
		//	CfaResContext ctx = new CfaResContext();
		//	LocationRepository locationRepository = new LocationRepository(ctx);
		//	ResEventRepository eventRepository = new ResEventRepository(ctx);
		//	OccurrenceRepository occurrenceRepository = new OccurrenceRepository(ctx);

		//	IEnumerable<LocationResult> locationResults = new LocationAPIService()
		//		.GetByPostalCode( "30328", 25 );

		//	// For join safety below
		//	IEnumerable<LocationResult> locationResultsList = 
		//		locationResults as IList<LocationResult> ?? locationResults.ToList();

		//	// Get location numbers
		//	List<string> ids = locationResultsList.Select(l => l.LocationNumber).ToList();

		//	// Case insensitive based on DB collation settings
		//	IEnumerable<OccurenceLocationResult> ol = eventRepository.GetActive()
		//		.Join( occurrenceRepository.GetActive(), e => e.ResEventId, o => o.ResEvent.ResEventId,
		//			(e, o) => new {
		//				Event = e,
		//				Occurence = o,
		//			}) // .Select( o => o.Occurence );

		//		//.Where( i => ids.Contains( i.Occurence.Store.LocationNumber ) )
		//		.ToList() // Only parameterless constructors and initializers are supported in LINQ to Entities
		//		.Join(locationResultsList, m => m.Occurence.Store.LocationNumber, l => l.LocationNumber,
		//			(m, l) => new {
		//				m.Event,
		//				m.Occurence,
		//				l.Distance,
		//				l.DistanceUOM
		//			})
		//		.Select( i => new OccurenceLocationResult( i.Event, i.Occurence.Store, 
		//			i.Distance, i.DistanceUOM ) );
			
		//	IList<Slot> sl = ol.First().SlotsList;
		//	Assert.IsTrue( sl.Count > 0 );
		//}


		[TestMethod]
		public void GetActiveValidDirect()
		{
			List<OccurrenceStatus> results = ( from o in new CfaResContext().Occurrences select o.Status ).ToList();
			Assert.IsTrue( results.Any() );
		}
	}
}
