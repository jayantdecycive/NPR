using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web;
using System.Net.Http;
using System.Net;

namespace cfares.site.modules.query
{
    public class ODataQueryable : QueryableAttribute
    {
        
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            long? originalsize = null;
            var inlinecount = HttpUtility.ParseQueryString(actionExecutedContext.Request.RequestUri.Query).Get("$inlinecount");

            object responseObject;
            actionExecutedContext.Response.TryGetContentValue(out responseObject);
            

            
            base.OnActionExecuted(actionExecutedContext);
            

            var originalquery = responseObject as IQueryable<object>;

            if (originalquery != null && inlinecount == "allpages")
                originalsize = originalquery.Count();
            if (ResponseIsValid(actionExecutedContext.Response))
            {
                actionExecutedContext.Response.TryGetContentValue(out responseObject);

                if (responseObject is IQueryable)
                {
                    var robj = responseObject as IQueryable<object>;

                    if (originalsize != null)
                    {
                        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, new ODataMetadata<object>(robj, originalsize));
                    }
                }
            }
        }

        public void ValidateQuery(HttpRequestMessage request)
        {
            //everything is allowed
        }

        private bool ResponseIsValid(HttpResponseMessage response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK || !(response.Content is ObjectContent)) return false;
            return true;
        }
        
    }
}
