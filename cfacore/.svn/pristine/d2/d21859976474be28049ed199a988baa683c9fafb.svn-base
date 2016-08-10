
#region Imports

using System.Web.Mvc;

#endregion

namespace cfacore.shared.modules.com.admin
{
    public class AreaAuthorizeAttribute : AuthorizeAttribute
    {
	    public string Area { get; set; }
        
	    private string _controller;
		public string Controller
		{
			get { return _controller ?? (_controller = "Account"); }
			set { _controller = value; }
		}

	    private string _action;
		public string Action
		{
			get { return _action ?? (_action = "Login"); }
			set { _action = value; }
		}

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "langCode", filterContext.RouteData.Values[ "langCode" ] },
                        { "area", Area },
                        { "controller", Controller },
                        { "action", Action },
                        { "ReturnUrl", filterContext.HttpContext.Request.RawUrl }
                    } );
            }
        }
    }
}
