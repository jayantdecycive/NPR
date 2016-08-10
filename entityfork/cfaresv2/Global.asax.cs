
#region Imports

using System;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.App_Start;
using cfaresv2.Controllers;
using com.Thinktecture.Web.Http.Formatters;
using ResViewEngine = cfares.Global.ResViewEngine;
using System.Net.Http.Formatting;

#endregion

namespace cfaresv2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RazorViewEngine razorViewEngine = ViewEngines.Engines.First(x=>x.GetType()==typeof(RazorViewEngine)) as RazorViewEngine;
           // ViewEngines.Engines.Remove(razorViewEngine);
            ViewEngines.Engines.Add(new ResViewEngine());
            
            AreaRegistration.RegisterAllAreas();

            var apiConfig = GlobalConfiguration.Configuration;        
          //  apiConfig.Formatters.Insert(0, new JsonpMediaTypeFormatter());
           apiConfig.Formatters.Insert(0, new JsonMediaTypeFormatter());
            WebApiConfig.Register(apiConfig);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

			BundleConfig.ConfigureOptimizations();
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AuthConfig.RegisterAuth();
			CacheConfig.Initialize();

            ModelConfig.Register();
			ModelMetadataProviders.Current = new DisplayMetaDataProvider();           

            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
//#if DEBUG
//            string auth = Request.Headers["Authorization"];
//            string hash = string.IsNullOrEmpty(auth) ? "" : Regex.Replace(auth, @"^Basic ", "");

//            string required = "res:hero";
//            string requiredhash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(required));
//            if (string.IsNullOrEmpty(hash) || requiredhash != hash)
//            {
//                Response.StatusCode = 401;
//                Response.AddHeader("WWW-Authenticate", "Basic realm='Restricted'");
//                Response.End();
//            }
//#endif


            AppContext.RegisterContext();
        }

		protected void Application_EndRequest()
		{
            if (Context.Response.StatusCode == 404 && Request.ContentType != "application/json")
			{
				Response.Clear();

				var rd = new RouteData();
				string targetArea = string.Empty;
				if( ReservationConfig.GetConfig().ApplicationId == cfares.site.modules.com.application.Application.NPR ) 
					targetArea = cfares.site.modules.com.application.Application.NPR;
				rd.DataTokens["area"] = targetArea; // In case controller is in another area
				rd.Values["controller"] = "Reservation";
				rd.Values["action"] = "404";

				var c = new ReservationControllerBase();
				if( AppContext.Current != null )
					c.ViewData.Model = AppContext.Current.Event;
				( (IController) c ).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
			}
		}

		// Below handler needed for SSO and subsequent immediate use by controllers
		// RE: http://stackoverflow.com/questions/1822548/mvc-how-to-store-assign-roles-of-authenticated-users/1826413#1826413

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
			SSO.SSOAuthenticateResponseVerification();

			HttpCookie authCookie = Context.Request.Cookies[ FormsAuthentication.FormsCookieName ];
			if (authCookie == null) return;
			FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			if (authTicket != null)
			{
				string[] roles = authTicket.UserData.Split(new[] { ',' });
				GenericPrincipal userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);
				Context.User = userPrincipal;
			}
		}
	}
}