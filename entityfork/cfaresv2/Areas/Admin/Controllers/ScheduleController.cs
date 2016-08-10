using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.domain._event;


using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using System.Data;
using cfacore.shared.modules.com.admin;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin" )]
    public class ScheduleController : AdminController
    {
        IResContext serv = ReservationConfig.GetContext();
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
            Schedule schedule = null;
            
            schedule = serv.Schedules.Find(id.ToString());
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
                

                schedule.Start = Occurrence.ConvertFromTimeZoneContext(DateTime.Parse(collection["Start"]));
                schedule.End = Occurrence.ConvertFromTimeZoneContext(DateTime.Parse(collection["End"]));

                serv.Schedules.Add(schedule);
                serv.SaveChanges();
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
            Schedule schedule = serv.Schedules.Find(id.ToString());
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
                

                schedule.Start = Occurrence.ConvertFromTimeZoneContext(DateTime.Parse(collection["Start"]));
                schedule.End = Occurrence.ConvertFromTimeZoneContext(DateTime.Parse(collection["End"]));

                serv.Entry(schedule).State = EntityState.Modified;
                serv.SaveChanges();
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
            Schedule schedule = serv.Schedules.Find(id.ToString());
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
            Schedule schedule = serv.Schedules.Find(id.ToString());

            try
            {
                // TODO: Add delete logic here
                serv.Schedules.Remove(schedule);
                serv.SaveChanges();
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
