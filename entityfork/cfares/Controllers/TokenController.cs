using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.service.common;
using cfacore.shared.modules.user;

using cfares.domain.user;
using cfacore.shared.domain.common;

namespace cfares.Controllers
{
    public class TokenController : Controller
    {

        public ActionResult SAML(string id)
        {
            return Content("Hello world");
        }

    }
}
