using System.Web;
using System.Web.Mvc;

namespace cfaresv2.AuthenticationSP
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}