using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.shared.modules.com.request;

namespace cfares.Global
{
    public class BrowserFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            Controller controller = context.Controller as Controller;
            
            if(controller!=null){

                controller.ViewData["browser"] = BrowserInfo.Read(context.HttpContext.Request);
            
            }

        }
    }
}