
#region Imports

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using cfacore.shared.modules.helpers;
using cfacore.shared.domain.user;
using cfares.domain.user;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfares.site.modules.mail;
using cfaresv2.Models;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.user;
using cfaresv2.Areas.Admin.Controllers._base;
using System.Text.RegularExpressions;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    public class AccountController : AdminController
    {
        UserMembershipRepository membershipService;
        IResContext context;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            context = ReservationConfig.GetContext();
            membershipService = new UserMembershipRepository(HttpContext, context);
            
            base.OnActionExecuting(filterContext);
        }


        public ContentResult SSOTest()
        {
            ResUser wayne = membershipService.LoadByWanye();
            Console.Write(wayne);
            ResUser newUser = SSO.ProcessClaim(
				new ResUser { 
					OperationRole = UserOperationRole.
					Operator,UID = "4aaf293018d7af4ebe3970b6066a87a7", 
					Username = "wayne.farr@chick-fil-a.com" }, 
					new[] { "00448", "00824" });
            
            ResUser newMattUser = SSO.ProcessClaim(new ResUser
			{
                Email = "mdrake@mediadrake.com",NameString = "Matthew Drake",
                OperationRole = UserOperationRole.Guide, UID = "990f37fbf00c4abca0020b6561776042", Username = "mdrake@mediadrake.com"
            }, new[] { "00448", "00824" });

            ResUser newGabbyUser = SSO.ProcessClaim(new ResUser()
            {
                Email = "gabby@mediadrake.com",
                NameString = "Gabby Acuna",
                OperationRole = UserOperationRole.Admin,
                UID = "990f37fbf00c4abca0020b6561776046",
                Username = "gabby@mediadrake.com"
            }, new[] { "00000" });

            return Content(newUser.UserId.ToString() + " " + newMattUser.UserId.ToString() + " " + newGabbyUser.UserId.ToString());
        }

        //
        // GET: /Account/LogOn
        [Authorize]
        public ActionResult Index()
        {
			//ResUser currentUser = membershipService.GetUserAndAccount().ApplicationUser;
			//membershipService.ChangePasswordForUser("gabby@thefoundryagency.com","foundry7");
            
            return RedirectToAction("Profile");
        }

        public ActionResult Profile()
        {
            ResUser currentUser = membershipService.GetUserAndAccount().ApplicationUser;
            return RedirectToAction("Details", "User", new {id = currentUser.UserId});
        }

        //
        // GET: /Account/LogOn
        public ActionResult LogOn(string id)
        {
            try
            {
				//membershipService.ChangePasswordForUser("rob_herold", "foundry7");
				//membershipService.ChangePasswordForUser("00475@chick-fil-a.com", "foundry7");
                //membershipService.ChangePasswordForUser("gabby@thefoundryagency.com", "foundry7");
                //membershipService.LoadOrCreateByDatasource(membershipService.GetAccount("carson.britt@thefoundryagency.com"));

				if( id == "401" )
					return View().AddModelStateError( ModelState, 
						"Your session has timed out or your account is not " + 
						"currently authorized for administrative access" );
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return View();
        }

        //
        // GET: /Account/LogOn   
        [Authorize]
        public ActionResult ResetPasswordEmail(int id)
        {
            ResEmailService serv = new ResEmailService();
            
            serv.SendPasswordReset(membershipService.Find(id));
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
        public ActionResult GeneratePassword(int? id)
        {
            string pass = Regex.Replace(Guid.NewGuid().ToString(), @"[^A-Za-z0-9]", "").Substring(0, 7);
            // Membership.GeneratePassword(6, 0);
            membershipService.ResetPassword(membershipService.Find(id.Value), pass);
            string msg = HttpUtility.HtmlEncode(string.Format("A new password has been generated for this user: {0}",pass));
            if (!string.IsNullOrEmpty(Request.QueryString["redirect"]))
            {
                return Redirect(HttpUtility.UrlDecode(Request.QueryString["redirect"]) + "?message=" +Server.UrlEncode(msg));
            }
            else
            {
                
                return RedirectToAction("Index", "User", new { message=msg});
            }

        }

        [Authorize]
        public ActionResult Unlock(string id)
        {
            ResUser user = membershipService.FindBySlug(id);
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
        public ActionResult LogOn(LoginModel model)
        {
            var returnkey = Request.QueryString.AllKeys.FirstOrDefault(x => x == "ReturnUrl" || x == "wreply"||x=="redirect"||x == "next");

            string returnUrl;
            if(returnkey!=null)
                returnUrl = Request.QueryString[returnkey] ?? "/Admin/Home";
            else
                returnUrl = "/Admin/Home";
            
            if (ModelState.IsValid)
            {
                if (membershipService.ValidatePassword(model.UserName, model.Password))
                {
					ResUser u = membershipService.GetUser( model.UserName );
                    
                    if( u == null )
						return View(model).AddModelStateError( ModelState, 
							"The user name or password provided is incorrect.");

                    // Check to see if the user account is locked out
                    // .. Currently handled by SQL vs the membership provider's lockout mechanism 
                    // .. FUTURE - Fix by proper use of the membership provider
                    if(u.AccountStatus== cfacore.domain.user.UserAccountStatus.Banned)
                    return View(model).AddModelStateError(ModelState,
                        "User account status is banned. Please contact system administrator");

                    if (u.AccountStatus == cfacore.domain.user.UserAccountStatus.UnderReview)
                        return View(model).AddModelStateError(ModelState,
                            "User account status is under review. You will be notified once approved.");

                    if (u.AccountStatus == cfacore.domain.user.UserAccountStatus.Untrusted)
                        return View(model).AddModelStateError(ModelState,
                            "User account is not functional. You will be notified once approved.");
                    
                    membershipService.Authenticate(model.UserName,model.Password,model.RememberMe);
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

					return RedirectToAction("Index", "Home");
                }

				return View(model).AddModelStateError( ModelState, 
					"The user name or password provided is incorrect." );
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
            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/Register
        //[Authorize]
        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserProfile()
        {

            return RedirectToAction("Edit", "User", new { id = membershipService.GetUser().UserId });
        }

        //
        // POST: /Account/Register
        
        //[Authorize]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            MembershipCreateStatus createStatus = MembershipCreateStatus.UserRejected;
            try
            {


                if (ModelState.IsValid)
                {
                    // Attempt to register the user


                    var userMembership = membershipService.CreateUser(model.UserName, model.Password, out createStatus);
                    
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        userMembership.ApplicationUser.SendMessage("Welcome to the reservation system!", @"You just registered your account through the reservation admin.
 
You will need an existing admin to upgrade your account in order for you to access the admin console. Please contact an administrator.");
                        membershipService.Authenticate(model.UserName, model.Password, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                }

                // If we got this far, something failed, redisplay form

            }
            catch (Exception ex)
            {
                if (createStatus == MembershipCreateStatus.Success)
                    membershipService.DeleteAccount(model.UserName);

				throw new ApplicationException( string.Format( "Unable to register new user [ {0} ]", model.UserName ), ex );
            }
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
        public ActionResult ChangePassword(LocalPasswordModel model)
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


        public ActionResult SiteNotAvailable()
        {
            return View("SiteNotAvailable");
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
