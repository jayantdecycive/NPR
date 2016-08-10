
#region Imports

using System.Web.Security;
using cfacore.shared.service.email;
using cfares.domain._event._ticket;
using cfares.domain._event._tickets;
using cfares.domain.user;
using cfares.site.modules.com.application;
using npr.domain._event.ticket;

#endregion

namespace cfares.site.modules.mail
{
	public class NprEmailContextModel : EmailContextModel
	{
		public AppContext Context { get; set; }
		public NPRTicket Ticket { get; set; }
		public MembershipUser User { get; set; }
	}
}
