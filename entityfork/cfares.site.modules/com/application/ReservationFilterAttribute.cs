
#region Imports

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using cfares.domain._event;

#endregion

namespace cfares.site.modules.com.application
{
    public class ReservationFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            Controller controller = context.Controller as Controller;

            if (controller != null)
            {
                controller.ViewData[ReservationConfig.ViewDataKey] = context.HttpContext.Items[ReservationConfig.ContextKey];
                string message = context.HttpContext.Request.QueryString["message"];
                if (!string.IsNullOrEmpty(message)) {
                    controller.ViewData["Message"] = HttpUtility.UrlDecode(message);
                }                
            }
        }
    }

    public static class CfaResExtensions
    {
        public static MvcHtmlString ResApplicationScript(this HtmlHelper helper)
        {
            if (!helper.ViewData.ContainsKey(ReservationConfig.ViewDataKey))
                return MvcHtmlString.Empty;
             
            ReservationConfig hint = (ReservationConfig)helper.ViewData[ReservationConfig.ViewDataKey];
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = (serializer.Serialize(hint));
            return MvcHtmlString.Create(string.Format("<script type='text/javascript'>var Application={0};</script>", json));
            
        }

		  public static bool IsDebugDefined {
			get {
		#if DEBUG
			  return true;
		#else
			  return false;
		#endif 
			}
		  }

        public static MvcHtmlString GlobalTimeData(this HtmlHelper helper)
        {
            if (!helper.ViewData.ContainsKey("TimeZone")) {
                helper.ViewData["TimeZone"]=(new Occurrence().TimeZoneContext());
            }
            TimeZoneInfo timeZone = (TimeZoneInfo)helper.ViewData["TimeZone"];

			// SH - Fix for day light savings time where _GMTOffset would be returned as -05:00
			// .. when the proper offset should be -04:00
			DateTimeOffset offset = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);
			TimeSpan baseoffset = offset.ToOffset(timeZone.GetUtcOffset(offset)).Offset;

            helper.ViewData["TimeZoneContextOffset"] = baseoffset;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string minus = timeZone.BaseUtcOffset.CompareTo(TimeSpan.Zero)<0?"-":"";
            string json = (serializer.Serialize(new { TimeZoneInfo = timeZone, 
				TimeZoneContext = (minus+string.Format("{0:hh\\:mm\\:ss}", baseoffset)),
                TimeZoneContextOffset = baseoffset.TotalHours
            }));
            return MvcHtmlString.Create(string.Format("<script type='text/javascript'>var Global={0};</script>", json));
        }

        public static MvcHtmlString ApplicationTitle(this HtmlHelper helper)
        {
			return new MvcHtmlString(AppContext.Current.Configuration.Admin.AppTitle);
        }
    }
}