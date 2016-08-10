
#region Imports

using System.Collections.Generic;
using System.Collections.ObjectModel;
using cfares.domain._event;
using cfares.domain._event.occ;

#endregion

namespace cfares.domain.Reservation
{
	public class SearchByLocationContext
	{
		public ITicket Ticket { get; set; }
		public string SelectedLocation { get; set; }
		public int SelectedRadius { get; set; }
		public ICollection<OccurenceLocationResult> EventSearchResults { get; set; }

		public IDictionary<string, string> AvailableRadiusOptions { get
		{
			return new Dictionary<string, string>
			{
				{ "10", "10 miles" }, 
				{ "25", "25 miles" }, 
				{ "50", "50 miles" }, 
				{ "100", "100 miles" }, 
				{ "200", "200 miles" }, 
				{ "500", "500 miles" }, 
			};
		} }

		public SearchByLocationContext()
		{
			SelectedLocation = "";
			SelectedRadius = 25;
			EventSearchResults = new Collection<OccurenceLocationResult>();
		}
	}
}
