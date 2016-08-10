
#region Imports

using System.Xml;
using cfacore.domain.user;
using cfacore.shared.domain.store;
using cfacore.shared.domain.user;
using cfacore.shared.service.Helpers;

#endregion

namespace cfacore.shared.service.Locations
{
	public static class LocationDataAdapters
	{
		public static LocationResult ToLocation( this XmlNode n )
		{
			return new LocationResult
			{
				Name = n.SelectInnerText("name").Replace(" FSU", ""),
				LocationNumber = n.SelectInnerText("clientkey"), // Primary key
				Distance = n.SelectInnerText<double>("_distance"),
				DistanceUOM = n.SelectInnerText("_distanceuom"),
				LocationContact = new User
				{
					HomePhone = new Phone(n.SelectInnerText("phone")),
					Address = new Address
					{
						Line1 = n.SelectInnerText("address1") + " " + n.SelectInnerText("address2"),
						City = n.SelectInnerText("city"),
						Zip = new Zip( 
							n.SelectInnerText<int>("postalcode"), 
							n.SelectInnerText<int>("zipextension") ),
						State = n.SelectInnerText("state"),
					}
				},
				StreetAddress = new Address
				{
					Line1 = n.SelectInnerText("address1") + " " + n.SelectInnerText("address2"),
					City = n.SelectInnerText("city"),
					Zip = new Zip( 
						n.SelectInnerText<int>("postalcode"), 
						n.SelectInnerText<int>("zipextension") ),
					State = n.SelectInnerText("state"),
				},
				RegionName = n.SelectInnerText("province"),
				Coordinates = new GeographicCoordinate( 
					n.SelectInnerText<double>("latitude"), 
					n.SelectInnerText<double>("longitude") ),
			};
		}

		public static Address ToAddress( this XmlNode n )
		{
			return new Address
			{
				Line1 = n.SelectInnerText("address1") + " " + n.SelectInnerText("address2"),
				City = n.SelectInnerText("city"),
				County = n.SelectInnerText("county"),
				State = n.SelectInnerText("state"),
				Zip = int.Parse(n.SelectInnerText("postalcode"))
			};
		}
	}
}
