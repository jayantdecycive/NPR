using System.Web.Mvc;

namespace cfaresv2.Areas.reception
{
	public class ReceptionAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "reception";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"reception_default",
				"reception/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
