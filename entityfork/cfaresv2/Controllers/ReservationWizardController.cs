using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Web.Mvc;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using cfacore.shared.domain._base;
using cfacore.shared.modules.helpers;
using cfares.domain.Reservation;
using cfares.domain._event;
using cfares.domain._event.occ;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.repository.store;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using cfares.site.modules.com.reservations.res;
using cfares.site.modules.repository.ticket;
using cfares.site.modules.user;
using cfaresv2.Models;
using cfaresv2.ViewModel;
using cfacore.shared.modules.com.admin;
using cfacore.domain.user;

namespace cfaresv2.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class WizardStepAttribute:Attribute
    {
        public string Step;

        public WizardStepAttribute()
        {
        }

        public WizardStepAttribute(string step)
        {
            Step = step;
        }
    }

    public abstract class ReservationWizardController<TWizard, TModel> : ReservationControllerBase
        where TModel : class, ITicket, new()
        where TWizard : class,IReservationWizard<TModel>
	{
        protected TWizard Wizard;
        protected TModel Model;

		private ResTicketRepository<TModel> _ticketRepository;
	    protected ResTicketRepository<TModel> TicketRepository
	    {
		    get
		    {
			    if( _ticketRepository == null )
			    {
					IResContext context = ReservationConfig.GetContext();
					_ticketRepository = new ResTicketRepository<TModel>(context);
			    }
				return _ticketRepository;
		    }
		    set { _ticketRepository = value; }
	    }

	    protected abstract TWizard GetReservationWizard(string step, TModel model);
        protected abstract TModel GetModel(System.Web.Routing.RequestContext requestContext);

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string step = "Reservation.Reservation";
            foreach (
                var filter in
                    filterContext.ActionDescriptor.GetCustomAttributes(typeof (WizardStepAttribute), false)
                                 .Cast<WizardStepAttribute>())
            {
                step = filter.Step;
            }

            Model = GetModel(filterContext.RequestContext);
            Wizard = GetReservationWizard(step, Model);
            
            Wizard.SetStep(step);
            Wizard.ApplyContext(filterContext.RequestContext);

            
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewData["wizard"] = Wizard;
            ViewData["Wizard"] = Wizard;
            ViewData["wiz"] = Wizard;
            base.OnActionExecuted(filterContext);
        }

        public RedirectResult NextStepResult()
        {
            return Redirect(Wizard.NextStep.Uri(ControllerContext));
        }

        public RedirectResult NextStepResult(IWizard wizard, bool nextIncomplete=false)
        {
            if(nextIncomplete)
            return Redirect(wizard.Steps.First(x=>!x.Complete).Uri(ControllerContext));
            return Redirect(wizard.NextStep.Uri(ControllerContext));
        }

        public RedirectResult PrevStepResult()
        {
            return Redirect(Wizard.PreviousStep.Uri(ControllerContext));
        }

		[DynamicOutputCache]
        public virtual ActionResult Occurrence(int occurrence)
        {
            var occurrenceRepo = new OccurrenceRepository(ResContext);

            var occurrenceModel = occurrenceRepo.Find(occurrence);

            Wizard.Prime(occurrenceModel);
            return NextStepResult(Wizard,true);
        }

		[DynamicOutputCache]
        public ActionResult FAQs()
        {
            string reservationTypeId = Event.ReservationTypeId;
            string resTemplateId = Event.TemplateId;
            const string defaultFaq = "Default";

            var _markdown = new MarkdownSharp.Markdown();
            

            string[] faqSources = new[] {defaultFaq,resTemplateId,reservationTypeId};
            ICollection<FAQGroup> faqGroups=new Collection<FAQGroup>();
            foreach (string faqSource in faqSources)
            {
                string path = String.Format("~/Data/Xml/FAQs/{0}.xml", faqSource);
                path = HttpContext.Request.MapPath(path);
                if (!System.IO.File.Exists(path))
                    continue;
                XDocument xmlDoc = XDocument.Load(path);
                var groups = xmlDoc.Descendants("group").ToList();

                if (!groups.Any())
                    continue;

				// ReSharper disable PossibleNullReferenceException
                IEnumerable<FAQGroup> toAddGroups = groups.Select(x=>new FAQGroup {
                        Title = x.Attribute("title").Value,
                        FAQs = x.Descendants("faq").Select(f => new FAQ { Question = (f.Element("q").Value.Trim()), Answer = _markdown.Transform(f.Element("a").Value.Trim()) }).ToList()
                } );
				// ReSharper restore PossibleNullReferenceException
	            IEnumerable<FAQGroup> addGroups = toAddGroups as List<FAQGroup> ?? toAddGroups.ToList();
	            if(addGroups.Any()) faqGroups = faqGroups.Concat(addGroups).ToList();
            }

            return View(faqGroups);
        }

        [WizardStep("Reservation.SearchByLocation")]
		[DynamicOutputCache]
        public virtual ActionResult SearchByLocation()
        {
            //carson - A wild hack appears. Temp fix for search by location not getting any resulats if resevent is not set
            if ( Request.Url != null && (Request.Url.Host == "chicagoarea.mothersondate.com" || Request.Url.Host == "www.mothersondate.com") && String.IsNullOrEmpty(Request.QueryString.ToString())){
                return Redirect("http://chicagoarea.mothersondate.com/?SelectedLocation=Chicago%2C+IL&SelectedRadius=100");
            }

	        IQueryable<IOccurrence> occurrenceListSource;
			if( AppContext.Event != null )
				occurrenceListSource = AppContext.Event.ReservationTypeId == "ChainwideProduct" ? 
					AppContext.Event.Occurrences.Cast<IOccurrence>().AsQueryable() : // Special business rule where status does not filter for Chainwide
					AppContext.Event.ActiveOccurrences.Cast<IOccurrence>().AsQueryable();
			else
				occurrenceListSource = OccurrenceRepo.GetActive()
					.Where(o => o.ResEvent.ReservationTypeId == AppContext.EventType.ReservationTypeId);

            if (Request.QueryString != null
                && !string.IsNullOrEmpty(Request.QueryString["SelectedLocation"])
                && !string.IsNullOrEmpty(Request.QueryString["SelectedRadius"]))
            {
                string locationSearchTerm = Request.QueryString["SelectedLocation"];
                int searchRadius = int.Parse(Request.QueryString["SelectedRadius"]);

				// Perform search of all active events *AND locations ( from zipcode )
                // .. If we already have an Event selected, filter by participating locations
                IEnumerable<OccurenceLocationResult> selectedOccurences;

                // SH - Proximity search disabled feature ( turned off by default )
                // .. However is presently turned on in staging due to API key issue
                // .. To override this value, "ProximityEnabled" may be used in the QA

                bool localProximityEnabled = (!string.IsNullOrWhiteSpace(Request.QueryString["ProximityEnabled"]));
                bool configProximityEnabled = (string.IsNullOrWhiteSpace(AppContext.Configuration.ProximitySearchDisabled));

                if (localProximityEnabled || configProximityEnabled || 
					( AppContext.Event != null && AppContext.Event.ReservationTypeId == "ChainwideProduct" ))
                {
                    // Proximity search by API enabled / default query set
                    try
                    {
                        if (AppContext.Event != null)
                            selectedOccurences = occurrenceListSource
                                .GetByLocationSearchTermAndRadiusAndLocations(
                                    locationSearchTerm, searchRadius);
                        else
                            selectedOccurences = OccurrenceRepo
                                .GetByLocationSearchTermAndRadiusAndLocations(locationSearchTerm, searchRadius,
                                    occurrenceListSource.Select(o => o.Store));
                    }
                    catch (InvalidDataException)
                    {
                        // Special case for invalid data exception ( message adjustment )
                        string msg = string.Format("Sorry, there are no participating Restaurants " +
                            "within the radius of {0}. Please expand the radius or enter in a different location.",
                            locationSearchTerm);

                        ModelState.AddModelError("SelectedLocation", msg);
                        ITicket tr = GetModel(Request.RequestContext);
                        return View(new SearchByLocationContext { Ticket = tr });
                    }
                    catch (InstanceNotFoundException)
                    {
                        // Special case for not found exception ( message adjustment )
                        string msg = string.Format("Sorry, {0} is an invalid zip code. Please enter a different zip code.",
                            locationSearchTerm);

                        ModelState.AddModelError("SelectedLocation", msg);
                        ITicket tr = GetModel(Request.RequestContext);
                        return View(new SearchByLocationContext { Ticket = tr });
                    }
                    catch (Exception)
                    {
                        //carson: general catch, was getting a 503 from wheretogetit
                        string msg = "Sorry there was a problem checking that location";
                        ModelState.AddModelError("SelectedLocation", msg);
                        ITicket tr = GetModel(Request.RequestContext);
                        return View(new SearchByLocationContext { Ticket = tr });
                    }
                }
                else
                {
                    // Proximity search by API disabled / fallback filtering queries
                    selectedOccurences = occurrenceListSource.Select(x => new OccurenceLocationResult(x, 0, ""));
                }

                ITicket ticket = GetModel(Request.RequestContext);
                selectedOccurences = selectedOccurences.AsQueryable().Include("SlotsList");
                return View(new SearchByLocationContext
                {
                    SelectedLocation = locationSearchTerm,
                    SelectedRadius = searchRadius,
                    EventSearchResults = selectedOccurences.ToList(),
                    Ticket = ticket,
                });
            }
	        if (AppContext.Event != null && AppContext.Event.Occurrences != null && AppContext.Event.Occurrences.Any() && AppContext.Event.Occurrences.Count() < 10)
	        {
		        var ticket = GetModel(Request.RequestContext);
		        var occurrences = (occurrenceListSource.AsQueryable()).Include("Store")
					.OrderBy(x => x.Store.Name)
					.Select(x => new OccurenceLocationResult(x, -1.0, "none")).ToList();

		        return View(new SearchByLocationContext { Ticket = ticket , EventSearchResults = occurrences, SelectedLocation = null, SelectedRadius = -1 });
	        }
	        else
	        {
		        var ticket = GetModel(Request.RequestContext);
		        return View(new SearchByLocationContext { Ticket = ticket });
	        }
        }

		[DynamicOutputCache]
	    protected ActionResult GetReservationView(ITicketAccountModel viewModel, IOccurrenceRepository occurrenceRepo)
        {
            if (Model.SlotId == null)
            {
                if (Wizard.OccurrenceId != null)
                    ViewBag.Occurrence = occurrenceRepo.Find(Wizard.OccurrenceId.Value);

				else if (!string.IsNullOrEmpty(Request.QueryString["location"]))
                {
                    var locationStr = Request.QueryString["location"];
                    ViewBag.Occurrence = occurrenceRepo.Find(x => x.StoreId == locationStr && x.ResEventId == Event.ResEventId);
                }

                else if (!string.IsNullOrEmpty(Request.QueryString["marketable"]))
                {
                    var locationStr = Request.QueryString["marketable"];
                    ViewBag.Occurrence =
                        occurrenceRepo.GetAll()
                                      .Include("Store")
                                      .FirstOrDefault(
                                          x => x.Store.MarketableName == locationStr && x.ResEventId == Event.ResEventId);
                }
                else
                    return PrevStepResult();
            }
            if (Model.TicketId != 0)
            {
                var storeRepo = new LocationRepository(ResContext);
                viewModel.StoreOptIn = storeRepo.HasEmailSubscription(Model.Slot.Occurrence.Store, Model.Owner);
            }
            return View(viewModel);
        }

        [WizardStep("Reservation.Reservation")]
		//[DynamicOutputCache]
        public abstract ActionResult Reservation(int? id);

        [WizardStep("Reservation.Reservation")]
        [HttpPost]
		//[DynamicOutputCache]
        public abstract ActionResult Reservation(int? id, TicketAccountModel<TModel> ViewModel, FormCollection collection);

		//[DynamicOutputCache]
        protected ActionResult PostReservationView(int? id, ITicketAccountModel ViewModel, FormCollection collection,ITicketRepository repo,ISlotRepository slotRepo)
        {
            ViewModel.GetTicket().SlotId = ViewModel.GetTicket().SlotId ?? collection["Ticket.SlotId"].ToInt();
			if( ViewModel.GetTicket().SlotId <= 0 )
                ModelState.AddModelError("Ticket.SlotId", "You must choose a restaurant for this event");

            //Extra Validation 
            if (ViewModel.AgeCheck != cfacore.shared.modules.com.datatypes.YesNo.Yes)
                ModelState.AddModelError("AgeCheck", "You must be of age to attend this event");

            if (ViewModel.User.FirstName != null && Regex.Match(ViewModel.User.FirstName, @"([^a-zA-Z'\- ]|^\s)").Success)
                ModelState.AddModelError("User.FirstName", "First Name can only contain letters");

            if (ViewModel.User.LastName != null && Regex.Match(ViewModel.User.LastName, @"([^a-zA-Z'\- ]|^\s)").Success)
                ModelState.AddModelError("User.LastName", "Last Name can only contain letters");

			if (ViewModel.User.Address != null && Regex.Match(ViewModel.User.Address.ZipString, @"^\d$").Success)
                ModelState.AddModelError("User.Address.ZipString", "Zip codes can only contain numbers");

            ViewModel.GetTicket().Owner.Gender = collection["User.Gender"] == null ? 
					ViewModel.GetTicket().Owner.Gender : (Gender)Convert.ToInt32(collection["User.Gender"].Split(',').LastOrDefault());

            bool userCreated = false;
	        ITicket ticket = null;
            try
            {
				// If logged in, disable new registration validation
                if (AppContext.IsAuthenticated) {
                    var userFields = new[] { "EmailAddress", "Password", "ConfirmPassword", "ConfirmEmail" };
                    var toRemove = ModelState.Where(x => x.Value.Errors.Count > 0 && userFields.Contains(x.Key));
                    toRemove.ToList().ForEach(x => x.Value.Errors.Clear());
                }

				// Special validation bypass logic for User.Email
                if (!ModelState.IsValid) {
                    IEnumerable<KeyValuePair<string, ModelState>> errors = ModelState.Where(x => x.Value.Errors.Count > 0);
                    foreach (var kvp in errors)
                        foreach (var error in kvp.Value.Errors.ToList())
                            if (kvp.Key == "User.Email" && error.ErrorMessage == ("The Email Address field is required.") 
								&& !string.IsNullOrEmpty(ViewModel.EmailAddress))
                                kvp.Value.Errors.Remove(error);

					if (!ModelState.IsValid)
                        return View(ViewModel);
                }

				// User saving / creation logic
                MembershipCreateStatus stat;
                if (!AppContext.IsAuthenticated)
                {
                    // New user - Attempt to create
                    // TODO - What if email address already in use ( address usability flow )
                    ViewModel.GetTicket().Owner.Username = ViewModel.GetTicket().Owner.Email;
                    ViewModel.GetTicket().Owner.OperationRole = UserOperationRole.Customer;
                    UserMembershipRepo.CreateUser(ViewModel.GetTicket().Owner, ViewModel.Password, false, out stat);
                    if (stat == MembershipCreateStatus.DuplicateUserName || stat == MembershipCreateStatus.DuplicateEmail)
                        throw new Exception("User name already exists");
                    // CreateUser above saves the user and ensures UserId is set
                    ViewModel.GetTicket().OwnerId = ViewModel.GetTicket().Owner.UserId;
                    if (stat == MembershipCreateStatus.Success)
                        userCreated = true;
                }
                else
                {
                    stat = MembershipCreateStatus.Success;
                    ViewModel.GetTicket().Owner = null;
                    ViewModel.User = null;
                    ViewModel.GetTicket().OwnerId = AppContext.User.UserId;
                }
                if (stat != MembershipCreateStatus.Success)
                    throw new Exception(UserMembershipRepository.ErrorCodeToString(stat));

				// Ticket processing ( addition / edit to repository )
	            ticket = ViewModel.GetTicket();
                
				if (id == null)
                    repo.Add(ticket);
                else
                {
                    ticket.TicketId = id.Value;
                    repo.Edit(ticket);
                }
                if( ticket == null ) throw new ApplicationException( "Invalid ticket" );
                if( ticket.SlotId == null ) throw new ApplicationException( "Invalid ticket slot selected" );

				// Get slot details and check to see if tickets area available
                ISlot slot = slotRepo.Find(ticket.SlotId.Value);
                ViewModel.GetTicket().Slot = slot as Slot;
                var resevent = slot.Occurrence.ResEvent;

                // if maximum capacity is set, check aginst master capacity instead of slot
                if((resevent.MaximumCapacity ?? 0) > 0){
                    if (resevent.TotalTicketsIssued >= resevent.MaximumCapacity){
                        ViewModel.GetTicket().Status = TicketStatus.Partial;
                        repo.Commit();
                        return RedirectToAction("EventFull");
                    }
                    if (resevent.TotalTicketsIssued + ticket.AllocatedCapacity > resevent.MaximumCapacity)
                    {
                        ViewModel.GetTicket().Status = TicketStatus.Partial;
                        repo.Commit();
                        return RedirectToAction("Reservation", new { eventid = slot.Occurrence.ResEventId, message = String.Format("Sorry, there are only {0} more tickets available for this event.", resevent.TicketsAvailable) });
                    }
                }
                else{
                    if (!slot.AreTicketsAvailable()){
                        ViewModel.GetTicket().Status = TicketStatus.Partial;
                        repo.Commit();
                        return RedirectToAction("Reservation", new { eventid = slot.Occurrence.ResEventId, message = "Sorry, that slot filled up during your registration, please try another." });
                    }
                }
                
                var storeRepo = new LocationRepository(ResContext);
	            var ownerId = ViewModel.GetTicket().OwnerId;
                if (ownerId != null)
                {
                    bool? favorite = Wizard == null?null: Wizard.IsFavorite;
                    storeRepo.AddOrUpdateEmailSubscription(slot.Occurrence.Store, ownerId.Value, ViewModel.StoreOptIn,favorite);
                }

                //Check For Multiple Tickets Registered
                //mdrake this will fail if user is not logged in
                string useremail = (ticket.Owner==null) ? AppContext.User.Email : ticket.Owner.Email;
                IEnumerable<ITicket> tickets = repo.Get(x => x.Owner.Email == useremail && x.Status == TicketStatus.Reserved).ToList();
                if (tickets.Any() && id==null)
                {
                    // if debugging allow multiple registrations with the same email
                    #if !DEBUG
                        ViewBag.AlreadyRegistered = "You have already registered for this event. Only one registration is permitted.";
                        ViewBag.Occurrence = ticket.GetOccurrence();
                        ModelState.AddModelError("", ViewBag.AlreadyRegistered);
                        return View(ViewModel);
                    #endif
                }

				// Validation for whether or not saving the current ticket would place the slot
				// .. into a state of capacity overage is performed in the ResTicketRepository 
				// .. ( "exceeds those available" error message )
                repo.Commit();

				if (Wizard != null) Wizard.Prime(ViewModel.GetTicket() as TModel);
	            if (userCreated) UserMembershipRepo.Authenticate(ViewModel.GetTicket().Owner);

                return NextStepResult();
            }
            catch (Exception ex)
            {
                if (userCreated)
                {
					// When deleting newly created users, must use a new context
					// .. Otherwise, when the commit is made on the delete, any existing context errors will
					// .. prevent the commit from taking place and another exception will be thrown
					// .. ( this time unhandled )
					IResContext ctx = ReservationConfig.GetContext();
					UserMembershipRepository userRepo = new UserMembershipRepository( HttpContext, ctx );
	                userRepo.DeleteUserAndAccount(ViewModel.User.Username);
                }
                string msg = ex.ToString();

                if ((msg.Contains("UIX_UserEmail") && msg.Contains("duplicate")) || msg.Contains("User name already exists"))
                    ModelState.AddModelStateError("EmailAddress",
                        "This email address is in use.  Please login or try another email address.");

				// Message translation for "Requested number of tickets exceeds those available 
				// .. for this event occurence [ Tickets Requested = 7, Tickets Available = 1 ]"
				else if( ex.Message.Contains( "exceeds those available" ) && ticket != null && ticket.Slot != null )
				{
					int ticketsAvailable = ticket.Slot.GetTicketsAvailableExcept( ticket.TicketId );
					string ticketsAdjusted = ticketsAvailable == 1 ? "ticket" : "tickets";
					string conditionAdjusted = ticketsAvailable == 1 ? "is" : "are";
					string template =  string.Format( "We're sorry, there {2} only {0} {1} available", ticketsAvailable, ticketsAdjusted, conditionAdjusted );
					if( ticketsAvailable <= 0 ) template = "We're sorry, there are no tickets available";
					if( AppContext.Configuration.ApplicationId != Application.CFA ) template += " for this event";
					else template += " for this Restaurant's guest list."; //" Please choose a different Restaurant!"; SH - Removed sentence per Gabby req

					//template = "Sorry, but we have run out of tickets for this event.";
                    if( ticket.Slot.Occurrence.ResEvent.ReservationTypeId == "reception" && ticketsAvailable <= 0 )
						template = "This restaurant has reached it's max capacity, but you are still invited to the event! " + 
							"Please select the 'other' option in the drop down to complete your reservation";

					ModelState.AddModelStateError( template );
				}

				else ModelState.AddModelStateError( ex.Message );

                return View(ViewModel);
            }
        }

        [WizardStep("Reservation.Review")]
		[DynamicOutputCache]
        public ActionResult Review(int? id)
        {
            if (id == null)
                return PrevStepResult();

            var storeRepo = new LocationRepository(ResContext);
            ViewBag.StoreUpdates = storeRepo.HasEmailSubscription(Model.Slot.Occurrence.Store, Model.Owner);
            return View(Model);
        }

        // POST: /Review
        [HttpPost]
        [WizardStep("Reservation.Review")]
		[DynamicOutputCache]
        public ActionResult Review(int? id, FormCollection collection)
        {
            // Init wizard
            
            if (id == null) return PrevStepResult();
            
            TModel ticket = (TModel)TicketRepository.Find(id.Value);
            if (collection["action"].ToLower().Contains("cancel"))
            {
                TicketRepository.Delete(ticket);
                TicketRepository.Commit();
                //new {message="That ticket has been deleted"}
                return UrlHelpers.RedirectToEventHome();
            }

            // Update ticket status on "confirmation"
            ticket.Status = TicketStatus.Reserved;
            TicketRepository.Commit();
            // Redirect to success
            Wizard.Prime(ticket);
            return NextStepResult();
        }

        [WizardStep("Reservation.Success")]
		[DynamicOutputCache]
        public ActionResult Success(int id)
        {
            return View(Model);
        }
    }
}
