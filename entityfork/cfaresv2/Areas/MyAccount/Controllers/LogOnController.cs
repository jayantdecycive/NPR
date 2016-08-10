
#region Imports

using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using cfares.site.modules.com.application;
using cfares.site.modules.mail;
using cfaresv2.Areas.MyAccount.Controllers._base;
using cfaresv2.Areas.MyAccount.Models;
using cfaresv2.Models;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.user;

#endregion

namespace cfaresv2.Areas.MyAccount.Controllers
{
    public class LogOnController : MyAccountController
    {
        // GET: /LogOn/
        public ActionResult Index()
        {
			if( ! AppContext.Configuration.Organization.UserLoginEnabled )
				return RedirectToAction( "LogOn", "Account", new { area="Admin" } );

			return View();
        }

        [HttpPost]
        public ActionResult Index( LoginModel model, string returnUrl )
        {
            IResContext context = ReservationConfig.GetContext();
            UserMembershipRepository membershipService = new UserMembershipRepository(HttpContext, context);

            if (ModelState.IsValid)
            {
	            if (membershipService.ValidatePassword(model.UserName, model.Password))
                {
                    membershipService.Authenticate(model.UserName, model.Password, model.RememberMe);

                    if ( ! string.IsNullOrWhiteSpace( returnUrl )
						&& ValidateUrl(returnUrl))
                        return Redirect(returnUrl);

	                if( Request.UrlReferrer != null )
	                {
						if( ! string.IsNullOrWhiteSpace( Request.UrlReferrer.Query ) )
						{
							NameValueCollection q = HttpUtility.ParseQueryString( Request.UrlReferrer.Query.TrimStart( new[] { '?' } ) );
							returnUrl = q[ "ReturnUrl" ] ?? string.Empty;
							if ( ! string.IsNullOrWhiteSpace( returnUrl )
								&& Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
								&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
								return Redirect( returnUrl );
						}

						if( Request.UrlReferrer.ToString().ToLower().Contains( "/logon" ) )
							return UrlHelpers.RedirectToEventHome();

		                return Redirect( Request.UrlReferrer.ToString() );
	                }

					return UrlHelpers.RedirectToEventHome();
                }

				ModelState.AddModelError("UserName", "The username or password provided is incorrect.");
            }

	        // If we got this far, something failed, redisplay form
            return View(model);
        }

        private bool ValidateUrl(string returnUrl)
        {
            if (returnUrl.StartsWith("//") || returnUrl.StartsWith("http://") || returnUrl.StartsWith("https://"))
            {
                var returnUri = new Uri(returnUrl);
                return returnUri.Host.EndsWith("chick-fil-a.com");
            }
            return Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                   && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\");
        }

        // GET: /LogOn/ForgotPassword/
        public ActionResult ForgotPassword()
        {
			return View();
        }

        // POST: /LogOn/ForgotPassword/
		[HttpPost]
        public ActionResult ForgotPassword( ForgotPasswordViewModel model )
        {
			MembershipUser currentUser = Membership.GetUser(model.EmailAddress);
            
			if( currentUser != null && ModelState.IsValid ) 
			{
				new ResEmailService().SendPasswordReset( currentUser );
				return View("ForgotPasswordSuccess");
			}

			ModelState.AddModelError("EmailAddress", "Unable to locate a user account with the provided email address.");
			return View("ForgotPassword");
        }

        // GET: /LogOn/ResetPassword/
        public ActionResult ResetPassword( string username, string reset )
        {
		  if ((reset != null) && (username != null))
		  {
			  MembershipUser currentUser = Membership.GetUser(username);
			  if ( currentUser != null && currentUser.ProviderUserKey != null 
				  && HashResetParams(currentUser.UserName, currentUser.ProviderUserKey.ToString()) == reset )
			  {
				  ViewBag.UserName = username;
				  ViewBag.NewPassword = currentUser.ResetPassword();
				  return View("ResetPasswordSuccess");
			   }
		  }

		  // TODO - ResetPassword error details / assistance page
		  return RedirectToAction("ForgotPassword");
		}

		//Method to hash parameters to generate the Reset URL
		public static string HashResetParams(string username, string guid)
		{
			 byte[] bytesofLink = System.Text.Encoding.UTF8.GetBytes(username + guid);
			 System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			 return BitConverter.ToString(md5.ComputeHash(bytesofLink));
		}
	}
}
