using DevTrends.MvcDonutCaching;

namespace cfacore.shared.domain._base
{
	public class DynamicOutputCacheAttribute : DonutOutputCacheAttribute
	{
		public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
		{
			CacheProfile = "StaticPage";
			VaryByParam = "*";

			base.OnActionExecuting(filterContext);
		}
	}
}
