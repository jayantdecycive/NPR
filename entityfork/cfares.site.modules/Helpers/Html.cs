
#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RazorEngine;
using RazorEngine.Templating;
using cfacore.domain._base;
using cfacore.shared.domain.media;
using cfacore.shared.modules.helpers;
using cfacore.shared.modules.repository;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.site.modules.com.application;

#endregion

public static class Html
{
	//public static string Partial()
	//{
	//	Controller controller;
	//	IView view;
	//	HtmlHelper h = GetHtmlHelper(controller, view);
	//	return h.RenderPartial()
	//}
	public static void RenderPart(this TemplateBase template, string path, object model = null)
	{
		string o = Razor.Parse(File.ReadAllText(path), model);
		template.Write(o);
	}

	public static string Render(this TemplateBase template, string path, object model = null)
	{
		return Razor.Parse(File.ReadAllText(path), model);
	}

	//public static void RenderPart<T>(this TemplateBase<T> template, string path, T model)
	//{
	//	template.Write(Razor.Parse(File.ReadAllText(path), model));
	//}

	public static HtmlHelper GetHtmlHelper(this Controller controller, IView view)
	{
		var viewContext = new ViewContext(controller.ControllerContext, view, 
			controller.ViewData, controller.TempData, TextWriter.Null);
		return new HtmlHelper(viewContext, new ViewPage());
	}

	public static string ToHtml(this string value)
    {
        return HttpUtility.HtmlEncode(value).Replace(Environment.NewLine, "<br />");
    }

    public static SelectList RepositorySelectList<T, TM, TK>
        (this HtmlHelper html, Expression<Func<TM, bool>> predicate=null, Func<TM,object> select=null,IResContext context = null) 
        where
        TM:DomainObject
        where T : GenericRepository<IResContext, TM, TK>, new()
    {
        var repo = new T{Context=context??ReservationConfig.GetContext()};
	    IQueryable<TM> query = predicate == null ? repo.GetAll() : repo.Get(predicate);
        var selectClause = select ?? (x =>
                                      new { Value = x.Id(), Name = x.ToString() });
        return new SelectList(query.ToList().Select(selectClause),
            "Value","Name"
        );
    }

    public static bool ViewExists(this HtmlHelper html,string name)
    {
        ViewEngineResult result = ViewEngines.Engines.FindView(html.ViewContext, name, null);
        return (result.View != null);
    }

    public static MvcHtmlString Media(this HtmlHelper html, int modelId, bool original = false, int? width = null, int? height = null, bool crop = true, object htmlAttributes = null)
    {
        WebMediaRepository repo = new WebMediaRepository(ReservationConfig.GetContext());
        
        return html.Media(repo.Find(modelId) as Media, original,width,height,crop,htmlAttributes);
    }

    public static MvcHtmlString Media(this HtmlHelper html, Media model, bool original=false,int? width=null, int? height=null, bool crop=true, object htmlAttributes=null)
    {
        WebMediaRepository repo = new WebMediaRepository(ReservationConfig.GetContext());
        if (model == null)
        {
            return new MvcHtmlString(string.Empty);
        }
        string url = !original ? repo.GetS3Thumb(model, crop, width, height) : model.MediaUriStr;
		if( string.IsNullOrWhiteSpace( url ) ) return new MvcHtmlString( string.Empty );

        var internalAttributes = new { data_original_width = model.Width, data_original_height = model.Height };
        return html.Image(url, model.Description, htmlAttributes, internalAttributes);
    }

    public static MvcHtmlString QrCode(this HtmlHelper html, string content, int size = 300, object htmlAttributes = null)
    {
        
        string url = WebMediaRepository.QrCodeUrl(content,size).ToString();
        if (string.IsNullOrWhiteSpace(url)) return new MvcHtmlString(string.Empty);
        
        return html.Image(url, content, htmlAttributes);
    }

    public static bool PartialExists(this HtmlHelper html, string name)
    {
        var controllerContext = html.ViewContext.Controller.ControllerContext;
        var result = ViewEngines.Engines.FindView(controllerContext, name, null);
            return result.View != null;
    }

    public static Type GetPOCO(this HtmlHelper html, Type baseType)
    {
	    return Regex.Match(baseType.Name, @"_[A-F0-9]{64}$").Success ? 
			baseType.BaseType : baseType;
    }

    public static MvcHtmlString ThemeText(this HtmlHelper htmlHelper, string key,bool plain=false,bool noParagraph=false)
    {
	    return ThemeText( htmlHelper.ViewData, key, plain, noParagraph );
    }
	
	private static MarkdownSharp.Markdown _markdown;
    public static MvcHtmlString ThemeText(this ViewDataDictionary viewData, string key,bool plain=false,bool noParagraph=false)
    {
        if (viewData["Text"] == null) return null;
        JObject obj = viewData["Text"] as JObject;
        if (obj == null) return null;
        string str = obj[key].ToStringSafe();
        if ( ! str.IsValid() ) return null;

        if (!plain)
        {
            _markdown = _markdown??new MarkdownSharp.Markdown();
            str = _markdown.Transform(str);
            if (noParagraph)
            {
                str = Regex.Replace(str, "^<p>", "");
                str = Regex.Replace(str, @"</p>$", "");
            }
        }
        return new MvcHtmlString(str);
    }

    public static JToken ThemeObject(this HtmlHelper htmlHelper, string key)
    {
        if (htmlHelper.ViewData["Text"] == null)
            return null;
        JObject obj = htmlHelper.ViewData["Text"] as JObject;
        return (obj[key]);
    }

    public static TObject ThemeObject<TObject>(this HtmlHelper htmlHelper, string key)
    {
        if (htmlHelper.ViewData["Text"] == null)
            return default(TObject);
        JObject obj = htmlHelper.ViewData["Text"] as JObject;
        return (TObject)JsonConvert.DeserializeObject(obj[key].ToString(), typeof(TObject));
    }

    public static MvcHtmlString RegisterThemeStylesheets(this HtmlHelper htmlHelper, bool throwIfNotFound = false)
    {
        if( AppContext.Current == null ) return new MvcHtmlString( string.Empty );

		// Event type default
	    string themeName = AppContext.Current.Configuration.ApplicationId;
        if (AppContext.Current.EventType != null)
        {
            themeName = AppContext.Current.EventType.ReservationTypeId;
        }

        // Event specific theme
		if( AppContext.Current.Event != null )
			themeName = AppContext.Current.Event.TemplateId;
        return htmlHelper.RegisterThemeStylesheets(themeName,throwIfNotFound);
    }

    public static MvcHtmlString RegisterThemeStylesheets( this HtmlHelper htmlHelper, string themeName,bool throwIfNotFound = false )
	{
		// SH - Using Application ID in the event that EventType is unavailable
		// Check for Admin / shared resolution
		// if( AppContext.Current.EventType == null )
			// return MvcHtmlString.Create( string.Empty );
			// throw new ApplicationException( "Unable to resolve required event type within application context" );

		

		string stylesheet = string.Format("~/Content/Themes/{0}/Screen.css", themeName);
		string filePath = htmlHelper.ViewContext.RequestContext
			.HttpContext.Request.MapPath( stylesheet );

		if( ! File.Exists( filePath ) ) 
			if( throwIfNotFound && AppContext.Current.Event != null )
				throw new ApplicationException( string.Format( 
					"Unable to locate event specific theme [ Event = {0}, " + 
					"Theme Name = {1}, Stylesheet = {2}, Resolved Path = {3} ]",
					AppContext.Current.Event.Name, AppContext.Current.Event.Template.Name,
					stylesheet, filePath ) );
			else
			{
				// Event / event type specific theme not found, attempt to reference parent
				if( AppContext.Current.Event != null )
				{
					themeName = AppContext.Current.EventType != null ? 
						AppContext.Current.EventType.ReservationTypeId : 
						AppContext.Current.Configuration.ApplicationId;

					stylesheet = string.Format("~/Content/Themes/{0}/Screen.css", themeName);
					filePath = htmlHelper.ViewContext.RequestContext
						.HttpContext.Request.MapPath( stylesheet );
	
					// If stylesheet *still not found, use global
					if( ! File.Exists( filePath ) ) 
						stylesheet = "~/Content/Screen.css";
				}
				else
					stylesheet = "~/Content/Screen.css";
			}

		// Special case for NPR where base-level stylesheet is not to be used or included
		if( AppContext.Current.Configuration.ApplicationId == Application.NPR
			&& stylesheet == "~/Content/Screen.css" ) stylesheet = string.Empty;

		if( ! stylesheet.IsValid() ) return new MvcHtmlString( string.Empty );

		UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
		string s = string.Format(
			@"<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />",
			urlHelper.Content( stylesheet ) );

		return MvcHtmlString.Create( s );
	}

    public static MvcHtmlString Image(this HtmlHelper helper, string src, string alt="", object htmlAttributes = null, object internalAttributes = null)
    {
        var builder = new TagBuilder( "img" );
        builder.MergeAttribute( "src", src );

		if( ! string.IsNullOrWhiteSpace( alt ) )
	        builder.MergeAttribute( "alt", alt );
	
		/*if( ! string.IsNullOrWhiteSpace( height ) )
			builder.MergeAttribute( "height", height );
            
		if( ! string.IsNullOrWhiteSpace( width ) )
			builder.MergeAttribute( "width", width );
            
		if( ! string.IsNullOrWhiteSpace( @class ) )
			builder.MergeAttribute( "class", @class );*/

        if (htmlAttributes != null)
            builder.MergeAttributes(
                (
                new RouteValueDictionary(htmlAttributes).ToDictionary(x=>x.Key.Replace("_","-"),x=>x.Value)
                )
            );

        if (internalAttributes != null)
            builder.MergeAttributes(
                (
                new RouteValueDictionary(internalAttributes).ToDictionary(x => x.Key.Replace("_", "-"), x => x.Value)
                )
            );
        
		return ( builder.ToMvcHtmlString( TagRenderMode.SelfClosing ) );
    }

	public static MvcHtmlString DescriptionFor<TModel, TValue>(
		this HtmlHelper<TModel> helper, 
		Expression<Func<TModel, TValue>> expression )
	{
		// RE: http://stackoverflow.com/questions/4973830/extract-display-name-and-description-attribute-from-within-a-html-helper

		ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
		string description = metadata.Description;
		if( string.IsNullOrWhiteSpace( description ) ) return new MvcHtmlString( string.Empty );

		return helper.LabelFor( expression, description, new { @class = "description" } );
	}

	public static MvcHtmlString DescriptionFor<TModel, TValue>(
		this HtmlHelper<TModel> helper, 
		Expression<Func<TModel, TValue>> expression, string descriptionText )
	{
		return helper.LabelFor( expression, descriptionText, new { @class = "description" } );
	}

    public static MvcHtmlString LabelFor<TModel, TValue>( this HtmlHelper<TModel> html, 
		Expression<Func<TModel, TValue>> expression, object htmlAttributes )
    {
        return html.LabelFor(expression, null, htmlAttributes);
    }

    public static MvcHtmlString LabelAndDescriptionFor<TModel, TValue>( this HtmlHelper<TModel> html, 
		Expression<Func<TModel, TValue>> expression )
    {
        return MvcHtmlString.Create( 
			html.LabelFor( expression, null ) +
			html.DescriptionFor( expression ).ToString() );
    }

    public static MvcHtmlString LabelAndDescriptionFor<TModel, TValue>( this HtmlHelper<TModel> html, 
		Expression<Func<TModel, TValue>> expression, string labelText )
    {
        return MvcHtmlString.Create( 
			html.LabelFor( expression, labelText, null ) +
			html.DescriptionFor( expression ).ToString() );
    }

    public static MvcHtmlString LabelAndDescriptionFor<TModel, TValue>( this HtmlHelper<TModel> html, 
		Expression<Func<TModel, TValue>> expression, string labelText, string descriptionText )
    {
        return MvcHtmlString.Create( 
			html.LabelFor( expression, labelText, null ) +
			html.DescriptionFor( expression, descriptionText ).ToString() );
    }

    public static MvcHtmlString LabelFor<TModel, TValue>( this HtmlHelper<TModel> html, 
		Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
    {
        return html.LabelHelper(
            ModelMetadata.FromLambdaExpression(expression, html.ViewData),
            ExpressionHelper.GetExpressionText(expression),
            HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
            labelText);
    }

    private static MvcHtmlString LabelHelper( this HtmlHelper html, ModelMetadata metadata, 
		string htmlFieldName, IDictionary<string, object> htmlAttributes, string labelText = null )
    {
        string str = labelText
            ?? (metadata.DisplayName
            ?? (metadata.PropertyName
            ?? htmlFieldName.Split(new[] { '.' }).Last()));

        if (string.IsNullOrEmpty(str))
            return MvcHtmlString.Empty;

        TagBuilder tagBuilder = new TagBuilder("label");
        tagBuilder.MergeAttributes(htmlAttributes);
        tagBuilder.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
        tagBuilder.SetInnerText(str);

        return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
    }

    private static MvcHtmlString ToMvcHtmlString( this TagBuilder tagBuilder, TagRenderMode renderMode )
    {
        return new MvcHtmlString(tagBuilder.ToString(renderMode));
    }
}
