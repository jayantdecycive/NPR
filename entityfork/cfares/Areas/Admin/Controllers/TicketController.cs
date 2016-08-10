using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers._event;
using cfares.domain._event;
using cfares.domain._event.ticket.tours;
using cfares.domain._event.slot.tours;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class TicketController : Controller
    {
        TicketService serv = new TicketService();
        


        //
        // GET: /Admin/Ticket/Confirm/

        public ActionResult Confirm()
        {
            return View();
        }


        //
        // GET: /Admin/Ticket/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Admin/Ticket/Details/5

        public ActionResult Details(int id)
        {
            Ticket ticket = serv.Load(id.ToString());

            return View(ticket);
        }

        //
        // GET: /Admin/Ticket/Create

        public ActionResult Create()
        {
            return View(new Ticket());
        }

        //
        // POST: /Admin/Ticket/Create

        [HttpPost]
        public ActionResult Create(Ticket ticket,FormCollection collection)
        {
            try
            {
                // TODO: Add create logic here
                serv.Save(ticket);
                return RedirectToAction("Details", new { id = ticket.TicketId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(ticket);
            }
        }

        //
        // GET: /Admin/Ticket/Edit/5

        public ActionResult Edit(int id)
        {
            Ticket ticket = serv.Load(id.ToString());
            
            return View(ticket);
        }

        //
        // POST: /Admin/Ticket/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Ticket ticket, FormCollection collection)
        {
            
            try
            {
                // TODO: Add update logic here
                serv.Save(ticket);
                return RedirectToAction("Details", new { id=id});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(ticket);
            }
        }

        //
        // GET: /Admin/Ticket/Delete/5

        public ActionResult Delete(int id)
        {
            Ticket ticket = serv.Load(id.ToString());

            return View(ticket);
        }

        //
        // POST: /Admin/Ticket/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Ticket ticket = serv.Load(id.ToString());
                        

            try
            {
                // TODO: Add delete logic here
                serv.Delete(ticket);
                return RedirectToAction("Index", new { message="That Ticket Has Been Deleted" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(ticket);
            }
        }




        //
        // GET: /Admin/Ticket/
        [ActionName("TourTicket-Index")]
        public ActionResult TourTicketIndex()
        {
            return View();
        }

        //
        // GET: /Admin/Ticket/
        [ActionName("TourTicketNoSlot-Index")]
        public ActionResult TourTicketNoSlotIndex()
        {
            return View();
        }

        //
        // GET: /Admin/Ticket/Details/5
        [ActionName("TourTicket-Details")]
        public ActionResult TourTicketDetails(int id)
        {
            TourTicket ticket = serv.LoadTour(id.ToString());

            return View(ticket);
        }

        //
        // GET: /Admin/Ticket/Create
        [ActionName("TourTicket-Create")]
        public ActionResult TourTicketCreate()
        {
            TourTicket t = new TourTicket();
            if (!string.IsNullOrEmpty(Request.QueryString["Slot"]))
                t.Slot = new TourSlot(Request.QueryString["Slot"]);
            
            return View(t);
        }

        //
        // POST: /Admin/Ticket/Create

        [HttpPost]
        [ActionName("TourTicket-Create")]
        public ActionResult TourTicketCreate(TourTicket ticket, FormCollection collection)
        {
            try
            {
                // TODO: Add create logic here
                if(!string.IsNullOrEmpty(collection["Slot"])&&collection["Slot"]!="0")                                        
                    ticket.Slot = new TourSlot(collection["Slot"]);
                else
                    ticket.Slot = null;

                ticket.Owner = new domain.user.ResUser(collection["Owner"]);
                ticket.CreationSrc = "Admin";
                serv.Save(ticket);
                return RedirectToAction("TourTicket-Details", new { id = ticket.TicketId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(ticket);
            }
        }

        //
        // GET: /Admin/Ticket/Edit/5
        [ActionName("TourTicket-Edit")]
        public ActionResult TourTicketEdit(int id)
        {
            TourTicket ticket = serv.LoadTour(id.ToString());

            return View(ticket);
        }

        //
        // POST: /Admin/Ticket/Edit/5

        [HttpPost]
        [ActionName("TourTicket-Edit")]
        public ActionResult TourTicketEdit(int id, TourTicket ticket, FormCollection collection)
        {

            try
            {
                // TODO: Add update logic here
                if (!string.IsNullOrEmpty(collection["Slot"]) && collection["Slot"] != "0")
                    ticket.Slot = new TourSlot(collection["Slot"]);
                else
                    ticket.Slot = null;

                ticket.Owner = new domain.user.ResUser(collection["Owner"]);
                serv.Save(ticket);
                return RedirectToAction("TourTicket-Details", new { id = id });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(ticket);
            }
        }

        //
        // GET: /Admin/Ticket/Delete/5
        [ActionName("TourTicket-Delete")]
        public ActionResult TourTicketDelete(int id)
        {
            Ticket ticket = serv.LoadTour(id.ToString());

            return View(ticket);
        }

        //
        // POST: /Admin/Ticket/Delete/5

        [HttpPost]
        [ActionName("TourTicket-Delete")]
        public ActionResult TourTicketDelete(int id, FormCollection collection)
        {
            TourTicket ticket = serv.LoadTour(id.ToString());
            string slotId = ticket.Id();

            try
            {
                // TODO: Add delete logic here
                serv.Delete(ticket);
                if(!string.IsNullOrEmpty(slotId))
                    return RedirectToAction("TourSlot-Details","Slot", new { id=slotId,message = "That Ticket Has Been Deleted" });
                else
                    return RedirectToAction("TourTicket-Index", "Ticket", new { message = "That Ticket Has Been Deleted" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(ticket);
            }
        }

    }
}
