
#region Imports

using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RazorEngine;
using cfares.domain._event;
using cfares.domain._event._tickets;
using cfares.repository.ticket;
using cfares.site.modules.com.Security;
using cfares.site.modules.mail;
using cfares.site.modules.user;
using cfaresv2.Areas.Admin.Controllers._base;
using cfares.site.modules.com.application;
using npr.domain._event.ticket;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize(Area = "Admin", Roles = "Admin,Operator")]
    public class HomeController : AdminController
    {
        //
        // GET: /Admin/Home/

        [ValidateInput(false)]
        public ActionResult Index()
        {
            var context = ReservationConfig.GetContext();
            var repo = new UserMembershipRepository(HttpContext, context);
            //var opRepo = new LocationRepository(repo.Context);
            //var resRepo = new ResEventRepository(repo.Context);
            var resUser = repo.Current;





            //redirect to site down page for operators if site is not in debug
            // remove this for operatores to login but still see dashboard mesaage
            /*
            #if !DEBUG
            if(resUser.OperationRole == cfares.domain.user.UserOperationRole.Operator && !resUser.isStoreNumberLogin)
            {
                FormsAuthentication.SignOut();
                return Redirect("/Admin/Account/SiteNotAvailable");
            }
            #endif
            */
            // message on home page 
            //remove this to hide message on dashboard
            //ViewBag.Message = "We are currently experiencing technical difficulties with the reservation system Operator dashboard. We greatly apologize for any inconvenience. Please bear with us as we work quickly to resolve the issue. We expect the system to be back up and running at 1:00 pm on Friday, August 16th. If you have any questions, please contact Amy Hunter at res-support@thefoundryagency.com or 404-549-8897.";

            string landingPage = ResolveViewString("Landing", resUser.OperationRole.ToString());
            // Check to see if landing page exists for the target profile
            // .. If unavailable, user is not intended to have access to Admin
            // .. at this time and should be logged out
            string filePath = HttpContext.Request.MapPath("~/Areas/Admin/Views/Home/" + landingPage + ".cshtml");
            if (!System.IO.File.Exists(filePath))
                return Logoff();

            return View(landingPage);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult EventDashboard()
        {
            return View();
        }

        public ActionResult TourDashboard()
        {
            return View();
        }

        public ActionResult MailTest()
        {
            var context = ReservationConfig.GetContext();
            var emailServ = new ResEmailService(context);
            var ticketServ = new TicketRepository<DateTicket>(context);
            var ticket = ticketServ.GetAll().First();
            emailServ.SendMessage(new ResEmailService.TemplatedMessage<ITicket>
            {
                EmailAddress = "matthew@thefoundryagency.com",
                Subject = "this is a test",
                Model = ticket,
                ForceSend = true,
                // ReSharper disable ExplicitCallerInfoArgument
            }, callerName: "NewReservationConfirmation");
            // ReSharper restore ExplicitCallerInfoArgument
            return Content("Success");
        }

        public ActionResult RazorTest()
        {
            return Content(Razor.Parse("Hello @Model.World", new { World = "Earth" }));
        }

        [ActionName("Request-Dash")]
        public ActionResult RequestDash()
        {
            return View();
        }

        public ActionResult Tours()
        {
            TicketRepository<NPRTicket> r = new TicketRepository<NPRTicket>(AppContext.Configuration.Context);
            ViewBag.NumberOfRequests = r.GetAll().Cast<NPRTicket>()
                .Count(o => o.DatesString != null && o.Status == TicketStatus.Reserved);
            return View("Npr-Tours-Landing");
        }

        public ActionResult Events()
        {
            return View("Npr-Events-Landing");
        }

        public ActionResult SiteNotAvailable()
        {
            return RedirectToAction("Index");
            //return View();
        }
    }
}
