using System;
using System.Linq;
using System.Web;

namespace cfares.site.modules.Helpers
{
    public static class UrlPortFix
    {
        public static Uri TranslatePort( this string uri )
        {
			return TranslatePort( uri.ToUri(), new HttpRequestWrapper( HttpContext.Current.Request ) );
        }

        public static Uri TranslatePort( this Uri uri )
        {
			return TranslatePort( uri, new HttpRequestWrapper( HttpContext.Current.Request ) );
        }

        public static Uri TranslatePort(this Uri uri, HttpRequestBase request)
        {
            var builder = new UriBuilder(uri);
            if (!request.Headers.AllKeys.Contains("Host"))
                return builder.Uri;

            var hostStr = request.Headers["Host"];
            var protocal = hostStr.EndsWith("43")?"https":"http";
            if(hostStr.Contains("http"))
                return new Uri(hostStr+ uri.PathAndQuery);
	
			return new Uri(protocal + "://" + hostStr + uri.PathAndQuery);
        }
    }
}
