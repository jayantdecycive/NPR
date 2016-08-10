using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers._event;
using cfares.domain._event;
using cfacore.shared.modules.com.admin;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class OccurrenceController : Controller
    {
        OccurrenceService serv = new OccurrenceService();
        //
        // GET: /Admin/Occurrences/

        public ActionResult Index()
        {
            return View();
        }


        //
        // GET: /Admin/Occurrences/Confirm/

        public ActionResult Confirm()
        {
            return View();
        }


        //
        // GET: /Admin/Occurrences/Details/5

        public ActionResult Details(int id)
        {
            Occurrence occurrence = serv.Load(id.ToString());
            
            return View(occurrence);
        }

        //
        // GET: /Admin/Occurrences/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Occurrences/Create

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
        // GET: /Admin/Occurrences/Edit/5

        public ActionResult Edit(int id)
        {
            Occurrence occurrence = serv.Load(id.ToString());
            
            return View(occurrence);
        }

        //
        // POST: /Admin/Occurrences/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            Occurrence occurrence = serv.Load(id.ToString());
            
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(occurrence);
            }
        }

        //
        // GET: /Admin/Occurrences/Delete/5

        public ActionResult Delete(int id)
        {
            Occurrence occurrence = serv.Load(id.ToString());
            return View(occurrence);
        }

        //
        // POST: /Admin/Occurrences/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Occurrence occurrence = serv.Load(id.ToString());

            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(occurrence);
            }
        }


        //
        // GET: /Admin/Occurrences/Edit-Dash
        [ActionName("Edit-Dash")]
        public ActionResult EditDash(int id)
        {
            

            try
            {
                Occurrence occurrence = serv.Load(id.ToString());
                ResEventService eServ = new ResEventService();
                occurrence.ResEvent = eServ.Load(occurrence.ResEvent.Id());

                Wizard wiz = EventController.initializeWizard(occurrence.ResEvent, 1, Session, this.ControllerContext);
                ViewBag.Wizard = wiz;

                return View(occurrence);
                
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (ViewBag.Wizard != null && ViewBag.Wizard is Wizard)
            {
                (ViewBag.Wizard as Wizard).CurrentStep.Uri = new Uri(Request.Url.ToString());
                EventController.setWizard(ViewBag.Wizard as Wizard, Session);
            }
        }

        //
        // GET: /Admin/Occurrences/Edit-Dash        
        public ActionResult Summary(int id)
        {
            Occurrence occurrence = serv.Load(id.ToString());

            return RedirectToAction("Summary", "Event", new { id = occurrence.ResEvent.ResEventId });
        }

    }
}
