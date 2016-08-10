using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.service.common;
using cfacore.shared.domain.common;
using cfares.Areas.tours.Models;
using cfacore.shared.modules.user;
using cfares.domain.user;

namespace cfares.Areas.tours.Controllers
{
    public class TokenController : Controller
    {
        //
        // GET: /tours/Token/
        TokenService tserv = null;
        UserMembershipService userService = null;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            tserv = new TokenService();
            userService = new UserMembershipService(HttpContext);
            base.OnActionExecuting(filterContext);
        }


        public ActionResult PasswordReset(string id)
        {
            Token token = tserv.LoadByUID(id);

            if (token != null && !token.IsExpired() && token.User != null)
            {
                ResUser user = userService.Load(token.User.Id()) as ResUser;
                //tserv.Delete(token);
                userService.Authenticate(user);
                return RedirectToAction("PasswordReset", "Account", new { Area = "tours" });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }
    }
}
