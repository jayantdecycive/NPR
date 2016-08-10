
#region Imports

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Omu.ValueInjecter;
using cfacore.shared.domain.user;
using cfacore.shared.modules.helpers;
using cfacore.shared.modules.repository;
using cfares.domain._event.menu;
using cfares.domain._event.occ;
using cfares.domain._event.resevent.store;
using cfares.domain.store;
using cfares.entity.dbcontext.res_event;
using cfares.repository.exceptions;
using cfares.repository.menuitem;
using cfares.repository.store;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Models;
using cfaresv2.ViewModel;
using cfares.domain._event;
using cfacore.shared.domain._base;
using cfares.domain._event.resevent;
using cfacore.shared.modules.com.admin;
using System.Data.Entity.Validation;
using cfaresv2.Areas.Admin.Controllers._base;
using cfacore.site.controllers._event;
using cfares.repository._event;
using cfares.domain.aggregates;
using System.Web.Script.Serialization;
using cfares.repository.slot;
using System.Collections.Generic;
using Ninject;
using npr.repository.slot;
using System.Globalization;
using HtmlAgilityPack;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin" )]
    public class EventController : AdminController
    {
        IResEventRepository _serv;
        ReservationTypeRepository _reservationTypeServ;
        IKernel _eventKernal;
        ResTemplateRepository _resTemplateServ;
		LocationRepository _locationServ;
		OccurrenceRepository _occurrenceServ;
        
        
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            var context = ReservationConfig.GetContext();
            _serv = new ResEventRepository<ResEvent>(context);
            _reservationTypeServ = new ReservationTypeRepository { Context=context};
            _resTemplateServ = new ResTemplateRepository(context);
			_locationServ = new LocationRepository( context );
			_occurrenceServ = new OccurrenceRepository( context );

            return base.BeginExecute(requestContext, callback, state);
        }

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Start(int? id)
        {
			EventCreateStartViewModel m = new EventCreateStartViewModel();
	        m.Event = (ResEvent) (id != null ? ResolveEvent(id.GetValueOrDefault()) : new ResEvent());
			if( m.Event == null )
                return RedirectToAction("Index", "Event", new { message = 
					"Selected event has been deleted and is no longer available ( " + id + " )" });

            // No location specified - Check for default location initialization
            ReservationConfig.AddElement a = AppContext.Configuration.EventTypes.GetByKey(m.Event.ReservationTypeId);

            
			if(m.Event.Occurrences!=null && m.Event.Occurrences.Any() )
			{
				// Use existing occurrence location
				m.LocationNumber = m.Event.Occurrences.First().Store.LocationNumber;
			}
			else
			{
				
				if( a != null && a.DefaultLocation.IsValid() )
					m.LocationNumber = a.DefaultLocation;
                
			}

		    if (string.IsNullOrEmpty(m.Event.TemplateId))
		    {
                if (a != null && a.DefaultTemplate.IsValid())
                    m.Event.TemplateId = a.DefaultTemplate;
		    }

          

		    Wizard<IResEvent> w = InitializeWizard(m.Event, "admin.start", ControllerContext.HttpContext.Request, ViewData);
 	        w.Name = string.Empty;
			return View( m );
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Start(int id, FormCollection collection)
        {
			EventCreateStartViewModel m = new EventCreateStartViewModel();
            m.Event = _serv.Find(x=>x.ResEventId==id,ResEventRepository.DefaultEntityIncludes) as ResEvent;
			if( m.Event == null )
                return RedirectToAction("Index", "Event", new { message = 
					"Selected event has been deleted and is no longer available ( " + id + " )" });

            Wizard<IResEvent> wiz  = InitializeWizard(m.Event, "admin.start", ControllerContext.HttpContext.Request, ViewData);

			m.Event.Name = collection["Event.Name"];
            m.Event.Urls = collection["Event.Url"];
           // m.Event.Urls = collection["Event.Urls"]; //"http://npr.decycivefarm.com/" + collection["Event.Urls"];//"npr.decycivefarm.com/" + collection["Event.Urls"];
           // m.Event.OriginalUrl = "http://localhost:1474/npr/Home/EventDetails/" + id;
            m.Event.TemplateId = collection["Event.TemplateId"];
			m.Event.CategoryId = collection["Event.CategoryId.Event.CategoryId"].ToNullableInt();


            m.Event.SiteStart = DateTime.Parse(Convert.ToString(collection["Event.SiteStart"]));
            m.Event.SiteEnd = DateTime.Parse(Convert.ToString(collection["Event.SiteEnd"]));
            m.Event.RegistrationStart = DateTime.Parse(Convert.ToString(collection["Event.RegistrationStart"]));
            m.Event.RegistrationEnd = DateTime.Parse(Convert.ToString(collection["Event.RegistrationEnd"]));

            //m.Event.SiteStart = DateTime.Now.AddDays(1);
            //m.Event.SiteEnd = DateTime.Now.AddDays(10);
            //m.Event.RegistrationStart = DateTime.Now.AddDays(1);
            //m.Event.RegistrationEnd = DateTime.Now.AddDays(10);

            m.Event.IsPaid = collection["Event.IsPaid"].ToBoolean();

            //make registration end on end of chosen day to allow for slots on that day
            m.Event.RegistrationEnd = m.Event.RegistrationEnd.Date + new TimeSpan(23, 58, 00);
            m.Event.SiteEnd = m.Event.SiteEnd.Date + new TimeSpan(23, 59, 00);


            // Ensure location number selected
            m.LocationNumber = collection["LocationNumber.LocationNumber"];
            if (string.IsNullOrWhiteSpace(m.LocationNumber)) // For tour field hidden support
                m.LocationNumber = collection["LocationNumber"];
            if (string.IsNullOrWhiteSpace(m.LocationNumber) || m.LocationNumber == "-1")
            {
                ModelState.AddModelError("LocationNumber.LocationNumber", "Location is required");
                ModelState.AddModelError("LocationNumber", "Location is required");
                return View(m);
            }
            ResStore s = _locationServ.Find(m.LocationNumber);
            if (s == null) throw new ApplicationException(string.Format(
               "Unable to find location [ ID = {0} ]", m.LocationNumber));

            // Date restriction checks ( registration dates must fall within the site date range )
            if (string.IsNullOrEmpty(m.Event.Name))
                return View(m).AddModelStateError(ModelState, "Event.Name",
                    "Event Name is Required");

			// Date restriction checks ( registration dates must fall within the site date range )
			if( m.Event.RegistrationStart < m.Event.SiteStart || m.Event.RegistrationStart > m.Event.SiteEnd )
				return View( m ).AddModelStateError( ModelState, "RegistrationStart", 
					"First Slot Date must fall between Site Visibility Start and End Dates" );

			if( m.Event.RegistrationEnd < m.Event.SiteStart || m.Event.RegistrationEnd > m.Event.SiteEnd )
				return View( m ).AddModelStateError( ModelState, "RegistrationEnd", 
					"Last Slot Date must fall between Site Visibility Start and End Dates" );

            if (String.IsNullOrEmpty(m.Event.TemplateId) && Equals(m.Event.ReservationTypeId, "Tour"))
                m.Event.TemplateId = "NPR-Reservations-Tour";
            else
                m.Event.TemplateId = "NPR-Reservations-SpecialEvent";


			//If IsPaid is provided, validate Ticket Amount
			if (m.Event.IsPaid)
			{
				string ticketAmountStr = collection["Event.TicketAmount"];
				decimal ticketAmount = 0;
				if (String.IsNullOrWhiteSpace(ticketAmountStr) || !Decimal.TryParse(ticketAmountStr, out ticketAmount) || ticketAmount <= 0)
				{
					ModelState.AddModelError("TicketAmount.TicketAmount", "If you specify an event as paid, you must provide a valid ticket amount.");
					ModelState.AddModelError("Event.TicketAmount", "TicketAmount is required");
					return View(m);
				}

				m.Event.TicketAmount = ticketAmount;
			}


			// If the occurence location is changing, delete all references to existing occurences
			// .. This will have the effect of also deleting any defined slots
			// .. FUTURE - warn user client side of this functional aspect
			if( m.Event.Occurrences.Any() && ! m.Event.Occurrences.First().Store.LocationNumber
                    .Equals(m.LocationNumber, StringComparison.OrdinalIgnoreCase))
				_occurrenceServ.DeleteByEvent( m.Event );

			// Selected location gets linked to the event by means of an Occurence entity
			// .. Slot range = [ sql min / max ]
			IOccurrence o = m.Event.GetOrCreateOccurrence( s );
	        o.Start = (DateTime) SqlDateTime.MinValue;
	        o.End = (DateTime) SqlDateTime.MaxValue;
	        o.SlotRangeStart = (DateTime) SqlDateTime.MinValue;
	        o.SlotRangeEnd = (DateTime) SqlDateTime.MaxValue;

            m.Event.IsFeatured = collection["Event.IsFeatured"].ToBoolean();
            m.Event.Visibility = (ResEventVisibility) Enum.Parse(typeof(ResEventVisibility), collection["Event.Visibility"] ?? collection["Visibility"] );
            m.Event.Status = (ResEventStatus) Enum.Parse(typeof(ResEventStatus), collection["Event.Status"]);
			if( m.Event.Status == ResEventStatus.Temp )
                m.Event.Status = ResEventStatus.Draft;

            //= InitializeWizard( m.Event, "admin.start", 
			//	ControllerContext.HttpContext.Request, ViewData);
            try
            {
                _serv.Edit( m.Event );
                _serv.Commit();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

				return View( m );
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("UIX_SiteUrl"))
                {
                    if (!string.IsNullOrEmpty(collection["Event.Urls"]))
                    {
                        string[] toAddUrls = collection["Event.Urls"].Split(',').Select(x=>x.Trim()).ToArray();
                        ResSiteUrl[] badUrls = _serv.FindUsedUrls(toAddUrls);
                        string[] badUrlStrings = badUrls.Where(x => x.ResEventId!=m.Event.ResEventId).Select(x=>x.Url).ToArray();
                        ModelState.AddModelError("Event.Urls", string.Format("Sorry, the following urls are in use: {0}",string.Join(", ",badUrlStrings)));
                    }
                    else
                        ModelState.AddModelError("Event.Urls", "Sorry, that url is in use.");
                }
                else
                    ModelState.AddModelError(string.Empty, ex.Message);

				return View( m );
            }
            catch (Exception ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View( m );
            }

            return Redirect(wiz.NextStep.Uri(ControllerContext));
        }

        //
        // GET: /Admin/Event/Past/

        public ActionResult Past()
        {
            return View();
        }

        //
        // GET: /Admin/Event/Upcoming/

        public ActionResult Upcoming()
        {
            return View();
        }


        //
        // GET: /Admin/Event/Verify/

        public ActionResult Verify()
        {
            return View();
        }

        //
        // GET: /Admin/Event/Confirm/

        public ActionResult Confirm()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
			//IResEvent resEvent = ((ResEventRepository<ResEvent>)_serv).FindWithOccurrences(id);
            IResEvent resEvent = _serv.Find(id);

            return View(resEvent as ResEvent);
        }

        public ActionResult DetailsSummary(int id)
       
        {
            IResEvent resEvent = _serv.Find(id);
            InitializeWizard(resEvent, "admin.details", ControllerContext.HttpContext.Request, ViewData);
			return View(resEvent as ResEvent);
            
           
        }

        [HttpPost, ValidateInput(false)]
		public ActionResult DetailsSummary(int id, FormCollection collection)
		{
            ResEvent resEvent = _serv.Find(x=>x.ResEventId==id,ResEventRepository.DefaultEntityIncludes) as ResEvent;
			if( resEvent == null )
                return RedirectToAction("Index", "Event", new { message = 
					"Selected event has been deleted and is no longer available ( " + id + " )" });

            Wizard<IResEvent> wiz = InitializeWizard( resEvent, "admin.details", 
				ControllerContext.HttpContext.Request, ViewData);

			resEvent.SubHeading = collection["SubHeading"];
			resEvent.Description = collection["Description"];
			resEvent.MediaId = collection["MediaId"].ToNullableInt();
			resEvent.MustBeOfAgeToAttend = collection["MustBeOfAgeToAttend"].ToBoolean();

            try
            {
                _serv.Edit( resEvent );
                _serv.Commit();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

				return View( resEvent );
            }
            catch (Exception ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View( resEvent );
            }

            return Redirect(wiz.NextStep.Uri(ControllerContext));
		}

        //
        // GET: /Admin/Event/Summary/5
        public ActionResult Summary(int id)
        {
            IResEvent resEvent = ResolveEvent(id);
			if( resEvent == null )
                return RedirectToAction("Index", "Event", new { message = 
					"Selected event has been deleted and is no longer available ( " + id + " )" });

            ViewBag.ParticipatingStores = resEvent.ParticipatingStoresListViewModel
                .ToList();

            SlotRepository slotRepo = new SlotRepository(_serv.Context);
            ViewBag.CurrentAggregates = GetAllAggregates(resEvent, slotRepo);
            

			var w = InitializeWizard(resEvent, "admin.summary", ControllerContext.HttpContext.Request, ViewData);
	        w.Name = resEvent.Name;
            return View(resEvent as ResEvent);
        }

        [ActionName("Tour-Summary")]
        public ActionResult TourSummary(int id)
        {
            IResEvent resEvent = ResolveEvent(id);
            if (resEvent == null)
                return RedirectToAction("Index", "Event", new
                {
                    message =
                        "Selected event has been deleted and is no longer available ( " + id + " )"
                });

            ViewBag.ParticipatingStores = resEvent.ParticipatingStoresListViewModel
                .ToList();

            SlotRepository slotRepo = new SlotRepository(_serv.Context);
            ViewBag.CurrentAggregates = GetAllAggregates(resEvent, slotRepo);


            var w = InitializeWizard(resEvent, "admin.summary", ControllerContext.HttpContext.Request, ViewData);
            w.Name = resEvent.Name;
            return View(resEvent as ResEvent);
        }

        private IResEvent ResolveEvent(int id)
        {
            IReservationType ty = _reservationTypeServ.FindByEvent(id);
			if( ty == null ) return null;
            _eventKernal = ty.GetKernel();
            _serv = ResEventRepository.Get(_eventKernal.Get<IResEvent>(), _serv.Context);
            return _serv.Find(id);
        }

        private void RigRepository(IKernel kernel)
        {
            _serv = ResEventRepository.Get(kernel.Get<IResEvent>(), _serv.Context);
        }

        //
        // GET: /Admin/Event/Summary/5
        public ActionResult OccurrenceSummary(int id)
        {
            IResEvent resEvent = _serv.Find(id);
			if( resEvent == null )
                return RedirectToAction("Index", "Event", new { message = 
					"Selected event has been deleted and is no longer available ( " + id + " )" });

			InitializeWizard(resEvent, "admin.summary", ControllerContext.HttpContext.Request, ViewData);
			// ReSharper disable Mvc.ViewNotResolved
            return View(resEvent as ResEvent);
			// ReSharper restore Mvc.ViewNotResolved
        }

        //
        // GET: /Admin/Event/Create
        [ActionName("Template")]
        public ActionResult ChooseTemplate(int? id)
        {
	        IResEvent resEvent = id != null ? ResolveEvent(id.GetValueOrDefault()) : new ResEvent();
            Wizard<IResEvent> wiz = InitializeWizard(resEvent, "admin.template", ControllerContext.HttpContext.Request, ViewData);

            if (!string.IsNullOrEmpty(Request.QueryString["template"])) {
                var template = _resTemplateServ.Find(Request.QueryString["template"]);
                if (template != null) {
                    resEvent.Template = template;
                    _serv.Edit(resEvent as ResEvent);
                    _serv.Commit();
                    return Redirect(wiz.NextStep.Uri(ControllerContext));
                }
            }

            var templates = _resTemplateServ.GetByReservationType( resEvent.ReservationTypeId );
            if (templates == null || !templates.Any())
            {
                templates = _resTemplateServ.GetAll().Include("Preview");
            }
            ViewBag.Templates = templates;
            return View(resEvent);
        }

        //
        // GET: /Admin/Event/Food        
        public ActionResult Food(int id)
        {
            IResEvent resEvent = ResolveEvent(id);
            InitializeWizard(resEvent, "admin.food", ControllerContext.HttpContext.Request, ViewData);
            return View(resEvent as GiveawayEvent);            
        }

     

        [HttpPost]
        public ActionResult Food(int id, FormCollection collection)
        {

            GiveawayEvent res = ResolveEvent(id) as GiveawayEvent;
            Regex numbers = new Regex(@"^\d+$");

            int maxtemp;
            int mintemp;
            bool maxparse = int.TryParse(collection["MaxItems"], out maxtemp);
            bool minparse = int.TryParse(collection["MinItems"], out mintemp);

            if (!maxparse || !minparse)
                return View(res as GiveawayEvent);

            res.MaxItems = maxtemp;
            res.MinItems = mintemp;

            
            var occurrenceRepo = OccurrenceRepository.Get(res.ReservationType.GetKernel().Get<IOccurrence>(), _serv.Context);
            Wizard<IResEvent> wiz = InitializeWizard(res, "admin.food", ControllerContext.HttpContext.Request, ViewData);
            try
            {

                
                Dictionary<string, MenuItemCondition> foodIds = collection.PluckArray<MenuItemCondition>("food", x => (MenuItemCondition)int.Parse(x));


                Dictionary<string, string> foodImages = collection.PluckArray("food_image");
                Dictionary<string, string> foodNames = collection.PluckArray("food_name");

                if (foodIds.Count > 0)
                {

                    //new Dictionary<string, int>();// = foodInput.Trim().Trim(',').Trim().Split(',');
                    List<MenuItemAllowance> oldFood = new List<MenuItemAllowance>();
                    if (res.ProductAllowances!=null)
                    oldFood =
                        res.ProductAllowances.AsQueryable()
                           .Include("AllowedItem")
                           .Where(x => !foodIds.Keys.Contains(x.AllowedItem.DomId))
                           .ToList();
                    foreach (var menuItem in oldFood)
                    {
                        res.ProductAllowances.Remove(menuItem);
                    }
                    var menuItemRepo = new MenuItemRepository(_serv.Context);
                    var webMediaRepo = new WebMediaRepository(_serv.Context);
                    if (res.ProductAllowances == null)
                        res.ProductAllowances = new Collection<MenuItemAllowance>();


                    foreach (var food in foodIds)
                    {

                        if (res.ProductAllowances.AsQueryable().Include("AllowedItem").Any(x => x.AllowedItem.DomId == food.Key))
                        {
                            var menuItemAllowance = res.ProductAllowances.First(x => x.AllowedItem.DomId == food.Key);
                            menuItemAllowance.Condition = food.Value;
                            var imgUrl = foodImages[food.Key];
                            var foodName = foodNames[food.Key];
                            if (menuItemAllowance.AllowedItem.MediaId == null)
                            {
                                
                                var media = webMediaRepo.FindOrCreateByExternalUri(imgUrl, AppContext.User.UserId);
                                menuItemAllowance.AllowedItem.MediaId = media.MediaId;
                                menuItemAllowance.AllowedItem.Name = foodName;

                            }
                            else
                            {
                                
                                menuItemAllowance.AllowedItem.Media.ExternalUriStr = imgUrl;
                                menuItemAllowance.AllowedItem.Name = foodName;
                                webMediaRepo.RefreshPulledImage(menuItemAllowance.AllowedItem.Media);
                            }
                        }

                    }
                    
                    foreach (var food in foodIds)
                    {
                        if (!res.ProductAllowances.AsQueryable().Include("AllowedItem").Any(x=>x.AllowedItem.DomId==food.Key))
                        {
                            var foodItem = menuItemRepo.FindOrCreateBySlug(food.Key);
                            foodItem.Name=foodNames[food.Key];
                            var imgUrl = foodImages[food.Key];
                            
                            var media = webMediaRepo.FindOrCreateByExternalUri(imgUrl,AppContext.User.UserId);
                            foodItem.MediaId = media.MediaId;

                            var menuItemAllowance = new MenuItemAllowance()
                                {
                                    AllowedItemId = foodItem.DomId,
                                    AllowedItem = foodItem,
                                    Condition = food.Value
                                };
                            
                            res.ProductAllowances.Add(menuItemAllowance);
                            
                        }
                    }


                   


                    _serv.Edit(res);


                    

                    _serv.Commit();
                    return Redirect(wiz.NextStep.Uri(ControllerContext));
                }
                else
                {
                    foreach (var product in res.ProductAllowances)
                    {
                        //Occurrence.Status = OccurrenceStatus.Deactivated;
                        //mdrake: no one asked for this
                        res.ProductAllowances.Remove(product);
                    }
                }
                _serv.Edit(res);
                _serv.Commit();
                return Redirect(wiz.NextStep.Uri(ControllerContext));
            }
            catch (Exception ex)
            {
                

                return View(res);
            }

        }

        

      
        //
        // GET: /Admin/Event/Create
        [ActionName("Create-Dash")]
        public ActionResult CreateDash(int id)
        {
            IResEvent evnt = ResolveEvent(id);
            

            Wizard<IResEvent> wiz = InitializeWizard(evnt, "admin.start", ControllerContext.HttpContext.Request, ViewData);
            
            //ViewBag.Suggested = true;

            //create suggested values for quick submission
            
            
            
            return View(evnt as ResEvent);
        }

        private IResEvent SuggestedEventFromTheme(ResEventTheme theme)
		{
            DateTime monthTime = DateTime.Parse(string.Format("{0}/01/{1}",DateTime.Now.AddMonths(2).Month,DateTime.Now.AddMonths(1).Year));
            DateTime monthStart = monthTime;
            DateTime monthEnd = monthTime.AddMonths(1).AddSeconds(-1);
            var reservationType = _reservationTypeServ.Find(theme.ReservationTypeId);
            _eventKernal = reservationType.GetKernel();
            IResEvent evnt = _eventKernal.Get<IResEvent>();

            evnt.Name = string.Format("New {0}", theme.ReservationTypeName);
		    evnt.Urls = string.Format("event-{0}", theme.ResEventThemeId);
            evnt.Description = string.Format("An awesome {0}.", theme.ReservationTypeName.ToLower());	        

	        evnt.SetRegistrationAvailability (new DateRange(monthStart, monthEnd));
            evnt.SetSiteAvailability(new DateRange(monthStart.AddMonths(-1), monthEnd));
            evnt.ReservationType = reservationType;
            evnt.Template = _resTemplateServ.Find(theme.TemplateId);
            
            return evnt;
        }
        

		/* SH - Unused
        private IResEvent SuggestedQuarterlyEventFromTheme(ResEventTheme theme)
        {
            var ReservationType = _reservationTypeServ.Find(theme.ReservationTypeId);
            _eventKernal = ReservationType.GetKernel();
            IResEvent resEvent = _eventKernal.Get<IResEvent>();
            
            resEvent.Template = _resTemplateServ.Find(theme.TemplateId) as ResTemplate;

            int quarter = (int)Math.Floor((DateTime.Now.AddMonths(3).Month) / 4.0)+1;
            DateTime janFirstThisYear = DateTime.Parse(DateTime.Today.ToString("01/01/yyyy"));

            DateTime quarterStart = janFirstThisYear.AddMonths(-3 + ((quarter-1) * 3));
            DateTime quarterEnd = janFirstThisYear.AddMonths(0 + ((quarter - 1) * 3)).AddDays(-1);

            resEvent.Name = "Q" + quarter + string.Format(" Home Office {0} Tours", theme.Name);

            resEvent.Description = theme.Name + string.Format("Tours for Q{0} of ",quarter) + janFirstThisYear.Year;
            resEvent.Urls = string.Format("{0}tours-q{1}-{2}", theme.ResEventThemeId, quarter, janFirstThisYear.Year);

            resEvent.SetRegistrationAvailability ( new DateRange(quarterStart, quarterEnd));
            resEvent.SetSiteAvailability(new DateRange(quarterStart.AddMonths(-1), quarterEnd));

            resEvent.ReservationType = ReservationType as ReservationType;

            resEvent.Status = ResEventStatus.Draft;
            return resEvent;
        }

        private IResEvent SuggestedMonthlyEventFromTheme(ResEventTheme theme, int month, int twodigityear)
        {
            var ReservationType = _reservationTypeServ.Find(theme.ReservationTypeId);
            _eventKernal = ReservationType.GetKernel();
            IResEvent resEvent = _eventKernal.Get<IResEvent>();
            
            resEvent.Template = _resTemplateServ.Find(theme.TemplateId) as ResTemplate;

            DateTime monthTime = DateTime.Parse(string.Format("{0}/01/{1}",month,twodigityear));
            
            DateTime monthStart = monthTime;
            DateTime monthEnd = monthTime.AddMonths(1).AddSeconds(-1);
            string strId = monthTime.ToString("MMMM yyyy");
            resEvent.Name = strId + string.Format(" Home Office {0} Tours", theme.Name);

            resEvent.Description = theme.Name + string.Format(" Tours for {0} ", strId);
            resEvent.Urls = string.Format("{0}tours-{1}", theme.ResEventThemeId, strId.Replace(" ","-")).ToLower();
            
            resEvent.SetRegistrationAvailability (new DateRange(monthStart, monthEnd));
            resEvent.SetSiteAvailability( new DateRange(monthStart.AddMonths(-1), monthEnd));

            resEvent.ReservationType = ReservationType as ReservationType;

            resEvent.Status = ResEventStatus.Draft;
            return resEvent;
        } */

        //
        // POST: /Admin/Event/Create-Dash

        [HttpPost]
        [ActionName("Create-Dash")]
        public ActionResult CreateDash(int id,ResEvent resEvent, FormCollection collection)
        {

            var wiz = InitializeWizard(resEvent, "admin.start", ControllerContext.HttpContext.Request, ViewData);
            var original = _serv.Find(x=>x.ResEventId==id,new []{"Occurrences"});


            original.RegistrationStartString = resEvent.RegistrationStartString;
            original.RegistrationEndString = resEvent.RegistrationEndString;
            original.SiteStartString = resEvent.SiteStartString;
            original.SiteEndString = resEvent.SiteEndString;

            original.ReservationTypeId = resEvent.ReservationTypeId;
            original.TemplateId = resEvent.TemplateId;
            original.Name = resEvent.Name;
            original.Description = resEvent.Description;
            original.Status = resEvent.Status;
            if(original.Status==ResEventStatus.Temp)
                original.Status = ResEventStatus.Draft;
            
            original.Urls = resEvent.Urls;
            

            try
            {
                
                _serv.Commit();

                if (original.Occurrences.Count() == 0)
                {
                    var config = ReservationConfig.GetConfig();
                    var defaultLocation = config.EventTypes.GetByKey(original.ReservationTypeId).DefaultLocation;
                    
                    var location = _locationServ.Find(defaultLocation);
                    var newOccurrence = original.ReservationType.GetKernel().Get<IOccurrence>();
                    newOccurrence.StoreId = location.LocationNumber;
                    newOccurrence.Start = original.SiteStart;
                    newOccurrence.End = original.SiteEnd;
                    newOccurrence.SlotRangeEnd = original.RegistrationEnd;
                    newOccurrence.SlotRangeStart = original.RegistrationStart;
                    newOccurrence.ResEventId = original.ResEventId;
                    _occurrenceServ.Add(newOccurrence);
                    _occurrenceServ.Commit();
                }

                return Redirect(wiz.NextStep.Uri(ControllerContext));
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError("", string.Format("{0}: {1}", validationErrors.Entry.Entity, validationError.ErrorMessage));
                    }
                }
                

                return View(resEvent as ResEvent);
            }

            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Url", StringComparison.Ordinal) > -1)
                {
                    ModelState.AddModelError("Url", "That Marketable name is already in use.");
                }
                ModelState.AddModelError("", ex);

                

                Console.Write(ex.Message);
                return View(resEvent as ResEvent);
            }
        }

        public ActionResult Wizard(string id) {

            ReservationType resType = _reservationTypeServ.Find(id); 
			if( resType == null	) throw new ApplicationException( string.Format( "Unable to locate reservation type [ {0} ]", id ) );

			IResEvent evnt = _serv.TempEvent(resType);
            evnt.ReservationType = resType;
            RigRepository(resType.GetKernel());
			_serv.Add(evnt);
			try
			{
			    if (evnt is SpeakerEvent)
			        ((SpeakerEvent)evnt).Schedule = new Schedule();
				_serv.Commit();
			}
			catch (DbEntityValidationException ex)
			{
				Console.WriteLine(ex.Message);
			}
            IWizard<IResEvent> wizard = InitializeWizard(evnt, "admin.begining", ControllerContext.HttpContext.Request, ViewData);
            return Redirect(wizard.FirstStep.Uri(ControllerContext));
        }

        
        
        public ActionResult Stores(int id)
        {
            IResEvent resEvent = ResolveEvent(id);
            /*<th data-column-name='LocationNumberI' data-id-column='true' >NprNprLocation Number</th>                
                <th data-column-name='Cta' data-searchable="false"  >Name</th>
                <th data-column-name='Name' data-visible="false">Store Name</th>
                <th data-column-name='MarketableUrlString' data-visible="false">Marketable Url</th>
                
                <th data-column-name='ConceptCodeIdI' data-on-display="EnumFormat.ConceptCode" >Concept Code</th>
                <th data-column-name='RegionName' data-editable="false" data-visible="false" >Region</th>                
                <th data-column-name='PhoneString'>Phone</th>		*/
            ViewBag.ParticipatingStores = resEvent.ParticipatingStoresListViewModel                
                .ToList();
            
            InitializeWizard(resEvent, "admin.stores", ControllerContext.HttpContext.Request, ViewData);
            return View(resEvent as ResEvent);
        }


        //
        // POST: /Admin/Event/Create

        [HttpPost]
        public ActionResult Stores(int id, FormCollection collection)
        {
            
            IResEvent res = ResolveEvent(id);
            var occurrenceRepo = OccurrenceRepository.Get(res.ReservationType.GetKernel().Get<IOccurrence>(),_serv.Context);
            Wizard<IResEvent> wiz = InitializeWizard(res, "admin.stores", ControllerContext.HttpContext.Request, ViewData);
            try
            {

                var storesInput = collection["stores"];
                if (!string.IsNullOrEmpty(storesInput))
                {
                    string[] storeIds = storesInput.Trim().Trim(',').Trim().Split(',');

                    if (res.ReservationType.IncludeCorporateByDefault() && !storeIds.Any(x => x == "00000"))
                    {
                        var list = storeIds.ToList();
                        list.Add("00000");
                        storeIds = list.ToArray();
                    }


                    var oldOccurrences = res.Occurrences.Where(x => !storeIds.Contains(x.Store.LocationNumber)).ToList();
                    foreach (var occurrence in oldOccurrences)
                    {


                        occurrenceRepo.Delete(occurrence);
                        occurrenceRepo.Commit();
                    }

                    foreach (var store in storeIds)
                    {
                        var occurrence = res.CreateOrDefaultOccurrenceByStoreId(store);
                        if (occurrence != null)
                        {
                            occurrenceRepo.Add(occurrence);
                        }
                    }


                    _serv.Edit(res);
                    _serv.Commit();
                    return Redirect(wiz.NextStep.Uri(ControllerContext));
                }
                else
                {
                    //no stores selected 
                    ViewBag.StoreError = "You must select at leat one store for this event.";
                    return View(res as ResEvent);
                }

	            foreach (var occurrence in res.Occurrences)
	            {
		            //Occurrence.Status = OccurrenceStatus.Deactivated;
		            //mdrake: no one asked for this
		            occurrenceRepo.Delete(occurrence);
	            }
	            _serv.Edit(res);
	            _serv.Commit();
	            return Redirect(wiz.NextStep.Uri(ControllerContext));
            }
            catch(Exception ex) {
                Console.Write(ex.Message);
                ViewBag.ParticipatingStores = res.ParticipatingStoresList
                    .Where(x=>x!=null)
                .Select(x =>
                    new
                    {
                        LocationNumber = x.LocationNumber,
                        Name = x.Name,
                        MarketableUrlString = x.MarketableUrlString,
                        ConceptCodeId = (int)x.ConceptCode,
                        ConceptCode = x.ConceptCode,
                        RegionName = x.RegionName,
                        PhoneString = x.PhoneString
                    })
                .ToList();

                return View();
            }
            
        }

  

        public ActionResult Reception(int id)
        {
            SpeakerEvent resEvent = _serv.Find(id) as SpeakerEvent;


            if (resEvent.Schedule.Start == default(DateTimeOffset))
            {
                resEvent.Schedule.Start = new DateTimeOffset(DateTime.Now.Ticks,
                                                                resEvent.GetFirstTimeZone().GetUtcOffset(DateTime.Now));
            }
            if (resEvent.Schedule.End == default(DateTimeOffset))
            {
                resEvent.Schedule.End = new DateTimeOffset(DateTime.Now.Ticks,
                                                                resEvent.GetFirstTimeZone().GetUtcOffset(DateTime.Now));
            }
            
            InitializeWizard(resEvent, "admin.reception", ControllerContext.HttpContext.Request, ViewData);
            return View(resEvent);
        }

        

        [HttpPost]
        public ActionResult Reception(int id, SpeakerEvent resEventViewModel, FormCollection collection)
        {
            var resEvent = ResolveEvent(id) as SpeakerEvent;
            
            /*Binding*/
            var startDateString = string.Format("{0} {1}{2}", collection["StartDate"], collection["StartTime"],
                                                collection["StartDateTz"]);
            resEvent.Schedule.Start = DateTimeOffset.Parse(startDateString);
            resEvent.Schedule.End = DateTimeOffset.Parse(string.Format("{0} {1}{2}", collection["EndDate"], collection["EndTime"], collection["EndDateTz"]));
            resEvent.RegistrationStart = resEvent.Schedule.Start.Date;
            resEvent.RegistrationEnd = resEvent.Schedule.End.Date;
            resEvent.Schedule.Capacity = resEventViewModel.Schedule.Capacity;
            resEvent.MaximumCapacity = resEventViewModel.MaximumCapacity;

            resEvent.SiteEnd = resEventViewModel.SiteEnd;
            resEvent.SiteStart = resEventViewModel.SiteStart;


            resEvent.SpeakerName = resEventViewModel.SpeakerName;
            resEvent.Name = resEventViewModel.Name;
            resEvent.Description = resEventViewModel.Name;
            resEvent.SpeakerMediaId = resEventViewModel.SpeakerMediaId;

            resEvent.OffSiteAddress = resEventViewModel.OffSiteAddress;
            resEvent.OffSiteAddressId = resEventViewModel.OffSiteAddressId;
            resEvent.OffSiteDescription = resEventViewModel.OffSiteDescription;
            resEvent.OffSiteParkingDescription = resEventViewModel.OffSiteParkingDescription;
            resEvent.HasChildCare = resEventViewModel.HasChildCare;
			//resEvent.MinOccurrenceCapacity = resEventViewModel.MinOccurrenceCapacity;
            resEvent.AutomaticallyEnableOccurrences = resEventViewModel.AutomaticallyEnableOccurrences;
            //resEvent.Schedule = resEventViewModel.Schedule;
            //resEvent.ScheduleId = resEventViewModel.ScheduleId;


            bool isNew = false;
            if (string.IsNullOrEmpty(resEvent.Schedule.Name))
            {
                isNew = true;
                resEvent.Schedule.Name = "Schedule for "+resEvent.Name;
            }

            /*End Binding*/

            var wiz = InitializeWizard(resEvent, "admin.reception", ControllerContext.HttpContext.Request, ViewData);

            

            try
            {



                var slotsRepo = new SlotRepository(_serv.Context);
                
                var occurrencesWithSlots = resEvent.Occurrences.Where(x => x.SlotsList.Any()).ToList();
                foreach (var occ in occurrencesWithSlots)
                {
                    occ.SlotsList.ToList().ForEach(x=>resEvent.SyncSlot(x));
                }

                var occurrencesWithoutSlots = resEvent.Occurrences.Where(x => !x.SlotsList.Any()).ToList();
                
                foreach(var occ in occurrencesWithoutSlots)
                {
                    var newSlot = resEvent.CreateSlot();
                    newSlot.OccurrenceId = occ.OccurrenceId;
                    resEvent.SyncSlot(newSlot);
                    slotsRepo.Add(newSlot);
                }

                var occurrencesWithExtraSlots = occurrencesWithSlots.Where(x => x.SlotsList.Count() > 1).ToList(); ;
                foreach (var occ in occurrencesWithExtraSlots)
                {
                    var extraSlots = occ.SlotsList.Skip(1);
                    foreach (var slot in extraSlots)
                    {
                        slotsRepo.Delete(slot);
                    }
                }
                if (isNew)
                {
					// SH - Commenting out below 2 lines and running w/ "resEvent.Schedule.Capacity"
					// .. Prior to this change admin for res events ( reception ) would report:
					// .. "Capacity / Tickets Issued 2 /  3" values, etc. - After speaking w/ Gabby the below
					// .. division logic should not be anywhere present in RES - For TOURS, this logic
					// .. may or may not apply, but since tours is going to launch at present w/ the old
					// .. code base, this logic will remain commented out for the time being

                    //var totalSlots = resEvent.Occurrences.Sum(x => x.SlotsList.Count());
                    //var capPerSlot = (int)Math.Floor((double)(resEvent.Schedule.Capacity / totalSlots));

                    // carson - update 8-1-13 Added a MasterCapacity to Resevent fields that should take preceident for CR Events
                    // SH - update 8-1-13 Renamed MasterCapacity to MaximumCapacity

                    int capPerSlot = resEvent.Schedule.Capacity;

					resEvent.Occurrences.ToList().ForEach(x => x.SlotsList.ToList().ForEach(xx => xx.Capacity = capPerSlot));
                }

                _serv.Edit(resEvent);
                

                _serv.Commit();
                return Redirect(wiz.NextStep.Uri(ControllerContext));
            }
            catch(Exception ex)
            {
                return View(resEvent);
            }
        }

        public ActionResult Name(int id)
        {
            IResEvent resEvent = _serv.Find(id);
            
            InitializeWizard(resEvent, "admin.name", ControllerContext.HttpContext.Request, ViewData);
            if (resEvent.IsTemp())
            {
                resEvent.Name = "";
                resEvent.Urls = "";
            }
            return View(resEvent as ResEvent);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Name(int id, FormCollection collection)
        {
            IResEvent resEvent = _serv.Find(x=>x.ResEventId==id,ResEventRepository.DefaultEntityIncludes);
            resEvent.Name = collection["Name"];
            resEvent.Urls = collection["Urls"];

            Wizard<IResEvent> wiz = InitializeWizard(resEvent, "admin.name", ControllerContext.HttpContext.Request, ViewData);
            try
            {
                if (resEvent.Status==ResEventStatus.Temp)
                    resEvent.Status = ResEventStatus.Draft;

                _serv.Edit(resEvent as ResEvent);
                _serv.Commit();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return View(resEvent as ResEvent);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("UIX_SiteUrl"))
                {
                    if (!string.IsNullOrEmpty(collection["Urls"]))
                    {
                        string[] toAddUrls = collection["Urls"].Split(',').Select(x=>x.Trim()).ToArray();
                        ResSiteUrl[] badUrls = _serv.FindUsedUrls(toAddUrls);
                        string[] badUrlStrings = badUrls.Where(x => x.ResEventId!=resEvent.ResEventId).Select(x=>x.Url).ToArray();
                        ModelState.AddModelError("Urls", string.Format("Sorry, the following urls are in use: {0}",string.Join(", ",badUrlStrings)));
                    }
                    else
                    {
                        ModelState.AddModelError("Urls", "Sorry, that url is in use.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                return View(resEvent as ResEvent);
            }
            catch (Exception ex){
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(resEvent as ResEvent);
            }

			return Redirect(wiz.NextStep.Uri(ControllerContext));
        }

        protected MultiGroupAggregates GetAllAggregates(IResEvent resEvent, SlotRepository slotRepo)
        {
            slotRepo.Context.Configuration.ProxyCreationEnabled = false;
            List<SlotDayAggregate> allAggregates = slotRepo.GetAggregates(SlotGrouping.All, resEvent).Select(x => x.Key).ToList();
            List<SlotDayAggregate> day = slotRepo.GetAggregates(SlotGrouping.Day, resEvent).Select(x => x.Key).ToList();
            List<SlotDayAggregate> date = slotRepo.GetAggregates(SlotGrouping.Date, resEvent).Select(x => x.Key).OrderBy(x => x.StartDay).ToList();
            var result= new
            MultiGroupAggregates{
                All = allAggregates,
                Day = day,
                Date = date,
            };
            slotRepo.Context.Configuration.ProxyCreationEnabled = true;
            return result;
        }

        public ActionResult Times(int id)
        {
            IResEvent resEvent = _serv.Find(id);
            ResEventViewModel resEventViewModel = new ResEventViewModel();
            resEventViewModel.Inject(resEvent as ResEvent);
            ViewBag.ViewModel = resEventViewModel;
            Wizard<IResEvent> wiz = InitializeWizard(resEvent, "admin.times", ControllerContext.HttpContext.Request, ViewData);

            SlotRepository slotRepo = new SlotRepository(_serv.Context);
            var currentAggregates = GetAllAggregates(resEvent, slotRepo);

            IOccurrence firstOccurrence = resEvent.Occurrences.FirstOrDefault();
            if (firstOccurrence != null)
            {
                ISlot firstSlot = firstOccurrence.SlotsList.FirstOrDefault();
                if (firstSlot != null)
                {
                    ViewBag.Cutoff = firstSlot.Cutoff;
                    ViewBag.CutoffDistance = firstSlot.Cutoff-firstSlot.Start;
                }
            }
            
            if (resEvent.Occurrences.Any())
            {
                if (resEvent.Occurrences.AsQueryable().Include("Store").Where(x => x.StoreId != null).Max(x => x.Store.MaximumCapacity) > 0)
                {
                    ViewBag.CapacityThreshold =
                        resEvent.Occurrences.AsQueryable().Include("Store").Min(x => x.Store.MaximumCapacity);
                }
            }

            ViewBag.CurrentAggregates = currentAggregates;

            return View(resEvent as ResEvent);
        }

		[HttpPost]
		public ActionResult Times(int id, FormCollection collection)
		{
            IResEvent resEvent = _serv.Find(x => x.ResEventId == id, ResEventRepository.DefaultEntityIncludes);
            
            ResEventViewModel resEventViewModel = new ResEventViewModel();
            resEventViewModel.Inject(resEvent as ResEvent);
            Wizard<IResEvent> wiz = InitializeWizard(resEvent, "admin.times", ControllerContext.HttpContext.Request, ViewData);
            try
            {
				ViewBag.ViewModel = resEventViewModel;

				SlotRepository<Slot> slotRepo;
				if( AppContext.Configuration.ApplicationId == Application.NPR )
					slotRepo = new NPRSlotRepository(_serv.Context);
				else
					slotRepo = new SlotRepository(_serv.Context);

				if( collection["SiteStart"] != null )
				{
					resEvent.SiteStart = DateTime.Parse(collection["SiteStart"]);
					resEvent.RegistrationStart = DateTime.Parse(collection["RegistrationStart"]);
					resEvent.SiteEnd = DateTime.Parse(collection["SiteEnd"]);
					resEvent.RegistrationEnd = DateTime.Parse(collection["RegistrationEnd"]);

                    //carson: set SiteEnd and RegistrationEnd time to 11:59 so you can have a slot on the last day
                    TimeSpan ts = new TimeSpan(23, 59, 59);
                    resEvent.RegistrationEnd = resEvent.RegistrationEnd.Date + ts;
                    resEvent.SiteEnd = resEvent.SiteEnd.Date + ts;
				}
				_serv.Commit();

				DateTime cutoff = DateTime.Parse("1/1/2000 "+collection["cutoff-time"]);
				var cutoffDistance = TimeSpan.Parse(collection["cutoff-day"]);

				if (!string.IsNullOrEmpty(collection["DeleteAggregate"]))
				{
					SlotDayAggregateEntry[] deleteAggregates = new JavaScriptSerializer().Deserialize<SlotDayAggregateEntry[]>(collection["DeleteAggregate"]);
					foreach (var slotDayAggregateEntry in deleteAggregates)
					{
						foreach (var agg in slotDayAggregateEntry.Aggregates)
						{
							agg.NewValue.EndMonth = slotDayAggregateEntry.BaseDate.Month;
							agg.NewValue.StartMonth = slotDayAggregateEntry.BaseDate.Month;
							if (agg.OldValue != null)
							{
								agg.OldValue.EndMonth = slotDayAggregateEntry.BaseDate.Month;
								agg.OldValue.StartMonth = slotDayAggregateEntry.BaseDate.Month;
							}
						}
					}
					slotRepo.RemoveAggregate(resEvent, deleteAggregates);
				}

				SlotDayAggregateEntry[] saveAggregates = new JavaScriptSerializer().Deserialize<SlotDayAggregateEntry[]>(collection["SaveAggregate"]);
				bool timesSelected = false;
				if (saveAggregates != null && saveAggregates.Length > 0)
				{
					foreach (var slotDayAggregateEntry in saveAggregates)
					{
						if (slotDayAggregateEntry.Grouping == SlotGrouping.All)
						{
							var templateDates =
								saveAggregates.Where(x => x.Grouping == SlotGrouping.Date).Select(x=>x.BaseDate).ToList();
							if (templateDates.Count > 0)
							{
								foreach (var agg in slotDayAggregateEntry.Aggregates)
								{
									agg.NewValue.TemplateDates = templateDates;
								}
							}
						}

						foreach (var agg in slotDayAggregateEntry.Aggregates)
						{
							agg.NewValue.EndMonth=slotDayAggregateEntry.BaseDate.Month;
							agg.NewValue.StartMonth = slotDayAggregateEntry.BaseDate.Month;
							if (agg.OldValue != null)
							{
								agg.OldValue.EndMonth = slotDayAggregateEntry.BaseDate.Month;
								agg.OldValue.StartMonth = slotDayAggregateEntry.BaseDate.Month;
							}
						}
					}
                
					slotRepo.AddAggregate(resEvent, saveAggregates, cutoff, cutoffDistance);

					foreach( SlotDayAggregateEntry ae in saveAggregates ) {
                    
						if( timesSelected ) break;
						if( ae.Aggregates.Any() ) timesSelected = true;
					}
				}
		    
				if( ! timesSelected )
				{
					ModelState.AddModelError( string.Empty, "One or more times are required." );
					return View(resEvent as ResEvent);
				}

				// We always validate this particular rule, even if admin - if( AppContext.User.OperationRole != UserOperationRole.Admin )
				resEvent.MinimumDailyCapacity= collection["MinimumDailyCapacity"].ToInt();
				try { resEvent.ValidateMinimumDailyCapacity(); }
	            catch( ApplicationException ex ) {
					return View(resEvent as ResEvent).AddModelStateError( ModelState, "MinimumDailyCapacity", ex.Message );
	            }

                if (resEvent.ReservationTypeId == "Tour" && resEvent.Occurrences.Count > 0)
                {
                    TimeSpan publicTourTime = new TimeSpan(11, 0, 0);
                    TimeSpan selectedTime = resEvent.Occurrences.First().FirstSlotStart.LocalDateTime.TimeOfDay;
                    if (selectedTime != publicTourTime)
                    {
                        resEvent.Visibility = ResEventVisibility.Private;
                    }
                }

				_serv.Edit(resEvent as ResEvent);
				_serv.Commit();                

				// Validate a second time ( could likely be improved ) specifically for newly created events
				// .. wherein LowestDailyCapacity results in null due to slots not yet existing
				try { resEvent.ValidateMinimumDailyCapacity(); }
	            catch( ApplicationException ex ) {
					return View(resEvent as ResEvent).AddModelStateError( ModelState, "MinimumDailyCapacity", ex.Message );
	            }
			}
			catch (DbEntityValidationException dbEx)
			{
				foreach (var validationErrors in dbEx.EntityValidationErrors)
				{
					foreach (var validationError in validationErrors.ValidationErrors)
					{
						Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
					}
				}
                return View(resEvent as ResEvent);
			}
			catch (Exception ex)
			{
	            Exception e = ex;
				while( e.InnerException != null ) e = e.InnerException;
				ModelState.AddModelError(string.Empty, e.Message);
                return View(resEvent as ResEvent);
			}

            return Redirect(wiz.NextStep.Uri(ControllerContext));
		}

        //
        // GET: /Admin/Event/Create

        public ActionResult Create()
        {
            return View(new ResEvent());
        }


        //
        // POST: /Admin/Event/Create

        [HttpPost]
        public ActionResult Create(IResEvent resEvent, FormCollection collection)
        {
            try
            {
                resEvent.SetSiteAvailability ( Occurrence.ConvertToTimeZoneContext(new DateRange(collection["SiteAvailability"])));
                resEvent.SetRegistrationAvailability ( Occurrence.ConvertToTimeZoneContext(new DateRange(collection["RegistrationAvailability"])));

				resEvent.Template = _resTemplateServ.Find(collection["Template"]);
                resEvent.ReservationType = _reservationTypeServ.Find(collection["ReservationType"]);

                _serv.Add(resEvent as ResEvent);
                _serv.Commit();
                return RedirectToAction("Details", new {id=resEvent.Id() });
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Event/Edit/5

        public ActionResult Edit(int id)
        {
            IResEvent resEvent = _serv.Find(id);

            return View(resEvent as ResEvent);
        }

        public ActionResult Activate(int id)
        {
            IResEvent resEvent = _serv.Find(id);
			if( resEvent.Status != ResEventStatus.Hidden ) // Set to live only if not already marked hidden
				resEvent.Status = ResEventStatus.Live;
			
            _serv.Edit(resEvent as ResEvent);
            _serv.Commit(true);

			// SH - Per ticket request, directing users not to details, but rather the "events dashboard"
			// .. for this event based on the type of event ( for ex. "Tour" -> Tours Dashboard, "SpecialEvent" -> Events Dash )
			//return RedirectToAction("Details", new { id,message="Event has been activated"});

			if( AppContext.Current.Configuration.ApplicationId == Application.CFA || 
				AppContext.Current.Configuration.ApplicationId == Application.CFATour )
				return RedirectToAction( "Index", "Home", new { id, message = "Event has been activated" } );

			if( resEvent.ReservationTypeId == "Tour" )
				return RedirectToAction( "TourDashboard", "Home", new { id, message = "Event has been activated" } );

			return RedirectToAction( "Index", "Home", new { id, message = "Event has been activated" } );
        }

        public ActionResult SaveAsDraft( int id )
        {
            IResEvent resEvent = _serv.Find( id );
			if( resEvent.Status == ResEventStatus.Live ) // Set to draft only if currently marked live
				resEvent.Status = ResEventStatus.Draft;
            _serv.Edit(resEvent as ResEvent); // Otherwise, maintain the current status ( hidden, archived, etc. )
            _serv.Commit(true);

			// SH - Per ticket request, directing users not to details, but rather the "events dashboard"
			// .. for this event based on the type of event ( for ex. "Tour" -> Tours Dashboard, "SpecialEvent" -> Events Dash )
			//return RedirectToAction("Details", new { id,message="Event has been activated"});

			if( AppContext.Current.Configuration.ApplicationId == Application.CFA || 
				AppContext.Current.Configuration.ApplicationId == Application.CFATour )
				return RedirectToAction( "Index", "Home", new { id, message = "Event has been saved" } );

			if( resEvent.ReservationTypeId == "Tour" )
				return RedirectToAction( "TourDashboard", "Home", new { id, message = "Event has been saved" } );

			return RedirectToAction( "Index", "Home", new { id, message = "Event has been saved" } );
        }

        [HttpPost]
        public ActionResult Edit(int id,ResEvent resEvent, FormCollection collection)
        {
			IResEvent e = _serv.Find(id);
            try
            {
				//IValueInjecter injecter = new ValueInjecter();
				//injecter.Inject(e, resEvent);

				e.Name = collection[ "Name" ];
				e.Urls = collection[ "Urls" ];
				e.Description = collection[ "Description" ];
				e.Status = (ResEventStatus) Enum.Parse(typeof(ResEventStatus), collection["Status"]);
				if( e.Status == ResEventStatus.Temp ) e.Status = ResEventStatus.Draft;
				e.ReservationTypeId = collection[ "ReservationTypeId" ];
				e.TemplateId = collection[ "TemplateId" ];
				
				//e.CategoryId = collection["Event.CategoryId.Event.CategoryId"].ToNullableInt();
				e.SiteStart = DateTime.Parse(collection["SiteStart"]);
				e.SiteEnd = DateTime.Parse(collection["SiteEnd"]);
				e.RegistrationStart = DateTime.Parse(collection["RegistrationStart"]);
				e.RegistrationEnd = DateTime.Parse(collection["RegistrationEnd"]);

				// Set registraion end time to be 11:59 pm to allow for slots on that day
                e.RegistrationEnd = e.RegistrationEnd.Date + new TimeSpan(23, 58, 00);
                e.SiteEnd = e.SiteEnd.Date + new TimeSpan(23, 59, 00);

                if( ! ModelState.IsValid ) return View( e as ResEvent );

                _serv.Edit(e as ResEvent);
                _serv.Commit();

                return RedirectToAction("Details", new { id = e.Id() });
            }
            catch( DbEntityValidationException ex )
            {
                foreach( DbEntityValidationResult error in ex.EntityValidationErrors )
                    foreach( DbValidationError modelError in error.ValidationErrors )
                        ModelState.AddModelError( modelError.PropertyName, modelError.ErrorMessage );

				return View( e as ResEvent );
            }
			catch( Exception ex )
            {
				while( ex.InnerException != null ) ex = ex.InnerException;
				return View( e as ResEvent ).AddModelStateError( ModelState, "Unable to save event - " + ex.Message );
            }
        }

        //
        // GET: /Admin/Event/Delete/5

        public ActionResult Delete(int id)
        {
            IResEvent resEvent = _serv.Find(id);

            return View(resEvent as ResEvent);
        }

        //
        // POST: /Admin/Event/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            
            IResEvent resEvent = _serv.Find(id);

            try
            {
                // TODO: Add delete logic here

                if (resEvent.HasTickets())
                {
                    throw new CustomerDataException("Cannot delete this event, some tickets have been reserved by end customers.");
                }
                var _occurrenceServ = new OccurrenceRepository(_serv.Context);
                var _slotServ = new SlotRepository(_serv.Context);
                foreach (var o in resEvent.Occurrences.ToList())
                {
                    foreach (var s in o.SlotsList.ToList())
                    {
                        _slotServ.Delete(s);
                    }
                    _occurrenceServ.Delete(o);
                }
                _serv.Delete(resEvent as ResEvent);
                _serv.Commit();
                return RedirectToAction("Index", new {message = "Event has been deleted"});
            }
            catch (CustomerDataException ex)
            {
                ModelState.AddModelError(string.Empty,ex.Message);
                return View(_serv.Find(id) as ResEvent);
            }
            catch
            {

                return View(resEvent as ResEvent);
            }
        }

        //
        // GET: /Admin/Occurrences/Edit-Dash
        [ActionName("Edit-Dash")]
        public ActionResult EditDash(int id)
        {
            

            try
            {
                // TODO: Add delete logic here
                IResEvent resEvent = _serv.Find(id);
                return RedirectToAction("Edit-Dash","Occurrence",new{id=resEvent.ProtoOccurrence.Id()});
            }
            catch
            {                
                return RedirectToAction("Index");
            }
        }
    }

    public class MultiGroupAggregates
    {
        public List<SlotDayAggregate> All { get; set; }
        public List<SlotDayAggregate> Day { get; set; }
        public List<SlotDayAggregate> Date { get; set; }

        public List<SlotDayAggregate> FirstAgg()
        {
            List<SlotDayAggregate> protoAgg=null;
            if (All.Count > 0)
                protoAgg = All;
            else if (Day.Count > 0)
                protoAgg = Day;
            else if (Date.Count > 0)
                protoAgg = Date;
            
            return protoAgg;
        }

        public SlotDayAggregate FirstAggregate()
        {
            var protoAgg = FirstAgg();
            if (protoAgg == null)
                return null;
            return protoAgg.First();
        }
    }
}
