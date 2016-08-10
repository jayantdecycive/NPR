
#region Imports

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using cfacore.domain._base;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;

#endregion

namespace cfares.domain._event
{
    [DataContract]
    [SyncUrl("/api/ReservationCategory")]    
    public class ReservationCategory : DomainObject, IReservationCategory
	{
		#region Constructors

		public ReservationCategory(int id) { _Id = id.ToString( CultureInfo.InvariantCulture ); }
        public ReservationCategory() {}

		#endregion

        [DataMember]
		[DatabaseGenerated( DatabaseGeneratedOption.Identity )]
        [Column]
		public int ReservationCategoryId
        {
            get { int i; int.TryParse( _Id, out i ); return i; }
            set { _Id = value.ToString( CultureInfo.InvariantCulture ); }
        }

        public ICollection<ResEvent> ResEvents { get; set; }

        [DataMember, System.Data.Linq.Mapping.Column, Required]
        public string Name { get; set; }

		[DataMember, System.Data.Linq.Mapping.Column, Required]
        public string Description { get; set; }

		#region Type Helpers / Class Support

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/rescategory/"; 
        }

        public override string ToChecksum()
        {
            return Name;
		}

		#endregion
	}
}
