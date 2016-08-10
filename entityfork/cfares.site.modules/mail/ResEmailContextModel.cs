
#region Imports

using System.Web.Security;
using cfacore.shared.service.email;
using cfares.domain._event;
using cfares.domain._event._ticket;
using cfares.domain._event._tickets;
using cfares.domain.user;
using cfares.site.modules.com.application;

#endregion

namespace cfares.site.modules.mail
{
	public class ResEmailContextModel : EmailContextModel
	{
	    public ResEmailContextModel()
	    {
	        IsSite = false;
	    }

	    public bool IsSite { get; set; }
	    public AppContext Context { get; set; }
		public ITicket Ticket { get; set; }
		public MembershipUser User { get; set; }
	}
}
