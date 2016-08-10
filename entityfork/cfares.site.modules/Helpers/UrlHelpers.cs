
#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using cfares.domain._event;
using cfares.domain._event.resevent;
using cfares.site.modules.Helpers;
using cfares.site.modules.com.application;
using cfares.site.modules.com.reservations.res;

#endregion

public static class UrlHelpers
{
	public static string GetReferrerUrl()
	{
		if( HttpContext.Current == null ) return string.Empty;
		if( HttpContext.Current.Request.UrlReferrer != null )
			if( ! HttpContext.Current.Request.UrlReferrer.AbsoluteUri.ToLower().Contains( "logon" ) )
				return HttpContext.Current.Request.UrlReferrer.AbsoluteUri;

		UrlHelper h = new UrlHelper( HttpContext.Current.Request.RequestContext );
		return h.Action( "Index", "Home" );
	}

	// URI Reference
	// RE: http://www.cambiaresearch.com/articles/53/how-do-i-get-paths-and-url-fragments-from-the-httprequest-object

	public static string GetEventUrlFromContext( this ICollection<ResSiteUrl> urls, bool throwExceptionOnNotFound = false )
	{
		if( HttpContext.Current == null ) throw new ApplicationException( 
			"HTTP context is required for URL resolution" );

		string currentUrl = GetFullUrlFromContext( HttpContext.Current.Request.RawUrl );

		// Pattern match against closest to current
		foreach( ResSiteUrl resSiteUrl in urls )
			if( currentUrl.IndexOf( resSiteUrl.Url, StringComparison.OrdinalIgnoreCase ) > -1 )
				return resSiteUrl.Url;

		// Else select 1st url in set
		foreach( ResSiteUrl resSiteUrl in urls )
			return resSiteUrl.Url;

		if( throwExceptionOnNotFound )
			throw new ApplicationException( "Unable to resolve event URL from current context" );

		return String.Empty;
	}

	public static bool UriMatchByHostName( this Uri uri, Uri uriToCompare ) 
	{
		// Cast to string to avoid potential host name variance
		Uri u1 = new Uri( uri.ToString() );
		Uri u2 = new Uri( uriToCompare.ToString() );

		return u1.FormatUrlBase().Equals( u2.FormatUrlBase(), 
			StringComparison.OrdinalIgnoreCase );
	}

	public static string FormatUrlBase( this Uri uri )
	{
		return String.Format("{0}{1}{2}{3}",
			uri.Scheme,
			Uri.SchemeDelimiter,
			uri.Host,
			uri.Port == 80 ? String.Empty : ":" + uri.Port );
	}

	public static string FormatUrlBaseAndPath( this Uri uri )
	{
		return String.Format("{0}{1}{2}{3}{4}",
			uri.Scheme,
			Uri.SchemeDelimiter,
			uri.Host,
			uri.Port == 80 ? String.Empty : ":" + uri.Port,
			uri.AbsolutePath );
	}

    public static Uri ToUri( this string uriAsString )
    {
		if( HttpContext.Current != null && uriAsString.StartsWith( "/" ) )
			uriAsString = HttpContext.Current.Request.Url.FormatUrlBase() + uriAsString;
		
		return new Uri( uriAsString );
	}

    public static Uri AppendToPath( this string uriAsString, string value )
    {
		return AppendToPath( uriAsString.ToUri(), value );
	}

    public static Uri AppendToPath( this Uri uri, string value )
    {
		if( string.IsNullOrWhiteSpace( value ) ) return uri;
        return new Uri( 
			FormatUrlBaseAndPath( uri ).TrimEnd( new[] { '/' } ) + 
			'/' + value + uri.Query );
    }

	public static string CurrentRawUrl()
	{
		HttpRequest r = HttpContext.Current.Request;
		return String.Format( "{0}://{1}{2}", r.Url.Scheme, r.Headers["host"], r.RawUrl );
	}

	public static string CurrentRootUrl()
	{
		HttpRequest r = HttpContext.Current.Request;
		return String.Format( "{0}://{1}", r.Url.Scheme, r.Headers["host"] );
	}

	public static string GetFullUrlFromContext( this string url )
	{
		if( url.StartsWith( "http", StringComparison.OrdinalIgnoreCase ) ) return url;
		if( HttpContext.Current == null || HttpContext.Current.Request.Url == null ) 
			throw new ApplicationException( "HTTP context is required for URL resolution" );

		return HttpContext.Current.Request.Url.TranslatePort().FormatUrlBase() 
			+ '/' + url.TrimStart( new[] { '/' } );
	}

	public static string ActionUrlEventHome()
	{
		return ActionUrl( ReservationWizard.FirstStepName );
	}

	public static ActionResult RedirectToEventHome()
	{
	    var u = ActionUrlEventHome();
	    if (string.IsNullOrEmpty(u))
	        u = null;
		return new RedirectResult( u??"/Account" );
	}

	public static ActionResult RedirectToArea( this ControllerContext currentAreaContext )
	{
		if( currentAreaContext.RouteData.DataTokens["area"] == null ) 
			throw new ApplicationException( "Unable to detect area from view context" );

		string area = currentAreaContext.RouteData.DataTokens["area"].ToString();
		return RedirectToArea( area );
	}

	public static ActionResult RedirectToAction( string areaName, string actionName, string controllerName )
	{
		return RedirectToArea( areaName, actionName, controllerName );
	}

	public static ActionResult RedirectToArea( string areaName )
	{
		return RedirectToArea( areaName, "Index", "Home" );
	}

	public static ActionResult RedirectToArea( this string areaName, string actionName, string controllerName )
	{
		if( HttpContext.Current == null ) 
			throw new ApplicationException( "HTTP context is required for URL resolution" );

		UrlHelper h = new UrlHelper( HttpContext.Current.Request.RequestContext );
		string url = h.Action( "Index", "Home", new { Area = areaName } );
		return new RedirectResult( url );
	}

	public static ActionResult RedirectToAction( string actionName, bool shared = false )
	{
		return new RedirectResult( ActionUrl( actionName, shared ) );
	}

	public static string ActionUrl( this string actionName, bool shared = false )
	{
		if( AppContext.Current == null )
			return ActionUrl( actionName, null, null, shared );
		else
			return ActionUrl( actionName, AppContext.Current.EventType, AppContext.Current.Event, shared );
	}

	public static string ActionUrl( this string actionName, IResEvent resEvent, bool shared = false )
	{
		return ActionUrl( actionName, resEvent.ReservationType, resEvent, shared );
	}

	public static string ActionUrl( this string actionName, IReservationType eventType, 
		IResEvent resEvent, bool shared = false )
	{
		if( HttpContext.Current == null || HttpContext.Current.Request.Url == null ) 
			throw new ApplicationException( "HTTP context is required for URL resolution" );

		UrlHelper url = new UrlHelper( HttpContext.Current.Request.RequestContext );
		if( shared ) return url.Action( actionName, "Reservation", new { area = "" } ).TranslatePort().ToString();

		/*if( eventType == null ) 
			throw new ApplicationException( "Unable to resolve event type from URL - " + 
				"Please check the website configuration in Admin" );*/

		//mdrake: need a way to handle admin urls
		string originalUrl;
	    if (eventType != null)
	    {
	        originalUrl = GetFullUrlFromContext(url.Action(actionName, "Reservation",
				new {area = eventType.ReservationTypeId}));
	    }
	    else
	    {
            //originalUrl = GetFullUrlFromContext(url.Action(actionName, "Reservation",
            //    new { area = ReservationConfig.GetConfig().Admin.Area }));
            originalUrl = GetFullUrlFromContext(url.Action(actionName, "Reservation",
             new { area = "npr" }));
	    }

	    // Event Mapping
		if( resEvent != null )
		{
			string eventUrl = GetEventUrlFromContext( resEvent.SiteUrls );
			if( ! String.IsNullOrWhiteSpace( eventUrl ) )
			{
				// Mapping
				// .. "http://<domain>/<eventType>/Reservation/SearchByLocation" ( orig )
				// >> "http://<domain>/Atlanta/SearchByLocation" ( event )
					
				// Replace "http://<domain>/<eventType>/Reservation/"
				// .. with "<eventUrl>".TrimEnd() + '/'

				// TODO - Considerations for configured URLs without domains in DB
				string targetUrl = originalUrl.Replace( GetFullUrlFromContext( "/" ) + 
					resEvent.ReservationType.ReservationTypeId + "/Reservation/",
					eventUrl.TrimEnd( new[] { '/' } ) + '/' );

				return targetUrl.TranslatePort().ToString();
			}
		}

		// Event Type Mapping
		string eventTypeUrl = string.Empty;
        if (eventType!=null)
            eventTypeUrl = GetEventUrlFromContext( eventType.SiteUrls );

		if( ! String.IsNullOrWhiteSpace( eventTypeUrl ) )
		{
			// Mapping
			// .. "http://<domain>/<eventType>/Reservation/SearchByLocation" ( orig )
			// >> "http://<domain>/Atlanta/SearchByLocation" ( event )
					
			// Replace "http://<domain>/<eventType>/Reservation/"
			// .. with "<eventUrl>".TrimEnd() + '/'

			// TODO - Considerations for configured URLs without domains in DB
			Debug.Assert(eventType != null, "eventType != null");
			string targetUrl = originalUrl.Replace( GetFullUrlFromContext( "/" ) + 
				eventType.ReservationTypeId + "/Reservation/",
				eventTypeUrl.TrimEnd( new[] { '/' } ) + '/' );

			return targetUrl.TranslatePort().ToString();
		}

	    if (eventType == null)
	        return originalUrl ?? string.Empty;

		return url.Action( actionName, "Reservation",
			new { area = eventType.ReservationTypeId } ).TranslatePort().ToString();
	}
}
