
#region Imports

using System.Collections.Generic;
using cfares.domain._event;

#endregion

namespace cfaresv2.Areas.MyAccount.Models
{
    public class DashboardViewModel
    {
	    public IList<ITicket> Tickets { get; set; }
    }
}