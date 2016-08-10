using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cfares.Global
{
    public class UserModeFilterAttribute: ActionFilterAttribute
    {
        public string userMode { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            filterContext.Controller.ViewBag.UserMode = userMode;

        }
    }
}