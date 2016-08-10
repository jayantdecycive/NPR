
#region Imports

using cfares.entity.dbcontext.res_event;
using cfares.site.modules.com.application;
using cfaresv2.Controllers;

#endregion

namespace cfaresv2.Areas.MyAccount.Controllers._base
{
	public abstract class MyAccountController : ControllerBase
    {
        protected IResContext _context { get; set; }
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            _context = ReservationConfig.GetContext();
            base.OnActionExecuting(filterContext);
        }
    }
}