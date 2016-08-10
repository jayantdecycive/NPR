
#region Imports

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using cfares.site.modules.com.application;

#endregion

public static class Theme
{
	#region Settings

	private const string ThemesBasePath = @"~/Content/Themes/";
	private const string ThemesImagesPath = @"/images/";

	#endregion

	#region Html Helpers

	public static MvcHtmlString ThemePath( this HtmlHelper htmlHelper )
	{
		if( AppContext.Current == null || AppContext.Current.Event == null ) 
			return new MvcHtmlString( string.Empty );

		return MvcHtmlString.Create( 
			htmlHelper.AppThemesPath() + AppContext.Current.Event.TemplateId );
	}

    public static MvcHtmlString ThemeContent(this HtmlHelper htmlHelper,string url)
    {
        if(!url.StartsWith("~"))
            return new MvcHtmlString(url);
        string templateId = "";
        if (AppContext.Current == null || AppContext.Current.Event == null)
        {
            templateId = AppContext.Current.EventType.ReservationTypeId;
        }
        else {
            templateId = AppContext.Current.Event.TemplateId;
        }
        string themeBase = htmlHelper.AppThemesPath() + templateId;
        if (themeBase.EndsWith("/")&&url.StartsWith("~/"))
            themeBase = themeBase.TrimEnd('/');
        return MvcHtmlString.Create(url.Replace("~",themeBase));
    }

	public static MvcHtmlString ThemeImage( this HtmlHelper htmlHelper, string fileName, string themeName = "" )
	{
		return ThemeImage( htmlHelper.ViewContext.RequestContext, fileName, themeName );
	}

	public static MvcHtmlString ThemeImage( this RequestContext context, string fileName, string themeName = "" )
	{
		if(  context == null || AppContext.Current == null ) return new MvcHtmlString( string.Empty );

		string imgUrl = ThemeImageUrl( context, fileName, themeName );
		if( string.IsNullOrWhiteSpace( imgUrl ) ) return new MvcHtmlString( string.Empty );

		TagBuilder img = new TagBuilder("img");
		img.Attributes[ "src" ] = imgUrl;
		return MvcHtmlString.Create( img.ToString() );
	}

	#endregion

	#region Theme Image Resolution

	public static string ThemeImageUrl( this string fileName, string themeName = "" )
	{
		return ThemeImageUrl( HttpContext.Current.Request.RequestContext, fileName, themeName );
	}

	public static string ThemeImageUrl( this RequestContext context, string fileName, string themeName = "" )
	{
		string baseUrl = ThemeImageUrlBase( context, themeName );
		if( string.IsNullOrWhiteSpace( baseUrl ) ) return string.Empty;
		return baseUrl + '/' + fileName;
	}

	public static string ThemeImageUrlBase( this RequestContext context, string themeName = "" )
	{
		if( context == null || AppContext.Current == null ) return string.Empty;

		string imgUrl;
		string imgPath;

		if( ! string.IsNullOrWhiteSpace( themeName ) )
		{
            imgUrl = context.AppThemesPath() + themeName +
                     ThemesImagesPath.TrimEnd( new[] { '/' } );
		}
		else if( AppContext.Current.Event != null )
		{
			// Image URL specific to this event's theme
			imgUrl = context.AppThemesPath() + 
				AppContext.Current.Event.TemplateId + 
				ThemesImagesPath.TrimEnd( new[] { '/' } );

			imgPath = context.HttpContext.Request.MapPath( imgUrl );

			// Check for event theme specific - else use event type fallback
			if( ! System.IO.Directory.Exists( imgPath ) ) 
				imgUrl = context.AppThemesPath() + 
					AppContext.Current.EventType.ReservationTypeId + 
					ThemesImagesPath.TrimEnd( new[] { '/' } );
		}
        else if (AppContext.Current.EventType != null)
        {
            imgUrl = context.AppThemesPath() +
                     AppContext.Current.EventType.ReservationTypeId +
                     ThemesImagesPath.TrimEnd( new[] { '/' } );
        }
        else
        {
            return string.Empty;
        }

		// Check for event type fallback - else return no image
	    imgPath = context.HttpContext.Request.MapPath( imgUrl );
		if( ! System.IO.Directory.Exists( imgPath ) ) 
			return string.Empty;

		return imgUrl;
	}

	#endregion

	#region Path Mapping Helpers

	private static string _appThemesPath;
	private static readonly object _appThemesPathLock = new object();

	private static string AppThemesPath( this HtmlHelper htmlHelper )
	{
		return AppThemesPath( htmlHelper.ViewContext.RequestContext );
	}

	private static string AppThemesPath( this RequestContext context )
	{
		if( _appThemesPath == null )
		{
			lock( _appThemesPathLock )
			{
				return _appThemesPath ?? (
					_appThemesPath = ThemesBasePath.CheckForAppRelativePath( context ) );
			}
		}
		return _appThemesPath;
	}

	private static string CheckForAppRelativePath( this string path, RequestContext context )
	{
		return ! path.Contains( "~/" ) ? path : 
			path.Replace( "~/", AppRoot( context ) );
	}

	private static string _appRoot;
	private static readonly object _appRootLock = new object();
	private static string AppRoot( this RequestContext context )
	{
		if( _appRoot == null )
		{
			lock( _appRootLock )
			{
				if( _appRoot == null )
				{
					var urlHelper = new UrlHelper( context ); // htmlHelper.ViewContext
					_appRoot = urlHelper.Content("~");
				}
				return _appRoot;
			}
		}
		return _appRoot;
	}

	#endregion
}
