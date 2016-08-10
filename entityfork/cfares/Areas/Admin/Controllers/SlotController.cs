using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.domain._event;
using cfacore.site.controllers.shared;
using cfares.domain._event.slot.tours;
using cfacore.site.controllers._event;
using cfares.domain.user;
using cfares.domain._event.slot;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class SlotController : Controller
    {
        SlotService serv = new SlotService();
        //
        // GET: /Admin/Slot/

        public ActionResult Index()
        {
            return View();
        }


        //
        // GET: /Admin/Slot/Confirm/

        public ActionResult Confirm()
        {
            return View();
        }


        //
        // GET: /Admin/Slot/Details/5
        public ActionResult Details(int id)
        {
            Slot slot;
            
            slot = serv.Load(id.ToString());
            if (slot == null)
            {
                return RedirectToAction("Index", new { message = "That slot was not found" });
            }
            return View(slot);
        }

        //
        // GET: /Admin/Slot/Create
        public ActionResult Create()
        {
            return View(new Slot());
        }

        //
        // POST: /Admin/Slot/Create

        [HttpPost]
        public ActionResult Create(Slot slot, FormCollection collection)
        {
            try
            {
                // TODO: Add create logic here
                OccurrenceService oServ = new OccurrenceService();

                slot.Start = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_Start"]));
                slot.End = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_End"]));
                slot.Cutoff = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Cutoff"]));
                slot.Occurrence = new Occurrence(collection["Occurrence"]);                
                serv.Save(slot);
                return RedirectToAction("Details", new { id = slot.SlotId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(slot);
            }
        }

        //
        // GET: /Admin/Slot/Edit/5

        public ActionResult Edit(int id)
        {
            Slot slot = serv.Load(id.ToString());
            if (slot == null)
            {
                return RedirectToAction("Index", new { message = "That slot was not found" });
            }
            return View(slot);
        }

        //
        // POST: /Admin/Slot/Edit/5

        [HttpPost]
        public ActionResult Edit(int id,Slot slot, FormCollection collection)
        {
            
            try
            {
                slot.Occurrence = new Occurrence(collection["Occurrence"]);
                OccurrenceService oServ = new OccurrenceService();                
                slot.Start = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_Start"]));
                slot.End = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_End"]));
                slot.Cutoff = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Cutoff"]));

                slot.Schedule = new Schedule(collection["Schedule"]);
                serv.Save(slot);
                return RedirectToAction("Details", new { id = slot.SlotId });                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(slot);
            }
        }

        //
        // GET: /Admin/Slot/Delete/5

        public ActionResult Delete(int id)
        {
            Slot slot = serv.Load(id.ToString());
            if (slot == null)
            {
                return RedirectToAction("Index", new { message = "That slot was not found" });
            }

            return View(slot);
        }

        //
        // POST: /Admin/Slot/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Slot slot = serv.Load(id.ToString());

            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(slot);
            }
        }

        [ActionName("TourSlot-Index")]
        public ActionResult TourIndex()
        {
            return View();
        }

        //
        // GET: /Admin/Slot/Details/5
        [ActionName("TourSlot-Details")]
        public ActionResult TourDetails(int id)
        {
            TourSlot slot;
            slot = serv.LoadTourWithGuideOccurrenceAndEvent(id.ToString());
            if (slot == null)
            {
                return RedirectToAction("TourSlot-Index", new { message = "That slot was not found" });
            }
            serv.LoadTourWithCameos(slot);

            return View(slot);
        }

        //
        // GET: /Admin/Slot/Create
        [ActionName("TourSlot-Create")]
        public ActionResult TourCreate()
        {
            TourSlot ts = new TourSlot();
            ts.Availability = new cfacore.shared.domain._base.DateRange(DateTime.Now.AddHours(-2), DateTime.Now);
            ts.Cutoff = DateTime.Now.AddDays(-3);            
            if (!string.IsNullOrEmpty(Request.QueryString["Occurrence"]))
                ts.Occurrence = new Occurrence(Request.QueryString["Occurrence"]);
            return View(ts);
        }

        //
        // POST: /Admin/Slot/Create

        [HttpPost]
        [ActionName("TourSlot-Create")]
        public ActionResult TourCreate(TourSlot slot, FormCollection collection)
        {
            try
            {
                // TODO: Add create logic here
                //serv.Save(slot);
                slot.Occurrence = new Occurrence(collection["Occurrence"]);
                OccurrenceService oServ = new OccurrenceService();
                slot.Start = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_Start"]));
                slot.End = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_End"]));
                slot.Cutoff = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Cutoff"]));
                
                slot.Guide = new ResAdmin(collection["Guide"]);
                serv.Save(slot);

                //"Cameos.AdditionalGuides_accept"                
                SaveCameosFromForm(slot.Id(),collection);

                return RedirectToAction("TourSlot-Details", new { id=slot.SlotId});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(slot);
            }
        }

        private bool SaveCameosFromForm(string id, FormCollection collection) {
            var values = Enum.GetValues(typeof(CameoType)).Cast<CameoType>();

            List<int> toAcceptCameoType = new List<int>();
            List<int> toRemoveCameoType = new List<int>();
            List<string> toAccept = new List<string>();
            List<string> toRemove = new List<string>();

            foreach (CameoType ct in values)
            {
                string name = Enum.GetName(ct.GetType(), ct);
                string desc = CameoSets.DescriptionFromType(ct);
                int val = (int)ct;
                string acceptColStr = collection[string.Format("Cameos.{0}_accept", desc)];
                string removeColStr = collection[string.Format("Cameos.{0}_remove", desc)];

                if (!string.IsNullOrEmpty(acceptColStr))
                {
                    string[] toAcceptCameo = acceptColStr.Split(',');
                    foreach (string pk in toAcceptCameo)
                    {
                        toAcceptCameoType.Add(val);
                        toAccept.Add(pk);
                    }
                }

                if (!string.IsNullOrEmpty(removeColStr))
                {
                    string[] toRemoveCameo = removeColStr.Split(',');
                    foreach (string pk in toRemoveCameo)
                    {
                        toRemoveCameoType.Add(val);
                        toRemove.Add(pk);
                    }
                }
            }
            return serv.SaveCameosForTourSlot(id, toAccept.ToArray(), toAcceptCameoType.ToArray()) &&
            serv.RemoveCameosForTourSlot(id,toRemove.ToArray(),toRemoveCameoType.ToArray());
            
        }

        //
        // GET: /Admin/Slot/Edit/5
        [ActionName("TourSlot-Edit")]
        public ActionResult TourEdit(int id)
        {
            TourSlot slot = serv.LoadTourWithCameos(id.ToString());

            return View(slot);
        }

        //
        // POST: /Admin/Slot/Edit/5

        [HttpPost]
        [ActionName("TourSlot-Edit")]
        public ActionResult TourEdit(int id,TourSlot slot, FormCollection collection)
        {
            

            try
            {
                // TODO: Add update logic here
                slot.Occurrence = new Occurrence(collection["Occurrence"]);
                OccurrenceService oServ = new OccurrenceService();
                slot.Start = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_Start"]));
                slot.End = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Availability_End"]));
                slot.Cutoff = oServ.ConvertFromTimeZoneContext(slot.Occurrence, DateTime.Parse(collection["Cutoff"]));
                
                slot.Guide = new ResAdmin(collection["Guide"]);
                serv.Save(slot);

                SaveCameosFromForm(slot.Id(), collection);

                return RedirectToAction("TourSlot-Details", new { id = slot.SlotId });                
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError("", ex);
                return View(slot);
            }
            
        }

        //
        // GET: /Admin/Slot/Delete/5
        [ActionName("TourSlot-Delete")]
        public ActionResult TourDelete(int id)
        {
            TourSlot slot = serv.LoadTourWithGuideOccurrenceAndEvent(id.ToString());
            if (slot == null) {
                return RedirectToAction("TourSlot-Index", new { message="That slot was not found"});
            }
            serv.LoadTourWithCameos(slot);

            return View(slot);
        }

        //
        // POST: /Admin/Slot/Delete/5

        [HttpPost]
        [ActionName("TourSlot-Delete")]
        public ActionResult TourDelete(int id, FormCollection collection)
        {
            TourSlot slot = serv.LoadTourWithCameos(id.ToString());

            try
            {
                // TODO: Add delete logic here
                serv.Delete(slot);
                return RedirectToAction("TourSlot-Index", new { message = "That slot has been deleted" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(slot);
            }
        }

    }
}
