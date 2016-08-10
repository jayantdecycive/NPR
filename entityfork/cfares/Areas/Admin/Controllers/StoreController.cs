using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers.shared;
using cfacore.shared.domain.store;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class StoreController : Controller
    {
        StoreService serv = new StoreService();
        //
        // GET: /Admin/Store/
        
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Admin/Store/Details/5

        public ActionResult Details(int id)
        {

            Store store = serv.Load(id.ToString());
            
            return View(store);
        }

        //
        // GET: /Admin/Store/Edit/5

        public ActionResult Edit(int id)
        {

            Store store = serv.Load(id.ToString());

            return View(store);
        }

        
    }
}
