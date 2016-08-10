
#region Imports

using System.ComponentModel.DataAnnotations;
using cfacore.domain._base;
using cfares.domain._event;

#endregion

namespace cfaresv2.Areas.Admin.Models
{
    public class EventCreateStartViewModel : DomainObject
    {
        public virtual ResEvent Event { get; set; }

        [Required, DataType("DropDown/_ResLocation"), Display(Name = "Location")]
		public virtual string LocationNumber { get; set; }
	    
		#region DomainObject Support

		public override string UriBase()
	    {
		    throw new System.NotImplementedException();
	    }

	    public override string ToChecksum()
	    {
		    throw new System.NotImplementedException();
		}

		#endregion
	}
}