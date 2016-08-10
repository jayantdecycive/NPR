
#region Imports

using System.Web.Mvc;
using cfares.site.modules.com.application;

#endregion

namespace cfares.Global
{
    public class ResViewEngine : RazorViewEngine
    {
		public override ViewEngineResult FindView( ControllerContext controllerContext, 
			string viewName, string masterName, bool useCache )
		{
            if (AppContext.Current != null && AppContext.Current.Event != null)
			{
				string controller = controllerContext.Controller.GetType().Name.Replace("Controller", string.Empty);
				string templateName = AppContext.Current.Event.Template.ResTemplateId; // Soon to be ".ViewResolution"
				string templateSpecificVirtualPath = 
					string.Format( "~/Views/Events/{0}/{1}/{2}.cshtml", 
						templateName, controller, viewName );

				if( base.FileExists( controllerContext, templateSpecificVirtualPath ) )
					viewName = templateSpecificVirtualPath;
			}

			return base.FindView(controllerContext, viewName, masterName, useCache);
		}
	}
}
