using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Ninject;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfacore.shared.modules.com.admin;
using cfares.domain._event.menu;
using cfares.domain._event.occ;
using cfares.domain._event.resevent.store;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository.menuitem;
using cfares.site.modules.com.Security;
using cfaresv2.Areas.Admin.Controllers._base;
using cfares.repository._event;
using cfaresv2.ViewModel;
using cfares.repository.slot;
using System.Web.Script.Serialization;
using cfares.domain.aggregates;
using System.Data.Entity.Validation;
using cfares.site.modules.com.application;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize(Area = "Admin", Roles = "Admin,Operator")]
    public class OccurrenceController : CrudController<OccurrenceRepository, Occurrence, Occurrence, int, IOccurrence>
    {

        public override OccurrenceRepository GetRepository(IResContext context)
        {
            _reservationTypeServ = new ReservationTypeRepository { Context = context };
            return new OccurrenceRepository(context);
        }



        protected override Occurrence InjectViewModel(IOccurrence entity)
        {
            /*IValueInjecter injecter = new ValueInjecter();
            var viewModel = new OccurrenceViewModel();
            injecter.Inject(viewModel, entity);*/
            return entity as Occurrence;
        }

        protected override void FormValid(IOccurrence entity, Occurrence entityViewModel, FormCollection collection)
        {




            //entity.SlotRange = servEvent.Find(entityViewModel.ResEventId.Value).GetRegistrationAvailability();


        }

        //
        // GET: /Admin/Occurrences/Edit-Dash
        [ActionName("Edit-Dash")]
        public ActionResult EditDash(int id)
        {
            try
            {
                IOccurrence occurrence = serv.Find(x => x.OccurrenceId == id, "ResEvent");

                Wizard<IResEvent> wiz = InitializeWizard(occurrence.ResEvent,
                                                         "admin.edit_dash", ControllerContext.HttpContext.Request,
                                                         ViewData);
                ViewBag.Wizard = wiz;

                return View(occurrence);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        protected MultiGroupAggregates GetAllAggregates(IOccurrence occurrence, SlotRepository slotRepo)
        {
            slotRepo.Context.Configuration.ProxyCreationEnabled = false;
            List<SlotDayAggregate> AllAggregates =
                slotRepo.GetAggregates(SlotGrouping.All, occurrence).Select(x => x.Key).ToList();
            List<SlotDayAggregate> Day =
                slotRepo.GetAggregates(SlotGrouping.Day, occurrence).Select(x => x.Key).ToList();
            List<SlotDayAggregate> Date =
                slotRepo.GetAggregates(SlotGrouping.Date, occurrence).Select(x => x.Key).ToList();
            var result = new
                MultiGroupAggregates
            {
                All = AllAggregates,
                Day = Day,
                Date = Date,
            };
            slotRepo.Context.Configuration.ProxyCreationEnabled = true;
            return result;
        }

        public ActionResult Capacity( int id )
        {
            IOccurrence occurrence = serv.Find( id );
            SlotRepository slotRepo = new SlotRepository( serv.Context );
            List<SlotDayAggregate> aggs = slotRepo.GetAggregates( SlotGrouping.Date, 
					occurrence.SlotsList.AsQueryable(), false )
				.Select( x => x.Key ).ToList();
            ViewBag.Aggregates = aggs;

            InitializeWizard( occurrence, "admin.capacity",
                ControllerContext.HttpContext.Request, ViewData );

            return View( occurrence as Occurrence );
        }

        public ActionResult Activate(int id)
        {
            var occurrenceRepo = new OccurrenceRepository<GiveawayOccurrence>(ReservationConfig.GetContext());
            GiveawayOccurrence o = (GiveawayOccurrence) occurrenceRepo.Find(id);
            o.Status = OccurrenceStatus.Live;
			var l = o.ItemsAvailable; // Otherwise items is null and triggers duplicate insertion errors - come back to revisit
            occurrenceRepo.Edit(o);
            occurrenceRepo.Commit();
            return RedirectToAction("Index", "Home", new { message = "Your Event has been activated" });
        }

        [HttpPost]
        public ActionResult Capacity(int id, Occurrence occurrenceViewModel, FormCollection collection)
        {
            IOccurrence occurrence = serv.Find(id);
            Wizard<IOccurrence> wiz = InitializeWizard(occurrence, "admin.capacity",
                                                       ControllerContext.HttpContext.Request, ViewData);
            try
            {
                if (occurrence.SlotsList.Count() != occurrenceViewModel.SlotsList.Count())
                    throw new Exception("Slots were modified during this transaction");

                for (int i = 0; i < occurrence.SlotsList.Count(); i++)
                {
                    Slot currentSlot = occurrence.SlotsList[i];
                    Slot formSubmittedSlot = occurrenceViewModel.SlotsList[i];
                    if( currentSlot.SlotId != formSubmittedSlot.SlotId ) throw new Exception("Integrity error");

					if( formSubmittedSlot.Capacity < currentSlot.OriginalCapacity && 
						occurrence.ResEvent.ReservationTypeId != "ChainwideProduct" && // Special business rule for Chainwide events to allow reduction of capacity by operators
						AppContext.User.OperationRole != UserOperationRole.Admin ) 
						return View(occurrence).AddModelStateError( ModelState, string.Format( 
							"Unable to reduce capacity to a value below what was originally defined by system administrators " + 
							"[ Slot = {0}, Original capacity = {1}, Entered capacity = {2} ]", currentSlot, currentSlot.OriginalCapacity, formSubmittedSlot.Capacity ) );

					//// carson: ech day should meet minimum daily capacity
					//foreach (var daygroup in occurrence.SlotsList.ToList().GroupBy(x => x.Start.Date)){
					//	int dailycapacity=0;
					//	foreach (var slot in daygroup){
					//		dailycapacity += occurrenceViewModel.SlotsList[occurrence.SlotsList.IndexOf(slot)].Capacity;
					//	}

					//	if (dailycapacity < occurrence.ResEvent.MinimumDailyCapacity){
					//		return View(occurrence).AddModelStateError(ModelState, String.Format("In order to lower a time slot capacity, you must meet a minimum daily capacity of {0}. Please raise the capacity of another slot.", occurrence.ResEvent.MinimumDailyCapacity));
					//	}
					//}

                    currentSlot.Capacity = formSubmittedSlot.Capacity;
                }

				// Check to see if new capacity values for this occurrence would invalidate businsess requirements
				// .. To do so, we update the in-memory values of the retreived event w/ form submitted values before testing
				if( AppContext.User.OperationRole != UserOperationRole.Admin )
				{
					ResEventRepository eventRepo = new ResEventRepository(serv.Context); // <GiveawayEvent>
					Debug.Assert(occurrence.ResEventId != null, "occurrence.ResEventId != null");
					IResEvent resEvent = eventRepo.Find(occurrence.ResEventId.Value);

					// Fill in-memory values from existing DB records for grouping in validation below
					foreach( var slot in occurrenceViewModel.SlotsList ) {
						slot.Start = occurrence.SlotsList.First( o => o.SlotId == slot.SlotId ).Start;
						slot.OccurrenceId = occurrence.OccurrenceId;
					}

					ResEvent.DailyCapacityResult lc = resEvent.GetLowestDailyCapacityByOccurrenceId( occurrenceViewModel.SlotsList );
					if( resEvent.MinimumDailyCapacity > lc.Capacity )
						return View(occurrence).AddModelStateError( ModelState, "MinimumDailyCapacity", 
							string.Format( "Minimum Daily Capacity is greater than the configured capacity for this event " + 
							"[ Minimum Daily Capacity = {0}, Configured Lowest Daily Capacity = {1} ]", resEvent.MinimumDailyCapacity, lc.Capacity ) );
				}

				serv.Edit(occurrence);
                serv.Commit();
			}
			catch( DbEntityValidationException ex ) 
			{
                foreach (var error in ex.EntityValidationErrors)
                    error.ValidationErrors.ToList().ForEach(e=>ModelState.AddModelError(e.PropertyName, e.ErrorMessage));

				return View(occurrence);
            }
			catch(Exception ex)
			{
	            Exception e = ex;
				while( e.InnerException != null ) e = e.InnerException;
                ModelState.AddModelError("",e.Message);
                return View(occurrence);
            }
            return Redirect(wiz.NextStep.Uri(ControllerContext));
        }

        public ActionResult Times(int id)
        {
            IOccurrence occurrence = serv.Find(id);
            var resEventViewModel = new OccurrenceViewModel();
            resEventViewModel.Inject(occurrence as Occurrence);
            ViewBag.ViewModel = resEventViewModel;
            //Wizard<IOccurrence> wiz = InitializeWizard(occurrence, "admin.times", ControllerContext.HttpContext.Request, ViewData);

            var slotRepo = new SlotRepository(serv.Context);
            ViewBag.CurrentAggregates = GetAllAggregates(occurrence, slotRepo);

            return View();
        }

        [HttpPost]
        public ActionResult Times(int id, FormCollection collection)
        {
            var occurrence = serv.Find(x => x.OccurrenceId == id, OccurrenceRepository.DefaultEntityIncludes);

            var resEventViewModel = new OccurrenceViewModel();
            resEventViewModel.Inject(occurrence as Occurrence);

            ViewBag.ViewModel = resEventViewModel;

            var slotRepo = new SlotRepository(serv.Context);

            //Wizard<IOccurrence> wiz = InitializeWizard(occurrence, "admin.times", ControllerContext.HttpContext.Request, ViewData);
            var cutoff = DateTime.Parse("1/1/2000 " + collection["cutoff-time"]);
            var cutoffDistance = TimeSpan.Parse(collection["cutoff-day"]);
            var SaveAggregates =
                new JavaScriptSerializer().Deserialize<SlotDayAggregateEntry[]>(collection["SaveAggregate"]);

            slotRepo.AddAggregate(occurrence, SaveAggregates, cutoff, cutoffDistance);

	        if (!string.IsNullOrEmpty(collection["DeleteAggregate"]))
            {
	            SlotDayAggregateEntry[] DeleteAggregates = new JavaScriptSerializer().Deserialize<SlotDayAggregateEntry[]>(collection["DeleteAggregate"]);
	            slotRepo.RemoveAggregate(occurrence, DeleteAggregates);
            }


	        try
            {
                serv.Edit(occurrence as Occurrence);
                serv.Commit();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName,
                                          validationError.ErrorMessage);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
	            Exception e = ex;
				while( e.InnerException != null ) e = e.InnerException;
                ModelState.AddModelError(string.Empty, e.Message);
                return View();
            }

            return View();
            //return Redirect(wiz.NextStep.Uri);
        }

		//protected override void OnActionExecuted(ActionExecutedContext filterContext)
		//{
		//	base.OnActionExecuted(filterContext);
		//}

        //
        // GET: /Admin/Occurrences/Edit-Dash        
        public ActionResult Summary(int id)
        {
            IOccurrence occurrence = serv.Find(id);
            InitializeWizard(occurrence, "admin.summary", ControllerContext.HttpContext.Request, ViewData);
            return View(occurrence as Occurrence);
        }

        public ActionResult CombinedSummary()
        {
            if (AppContext.Event != null)
                return View(AppContext.Event);
            else
                return View(new ResEvent());
        }

        private ReservationTypeRepository _reservationTypeServ;
        private IKernel _eventKernal;

        private IOccurrence ResolveOccurrence(int id)
        {

            IReservationType ty = _reservationTypeServ.FindByOccurrence(id);
            if (ty == null) return null;
            _eventKernal = ty.GetKernel();
            var _serv = OccurrenceRepository.Get(_eventKernal.Get<IOccurrence>(), serv.Context);
            return _serv.Find(id);
        }

        //
        // GET: /Admin/Event/Food        
        public ActionResult Food(int id)
        {
            IOccurrence occurrence = ResolveOccurrence(id);
            var evntRepo = new ResEventRepository<GiveawayEvent>(serv.Context);
	        Debug.Assert(occurrence.ResEventId != null, "occurrence.ResEventId != null");
	        ViewBag.ResEvent = evntRepo.Find(occurrence.ResEventId.Value);
            InitializeWizard(occurrence, "admin.food", ControllerContext.HttpContext.Request, ViewData);
            return View(occurrence as GiveawayOccurrence);
        }

        [HttpPost]
        public ActionResult Food(int id, FormCollection collection)
        {
            GiveawayOccurrence occurrence = ResolveOccurrence(id) as GiveawayOccurrence;
            var evntRepo = new ResEventRepository<GiveawayEvent>(_reservationTypeServ.Context);

            var menuItemRepo = new MenuItemRepository(_reservationTypeServ.Context);
	        Debug.Assert(occurrence != null, "occurrence != null");
	        Debug.Assert(occurrence.ResEventId != null, "occurrence.ResEventId != null");
	        var resEvent = evntRepo.Find(occurrence.ResEventId.Value) as GiveawayEvent;
            ViewBag.ResEvent = resEvent;

            occurrence.ItemsAvailable = occurrence.ItemsAvailable ?? new List<MenuItem>();

            var wiz = InitializeWizard(occurrence, "admin.food", ControllerContext.HttpContext.Request, ViewData);
            try
            {
                Dictionary<string, bool> foodIds = collection.PluckArray("food", x => x.ToLower() == "true");
                var usedCount = foodIds.Count(x => x.Value);
	            Debug.Assert(resEvent != null, "resEvent != null");

				if (resEvent.MaxItems != 0) // Temporary fix for if the maximum number of items has been left at 0. 
                { 
                    if (usedCount > resEvent.MaxItems || usedCount < resEvent.MinItems) 
                        return View(occurrence).AddModelStateError(ModelState, "", string.Format( 
                            "Sorry, you must select a minimum of {0} items and a maximum of {1} items. " + 
                            "You have selected {2} items.", resEvent.MinItems, resEvent.MaxItems, usedCount)); 
                }

				foreach (var kvp in foodIds)
                {
                    bool isOn = kvp.Value;
                    string itemId = kvp.Key;

                    if (!isOn && occurrence.ItemsAvailable.Any(x => x.DomId == itemId))
                    {
                        occurrence.ItemsAvailable.Remove(occurrence.ItemsAvailable.First(x => x.DomId == itemId));
                    }
                    else if (isOn && occurrence.ItemsAvailable.All(x => x.DomId != itemId))
                    {
                        var menuItem = menuItemRepo.Find(itemId);
                        occurrence.ItemsAvailable.Add(menuItem);
                    }

                }
                var itemIds = foodIds.Select(x => x.Key);
                var unmentionedItems = occurrence.ItemsAvailable.Where(x => !itemIds.Contains(x.DomId)).ToList();
                foreach (var unmentionedItem in unmentionedItems)
                {
                    occurrence.ItemsAvailable.Remove(unmentionedItem);
                }
                serv.Edit(occurrence);
                serv.Commit();
                return Redirect(wiz.NextStep.Uri(ControllerContext));
            }
            catch (Exception ex)
            {
	            Exception e = ex;
				while( e.InnerException != null ) e = e.InnerException;
                ModelState.AddModelError("", e.Message);
                return View(occurrence);
            }


        }
    }
}
