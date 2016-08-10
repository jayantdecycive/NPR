using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using cfacore.shared.modules.user;
using cfares.domain.user;
using cfares.Models;
using cfacore.shared.domain.user;
using cfares.site.modules.mail;

namespace cfares.Areas.mobile.Controllers
{
    public class AccountController : Controller
    {
        UserMembershipService membershipService = new UserMembershipService();
        ResEmailService emailService;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            emailService = new ResEmailService();
            membershipService = new UserMembershipService(HttpContext);
            base.OnActionExecuting(filterContext);
        }


        #region Logon/off
        //
        // GET: /Account/LogOn
        public ActionResult LogOn(string returnUrl)
        {
            if (membershipService.GetUser() != null)
            {
                if (!String.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("/mobile/Ticket/Dash");
                }
            }
            else{
                return View();
            }
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                if (membershipService.ValidatePassword(model.UserName, model.Password))
                {

                    membershipService.Authenticate(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Dash", "Ticket");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/LogOff

        public ActionResult LogOff(string returnUrl)
        {
            membershipService.LogOff();
            Session.Clear();

            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion Logon/off


        public ActionResult OperatorLogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OperatorLogOn(FormCollection collection, String id)
        {
            int groupSize;
            if (id != null) { groupSize = Int32.Parse(id); }
            else { groupSize = 1; }


            if (ModelState.IsValid)
            {
                string Name = collection["Name"];
                string Email = collection["Email"];
                string AuthorityUID = collection["AuthorityUID"];
                string LocationID = collection["storeNumber"];
                ResUser user = membershipService.LoadByUsername(Email);


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
                    Roles.AddUserToRole(user.Username, "cfa");
                }

                //user might be active, but an account not yet set up??
                MembershipUser account = membershipService.GetAccount(Email);

                string pass;
                MembershipCreateStatus createStatus;
                string message = "";

                if (account == null)
                {
                    UserAccount<ResUser> userAccount = membershipService.CreateUser(user, out createStatus, out pass);
                    message = string.Format("?message={0}", HttpUtility.UrlEncode(string.Format("Temporary account created with password: {0}.\nYour temporary user name is your store's email address.", pass)));
                }
                else
                {
                    membershipService.Authenticate(user);
                }
                return Redirect(string.Format("/mobile/Ticket/slot/{0}{1}", groupSize, message));
            }
            return View();
        }


        #region Register
        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        public ActionResult Profile()
        {

            return RedirectToAction("Edit", "User", new { id = membershipService.GetUser().UserId });
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;

                membershipService.CreateUser(model.Email, model.Password, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    membershipService.Authenticate(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion Register


        #region Passwords

        public ActionResult GeneratePassword(string id)
        {

            string pass = Membership.GeneratePassword(6, 0);
            membershipService.ResetPassword(membershipService.Load(id), pass);
            string msg = HttpUtility.HtmlEncode(string.Format("A new password has been generated for this user: {0}", pass));
            if (!string.IsNullOrEmpty(Request.QueryString["redirect"]))
            {

                return Redirect(HttpUtility.UrlDecode(Request.QueryString["redirect"]) + "?message=" + msg);
            }
            else
            {

                return RedirectToAction("Index", "User", new { message = msg });
            }

        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                string additionalInfo = "";
                try
                {


                    UserAccount<ResUser> resUser = membershipService.GetUserAndAccount(User.Identity.Name, true);

                    MembershipUser currentUser = resUser.Membership;
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception ex)
                {
                    changePasswordSucceeded = false;
                    additionalInfo = string.Format("<br />Additional Information: {0}", ex.Message);
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Index", "Home", new { message = "Your password has been changed" });
                }
                else
                {
                    ModelState.AddModelError("", string.Format("The current password is incorrect or the new password is invalid. ", additionalInfo));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        
        public ActionResult Forgot()
        {
            return View();
        }

        [HttpPost]        
        public ActionResult Forgot(string email, FormCollection Collection)
        {
            if (!String.IsNullOrEmpty(email))
            {
                try
                {
                    ResUser user = membershipService.LoadByEmail(email);
                    emailService.ResetPasswordEmail(user);
                }
                catch (Exception e)
                {
                    ViewBag.Result = "Email not found.";
                    return View();
                }
            }
            else
            {
                ViewBag.Result = "Please enter a vaild email.";
            }
            return Redirect(redirectToMessage("Success", "An email has been sent ot your inbox."));

        }
        #endregion Password Utils

        private string redirectToMessage(string head, string message)
        {
            return (string.Format("/mobile/Ticket/message/?message={0}&head={1}", HttpUtility.UrlEncode(message), HttpUtility.UrlEncode(head)));
        }

        #region StoreStatus Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion




    }
}
