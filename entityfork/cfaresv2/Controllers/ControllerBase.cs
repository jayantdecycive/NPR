
#region Imports

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using cfares.site.modules.com.application;
using cfares.site.modules.com.reservations.res;

#endregion

namespace cfaresv2.Controllers
{
    public class ControllerBase : Controller
    {

        public int? ToNullableInt32(string s)
        {
            int i;
            if (Int32.TryParse(s, out i)) return i;
            return null;
        }

	    public AppContext AppContext { get; private set; }

		public ControllerBase() 
		{
			AppContext = AppContext.Current;
		}

		#region Exception Handling

		// ReSharper disable RedundantOverridenMember
		protected override void OnException( ExceptionContext filterContext )
		{
			// ReSharper disable JoinDeclarationAndInitializer
			bool showExceptionDetailsWithoutFormatting = false;
			// ReSharper restore JoinDeclarationAndInitializer

			#if DEBUG
				showExceptionDetailsWithoutFormatting = true;
			#endif
			#if QA
				showExceptionDetailsWithoutFormatting = false;
			#endif
			#if STAGING
				showExceptionDetailsWithoutFormatting = false;
			#endif

			// ReSharper disable ConditionIsAlwaysTrueOrFalse
			if( showExceptionDetailsWithoutFormatting )
			{
				base.OnException( filterContext );
				return;
			}
			// ReSharper restore ConditionIsAlwaysTrueOrFalse

			// RE: http://stackoverflow.com/questions/812235/error-handling-in-asp-net-mvc

			// Bail if we can't do anything; app will crash.
			// ReSharper disable HeuristicUnreachableCode
			if (filterContext == null) return;
				// since we're handling this, log to elmah

			var ex = filterContext.Exception ?? new Exception("No further information exists.");
			LogException(ex);

			filterContext.ExceptionHandled = true;
			HandleErrorInfo data = new HandleErrorInfo(ex,
				(filterContext.RouteData.Values["Controller"] ?? string.Empty).ToString(),
				(filterContext.RouteData.Values["Action"] ?? string.Empty).ToString());

			if( AppContext.Configuration.ApplicationId == Application.NPR )
				filterContext.Result = new RedirectResult( string.Format( "~/npr/Home/Error" +
					"?Controller={0}&Action={1}&Ex={2}",
					data.ControllerName, data.ActionName, 
					data.Exception.ToString().Replace( Environment.NewLine, "--" ) ) );
			else		
				filterContext.Result = View("Error", data);
			// ReSharper restore HeuristicUnreachableCode
		}
		// ReSharper restore RedundantOverridenMember

		// ReSharper disable UnusedParameter.Local
	    private void LogException(Exception exception)
	    {
		    // TODO - SH - Exception logging ( log4net )
	    }
		// ReSharper restore UnusedParameter.Local

		#endregion

		#region Wizard Support

		public Wizard<ITicket> InitializeWizard( string key, ITicket ticket ) { 
            return InitializeWizard( ticket, key, ControllerContext.RequestContext, ViewData );
        }

        public Wizard<ITicket> InitializeWizard(ITicket ticket, string key, RequestContext request, ViewDataDictionary viewData)
        { 
            var wiz= InitializeWizard( ticket, key, viewData );
		    return wiz;
		}

        public Wizard<T> InitializeWizard<T>(T model, string key, ViewDataDictionary viewData) // where T : DomainObject, new()
        {
            return InitializeWizard(model, key, null, viewData);
        }

        public Wizard<T> InitializeWizard<T>(T model, string key, RequestContext  request, ViewDataDictionary viewData)
        {
            IResEvent evt;
            if (key.ToLower().StartsWith("admin"))
            {
                evt = (model is IResEvent
                           ? (IResEvent) model
                           : (model is IResEventBound
                                  ? ((IResEventBound) model).GetEvent()
                                  : AppContext.Event));
            }
            else
            {
                evt = (model is IResEvent
                           ? (IResEvent)model
                                  : AppContext.Event);
            }
            Wizard<T> wiz = (Wizard<T>)GetWizard( evt,
                model, ReservationConfig.GetConfig().ApplicationId, key);
            
            wiz.SetStep(key);
            viewData["wizard"] = wiz;
            return wiz;
        }

		public Wizard<IResEvent> InitializeWizard( string key, IResEvent resEvent ) { 
            return InitializeWizard( resEvent, key, ControllerContext.HttpContext.Request, ViewData );
        }

		public Wizard<IResEvent> InitializeWizard(IResEvent resEvent, string key, HttpRequestBase request, ViewDataDictionary viewData) { 
            return InitializeWizard(resEvent, key, viewData);
        }

        public Wizard<IOccurrence> InitializeWizard(IOccurrence occurrence, string key, HttpRequestBase request, ViewDataDictionary viewData)
        {
            return InitializeWizard(occurrence, key, viewData);
        }

        public Wizard<IResEvent> InitializeWizard(IResEvent resEvent, string key, ViewDataDictionary viewData)
        {
            return InitializeWizard<IResEvent>(resEvent,key,viewData);
        }

      

		public IWizard<T> GetWizard<T>( IResEvent e, T model, string applicationId, string view ) // where T : DomainObject, new()
		{
            string wizardId = "event." + applicationId;
			string eventType;
            if (e != null)
                eventType = e.ReservationTypeId;
            else if (model is IResEventBound && ((IResEventBound)model).HasEvent())
                eventType = ((IResEventBound)model).GetEvent().ReservationTypeId;
            else if (AppContext.EventType != null)
                eventType = AppContext.EventType.ReservationTypeId;
            else
                eventType = "";

			// SH - Commenting out for NPR / all systems ( applications should now all function the same wizard-factory-wise )
			//switch( applicationId ) {
			//	case "res":   
			//	default:
			//		return (IWizard<T>) new CreateTourWizard(e, wizardId); }
  
			// Admin control wizard system - Underpinnings based on <Event Type> and <Entity Type> ( e.g. "Event" vs. "Occurence" )
			if (view.StartsWith("admin.", StringComparison.OrdinalIgnoreCase))
			{
				string entityTypeName = typeof (T).Name;
				if( typeof(T).IsInterface && entityTypeName.StartsWith( "i", StringComparison.OrdinalIgnoreCase ) ) 
					entityTypeName = entityTypeName.Substring( 1 ).Replace( "ResEvent", "Event" );

				string adminNs = "cfares.site.modules.com.admin.res";
				string adminTypeName = string.Format("Create{0}{1}Wizard", eventType.Substring(0,1).ToUpper()+eventType.Substring(1), entityTypeName );
				Type adminType = typeof( IReservationWizard ).Assembly.GetType( adminNs + '.' + adminTypeName );

			    if (adminType == null)
			    {
			        adminNs = "cfares.site.modules.com.admin." + applicationId;
			        adminType = typeof (IReservationWizard).Assembly.GetType(adminNs + '.' + adminTypeName);
			    }
			    if( adminType == null )
					throw new ApplicationException( string.Format( "Unable to locate wizard [ {0} ]", adminNs + '.' + adminTypeName ) );
					
				if( entityTypeName == "Event" )
					return (IWizard<T>) Activator.CreateInstance( adminType, e, wizardId );

				return (IWizard<T>) Activator.CreateInstance( adminType, model, wizardId );
			}

			// Primary reservation system - Wizard underpinnings based on <Event Type>
			// .. e.g. cfares.site.modules.com.reservations.res.FamilyInfluenceReservationWizard
            string ns = "cfares.site.modules.com.reservations." + applicationId;
			string typeName = eventType + "ReservationWizard";
			Type type = typeof( IReservationWizard ).Assembly.GetType( ns + '.' + typeName );
			if( type == null )
			{
				typeName = "ReservationWizard";
				type = typeof( IReservationWizard ).Assembly.GetType( ns + '.' + typeName );
				if( type == null )
					throw new ApplicationException( string.Format( "Unable to locate wizard [ {0} ]", ns + '.' + typeName ) );
			}
					
			return (IWizard<T>) Activator.CreateInstance( type, model, wizardId,Request.QueryString.ToString() );
		}

		#endregion
	}
}
