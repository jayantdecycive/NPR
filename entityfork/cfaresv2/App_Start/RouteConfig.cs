
#region Imports

using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcDomainRouting.Code;
using cfares.domain._event;
using cfares.domain._event.resevent;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.site.modules.com.application;
using cfares.site.modules.com.reservations.res;
using System.Web;

#endregion

namespace cfaresv2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {          
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// Email routing
			routes.MapRoute( "Email",
					"Email/{action}/{id}",
					new { controller = "Email", action = "Index", id = UrlParameter.Optional },
					new [] { "cfaresv2.Controllers" }
				);

			// Get all defined / active events in the system for host name routing
			// .. to the appropriate EventType controllers / area
			// .. e.g. http://mothersondateknight.res.local.chick-fil-a.com/

			// mdrake: get all res site urls and iterate any non-nulls
			// if resevent exists, route that, otherwiste, use types
            IResContext context = ReservationConfig.GetContext();
            var r = new ResSiteUrlRepository(context);
			foreach( var url in r.Get(o => o.Url != string.Empty && o.Url != null,
				new[] { "ReservationType","ResEvent" } ).ToList() )
			{
				if (url.Uri == null) continue;

				// Event specific matching
				if (url.ResEvent != null)
				{
					ResEvent e = url.ResEvent;
					string routePattern;
					if (!string.IsNullOrEmpty(url.Uri.AbsolutePath.Trim('/')))
						routePattern = "{eventkey}/{action}/{id}";
					else
						routePattern = "{action}/{id}";

					var routeName = ResSiteUrl.GetRouteName(e,url);
					var domainRoute = new DomainRoute(
						url.Uri.Host,
						e.ReservationType.ReservationTypeId,
						routePattern,
						new
							{
								controller = "Reservation",
								eventkey = url.Uri.AbsolutePath.Trim('/'),
								action = ReservationWizard.FirstStepName,
								id = UrlParameter.Optional
							}, new[]
								   {
									   string.Format("cfaresv2.Areas.{0}.Controllers",
									                 e.ReservationType.ReservationTypeId)
								   }
						);
					domainRoute.Name = routeName;
					routes.Add(routeName,
					           domainRoute);

					var routeNameDynamic = ResSiteUrl.GetRouteName(e,url) + "-Dynamic";
					var domainRouteDynamic = new DomainRoute(
						url.Uri.Host,
						e.ReservationType.ReservationTypeId,
						"DynamicReservation/{action}/{id}",
						new
							{
								controller = "DynamicReservation",
								eventkey = url.Uri.AbsolutePath.Trim('/'),
								action = ReservationWizard.FirstStepName,
								id = UrlParameter.Optional
							}, new[]
								   {
									   "cfaresv2.Controllers"
								   }
						);
					domainRouteDynamic.Name = routeNameDynamic;
					routes.Add(routeNameDynamic,
					           domainRouteDynamic);
				}

					// Fallback route definitions for URLs which do not match against a specific event URL
					// .. ( based on defined EventType urls in the DB )
				else if (url.ReservationType != null)
				{
					ReservationType t = url.ReservationType;
					//var routeName = string.Format("EventTypeFallback-{0}-{1}-{2}", t.ReservationTypeId, t.Name, url.SiteUrlId);
					var routeName = ResSiteUrl.GetRouteName(t, url);
					var domainRoute = new DomainRoute(
						url.Uri.Host,
						t.ReservationTypeId,
						"{action}/{id}", new
							                 {
								                 controller = "Reservation",
								                 eventkey = string.Empty, // <eventkey> unavailable in this scenario
								                 action = ReservationWizard.FirstStepName,
								                 id = UrlParameter.Optional
							                 }, new[]
								                    {
									                    string.Format("cfaresv2.Areas.{0}.Controllers",
									                                  t.ReservationTypeId)
								                    }
						);
					domainRoute.Name = routeName;
					routes.Add(routeName,
					           domainRoute);
				}
			}

	        // Admin routing ( hostName support required at this time )
			ReservationConfig c = ReservationConfig.GetConfig();
            var adminRoute = new DomainRoute(
                c.Admin.HostName,
                c.Admin.Area,
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] {"cfaresv2.Areas.Admin.Controllers"}
                );
            var adminRouteName = "Admin";
            adminRoute.Name = adminRouteName;
            routes.Add(adminRouteName, adminRoute);

            var adminRouteRootName = "AdminRoot";
			routes.MapRoute(adminRouteRootName,
					c.Admin.HostName,
					"{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
					new [] { "cfaresv2.Areas.Admin.Controllers" }
				);

            // www redirection if no other res systems were found
            routes.MapRoute("WWWRedirect",
                    "{*url}",
                    new { controller = "Meta", action = "WWW" },
                    new { url=@"^www"},
                    new[] { "cfaresv2.Controllers" }
                );

			// If no host name resolution match, 404 ( last in routing order )
            string viewKey=c.ApplicationId=="npr"?"npr":"FamilyInfluence";
            string Namespace404 = string.Format("cfaresv2.Areas.{0}.Controllers",viewKey);
	        string targetArea = string.Empty;
			if( ReservationConfig.GetConfig().ApplicationId == Application.NPR ) 
				targetArea = Application.NPR;
			routes.MapRoute( "404-PageNotFound", "{*url}",
				new { controller = "Reservation", action = "404", area = targetArea }, 
				new [] { Namespace404 });


			//// Dynamic cache invalidated routing
			//routes.MapRoute( "DynamicReservation",
			//		"{controller}/{action}/{id}",
			//		new { controller = "DynamicReservation", action = "Index", id = UrlParameter.Optional },
			//		new [] { "cfaresv2.Controllers" }
			//	);            

        }



    }

    //public class CmsUrlConstraint : IRouteConstraint
    //{
    //    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    //    {
    //        IResContext context = ReservationConfig.GetContext();
    //        var r = new ResSiteUrlRepository(context);
           
    //        if (values[parameterName] != null)
    //        {
    //            var permalink = values[parameterName].ToString();
    //            var url = httpContext.Request.Url.AbsoluteUri;                
    //            return true;
               
    //        }
    //        return false;
    //    }
    //}
}