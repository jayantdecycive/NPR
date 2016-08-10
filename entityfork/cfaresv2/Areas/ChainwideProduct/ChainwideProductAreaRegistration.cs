using System.Web.Mvc;

namespace cfaresv2.Areas.ChainwideProduct
{
	public class ChainwideProductAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "ChainwideProductGiveaways";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"ChainwideProductGiveaways_default",
				"ChainwideProductGiveaways/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
