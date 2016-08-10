using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.shared.domain.attributes;
using System.Collections;
using cfacore.domain._base;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ComponentModel.DataAnnotations;

namespace cfares.Controllers
{
    public class AutoScriptController : Controller
    {
        //
        // GET: /Default1/
        [ActionName("Default.js")]
        public ActionResult Index()
        {
            
            return JavaScript("alert('hello world');");
        }

        

    }
}
