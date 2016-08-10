
#region Imports

using System.Collections.Generic;
using cfacore.shared.domain.user;

#endregion

namespace cfacore.shared.service.Locations
{
	public class LocationSearchResult
	{
		public LocationSearchResult()
		{
            Locations = new List<LocationResult>();
            MultipleAddresses = new List<Address>();
        }

		public List<LocationResult> Locations { get; set; }
		public List<Address> MultipleAddresses { get; set; }

		public bool Empty { get  {
            return Locations.Count == 0 && MultipleAddresses.Count == 0;
        } }

        public bool HasLocations { get {
            return !Empty && Locations.Count != 0;
        } }
	}
}
