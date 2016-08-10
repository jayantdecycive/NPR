using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using cfares.Areas.tours.Models;
using cfares.domain.user;
using cfacore.shared.modules.user;
using cfares.service.common;
using cfacore.shared.domain.common;
using cfacore.site.controllers._event;
using cfares.domain._event.ticket.tours;
using cfares.domain._event;
using cfacore.site.controllers.shared;
using cfacore.shared.domain.user;

namespace cfares.Areas.tours.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /tours/Account/

        UserMembershipService userService = null;
        TicketService tserv = null;
        SlotService slotService = null;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            slotService = new SlotService();
            userService = new UserMembershipService(HttpContext);
            tserv = new TicketService();

            base.OnActionExecuting(filterContext);
        }


        public ActionResult Login()
        {
            return RedirectToAction("Logon");
        }

        public ActionResult LogOn(String id, String url)
        {

            //if already loged in
            if (id == null || slotService.Load(id) == null)
            {
                if (Request.IsAuthenticated)
                {
                    return RedirectToAction("ModifyTickets", "Reservation");
                    
                }
                else
                {
                    return View("LogOn", new LogOnModel { callbackUrl = url });
                }
            }
            else
            {
                ViewBag.Url = url;
                return View("LogOnReturning", new LogOnModel { callbackUrl = url });
            }
        }


        [HttpPost]
        public ActionResult LogOn(String id, LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.email, model.password))
                {
                    userService.Authenticate(model.email, false);
                    ResUser user = userService.GetUser();
                    if (model.callbackUrl == null)
                    {
                        return RedirectToAction("ModifyTickets", "Reservation");
                    }
                    else
                    {
                        return Redirect(model.callbackUrl);
                    }
                }
                else
                {
                    ViewBag.Result = "Email/Password is incorrect.";
                }
            }
            if (id == null || slotService.Load(id) == null)
            {
                return View();
            }
            else{
                return View("LogOnReturning");
            }
           
        }

        public PartialViewResult LogOnOffLink()
        {
            if (Request.IsAuthenticated)
            {
                ResUser user = userService.GetUser();
                ViewBag.FirstName = user.Name.First;
            }

            return PartialView("UserSignIn");
        }

        public ActionResult LogOff(string url)
        {
            FormsAuthentication.SignOut();
            if (url == null)
            {
                return RedirectToAction("LogOn");
            }
            else
            {
                return Redirect(url);
            }
            
        }


        public ActionResult OperatorLogOn()
        {

            return View("TeamLogOn");
        }


        [HttpPost]
        public ActionResult OperatorLogOn(FormCollection collection, String id)
        {
            int groupSize;
            if (id != null){ groupSize = Int32.Parse(id); }
            else{ groupSize = 1; }


            if (ModelState.IsValid)
            {
                string Name = collection["Name"];
                string Email = collection["Email"];
                string AuthorityUID = collection["AuthorityUID"];
                string LocationID = collection["storeNumber"];
                ResUser user = userService.LoadByUsername(Email);


                if (user == null || !user.IsBound())
                {
                    //ModelState.AddModelError("","User does not exist");
                    //return View("TeamLogOn");
                    user = new ResUser();
                    user.Authority = "CFAPeople";
                    user.AuthorityUID = AuthorityUID;
                    user.Name = new Name(Name);
                    user.Email = Email;
                    user.Username = Email;
                    
                }

                
                MembershipUser account = userService.GetAccount(Email);

                string pass;
                MembershipCreateStatus createStatus;
                string message = "";

                if (account == null)
                {
                    UserAccount<ResUser> userAccount = userService.CreateUser(user, out createStatus, out pass);
                    message = string.Format("?message={0}", HttpUtility.UrlEncode(string.Format("Temporary account created with password: {0}.\nYour temporary user name is your store's email address.", pass)));
                }
                else
                {
                    userService.Authenticate(user);
                }

                if (!Roles.IsUserInRole(user.Username, "cfa"))
                    Roles.AddUserToRole(user.Username, "cfa");
                

                return Redirect(string.Format("/Reservation/Date/{0}{1}", groupSize, message));
               
            }
            else
            {
                return View("TeamLogOn");
            }
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgotPassword(LogOnModel model)
        {
            TokenService tokServ = new TokenService();
            Token token = null;

            if (model.email != null)
            {
                try
                {
                    token = tokServ.GeneratePasswordToken(userService.GetUser(model.email));
                    return RedirectToAction("ResetPasswordEmail", "Email", new { id=model.email });
                }
                catch (Exception e)
                {
                    ViewBag.Result = "Email not found. "+e.Message;
                    return View();
                }
            }
            else
            {
                ViewBag.Result = "Please enter a vaild email.";
            }

            return View();
        }


        public ActionResult PasswordReset()
        {
            if(userService.GetUser() == null){
                return RedirectToAction("LogOn", "Account");
            }
            UserCreationForm model = new UserCreationForm();
            return View("Reset", model);
        }


        [HttpPost]
        public ActionResult PasswordReset(string id, UserCreationForm model)
        {
            ResUser user = userService.GetUser();

            if (user != null)
            {
                userService.ResetPassword(user, model.Password);
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                ViewBag.Result = "Problem reseting password.";
                return View("Reset");
            }
        }


        public ActionResult LostInvitation()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LostInvitation(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.email, model.password))
                {
                    userService.Authenticate(model.email, false);
                    ResUser user = userService.GetUser();
                    try
                    {   
                        Ticket ticket = tserv.LoadByUser(user.Id())[0];
                        return RedirectToAction("ResendEmail", "Email", new { id = ticket });
                    }
                    catch(Exception e)
                    {
                        ViewBag.Result = "User not registered for any tours.";
                    }
                }
                else
                {
                    ViewBag.Result = "Email/Password is incorrect.";
                }
            }
            return View();
        }
    }
}
