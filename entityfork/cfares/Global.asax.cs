using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using cfares.infrastructure;
using cfares.Global;
using Ninject.Modules;
using Ninject;
using cfacore.site.controllers._event;
using cfares.domain._event;
using cfares.domain._event.resevent;
using System.Text.RegularExpressions;
using MvcDomainRouting.Code;



namespace cfares
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    internal class MyNinjectModules : NinjectModule
    {
        public override void Load()
        {
            //addBindings here
        }
    }

    

    public class MvcApplication : System.Web.HttpApplication
    {
        private IKernel _kernel = new StandardKernel(new MyNinjectModules());

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new BrowserFilterAttribute());
            filters.Add(new ReservationFilterAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /*

            routes.MapRoute(
                    "Tours_root", // Route name
                    "{controller}/{action}/{id}", // URL with parameters
                    new { controller = "Reservation", action = "BeginReservation", id = UrlParameter.Optional }, // Parameter defaults,
                    new string[] { "cfares.Areas.tours.Controllers" }
                ).DataTokens.Add("Area", "tours");*/
        }        

        public static void RegisterReservationInstanceRoutes(RouteCollection routes)
        {

            
            string subdomain = GetSubDomain(HttpContext.Current.Request.Url);
            if (string.IsNullOrEmpty(subdomain))
                subdomain = HttpContext.Current.Request.QueryString["subdomain"];
            if (string.IsNullOrEmpty(subdomain))
                subdomain = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("default-subdomain");

            //we should be able to clean this up, though we may not have to
            ReservationContextHint hint = (ReservationContextHint)HttpContext.Current.Cache["reservation-route-" + subdomain];
            if (hint != null) {
                HttpContext.Current.Items["reservation-context"] = hint;
                return;
            }

            string AreaName = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("default-area");
            string Controller = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("default-controller");
            string Action = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("default-action");
            string subdomainMode = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("subdomain-mode");
            
            


            string subdomainExpression = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("subdomain-regex");
            Match evaluatedDomain = Regex.Match(subdomain, subdomainExpression);

            
            string media = "desktop";

            string eventId = subdomain;
            string eventName = subdomain;
            
            if (evaluatedDomain.Success)
            {
                if (evaluatedDomain.Groups.Count > 0)
                    eventId = evaluatedDomain.Groups[evaluatedDomain.Groups.Count - 1].Value;

                if (evaluatedDomain.Groups.Count > 1)
                    media = evaluatedDomain.Groups[evaluatedDomain.Groups.Count - 2].Value.Replace(".","");
            }

            if (subdomainMode == "template-type")
            {//Event Type
                ResEventTemplateService serv = new ResEventTemplateService();

                ResTemplate template;
                template = serv.DeCacheOrLoad(eventId);
                
                //TODO cache template and make globally accessible
                eventName = template.ResTemplateId;
                AreaName = eventId;
            }
            else if (subdomainMode == "theme")
            {//Event Theme
                ResEventThemeService serv = new ResEventThemeService();

                ResEventTheme theme;
                theme = serv.DeCacheOrLoad(eventId);

                //TODO cache template and make globally accessible
                if(theme!=null)
                switch(media){
                    case "m":
                    case "mobile":                    
                        AreaName = theme.MobileTemplateId;
                        break;
                    case "tablet":
                    case "t":                    
                        AreaName = theme.TabletTemplateId;
                        break;
                    case "desktop":
                    default:
                        AreaName = theme.TemplateId;
                        break;
                }

                
            }
            else
            { //Event Name
                ResEventService serv = new ResEventService();
                ResEvent evnt = serv.DeCacheOrLoadByName(eventId);
                //TODO cache event and make globally accessible
                AreaName = evnt.TemplateName;

                switch (media)
                {
                    case "m":
                        media = "mobile";
                        break;                    
                    case "t":
                        media = "tablet";
                        break;                    
                    default:                        
                        break;
                }

                switch(media){                    
                    case "mobile":                    
                        AreaName = evnt.MobileTemplateName;
                        break;
                    case "tablet":                                     
                        AreaName = evnt.TabletTemplateName;
                        break;
                    case "desktop":
                    default:
                        AreaName = evnt.TemplateName;
                        break;
                }
                eventName = evnt.UrlName;
                eventId = evnt.Id();
            }

            string siteMode = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("site-mode");
            hint = new ReservationContextHint ( media, eventId, eventName,siteMode );
            
            HttpContext.Current.Cache["reservation-route-" + subdomain] = hint; 
            HttpContext.Current.Items["reservation-context"] = hint;
            string ControllerType = string.Format("cfares.Areas.{0}.Controllers",AreaName);

            try {
                SubdomainRoute domainRoute = new SubdomainRoute(
                         // Route name
                        //string.Format("{0}",subdomain),// URL with parameters                        
                        string.Format("{0}", subdomain),// URL with parameters                        
                        "{controller}/{action}/{id}",
                        new { controller = Controller, action = Action, id = UrlParameter.Optional, area=AreaName }, // Parameter defaults,                 
                        new string[]{ControllerType}
                    );

                //domainRoute.DataTokens = new System.Web.Routing.RouteValueDictionary();
                //domainRoute.DataTokens.Add("Area",AreaName);
                //domainRoute.DataTokens.Add("namespaces", new string[] { ControllerType });
                routes.Add(subdomain+"_root", domainRoute);//.DataTokens.Add("Area", AreaName);
            }catch(Exception ex){
                Console.Write(ex.Message);
            }
        }

        /// Retrieves the subdomain from the specified URL.
        /// </summary>
        /// <param name="url">The URL from which to retrieve the subdomain.</param>
        /// <returns>The subdomain if it exist, otherwise null.</returns>
        private static string GetSubDomain(Uri url)
        {
            if (url.HostNameType == UriHostNameType.Dns)
            {
                string host = url.Host;
                if (host.Split('.').Length > 2)
                {
                    int lastIndex = host.LastIndexOf(".");
                    int index = host.LastIndexOf(".", lastIndex - 1);
                    return host.Substring(0, index);
                }
            }

            return null;
        }

        protected void Application_BeginRequest() {
            RegisterReservationInstanceRoutes(RouteTable.Routes);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}