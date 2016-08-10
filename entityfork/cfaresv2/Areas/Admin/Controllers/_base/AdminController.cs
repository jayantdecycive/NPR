
#region Imports

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using ControllerBase = cfaresv2.Controllers.ControllerBase;
using cfares.site.modules.user;
using cfares.repository.store;
using System.Linq;

#endregion

namespace cfaresv2.Areas.Admin.Controllers._base
{
	public abstract class AdminController : ControllerBase
    {
        ResUser _currentUser;
        UserMembershipRepository _userRepo;

        protected override System.IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, System.AsyncCallback callback, object state)
        {
            var context = ReservationConfig.GetContext();
            _userRepo = new UserMembershipRepository(requestContext.HttpContext, context);

            /*MembershipCreateStatus stat = MembershipCreateStatus.DuplicateEmail;
            var u = userRepo.CreateUser("gabby@thefoundryagency.com", "foundry7", out stat);
            u.ApplicationUser.OperationRole = UserOperationRole.Admin;
            userRepo.Commit();*/

			AutoDetectSection( requestContext, ViewBag );

            _currentUser = _userRepo.Current;
            if (_currentUser!=null && _currentUser.OperationRole == UserOperationRole.Operator)
            {
                var opRepo = new LocationRepository(_userRepo.Context);
                string[] stores = opRepo.Get(x => x.OperatorId == _currentUser.UserId).Select(x => x.LocationNumber).ToArray();
				if( stores.Length == 0 && _currentUser.AutoDetectedLocationNumber != string.Empty )
					stores = new[] { _currentUser.AutoDetectedLocationNumber };

                ViewData["stores"] = string.Join(",", stores);
            }
            if(_currentUser!=null)
                ViewData["auth_user"] = _currentUser.UserId;

            return base.BeginExecute(requestContext, callback, state);
        }

        protected string ResolveViewString(string str,string role)
        {
            var appId = ReservationConfig.GetConfig().ApplicationId;
            if( str == "Landing" )
                return string.Format( "{0}-{1}-Landing",
                    appId[0].ToString( CultureInfo.InvariantCulture ).ToUpper()
					+ appId.Substring( 1 ), role );
            return str;
        }


      


		public static void AutoDetectSection( System.Web.Routing.RequestContext requestContext, dynamic viewBag )
		{
            IResContext context = ReservationConfig.GetContext();

			// Depending on route, set Section designator [ Global, Tours, Events ]
			if( requestContext.RouteData.Values["controller"].ToString() == "Event" &&
				requestContext.RouteData.Values["id"] != null )
			{
				ResEventRepository<ResEvent> service = new ResEventRepository<ResEvent>(context);
				int id = requestContext.RouteData.Values["id"].ToString().ToInt();
				ResEvent e = service.Find( x => x.ResEventId == id, ResEventRepository.DefaultEntityIncludes) as ResEvent;
				if( e != null )
					viewBag.Section = ( e.ReservationTypeId == "Tour" ? "Tours" : "Events" );
			}
			else if( requestContext.RouteData.Values["controller"].ToString().EndsWith( "Slot" ) &&
				requestContext.RouteData.Values["id"] != null )
			{
				SlotRepository service = new SlotRepository(context);
				int id = requestContext.RouteData.Values["id"].ToString().ToInt();
				Slot s = (Slot)service.Find( o => o.SlotId == id );
				if( s != null )
					viewBag.Section = ( s.Occurrence.ResEvent.ReservationTypeId == "Tour" ? "Tours" : "Events" );
			}
			else if( requestContext.RouteData.Values["controller"].ToString().EndsWith( "Ticket" ) &&
				requestContext.RouteData.Values["id"] != null )
			{
				TicketRepository service = new TicketRepository(context);
				int id = requestContext.RouteData.Values["id"].ToString().ToInt();
				Ticket s = (Ticket)service.Find( o => o.SlotId == id );
				if( s != null )
					viewBag.Section = ( s.Slot.Occurrence.ResEvent.ReservationTypeId == "Tour" ? "Tours" : "Events" );
			}
			else if( requestContext.RouteData.Values["controller"].ToString().EndsWith( "Ticket" ) &&
				(((requestContext.HttpContext).Request).QueryString).AllKeys.Length > 0 &&
				(((requestContext.HttpContext).Request).QueryString).AllKeys[0] == "SlotId" )
			{
				SlotRepository service = new SlotRepository(context);
				int id = ((requestContext.HttpContext).Request).QueryString["SlotId"].ToInt();
				Slot s = (Slot)service.Find( o => o.SlotId == id );
				if( s != null )
					viewBag.Section = ( s.Occurrence.ResEvent.ReservationTypeId == "Tour" ? "Tours" : "Events" );
			}
			else if( requestContext.RouteData.Values["controller"].ToString().EndsWith( "Slot" ) &&
				(((requestContext.HttpContext).Request).QueryString).AllKeys.Length > 0 &&
				(((requestContext.HttpContext).Request).QueryString).AllKeys[0] == "OccurrenceId" )
			{
				OccurrenceRepository service = new OccurrenceRepository(context);
				int id = ((requestContext.HttpContext).Request).QueryString["OccurrenceId"].ToInt();
				IOccurrence s = service.Find( o => o.OccurrenceId == id );
				if( s != null )
					viewBag.Section = ( s.ResEvent.ReservationTypeId == "Tour" ? "Tours" : "Events" );
			}
			else
			{
				string section = null;
				if( requestContext.RouteData.Values.Any(
					kv => kv.Value.ToString().IndexOf( "Request-Dash", System.StringComparison.Ordinal ) >= 0 ) ) section = "Tours";
				if( requestContext.RouteData.Values.Any(
					kv => kv.Value.ToString().IndexOf( "Tour", System.StringComparison.Ordinal ) >= 0 ) ) section = "Tours";
				if( section == null && requestContext.RouteData.Values.Any(
					kv => kv.Value.ToString().IndexOf( "Event", System.StringComparison.Ordinal ) >= 0 ) ) section = "Events";
				viewBag.Section = section ?? "Global";
			}            
		}



        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (ModelState != null && 
                (Request.AcceptTypes != null && Request.AcceptTypes.Any(x => x == "application/json")||
                !string.IsNullOrEmpty(Request.Params["iframe"])))
            {
                var errorList = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 500;
                    filterContext.Result = Json(errorList);
                }
                else if (filterContext.Result is RedirectResult)
                {
                    filterContext.Result = Json(new { success = (filterContext.Result as RedirectResult).Url });
                }
                else if (filterContext.Result is RedirectToRouteResult)
                {
                    var routeDict = (filterContext.Result as RedirectToRouteResult).RouteValues.ToDictionary(x =>x.Key,
                                                                                                             x =>x.Value);
                    routeDict["url"] =
                        Url.HttpRouteUrl((filterContext.Result as RedirectToRouteResult).RouteName,
                                         (filterContext.Result as RedirectToRouteResult).RouteValues);

                    filterContext.Result =
                        Json(new
                        {
                            success = routeDict
                        });
                }
                else if (filterContext.Result is ViewResult)
                {
                    filterContext.Result =
                        Json(new { success = (filterContext.Result as ViewResult).Model });
                }
            }
            base.OnActionExecuted(filterContext);
        }



        

    }

    public static class FormCollectionExtensions
    {
        public static Dictionary<string, TParam> PluckArray<TParam>(this FormCollection collection, string p,Func<string,TParam> exp)
        {
            string expression = p + @"\[([A-Za-z0-9_\-]+)\]";
            return collection.AllKeys.Where(x => Regex.Match(x, expression).Groups.Count > 1 && !string.IsNullOrEmpty(collection[x]))
                                     .Select(x => Regex.Match(x, expression).Groups[1].Value)
                                     .ToDictionary(x=>x,x=>exp.Invoke(collection[string.Format(p+"[{0}]",x)]));
        }

        public static Dictionary<string, string> PluckArray(this FormCollection collection, string p)
        {

            return collection.PluckArray<string>(p, x => x);
        }
    }

}