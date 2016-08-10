
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Redis;
using cfacore.domain._base;
using cfacore.shared.domain._base;
using cfares.domain._event;
using cfares.domain.store;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.site.modules.Helpers;
using cfares.site.modules.user;
using Ninject;

#endregion

namespace cfares.site.modules.com.application
{
	public class AppContext
	{
		protected const string ContextKey = "reservation-context";

		public IReservationType EventType { get; private set; }
		public IResEvent Event { get; private set; }
        public IKernel Kernel { get; private set; }
		public ReservationConfig Configuration { get; private set; }
		public string ApplicationBasePath { get; private set; }

		public AppContext()
		{
            Configuration = Cache.Resolve( "Configuration", ReservationConfig.GetConfig );
			ApplicationBasePath = Cache.Resolve( "ApplicationBasePath", () => HttpContext.Current.Server.MapPath(@"~/") );
			Uri uri = HttpContext.Current.Request.Url.TranslatePort();

			// Attempt to determine event type - If unable and *not within the Administrative context > ex
			EventType = Cache.Resolve( "EventType-" + uri.Host, delegate {
				IResContext context = Configuration.Context;
				ReservationTypeRepository rt = new ReservationTypeRepository( context );
				return rt.FindByUri( uri );
			} );

			Event = Cache.Resolve( "Event-" + uri.Host, delegate {
				IResContext context = Configuration.Context;
				ResEventRepository re = new ResEventRepository( context );
				IResEvent e = re.FindByUri( uri );
				if( e != null )
				{
					e.OrganizationName = Configuration.Organization.Name;
					e.OrganizationDisplayName = Configuration.Organization.DisplayName;
					Kernel = Cache.Resolve( "Kernel-" + uri.Host, () => e.ReservationType.GetKernel() );
				}
				return e;
			} );

			if( EventType == null && Event != null )
				EventType = Event.ReservationType;

            ReservationType.Configuration = Configuration;
		}

	    public IList<DateTime> GetDates(bool withPastEvents = true)
	    {
            if (this.Event != null){
                return this.Event.Occurrences.SelectMany(x => x.GetDates()).GroupBy(x => x).Select(x => x.Key).OrderBy(x => x).ToList();
            }
            if (!withPastEvents){
                return this.EventType.EventList.SelectMany(e => e.Occurrences.SelectMany(x => x.GetDates()).Where(x => x.Date > DateTime.Now).GroupBy(x => x).Select(x => x.Key)).OrderBy(x => x).ToList();
            }
            else{
                return this.EventType.EventList.SelectMany(e => e.Occurrences.SelectMany(x => x.GetDates()).GroupBy(x => x).Select(x => x.Key)).OrderBy(x => x).ToList();
            }
            
	    }

        public string GetLayoutLocation() {
            var typ = this.GetTypeId();
            if (typ == null)
                return "~/Views/Shared/_Layout.cshtml";
            return string.Format("~/Areas/{0}/Views/Shared/_Layout.cshtml", typ);
        }

	    public string GetTypeId()
	    {
            if(this.Event!=null)
	        return this.Event.ReservationTypeId;
            if(this.EventType!=null)
	        return this.EventType.ReservationTypeId;
            return null;
	    }

		public static IExtendedCacheClient Cache { get { return DomainObject.Cache; } }

	    public static void RegisterContext()
		{
			// TODO - Caching .. check w/ Matt regarding use of ICaching interfaces / SS modules

            HttpContext.Current.Items[ ContextKey ] = new AppContext();
		}

		public static AppContext Current 
		{ 
			get 
			{ 
				// TODO - Caching .. check w/ Matt regarding use of ICaching interfaces / SS modules
				return HttpContext.Current.Items[ ContextKey ] as AppContext;
			} 
		}

		public bool IsLoggedIn { get
		{
			if( User == null ) return false;
			return HttpContext.Current != null && 
				HttpContext.Current.User.Identity.IsAuthenticated;
		} }

		private ResUser _user;
		public ResUser User // Anonymous = null
		{
			get
			{
				return _user ?? ( _user = new UserMembershipRepository(
					new HttpContextWrapper( HttpContext.Current ),
					new CfaResContext() ).Current );
			}
		}
		
		public IEnumerable<ResStore> UserPreferredLocations // Anonymous = null
		{
			get
			{
				return Current.User == null ? null
					: Current.User.UserPreferredLocations;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return Current.User != null;
			}
		}

		public bool IsBrowserSupported { get 
		{
			if( HttpContext.Current == null ) return true;
			
			// IE8+, FF2+, GC11+, SF3+
			HttpBrowserCapabilities b = HttpContext.Current.Request.Browser;


            //carson - prevents redirect to browser not supported page for fb crawler
            string ua = HttpContext.Current.Request.UserAgent.ToLower();
            if ( ua.Contains("facebook") )
                return true;


            if (b.IsMobileDevice)
                return true;


			switch( b.Browser )
			{
				case "IE":
					return b.MajorVersion >= 7;

				case "Firefox":
					return b.MajorVersion >= 2;

				case "Chrome":
					return b.MajorVersion >= 11;

				case "Safari":
					return b.MajorVersion >= 3;

				default:
					return false;
			}
		} }
	}
}
