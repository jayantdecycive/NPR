using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using cfares.Models;
using cfacore.shared.modules.user;
using cfacore.shared.domain.user;
using cfares.domain.user;
using cfares.site.modules.mail;

namespace cfares.Areas.Admin.Controllers
{
    
    public class AccountController : Controller
    {
        UserMembershipService membershipService = new UserMembershipService();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            membershipService = new UserMembershipService(HttpContext);
            
            base.OnActionExecuting(filterContext);
        }


        //
        // GET: /Account/LogOn
        [Authorize]
        public ActionResult Index()
        {
            

            

            ResUser currentUser = membershipService.GetUserAndAccount().ApplicationUser;

            Console.Write(currentUser.Name);
            return JavaScript("hello world;");
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            
            return View();
        }

        //
        // GET: /Account/LogOn   
        [Authorize]
        public ActionResult ResetPasswordEmail(string id)
        {
            ResEmailService serv = new ResEmailService();
            
            serv.ResetPasswordEmail(membershipService.Load(id));
            string msg = HttpUtility.HtmlEncode(("The email was successfully sent!"));
            if (!string.IsNullOrEmpty(Request.QueryString["redirect"]))
            {
                
                return Redirect(HttpUtility.UrlDecode(Request.QueryString["redirect"])+"?message="+msg);
            }
            else {
                return RedirectToAction("Index", "User", new { message=msg});
            }
            
        }

        [Authorize]
        public ActionResult GeneratePassword(string id)
        {

            string pass = Membership.GeneratePassword(6, 0);
            membershipService.ResetPassword(membershipService.Load(id), pass);
            string msg = HttpUtility.HtmlEncode(string.Format("A new password has been generated for this user: {0}",pass));
            if (!string.IsNullOrEmpty(Request.QueryString["redirect"]))
            {
                
                return Redirect(HttpUtility.UrlDecode(Request.QueryString["redirect"]) + "?message=" + msg);
            }
            else
            {
                
                return RedirectToAction("Index", "User", new { message=msg});
            }

        }

        [Authorize]
        public ActionResult Unlock(string id)
        {
            ResUser user = membershipService.Load(id);
            MembershipUser memUser = membershipService.GetAccount(user.Username, false);
            memUser.UnlockUser();

            string msg = HttpUtility.HtmlEncode(string.Format("User {0} has been unlocked.", user.Username));
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
        // POST: /Account/LogOn

        [HttpPost]        
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            
            if (ModelState.IsValid)
            {
                if (membershipService.ValidatePassword(model.UserName, model.Password))
                {
                    
                    membershipService.Authenticate(model.UserName,model.RememberMe);

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

        public ActionResult LogOff()
        {
            membershipService.LogOff();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        [Authorize]
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
        [Authorize]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                
                membershipService.CreateUser(model.Email,model.Password,out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    membershipService.Authenticate(model.UserName,false);
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
                    additionalInfo = string.Format("<br />Additional Information: {0}",ex.Message);
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Index", "Home", new { message="Your password has been changed" });
                }
                else
                {
                    ModelState.AddModelError("", string.Format("The current password is incorrect or the new password is invalid. ",additionalInfo));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
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
