using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.domain._event;
using cfacore.site.controllers.shared;
using cfacore.site.controllers._event;
using cfares.domain.user;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class ScheduleController : Controller
    {
        ScheduleService serv = new ScheduleService();
        //
        // GET: /Admin/Schedule/

        public ActionResult Index()
        {
            return View();
        }


        //
        // GET: /Admin/Schedule/Confirm/

        public ActionResult Confirm()
        {
            return View();
        }


        //
        // GET: /Admin/Schedule/Details/5
        public ActionResult Details(int id)
        {
            Schedule schedule;
            
            schedule = serv.Load(id.ToString());
            if (schedule == null)
            {
                return RedirectToAction("Index", new { message = "That schedule was not found" });
            }
            return View(schedule);
        }

        //
        // GET: /Admin/Schedule/Create
        public ActionResult Create()
        {
            return View(new Schedule());
        }

        //
        // POST: /Admin/Schedule/Create

        [HttpPost]
        public ActionResult Create(Schedule schedule, FormCollection collection)
        {
            try
            {
                // TODO: Add create logic here
                OccurrenceService oServ = new OccurrenceService();

                schedule.Start = oServ.ConvertFromTimeZoneContext(DateTime.Parse(collection["Start"]));
                schedule.End = oServ.ConvertFromTimeZoneContext(DateTime.Parse(collection["End"]));

                serv.Save(schedule);
                return RedirectToAction("Details", new { id = schedule.ScheduleId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(schedule);
            }
        }

        //
        // GET: /Admin/Schedule/Edit/5

        public ActionResult Edit(int id)
        {
            Schedule schedule = serv.Load(id.ToString());
            if (schedule == null)
            {
                return RedirectToAction("Index", new { message = "That schedule was not found" });
            }
            return View(schedule);
        }

        //
        // POST: /Admin/Schedule/Edit/5

        [HttpPost]
        public ActionResult Edit(int id,Schedule schedule, FormCollection collection)
        {
            
            try
            {
                OccurrenceService oServ = new OccurrenceService();

                schedule.Start = oServ.ConvertFromTimeZoneContext(DateTime.Parse(collection["Start"]));
                schedule.End = oServ.ConvertFromTimeZoneContext(DateTime.Parse(collection["End"]));

                serv.Save(schedule);
                return RedirectToAction("Details", new { id = schedule.ScheduleId });                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(schedule);
            }
        }

        //
        // GET: /Admin/Schedule/Delete/5

        public ActionResult Delete(int id)
        {
            Schedule schedule = serv.Load(id.ToString());
            if (schedule == null)
            {
                return RedirectToAction("Index", new { message = "That schedule was not found" });
            }

            return View(schedule);
        }

        //
        // POST: /Admin/Schedule/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Schedule schedule = serv.Load(id.ToString());

            try
            {
                // TODO: Add delete logic here
                serv.Delete(schedule);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(schedule);
            }
        }

    }
}
