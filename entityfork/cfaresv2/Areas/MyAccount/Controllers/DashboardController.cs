
#region Imports

using System;
using System.Linq;
using System.ServiceModel.Security;
using System.Web.Mvc;
using System.Web.Security;
using cfacore.shared.modules.com.admin;
using cfares.domain.user;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using cfaresv2.Areas.MyAccount.Controllers._base;
using cfaresv2.Areas.MyAccount.Models;
using cfares.entity.dbcontext.res_event;

#endregion

namespace cfaresv2.Areas.MyAccount.Controllers
{
    public class DashboardController : MyAccountController
    {
		[AreaAuthorize( Area = "MyAccount", Roles = "Customer", Controller = "LogOn", Action = "Index" )]
        public ActionResult Index()
        {
            IResContext context = ReservationConfig.GetContext();
			return View( new DashboardViewModel
			{
                Tickets = new TicketRepository(context)
					.GetByActiveAndUser( AppContext.User ).ToList()
			} );
        }

        [AreaAuthorize(Area = "MyAccount", Roles = "Customer", Controller = "LogOn", Action = "Index")]
        public ActionResult Delete(int id)
        {
            IResContext context = ReservationConfig.GetContext();
            var ticketRepository = new TicketRepository(context);
            var ticket = ticketRepository.Find(id);
            if (AppContext.User.OperationRole != UserOperationRole.Admin && ticket.OwnerId!=AppContext.User.UserId)
            {
                throw new Exception("You do not have permissions to delete that ticket");
            }
            return View(ticket);
        }

        [HttpPost]
        public ActionResult Delete(int id,FormCollection collection)
        {
            IResContext context = ReservationConfig.GetContext();
            var ticketRepository = new TicketRepository(context);
            var ticket = ticketRepository.Find(id);
            if (AppContext.User.OperationRole != UserOperationRole.Admin && ticket.OwnerId != AppContext.User.UserId)
            {
                throw new Exception("You do not have permissions to delete that ticket");
            }

            ticketRepository.Delete(ticket);
            ticketRepository.Commit();
            return RedirectToAction("Index", "Dashboard",new {message="Your ticket has been deleted"});
        }

        public ActionResult LogOff()
        {
			FormsAuthentication.SignOut();

			return UrlHelpers.RedirectToEventHome();
        }

        
    }
}
