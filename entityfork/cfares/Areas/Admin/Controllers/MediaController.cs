using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers._event;
using cfares.domain._event;
using cfacore.site.controllers.shared.media;
using cfacore.shared.domain.media;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class MediaController : Controller
    {
        MediaService serv = new MediaService();
        //
        // GET: /Admin/Media/

        public ActionResult Index()
        {
            return View();
        }


        //
        // GET: /Admin/Media/Confirm/

        public ActionResult Confirm()
        {
            return View();
        }


        //
        // GET: /Admin/Media/Details/5

        public ActionResult Details(int id)
        {
            Media media = serv.Load(id.ToString());

            return View(media);
        }

        //
        // GET: /Admin/Media/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Media/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add create logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Media/Edit/5

        public ActionResult Edit(int id)
        {
            Media media = serv.Load(id.ToString());

            return View(media);
        }

        //
        // POST: /Admin/Media/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            Media media = serv.Load(id.ToString());
            
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(media);
            }
        }

        //
        // GET: /Admin/Media/Delete/5

        public ActionResult Delete(int id)
        {
            Media media = serv.Load(id.ToString());
            return View(media);
        }

        //
        // POST: /Admin/Media/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Media media = serv.Load(id.ToString());

            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(media);
            }
        }

    }
}
