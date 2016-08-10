using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using cfacore.shared.modules.com.util;
using System.IO;
using System.Runtime.Serialization.Json;
using cfacore.shared.modules.com.request;

namespace cfacore.shared.Helpers
{
    public static class CfaExtensions
    {
        public static string CssClass(this HtmlHelper helper, string viewDataKey)
        {
            if (!helper.ViewData.ContainsKey(viewDataKey))
                return string.Empty;
            string[] classes = (string[])helper.ViewData[viewDataKey];
            return CssClass(helper, classes);
        }

        public static string CssClass(this HtmlHelper helper, string[] classes)
        {            
            List<string> allClasses = new List<string>(classes);
            List<string> formattedClasses = allClasses.Distinct().ToList();

            for (int i = 0; i < formattedClasses.Count; i++)
            {
                formattedClasses[i] = Util.Urlify(formattedClasses[i]).ToLower();
            }

            return string.Join(" ", classes);

        }

        public static MvcHtmlString Wiki(this HtmlHelper helper, string src)
        {
            WikiPlex.WikiEngine engine = new WikiPlex.WikiEngine();

            return MvcHtmlString.Create(engine.Render(src));

        }

        public static string TourAdminCrumbs(this HtmlHelper helper)
        {
            return "";
        }


        public static string Urlify(this HtmlHelper helper, string url) {
            return Util.Urlify(url);
        }

        public static string Json(this HtmlHelper helper, string key)
        {
            if (!helper.ViewData.ContainsKey(key))
                return string.Empty;
            return CfaExtensions.Json(helper,helper.ViewData[key]);
        }
        public static string Json(this HtmlHelper helper, object obj)
        {            
            
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(object));
            ser.WriteObject(stream1, obj);
            StreamReader sr = new StreamReader(stream1);
            string myStr = sr.ReadToEnd();
            return myStr;
        }



        public static string BrowerBodyClasses(this HtmlHelper helper,object binfo) {
            BrowserInfo info = binfo as BrowserInfo; 
            if (info == null)
                return string.Empty;

           
            return String.Format("flag-{0} browser-{1} version-{2} platform-{3} {4} {5}", 
                info.flag, info.id, info.majorVersion, info.clientOS, 
                info.tablet ? "browser-tablet" : "", info.legacy ? "browser-legacy" : "").ToLower();

        }

        public static string RouteClasses(this HtmlHelper helper)
        {
            string act = helper.ViewContext.RouteData.Values["action"].ToString().ToLower();
            string con = helper.ViewContext.RouteData.Values["controller"].ToString().ToLower();
            
            string result = String.Format("action-{0} controller-{1}", act,con);

            object idRoute = helper.ViewContext.RouteData.Values["id"];
            if(idRoute!=null)
                result = result+ String.Format(" id-{0}", idRoute.ToString().ToLower());

            return result;
        }
    }
}