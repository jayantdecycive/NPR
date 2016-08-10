
#region Imports

using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using cfacore.shared.domain._base;
using cfacore.shared.domain.user;
using cfacore.shared.modules.com.admin;
using System.Linq;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.site.modules.com.application;
using cfares.site.modules.user;
using System.Collections.Generic;
using cfaresv2.Models;

#endregion

namespace cfaresv2.Controllers
{
    public class ReservationControllerBase : ControllerBase
    {
        #region Properties

        protected Wizard<ResEvent> ReservationWizard { get; set; }

        protected ResEvent Event { get; set; }

	    protected ActionExecutingContext FilterContext;

		private IResContext _resContext;
	    protected IResContext ResContext
	    {
		    get { return _resContext ?? (_resContext = ReservationConfig.GetContext()); }
		    set { _resContext = value; }
	    }

		private UserMembershipRepository _userMembershipRepo;
	    protected UserMembershipRepository UserMembershipRepo
	    {
		    get { return _userMembershipRepo ?? (_userMembershipRepo = new UserMembershipRepository( FilterContext.HttpContext, ResContext )); }
		    set { _userMembershipRepo = value; }
	    }

		private ResEventRepository _resEventRepo;
	    protected ResEventRepository ResEventRepo
	    {
		    get { return _resEventRepo ?? (_resEventRepo = new ResEventRepository( ResContext )); }
		    set { _resEventRepo = value; }
	    }
		
		private OccurrenceRepository _occurrenceRepo;
	    protected OccurrenceRepository OccurrenceRepo
	    {
		    get { return _occurrenceRepo ?? (_occurrenceRepo = new OccurrenceRepository( ResContext )); }
		    set { _occurrenceRepo = value; }
	    }
		
		//public ICacheContext CacheContext { get; private set; }

        #endregion

        #region BeginExecute

        private void RegisterThemeText(RequestContext requestContext)
        {
            // Check for Admin / shared resolution
            if (AppContext.Current.EventType == null) return;

            // Event type default
            string themeName = AppContext.Current.EventType.ReservationTypeId;

            // Event specific theme
            if (AppContext.Current.Event != null)
                themeName = AppContext.Current.Event.Template.ResTemplateId;

			ViewData["Text"] = AppContext.Cache.Resolve( "RegisterThemeText-" + themeName, delegate {

				string contentfile = string.Format("~/Content/Text/{0}.json", themeName);
				string filePath = requestContext.HttpContext.Request.MapPath(contentfile);
				if( ! System.IO.File.Exists( filePath ) ) return null;

				JObject results;
				using (StreamReader re = System.IO.File.OpenText(filePath))
				{
					JsonTextReader reader = new JsonTextReader(re);
					var job = JObject.Load(reader);
					results = job;
					reader.Close();
					re.Close();
				}
				return results;

			} );
        }

        // GET: /Reservation/*
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
			//CacheContext = new CacheContext( AppContext.Cache );
			Event = (ResEvent)AppContext.Event;

            RegisterThemeText(requestContext);

			//var cs = (System.Web.Configuration.OutputCacheSettingsSection)System.Configuration.ConfigurationManager.GetSection("system.web/caching/outputCacheSettings");
			//int csi = cs.OutputCacheProfiles[0].Duration;

            return base.BeginExecute(requestContext, callback, state);
        }

        protected override void OnActionExecuting( ActionExecutingContext filterContext )
        {
	        FilterContext = filterContext;
            IResEvent e = AppContext.Event;

			// SH - The AppContext event is set by URL match only at this time .. While ideally i'd like to 
			// .. enhance by making the context event also autoload by "?event=115" querystring lookup, 
			// .. such a change would have a large impact, so for this fix, we're just going to leave context
			// .. as-is and provide an additional querystring based lookup here

			if( e == null ) {
				int eventId = ( filterContext.HttpContext.Request.QueryString["event"] ?? string.Empty ).ToInt();
				if( eventId > 0 ) e = ResEventRepo.FindById( eventId );
			}

			// Avoid redirect loops
			string[] systemRoutes = { "BrowserNotSupported", "ComingSoon", "EventEnded", "EventUpcoming", "EventFull", "404", "FAQ", "Receptions", "Review", "Success" };
            if ((new List<String>(systemRoutes)).Contains(filterContext.ActionDescriptor.ActionName))
                base.OnActionExecuting(filterContext);

            else if (e != null && (e.Status != ResEventStatus.Hidden && e.Status != ResEventStatus.Live))
                filterContext.Result = UrlHelpers.RedirectToAction("404");

            // IsBrowserSupported redirect via user agent parsing in App_Browsers
            else if (AppContext != null && !AppContext.IsBrowserSupported)
                filterContext.Result = UrlHelpers.RedirectToAction("BrowserNotSupported");

			// IsEventEnded check / redirect via AppContext
			else if (AppContext != null && e != null && e.IsEventEnded)
				filterContext.Result = UrlHelpers.RedirectToAction("EventEnded");

            else if (AppContext != null && e != null && e.IsEventUpcoming)
                filterContext.Result = UrlHelpers.RedirectToAction("EventUpcoming");

			else if (e != null && e.FullyBooked)
				filterContext.Result = UrlHelpers.RedirectToAction("EventFull");

            else if (AppContext.EventType!=null  && !AppContext.EventType.IsEventActive)
                filterContext.Result = UrlHelpers.RedirectToAction("EventUpcoming");

            else base.OnActionExecuting(filterContext);
        }

        #endregion

       #region Routes

        public virtual ActionResult Slot(int id)
        {
            var slotRepo = new SlotRepository(ResContext);

            var slot = slotRepo.Find(x => x.SlotId == id, "Occurrence.ResEvent");
            var eventType = slot.Occurrence.ResEvent.ReservationTypeId;

            return RedirectToAction("Register", "Reservation", new RouteValueDictionary
	                                                               {
                    
                    {"Area",eventType},
                    {"slotId",id}
                });
        }

        public ActionResult ContactUs()
        {
            var cvm = new ContactUsViewModel();
            if (AppContext.IsAuthenticated)
                cvm.User = AppContext.User;
            return View(cvm);
        }

        public virtual ActionResult City()
        {
            IList<CityStateZip> cities = null;
            if (AppContext.Event != null)
            {
                cities = AppContext.Event.ActiveOccurrences.Select(x => x.Store.StreetAddress).CityStateZips();
            }
            else if (AppContext.EventType != null)
            {
                cities = AppContext.EventType.ActiveEvents.SelectMany(x => x.ActiveOccurrences).Select(x => x.Store.StreetAddress).CityStateZips();
            }

            return View(cities);
        }

		[DynamicOutputCache]
        public ActionResult LostInvite()
        {
            return View();
        }

		[DynamicOutputCache]
        public ActionResult BrowserNotSupported()
        {
            return View(Event);
        }

		[DynamicOutputCache]
        public ActionResult ComingSoon()
        {
             return View(Event);
       }

		[DynamicOutputCache]
        public ActionResult EventEnded()
        {
			if( ! String.IsNullOrWhiteSpace( ViewData.ThemeText("UpcomingPromo", noParagraph: true).ToStringSafe() ) )
				return RedirectToAction( "ComingSoon" );

            return View(Event);
        }

		[DynamicOutputCache]
        [ActionName("EventUpcoming")]
        public ActionResult EventUpcoming()
        {
            return View(Event);
        }

		[DynamicOutputCache]
        public ActionResult EventFull()
        {
            return View(Event);
        }

		[DynamicOutputCache]
        [ActionName("404")]
        public ActionResult PageNotFound()
        {
            // RE: http://stackoverflow.com/questions/717628/asp-net-mvc-404-error-handling
            ResSiteUrlRepository repo = new ResSiteUrlRepository(ResContext);
            string url = Request.Url.Host + (!string.IsNullOrEmpty(Request.Url.PathAndQuery) ? Request.Url.PathAndQuery.Split('?')[0] : "").TrimEnd('/');
            if (url.IndexOf("www") == 0)
            {
                string urlProtocol = "http://" + url.Substring(4).TrimEnd('/');
                var otherUrl = repo.Find(x => x.Url.StartsWith(urlProtocol));
                if (otherUrl != null)
                {
                    return RedirectPermanent(otherUrl.Url);
                }

                if (url.Contains(AppContext.Configuration.Admin.HostName))
                {
                    urlProtocol = AppContext.Configuration.Admin.HostName;
                    urlProtocol = urlProtocol.StartsWith("http") ? urlProtocol : "http://" + urlProtocol;
                    return RedirectPermanent(urlProtocol);
                }
            }

            Response.StatusCode = 404;
            return View(Event);
        }

        public ActionResult Error()
        {
            return View();
        }

		[DynamicOutputCache]
        public ActionResult SiteNotAvailable()
        {
            return View();
        }

        #endregion
    }
}
