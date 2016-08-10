using System.Web.Mvc;

namespace cfaresv2.Areas.FamilyInfluence
{
	public class FamilyInfluenceAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "FamilyInfluence";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"FamilyInfluence_default",
				"FamilyInfluence/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
