
#region Imports

using System.Web.Mvc;
using cfares.site.modules.com.Security;
using cfaresv2.Areas.Admin.Controllers._base;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin" )]
    public class TourController : AdminController
    {
		public ActionResult Campaigns()
        {
			return View();
        }
    }
}
