using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using cfares.Areas.Tours.Models;
using System.IO;
using System.Web.Security;

namespace cfares.Areas.Tours.Controllers
{
    public class EmailController : Controller
    {

        protected SmtpClient emailClient;

        public EmailController()
        {
            emailClient = new SmtpClient();
            emailClient.UseDefaultCredentials = false;
            emailClient.Credentials = new System.Net.NetworkCredential("AKIAJ3QUIF4FBYJW5OSA", "AgQmwu0eVD8XcLE1tFu5YIPiThHonITGMYrcxiiZhjIK");
            emailClient.EnableSsl = true;
        }

        [Authorize]
        public ActionResult SendOutConfirmationEmails()
        {
            TourRegistrationForm model = (TourRegistrationForm)Session["TeamRegistration"];
            //Send Emails
            MailMessage customerMessage = new MailMessage();
            customerMessage.IsBodyHtml = true;
            customerMessage.From = new MailAddress("matthew@thefoundryagency.com");
            customerMessage.To.Add(new MailAddress(model.userForm.Email));
            customerMessage.Subject = "Reservation Confirmation";
            customerMessage.Body = RenderRazorViewToString("EmailConfirmation", model);

            MailMessage adminMessage = new MailMessage();
            adminMessage.IsBodyHtml = true;
            adminMessage.From = new MailAddress("matthew@thefoundryagency.com");
            adminMessage.To.Add(new MailAddress("it-admin@thefoundryagency.com"));
            adminMessage.To.Add(new MailAddress("sprockow@gmail.com"));
            adminMessage.Subject = "New Home Office Backstage Tours Reservation";
            adminMessage.Body = RenderRazorViewToString("AdminConfirmation", model);

            try
            {
                emailClient.Send(adminMessage);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.ToString();
                return View("ReservationOverview", model);
            }
            try
            {
                emailClient.Send(customerMessage);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.ToString();
                return View("ReservationOverview", model);
            }

            string returnAction = (string)TempData["returnAction"];
            string returnController = (string)TempData["returnController"];

            return RedirectToAction(returnAction, returnController);
        }

        [Authorize]
        public ActionResult SendOutCancellationEmails()
        {
            /*move this to service*/
            string emailAddress = User.Identity.Name;
            //Send Emails
            MailMessage customerMessage = new MailMessage();
            customerMessage.IsBodyHtml = true;
            customerMessage.From = new MailAddress("matthew@thefoundryagency.com");
            customerMessage.To.Add(new MailAddress(emailAddress));
            customerMessage.Subject = "Reservation Cancellation Notification";
            

            MailMessage adminMessage = new MailMessage();
            adminMessage.IsBodyHtml = true;
            adminMessage.From = new MailAddress("matthew@thefoundryagency.com");
            adminMessage.To.Add(new MailAddress("it-admin@thefoundryagency.com"));
            adminMessage.To.Add(new MailAddress("sprockow@gmail.com"));
            adminMessage.Subject = "Backstage Tours Reservation Cancellation Notification";
            ViewBag.emailAddress = emailAddress;
            /*move this to service*/


            customerMessage.Body = RenderRazorViewToString("CustomerCancellationNotification", null);
            adminMessage.Body = RenderRazorViewToString("AdminCancellationNotification", null);
            
            /*move this to service*/
            try
            {
                emailClient.Send(adminMessage);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.ToString();
                return View("LogOn");
            }
            try
            {
                emailClient.Send(customerMessage);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.ToString();
                return View("LogOn");
            }
            /*move this to service*/

            string returnAction = (string)TempData["returnAction"];
            string returnController = (string)TempData["returnController"];

            return RedirectToAction(returnAction, returnController);
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
