using cfacore.domain._base;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.site.modules.user;
using cfares.site.modules.com.application;
using cfares.domain._event;
using npr.domain._event.ticket;

namespace cfaresv2.Areas.Admin.Controllers._base
{
    public abstract class CrudController<TRepository, TEntity, TKey> : CrudController<TRepository, TEntity, TEntity, TKey, TEntity>
        where TEntity : class,IDomainObject, new()
        where TRepository : GenericRepository<IResContext, TEntity, TKey>
    { }

    public abstract class ExtendableCrudController<TRepository, TEntity, TKey, ITEntity> : CrudController<TRepository, TEntity, TEntity, TKey, ITEntity>
        where TEntity : class,IDomainObject, ITEntity, new()
        where TRepository : GenericRepository<IResContext, TEntity, TKey, ITEntity>
    { }

    public abstract class CrudController<TRepository, TEntity, TEntityViewModel, TKey, ITEntity> : CrudControllerBase<TRepository, TEntity, TEntityViewModel, TKey, ITEntity>
        where TEntity : class,IDomainObject, ITEntity, new()
        where TEntityViewModel : class,new()
        where TRepository : GenericRepository<IResContext, TEntity, TKey, ITEntity>
    {


        //
        // POST: /Admin/TEntity/Create

        [HttpPost]
        public virtual ActionResult Create(TEntityViewModel entity, FormCollection collection)
        {
            CheckCreateRights();
            TEntity newEntity;
            try
            {
                // TODO: Add create logic here                
                //newEntity = new TEntity();
                newEntity = (TEntity)Inject(default(TKey), entity);
                SetViewBag();
                IValueInjecter injecter = new ValueInjecter();
                injecter.Inject(newEntity, entity);
                FormValid(newEntity, entity, collection);

                var ticketrecord = entity as NPRTicket;
                var ticket = entity as cfares.domain._event.Ticket;

                if (!ModelState.IsValid)
                    return View(entity);

                serv.Add(newEntity);
                serv.Commit();

                TicketGuests ticketguest;

                foreach (var guestname in ticketrecord.TicketGuestList)
                {
                    ticketguest = new TicketGuests();

                    ticketguest.TicketId = ticket.TicketId;
                    ticketguest.GuestName = guestname;
                    guestRepo.Add(ticketguest);
                    guestRepo.Commit();
                }
                return RedirectToAction("Details", new { id = newEntity.Id() });

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors)
                {
                    foreach (var modelError in error.ValidationErrors)
                        ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                    // SH - Sometimes model errors manifest that are not displayed on screen as fields
                    // .. Due to these instances, the below line is commented out and replaced w/ the 1 above
                    //ModelState.AddModelError(modelError.PropertyName,modelError.ErrorMessage);
                }
                return View(entity);
            }
        }

        [HttpPost]
        public virtual ActionResult Edit(TKey id, TEntityViewModel entity, FormCollection collection)
        {

            try
            {
                cfares.domain._event.Ticket oldTicket = new cfares.domain._event.Ticket();
                ITEntity original = Inject(id, entity);
                var ticket = original as cfares.domain._event.Ticket;
                var ticketrecord = original as NPRTicket;
                if (ticket != null)
                {
                    int ticketID = Int32.Parse(id.ToString());
                    // TODO: Find a better way than the one commented out below to check if a ticket was already previously canceled, 
                    //       so as not to overwrite a previous canceled timestamp
                    //oldTicket = serv.Find(id) as cfares.domain._event.Ticket;
                }
                CheckEditRights(original);
                SetViewBag();
                FormValid(original, entity, collection);
                serv.Edit(original);
                serv.Commit();
                if (ticket != null)
                {
                    if ((ticket.Status != oldTicket.Status) && (ticket.Status == cfares.domain._event.TicketStatus.Canceled))
                    {
                        ticket.CanceledDate = DateTime.Now;
                        //serv.Edit(original);
                        serv.Commit();
                    }

                    var oldTicketGuests = guestRepo.GetAll().Where(x => x.TicketId == ticket.TicketId).ToList();

                    if (oldTicketGuests.Count != 0)
                    {
                        foreach (var oldGuestRecord in oldTicketGuests)
                        {
                            guestRepo.Delete(oldGuestRecord);
                            guestRepo.Commit();
                        }
                    }

                    TicketGuests ticketguest;

                    foreach (var guestname in ticketrecord.TicketGuestList)
                    {
                        ticketguest = new TicketGuests();

                        ticketguest.TicketId = ticket.TicketId;
                        ticketguest.GuestName = guestname;
                        guestRepo.Add(ticketguest);
                        guestRepo.Commit();
                    }

                }
                return RedirectToAction("Details", new { id = id });
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors)
                    foreach (var modelError in error.ValidationErrors)
                        ModelState.AddModelError(string.Empty, modelError.ErrorMessage);

                return View(entity);
            }
        }


    }
}