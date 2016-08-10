using System.Web.Mvc;

namespace cfaresv2.Areas.product
{
	public class ProductGiveawaysAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "ProductGiveaways";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"ProductGiveaways_default",
				"ProductGiveaways/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
