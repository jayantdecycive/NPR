
#region Imports

using System;
using cfacore.shared.domain.store;

#endregion

namespace cfacore.shared.service.Locations
{
	public class LocationResult : Store
	{
        public double Distance = 0.0;
        public string DistanceUOM = String.Empty;
	}
}
