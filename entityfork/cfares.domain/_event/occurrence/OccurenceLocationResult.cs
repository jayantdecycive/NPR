
namespace cfares.domain._event.occ
{
	public class OccurenceLocationResult
	{
        public OccurenceLocationResult() {}

		public OccurenceLocationResult( IOccurrence occurrence,
			double distance, string distanceUOM )
        {
	        Occurence = occurrence;
	        Distance = distance;
	        DistanceUOM = distanceUOM;
        }

		public IOccurrence Occurence { get; set; }
		public double Distance = 0.0;
		public string DistanceUOM = string.Empty;
	}
}
