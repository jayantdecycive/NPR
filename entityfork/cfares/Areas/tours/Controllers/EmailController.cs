using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using cfares.Areas.tours.Models;
using System.IO;
using System.Web.Security;
using cfacore.shared.modules.user;
using cfares.domain.user;
using cfares.site.modules.mail;
using cfacore.site.controllers._event;
using cfares.domain._event.ticket.tours;
using System.Security;

namespace cfares.Areas.tours.Controllers
{
    public class EmailController : Controller
    {
        ResEmailService serv = null;
        TicketService ticketServ = new TicketService();
        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            serv = new ResEmailService();
            base.OnActionExecuting(filterContext);
        }
        
        

        public ActionResult ResendEmail(string id)
        {
            UserMembershipService userv = new UserMembershipService(HttpContext);
            TourTicket ticket = ticketServ.LoadTourWithOwnerAndSlot(id);

           
            
            //send message
            if (!serv.SendOutConfirmationEmail(ticket))
                return View(); 
            
            return View("LostConfirmation");
        }

        
        


        public ActionResult ResetPasswordEmail(string id)
        {
            //send message
            UserMembershipService userv = new UserMembershipService(HttpContext);
            ResUser user = userv.GetUser(id);
            if (user == null || !user.IsBound())
                ViewBag.Result = "That user was not found.";
            else if (!serv.ResetPasswordEmail(user))
                ViewBag.Result = "Problem sending email!";
            else
                ViewBag.Result = "An email has been sent to your inbox to reset your password";
            return Redirect("/Account/ForgotPassword?message="+HttpUtility.UrlDecode(ViewBag.Result));

        }

        public ActionResult EmailConfirmation(String id)
        {
            UserMembershipService userv = new UserMembershipService(HttpContext);
            if (!Request.IsAuthenticated)
            {
                return Redirect("/Account/LogOn?url=/Email/EmailConfirmation/" + id);
            }
                        
            TourTicket ticket = ticketServ.LoadTourWithOwnerAndSlot(id);

            if (!Roles.IsUserInRole("Admin") && ticket.OwnerId != userv.QuickId)
                throw new SecurityException("You do not have permission to see this ticket.");

            if(ticket == null){
                return Redirect("/");
            }
            else{
                return View("Email/EmailConfirmationView", ticket);
            }
        }
    }
}
