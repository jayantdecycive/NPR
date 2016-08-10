using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.Areas.tours.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;
using System.Web.Profile;
using System.IO;
using System.Net.Mail;
using cfacore.shared.modules.com.admin;
using cfacore.site.controllers.shared;
using cfares.domain._event.slot.tours;
using cfacore.shared.modules.user;
using cfares.domain.user;
using cfares.domain._event.ticket;
using cfares.domain._event.ticket.tours;
using cfares.Global;
using cfacore.site.controllers._event;
using System.Net;
using cfares.service.common;
using cfacore.shared.domain.common;
using cfares.site.modules.mail;

namespace cfares.Areas.tours.Controllers
{

    public class ReservationController : Controller
    {
        #region global
        TicketService ticketService = null;
        OccurrenceService occurenceService = null;
        UserMembershipService userService = null;
        ReservationContextHint applicationInfo = null;
        ResEmailService emailService = null;
        string clientId;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            userService = new UserMembershipService(HttpContext);
            ticketService = new TicketService();
            occurenceService = new OccurrenceService();
            applicationInfo = (ReservationContextHint)HttpContext.Items["reservation-context"];
            emailService = new ResEmailService();


            clientId = userService.GetClientId();

            if (applicationInfo.EventName == "team.tours")
            {
                ViewBag.UserMode = "Operator";
                if (!User.IsInRole("cfa"))
                {
                    filterContext.Result = RedirectToAction("OperatorLogOn", "Account");
                    return;
                }
            }
            else
            {
                ViewBag.UserMode = "Customer";
                if (applicationInfo.EventName == "lgstory.tours")
                {
                    ViewBag.TourSize = "Large";
                }
                else
                {
                    ViewBag.TourSize = "Small";
                }
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (ViewBag.Wizard != null && ViewBag.Wizard is Wizard)
            {
                (ViewBag.Wizard as Wizard).CurrentStep.Uri = new Uri(Request.Url.ToString());
                setWizard(ViewBag.Wizard as Wizard, Session);
            }
        } 
        #endregion

        #region calendar
        public ActionResult Date(String id)
        {
            int groupSize;
            if (id != null)
            {
                groupSize = Int32.Parse(id);
            }
            else
            {
                groupSize = 1;
            }
            if (groupSize > 60)
            {
                if (applicationInfo.EventName != "lgstory.tours")
                {
                    return Redirect("http://lgstory.tours.chick-fil-a.com/tours/Reservation/Date/" + groupSize);
                }
            }

            Wizard wiz = getWizard(Session, 1);
            wiz.PreviousStep.Summary = setWizardGroupSize(groupSize);
            ViewBag.Wizard = wiz;
            TempData["groupSize"] = groupSize;
            ViewBag.TicketID = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["ticket"])) {
                ViewBag.TicketID = Request.QueryString["ticket"];
            }

            ViewBag.GroupSize = groupSize;
            return View("Calendar");
        } 
        #endregion

        #region register
        public ActionResult Begin(String id)
        {
            

            //check slot id 
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Home");

            TourTicket ticket = new TourTicket();
            TourTicket cachedTicket=null;
            try{
                cachedTicket = decacheOrLoadTicket(userService.GetClientId());
            }
            catch (Exception e) { }
            if (cachedTicket != null)
                ticket = cachedTicket;
            //ensure that this a new, blank ticket.
            ticket.TicketId = 0;


            //get guest size
            if (TempData["groupSize"] != null && !string.IsNullOrEmpty(TempData["groupSize"].ToString()))
                ticket.GuestCount = int.Parse(TempData["groupSize"].ToString());


            //check guest size
            if (ticket.GuestCount <= 0 || ticket.GuestCount > 120)
                ticket.GuestCount = 1;

            
            
            //Now use the given slotID to populate the Tour Ticket's slot. This is required, and will direct
            //the user back if error.
            try{
                ticket.Slot = loadTourSlotWithTotalCount(id);
            }
            catch (Exception e) {
                ViewBag.Error = e.Message;
                return RedirectToAction("Date");

            }

            StoryTourTicketFormModel storyTicket;

            if (TempData["register"] == null)
            {
                storyTicket = new StoryTourTicketFormModel(ticket);
            }
            else
            {
                storyTicket = TempData["register"] as StoryTourTicketFormModel;
            }

            if (Request.IsAuthenticated)
            {
                storyTicket.reservationUser = userService.GetUserWithAddress();
                storyTicket.tourTicket.Owner = userService.GetUserWithAddress();
            }

            else
            {
                ReservationUserCreationForm userFormModel = new ReservationUserCreationForm();
                storyTicket.userForm = userFormModel;
            }

            setWizardDate(ticket, 2);
            ticketService.Cache(userService.GetClientId(), ticket);
            return View(storyTicket);
        }

        [HttpPost]
        public ActionResult Begin(String id, StoryTourTicketFormModel model, FormCollection form)
        {
            if (ModelState.ContainsKey("tourTicket.GuestCount"))
                ModelState["tourTicket.GuestCount"].Errors.Clear();

            if (!String.IsNullOrEmpty(form.Get("reservationUser.MobilePhone"))){
                model.reservationUser.MobilePhone = new cfacore.shared.domain.user.Phone(form.Get("reservationUser.MobilePhone").ToString());
            }
            
            if (applicationInfo.EventName == "team.tours")
            {
                ModelState.Remove("tourTicket.OtherTypeDescription");
            }

            if (applicationInfo.EventName == "lgstory.tours" && model.tourTicket.GuestCount < 60)
            {
                ModelState.AddModelError("LargeGroupSize", "You must have at least 60 people to register for a large tour.");
            }

            try{
                model.tourTicket.Slot = loadTourSlotWithTotalCount(id);
            }
            catch (Exception e){
                ModelState.AddModelError("Slot i does not exist.", e);
            }

            //returning user
            if (Request.IsAuthenticated)
            {
                ModelState.Remove("userForm.Password");
                model.reservationUser = userService.GetUserWithAddress();
            }


            runValidation(model);

            if (ModelState.ContainsKey("reservationUser.MobilePhone"))
                ModelState["reservationUser.MobilePhone"].Errors.Clear();

            // Operator form does not have group type selection
            if (ViewBag.UserMode == "Operator" && ModelState.ContainsKey("tourTicket.OtherTypeDescription"))
                ModelState["tourTicket.OtherTypeDescription"].Errors.Clear();

            //rechecks model state
            if (!ModelState.IsValid)
            {
                if (!model.firstPassValidation)
                {
                    runValidation(model);
                }
                setWizardDate(model.tourTicket, 2);
                return View("Begin", model);
            }


            string slotId = "";
            slotId = model.tourTicket.Id();
            
            ticketService.Cache(userService.GetClientId(), model.tourTicket);
            model.tourTicket.Id(userService.GetClientId(), false);
            TempData["register"] = model;

            return RedirectToAction("ReviewNew", new { id = slotId });
        }

        //to validate email address during form process
        [HttpPost]
        public ActionResult CheckEmail(FormCollection collection)
        {
            bool userExists = false;
            if (Membership.FindUsersByEmail(collection["value"]).Count > 0)
            {
                userExists = true;
            }
            return Json(new { userExists = userExists }, JsonRequestBehavior.DenyGet);
        } 
        #endregion

        #region review
        public ActionResult ReviewNew(string id)
        {
            StoryTourTicketFormModel storyTicket = TempData["register"] as StoryTourTicketFormModel;

            if (storyTicket == null)
                RedirectToAction("Date");
            

            Wizard wiz = getWizard(Session, 3);
            ViewBag.Wizard = wiz;
            wiz.Steps[0].Summary = setWizardGroupSize(storyTicket.tourTicket.GuestCount);
            wiz.PreviousStep.Summary = "<span class='wizard-check' />";
            TempData["register"] = storyTicket;
            return View(storyTicket);
        }

        [HttpPost]
        public ActionResult ReviewNew(string id, FormCollection form)
        {
            StoryTourTicketFormModel storyTicket = TempData["register"] as StoryTourTicketFormModel;

            //create user
            if(!createUser(storyTicket))
                return View("Begin", storyTicket);

            TourTicket ticket = storyTicket.tourTicket;
            ticket.Slot = loadTourSlotWithTotalCount(id);
            ticket.OwnerId = userService.QuickId;
            ticket.CreationSrc = "Main Site";
            bool success = ticketService.Save(ticket);
            string databaseTicketId = ticket.Id();
            if (!success)
            {
                StoryTourTicketFormModel model = new StoryTourTicketFormModel(ticket);
                ModelState.AddModelError("Database", "Error saving to database");
                return View(model);
            }
            emailService.SendOutConfirmationEmail(ticket);
            ticketService.Forget(userService.GetClientId(), ticket);
            TempData["register"] = null;
            return RedirectToAction("Confirm", new { id = databaseTicketId });
        } 
        #endregion

        #region Confirmation
        public ActionResult Confirm(string id)
        {

            TourTicket ticket = decacheOrLoadTicket(id);

            if (ticket == null || string.IsNullOrEmpty(ticket.Id()))
            {
                return RedirectToAction("Date");
            }

            StoryTourTicketFormModel model = new StoryTourTicketFormModel(ticket);

            Wizard wiz = getWizard(Session, 4);
            ViewBag.Wizard = wiz;

            wiz.PreviousStep.Summary = "<span class='wizard-check' />";
            foreach (var step in wiz.Steps)
            {
                step.Complete = false;
            }

            //save the user
            if (Request.IsAuthenticated)
            {
                model.reservationUser = userService.GetUserWithAddress();
            }

            return View("Confirm", model);
        }
        
        #endregion

        #region Resend email
        [Authorize]
        public ActionResult ReSend(String id)
        {
            try
            {
                TourTicket ticket = decacheOrLoadTicket(id);
                emailService.SendOutConfirmationEmail(ticket, true);
            }
            catch (Exception e)
            {
            }

            return View("LostConfirmation");
        } 
        #endregion

        #region delete
        public ActionResult Forget(string id)
        {
            TourTicket ticket = null;
            if (id != null)
            {
                ticket = decacheOrLoadTicket(id);

            }
            if (ticket == null)
            {
                return RedirectToAction("Date");

            }
            ticketService.Forget(userService.GetClientId(),ticket);
            return View("Delete");
            
        }

        [Authorize]
        public ActionResult Delete(string id)
        {
            TourTicket ticket = ticketService.LoadTourWithOwnerAndSlot(id);
            if (ticket != null)
            {
                ResEmailService serv = new ResEmailService();
                TicketService ticketServ = new TicketService();


                ticketService.Forget(ticket);
                /*serv.SendOutCancellationEmails(ticket)*/
                ticketServ.Delete(ticket);

                string next = Request.QueryString["next"];
                if(string.IsNullOrEmpty(next))
                    return View();
                return Redirect(next);
            }
            else
            {
                return RedirectToAction("LogOn", "Account");
            } 

        }
        #endregion delete

        #region dashboard
        //Customer Dashboard
        [Authorize]
        public ActionResult ModifyTickets()
        {
            Session["RegistrationForm"] = null;
            ResUser user;
            UserMembershipService userService = new UserMembershipService(HttpContext);
            user = userService.GetUser();
            userService.LoadTickets(user);
            
            return View(user);
        }
        
        //Need to rename this to Display!
        [Authorize]
        public ActionResult ModifySlot(String id)
        {
            StoryTourTicketFormModel model = new StoryTourTicketFormModel();
            model.tourTicket = ticketService.LoadTourWithOwnerAndSlot(id);
            model.reservationUser = userService.GetUserWithAddress();
            

            ticketService.Cache(id, model.tourTicket);
            return View("Display", model);
        }
        #endregion dashboard

        #region review

        [Authorize]
        public ActionResult Review(String id)
        {
            
            if (id == null)
            {
                id = userService.GetClientId();
            }
            bool cached = id == userService.GetClientId();
            TourTicket ticket = decacheOrLoadTicket(id);
            if(cached)
                ticket.UnBind();

            StoryTourTicketFormModel model = new StoryTourTicketFormModel();
            model.tourTicket = ticket;
            model.reservationUser = userService.GetUserWithAddress();

            return View("Review", model);

        }

        [Authorize]
        [HttpPost]
        public ActionResult Review(String id, StoryTourTicketFormModel model)
        {
            if (id == null)
            {
                id = userService.GetClientId();

            }
            TourTicket ticket = decacheOrLoadTicket(id);
            
            SlotService slotService = new SlotService();
            TourSlot slot = slotService.LoadTourSlotWithCount(ticket.Id());
            
            

            model.tourTicket = ticket;
            model.reservationUser = userService.GetUserWithAddress();

            try
            {
                ticketService.Save(ticket);                
            }catch(Exception ex){
                ModelState.AddModelError(string.Empty,ex.Message);
                return View("Review",model);
            }

            try {
                emailService.SendOutConfirmationEmail(ticket);
                //this is bad bad bad - should be using logic instead of just letting errors fail silently
                //bad code Matt, bad - you're not allowed on the couch any more
            }catch(Exception ex){
                Console.Write(ex.Message);
            }

            ticketService.Forget(ticket);

            return RedirectToAction("ModifyTickets");

        }
        #endregion review

        #region modify
        [Authorize]
        public ActionResult Modify(String id)
        {
            TourTicket ticket = decacheOrLoadTicket(id);

            ticket.Slot = loadTourSlotWithTotalCount(ticket.Id());

            StoryTourTicketFormModel model = new StoryTourTicketFormModel();
            model.tourTicket = ticket;
            model.reservationUser = userService.GetUserWithAddress();

            if (!string.IsNullOrEmpty(Request.QueryString["slot"])) {
                ViewBag.NewSlot = loadTourSlotWithTotalCount(Request.QueryString["slot"]);
            }

            return View("Modify", model);

        }

        [Authorize]
        [HttpPost]
        public ActionResult Modify(String id, StoryTourTicketFormModel model,FormCollection formcollection)
        {

            TourTicket previousTicket = decacheOrLoadTicket(id);

            runValidation(model);
            
            //ensure that owner did not change
            model.tourTicket.OwnerId = previousTicket.OwnerId;

            //ensure that slot id did not change
            if (!string.IsNullOrEmpty(formcollection["new_slot"]))
            {
                model.tourTicket = previousTicket;
                
                
                model.tourTicket.Slot = loadTourSlotWithTotalCount(formcollection["new_slot"]);
                int currentHour = (new OccurrenceService().ConvertToTimeZoneContext(model.tourTicket.Slot.Start)).Hour;
                if (currentHour < 9 || currentHour >= 10) {
                    model.tourTicket.OptInForLunch = false;
                }
                
            }
            else
            {
                model.tourTicket.Slot = loadTourSlotWithTotalCount(previousTicket.Id());
            }
            if(string.IsNullOrEmpty(id))
                id=userService.GetClientId();
            ticketService.Cache(id, model.tourTicket);
            return RedirectToAction("Review",new{id=id});

        }
        #endregion modify

        #region wizard
        static public Wizard getWizard(HttpSessionStateBase Session, int index)
        {

            string wizardId = "homeofficeidwizard";

            cfacore.shared.modules.com.admin.Wizard wiz = null;

            if (Session[wizardId] != null)
            {
                wiz = (cfacore.shared.modules.com.admin.Wizard)Session[wizardId];
            }
            if (wiz == null)
            {
                wiz = new cfacore.shared.modules.com.admin.Wizard(wizardId, 5);

                wiz.Steps.Add(new WizardStep { Index = 0, Complete = true, Name = "Choose a Tour", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });
                wiz.Steps.Add(new WizardStep { Index = 1, Name = "Choose A Date", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });
                wiz.Steps.Add(new WizardStep { Index = 2, Name = "Registration", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });
                wiz.Steps.Add(new WizardStep { Index = 3, Name = "Overview", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });
                wiz.Steps.Add(new WizardStep { Index = 4, Name = "Confirmation", Uri = new Uri("http://tours.chick-fil-a.com"), Summary = "" });

                Session[wizardId] = wiz;
            }
            wiz.Index = index;
            return (wiz);
        }

        public void setWizard(Wizard wiz, HttpSessionStateBase Session)
        {
            Session[wiz.Id] = wiz;
        }

        public void setWizardDate(TourTicket ticket, int index)
        {
            DateTime startTime = ticket.Slot.Start;
            DateTime endTime = ticket.Slot.End;

            
            Wizard wiz = getWizard(Session, index);
            ViewBag.Wizard = wiz;

            wiz.PreviousStep.Summary = String.Format("<strong>{0}</strong><br/><span class='clean-timezone' data-base-date='{0}' data-time-format='h:mmtt'>{1}</span> - <span class='clean-timezone' data-base-date='{0}' data-time-format='h:mmtt'>{2}</span>",
                startTime.ToString("dddd, MMMM, dd, yyyy"), startTime.ToString("h:mmtt").ToLower(), endTime.ToString("h:mmtt").ToLower());
        }

        public void buildWizardSteps(StoryTourTicketFormModel model, Wizard wiz)
        {
            wiz.Steps[0].Summary = "Choose a Tour";
            setWizardDate(model.tourTicket, 2);

            foreach (WizardStep step in wiz.Steps)
            {
                step.Summary = "<span class='wizard-check' />";
            }
        }

        public string setWizardGroupSize(int groupSize){
            string summary;
            StoryTourTicketFormModel model = Session["RegistrationForm"] as StoryTourTicketFormModel;
            if (model != null){
                summary = " Group Tour Size: " + model.tourTicket.GuestCount;
            }
            else
            {
                summary = " Group Tour Size: " + groupSize;
            }
            return (summary);
        }
        #endregion wizard
        
        #region utils
        /****************************************|
        |   Reservation utility methods          |
        |****************************************/
        private void CheckOwnership(TourTicket myTicket)
        {
            if (!userService.IsAccountInRole("Admin") &&
                    myTicket.OwnerId != userService.QuickId)
            {
                throw new Exception("You do not own this ticket.");
            }
        }

        protected TourTicket initializeTicket() 
        {
            TourTicket ticketModel;

            if (Session["RegistrationForm"] != null)
            {
                //logic for dealing with existing info
                StoryTourTicketFormModel previousFormInfo = (StoryTourTicketFormModel)Session["RegistrationForm"];

                ticketModel = previousFormInfo.tourTicket;

            }
            else
            {
                ticketModel = new TourTicket { Owner = new ResUser() };
            }

            return ticketModel;
        }

        protected TourTicket decacheOrLoadTicket(string id) {
            bool cached = (userService.GetClientId() == id);

            TourTicket ticket = null;

            if (cached)
            {
                ticket = ticketService.DeCacheTour(id);
                if (ticket == null)
                    return null;
                //load in user
                if (Request.IsAuthenticated)
                {
                    ticket.Owner = userService.GetUserWithAddress();
                }
            }
            else
            {
                ticket = ticketService.DeCacheTour(id);
                if (ticket == null)
                    ticket = ticketService.LoadTourWithOwnerAndSlot(id);
            }
            return ticket;
        }

        protected TourSlot loadTourSlotWithTotalCount(String id) 
        {
            TourSlot slotModel;

            SlotService slotservice = new SlotService();
            slotModel = slotservice.LoadTourSlotWithCount(id);

            if (slotModel == null)
            {
                throw new Exception("Slot does not exist");
            }
            return slotModel;
        }

        private bool createUser(StoryTourTicketFormModel storyTicket)
        {
            //new user, create account
            if (!Request.IsAuthenticated)
            {
                try
                {
                    MembershipCreateStatus status;
                    storyTicket.reservationUser.Username = storyTicket.reservationUser.Email;
                    storyTicket.reservationUser = userService.CreateUser(storyTicket.reservationUser, storyTicket.userForm.Password, out status).ApplicationUser;

                    if (status != MembershipCreateStatus.Success)
                    {
                        ModelState.AddModelError("ExistingEmail", UserMembershipService.ErrorCodeToString(status));
                        return false;
                    }
                    userService.Authenticate(storyTicket.reservationUser);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ExistingEmail", ex);
                    return false;
                }
            }
            return true;
        }

        public void runValidation(StoryTourTicketFormModel model)
        {
            var validationResults = model.Validate(new ValidationContext(model, null, null));
            foreach (var error in validationResults)
                foreach (var memberName in error.MemberNames)
                    ModelState.AddModelError(memberName, error.ErrorMessage);
        }

        #endregion utils

    }
}

