using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using cfares.domain._event;
using cfares.domain._event._ticket.tours;
using cfares.domain._event.slot.tours;
using cfares.entity.dbcontext.res_event;
using System.Data;
using cfares.repository.ticket;
using cfares.site.modules.com.Security;
using cfares.site.modules.user;
using cfaresv2.Areas.Admin.Controllers._base;
using npr.domain._event.ticket;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin,Operator" )]
    public class TicketController : CrudController<TicketRepository, Ticket, Ticket,int,ITicket>
    {

        protected override void FormValid(ITicket entity, Ticket entityViewModel, FormCollection collection)
        {

            
            

        }



        protected override Dictionary<string, Action<Ticket, string>> QueryInjector()
        {
            return new Dictionary<string, Action<Ticket, string>>()
                {
                    {"SlotId",(x,y)=>x.SlotId=NullableInt(y)},
                    {"OwnerId",(x,y)=>x.OwnerId=NullableInt(y)},
                };
        }

        public override ITicket Inject(int id, Ticket entity)
        {
            entity.TicketId = id;
            return entity;
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
            ITicket ticket = serv.Find(id);

            return View(ticket as TourTicket);
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

                ticket.Owner = new cfares.domain.user.ResUser(collection["Owner"]);
                ticket.CreationSrc = "Admin";
                serv.Add(ticket);
                serv.Commit();
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
            ITicket ticket = serv.Find(id);

            return View(ticket as TourTicket);
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

                ticket.Owner = new cfares.domain.user.ResUser(collection["Owner"]);

                serv.Edit(ticket);
                serv.Commit();

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
            ITicket ticket = serv.Find(id);

            return View(ticket as TourTicket);
        }

        //
        // POST: /Admin/Ticket/Delete/5

        [HttpPost]
        [ActionName("TourTicket-Delete")]
        public ActionResult TourTicketDelete(int id, FormCollection collection)
        {
            ITicket ticket = serv.Find(id);
            int? slotId = ticket.SlotId;

            try
            {
                // TODO: Add delete logic here
                serv.Delete(ticket as Ticket);
                serv.Commit();
                if(slotId!=0)
                    return RedirectToAction("TourSlot-Details","Slot", new { id=slotId,message = "That Ticket Has Been Deleted" });
                else
                    return RedirectToAction("TourTicket-Index", "Ticket", new { message = "That Ticket Has Been Deleted" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View(ticket as TourTicket);
            }
        }





        #region specialty

        public ActionResult SpecialtyTicket(int id)
        {
            return View();
        }

        public ActionResult SpecialtyDetails(int id)
        {
            return View();
        }

        public ActionResult SpecialtyCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SpecialtyCreate(NPRTicket ticket, FormCollection collection)
        {
            return View();
        }

        public ActionResult SpecialtyEdit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SpecialtyEdit(int id, NPRTicket ticket, FormCollection collection)
        {
            return View();
        }

        public ActionResult SpecialtyDelete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SpecialtyDelete(int id, NPRTicket ticket)
        {
            return View();
        }

        #endregion





        public override TicketRepository GetRepository(IResContext context)
        {
            return new TicketRepository(context);
        }
    }
}
