
#region Imports

using System.Web.Mvc;
using cfares.site.modules.com.application;

#endregion

namespace cfares.site.modules.com.Security
{
    public class ReservationSystemAuthorizeAttribute : AuthorizeAttribute
    {
        public string Area { get; set; }

        public override void OnAuthorization( AuthorizationContext filterContext )
        {
			// RE http://stackoverflow.com/questions/10551383/execute-code-in-custom-attribute-before-the-mvc-authorizeattribute

			// For all admin controllers requiring authentication, support acceptance of SSO 
			// .. response messages - which may authenticate the submitted user ( log them in ) 
			// .. once the msg signature is verified ( in below check, it is important not to 
			// .. call "base.OnAuthorization( filterContext );" or the web app will attempt to 
			// .. display the default login URL to the user, so we simply return and utilize
			// .. the identified user credentials since OnAuthorization occurs very early on

			if (SSO.SSOAuthenticateResponseVerification()) return;
	        
            base.OnAuthorization( filterContext );
			if( ! ( filterContext.Result is HttpUnauthorizedResult ) ) return;

			// Unauthorized ( as of this moment ) .. route user to the logon page:

			if( AppContext.Current.Configuration.SSO.IsEnabled )
		        SSO.SSOSignInRequestRedirect();

	        else filterContext.Result = new RedirectToRouteResult(
		        new System.Web.Routing.RouteValueDictionary
			    {
				    { "langCode", filterContext.RouteData.Values[ "langCode" ] },
				    { "area", Area },
				    { "controller", "Account" },
				    { "action", "Logon" },
				    { "id", ( filterContext.Result as HttpUnauthorizedResult ).StatusCode },
				    { "ReturnUrl", UrlHelpers.CurrentRawUrl() }
			    } );
        }
    }
}
