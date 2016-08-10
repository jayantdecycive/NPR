using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.Areas.tours.Models;
using cfares.domain._event.ticket.tours;
using cfacore.site.controllers._event;
using cfacore.shared.modules.user;
using cfares.domain.user;
using cfares.domain._event.slot.tours;
using cfacore.site.controllers.shared;
using cfares.Global;
using System.ComponentModel.DataAnnotations;
using cfares.domain._event;
using cfares.domain._event.ticket;
using System.Web.Security;
using cfacore.shared.service.email;
using cfares.site.modules.mail;

namespace cfares.Areas.mobile.Controllers
{
    public class TicketController : Controller
    {
        #region Global
        TicketService serv = new TicketService();
        UserMembershipService membershipService = null;
        SlotService slotServ;
        ReservationContextHint applicationInfo;
        ResEmailService emailService;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            membershipService = new UserMembershipService(HttpContext);
            slotServ = new SlotService();
            emailService = new ResEmailService();
            applicationInfo = (ReservationContextHint)HttpContext.Items["reservation-context"];

            if (applicationInfo.EventId == "team")
                ViewBag.UserMode = "team";

            base.OnActionExecuting(filterContext);
        }
        #endregion Global

        #region Slot
        public ActionResult Index()
        {
            return RedirectToAction("Slot");
        }

        public ActionResult Slot(String id)
        {
            setCookie("groupSize", id, "1");
            return View();
        }
        #endregion Slot

        #region Register
        public ActionResult Register(String id)
        {
            //no slot
            if (String.IsNullOrEmpty(id))
                return Redirect("/mobile/ticket/slot");

            //init
            StoryTourTicketFormModel storyTicket = new StoryTourTicketFormModel();
            storyTicket.initialUserCreation = !checkUser();
            storyTicket.userForm = initializeUserCreationForm();
            storyTicket.tourTicket = initializeTicket(membershipService.GetClientId());
            storyTicket.tourTicket.Slot = intializeTourSlot(id);

            //groupsize
            if (storyTicket.tourTicket.GuestCount == 0)
                storyTicket.tourTicket.GuestCount = Int32.Parse(getCookie("groupSize"));

            //get user
            if (!storyTicket.initialUserCreation)
                storyTicket.reservationUser = membershipService.GetUserWithAddress();

            //cache
            serv.Cache(membershipService.GetClientId(), storyTicket.tourTicket);
            return View(storyTicket);
        }

        public ActionResult RegisterTeam()
        {
            StoryTourTicketFormModel storyTicket = new StoryTourTicketFormModel();
            return View("Register", storyTicket);
        }

        [HttpPost]
        public ActionResult Register(StoryTourTicketFormModel storyTicket, FormCollection Collection)
        {
            if (!String.IsNullOrEmpty(Collection.Get("reservationUser.MobilePhone"))){
                storyTicket.reservationUser.MobilePhone = new cfacore.shared.domain.user.Phone(Collection.Get("reservationUser.MobilePhone").ToString());
            }

            storyTicket.tourTicket.Slot = intializeTourSlot(storyTicket.tourTicket.Id());
            ModelState.Clear();
            TryValidateModel(storyTicket);

            //existing user
            if (checkUser()){
                storyTicket.tourTicket.Owner = membershipService.GetUserWithAddress();
                ModelState.Remove("Password");
            }

            //team
            if (applicationInfo.EventId == "team")
                ModelState.Remove("tourTicket.OtherTypeDescription");

            //large tours
            if (applicationInfo.EventName == "lgstory.tours" && storyTicket.tourTicket.GuestCount < 61)
                ModelState.AddModelError("LargeGroupSize", "You must have at least 61 people to register for a large tour.");

            if (ModelState.ContainsKey("reservationUser.MobilePhone"))
                ModelState["reservationUser.MobilePhone"].Errors.Clear();

            //not validated! 
            if (!ModelState.IsValid)
                return View(storyTicket);

            //create account
            if (!createUser(storyTicket))
                return View(storyTicket);

            //validated! 
            serv.Cache(membershipService.GetClientId(), storyTicket.tourTicket);
            return RedirectToAction("Overview");
        }

        [HttpPost]
        public ActionResult RegisterTeam(StoryTourTicketFormModel storyTicket, FormCollection Collection)
        {
            return Register(storyTicket, Collection);
        }
        #endregion Register

        #region Overview
        public ActionResult Overview()
        {
            StoryTourTicketFormModel storyTicket = new StoryTourTicketFormModel(serv.DeCacheTour(membershipService.GetClientId()));

            //problem decaching tour
            if (storyTicket.tourTicket == null)
                return Redirect("/mobile/ticket/slot");

            return View(storyTicket);
        }

        public ActionResult OverviewTeam()
        {
            return Overview();
        }

        [HttpPost]
        public ActionResult Overview(FormCollection Collection)
        {
            StoryTourTicketFormModel storyTicket = new StoryTourTicketFormModel(serv.DeCacheTour(membershipService.GetClientId()));

            //save tour
            TourTicket newTicket = serv.DeCacheTour(membershipService.GetClientId());
            newTicket.CreationSrc = "Mobile Site";
            newTicket.UnBind();
            serv.Save(newTicket);

            //send confirm email
            emailService.SendOutConfirmationEmail(newTicket);

            //clear cache
            serv.Forget(newTicket);
            return Redirect("/mobile/Ticket/Confirmation/" + newTicket.TicketId);
        }

        [HttpPost]
        public ActionResult OverviewTeam(StoryTourTicketFormModel storyTicket, FormCollection Collection)
        {
            return Overview(Collection);
        }
        #endregion Overview

        #region Confirm
        public ActionResult Confirmation(string id)
        {
            TourTicket ticket = serv.LoadTourWithOwnerAndSlot(id);
            return View(ticket);
        }
        #endregion Confirm

        #region Resend
        [Authorize]
        public ActionResult ReSend(string id)
        {
            TourTicket ticket = serv.LoadTourWithOwnerAndSlot(id);
            emailService.SendOutConfirmationEmail(ticket, true);
            return Redirect(redirectToMessage("Sent", "Your confirmation email has been sent to your inbox."));
        }

        #endregion Resend

        #region Dashboard
        [Authorize]
        public ActionResult Dash()
        {
            TourTicketCollection tourTickets = serv.LoadToursByOwner(membershipService.GetUser().Id());
            TourTicketCollection results = new TourTicketCollection();
            for (int i = 0; i < tourTickets.Count; i++)
            {
                tourTickets[i] = serv.LoadTourWithOwnerAndSlot(tourTickets[i].Id());
                if (tourTickets[i]!=null&&tourTickets[i].Slot!=null && tourTickets[i].Slot.Start > DateTime.Now)
                    results.Add(tourTickets[i]);
            }
            return View(results);
        }

        [Authorize]
        public ActionResult Details(string id)
        {
            TourTicket myTicket = serv.LoadTourWithOwnerAndSlot(id);
            myTicket.Owner = membershipService.GetUserWithAddress(myTicket.Owner.Username);
            CheckOwnership(myTicket);
            return View(myTicket);
        }
        #endregion Dashbord

        #region Modification
        //Modify ticket in current registration
        public ActionResult Modify()
        {
            TourTicket myTicket = serv.DeCacheTour(membershipService.GetClientId());
            return Redirect("/mobile/Ticket/Register?id=" + myTicket.SlotId);
        }

        //Make changes to an existing ticket
        [Authorize]
        public ActionResult Edit(string id)
        {
            TourTicket myTicket = serv.LoadTourWithOwnerAndSlot(id);
            myTicket.Owner = membershipService.GetUserWithAddress(myTicket.Owner.Email);
            CheckOwnership(myTicket);
            return View(myTicket);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(string id, TourTicket newTicket, FormCollection Collection)
        {
            TourTicket protoTicket = serv.LoadTourWithOwner(id);

            newTicket.Owner = membershipService.GetUserWithAddress(protoTicket.Owner.Email);
            CheckOwnership(newTicket);
            newTicket.Owner = updateUser(newTicket.Owner);//update user
            serv.Save(newTicket);

            return Redirect(redirectToMessage("Ticket Updated", "Your reservation ticket has been updated"));
        }

        #endregion Modification

        #region Delete
        public ActionResult Cancel()
        {
            try
            {
                TourTicket tour = serv.DeCacheTour(membershipService.GetClientId());
                serv.Forget(tour);
                serv.Cache(membershipService.GetClientId(), new TourTicket());
                membershipService.LogOff();
                return Redirect(redirectToMessage("Success", "Your registration has been canceled."));
            }
            catch (Exception e) { }
            return Redirect(redirectToMessage("Success", "Your registration has been canceled."));
        }

        [Authorize]
        public ActionResult Delete(string id)
        {
            TourTicket myTicket = serv.LoadTourWithOwnerAndSlot(id);
            CheckOwnership(myTicket);
            return View(myTicket);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(string id, TourTicket myTicket, FormCollection Collection)
        {
            myTicket = serv.LoadTourWithOwnerAndSlot(id);
            CheckOwnership(myTicket);
            emailService.SendOutCancellationEmails(myTicket);
            serv.Delete(myTicket);
            return Redirect(redirectToMessage("Success", "Your ticket has been deleted."));
        }
        #endregion Delete

        #region Message
        public ActionResult Message(string message, string head)
        {
            if (!String.IsNullOrEmpty(message) && !String.IsNullOrEmpty(head))
            {
                ViewData["message"] = HttpUtility.UrlDecode(message);
                ViewData["head"] = HttpUtility.UrlDecode(head);
                return View();
            }
            else
            {
                return Redirect("/Home");
            }
        }

        private string redirectToMessage(string head, string message)
        {
            return (string.Format("/mobile/Ticket/message/?message={0}&head={1}", HttpUtility.UrlEncode(message), HttpUtility.UrlEncode(head)));
        }
        #endregion Message

        #region utils

        private void CheckOwnership(TourTicket myTicket)
        {
            if (!membershipService.IsAccountInRole("Admin") &&
                    myTicket.OwnerId != membershipService.QuickId)
            {
                throw new Exception("You do not own this ticket.");
            }
        }

        private bool checkUser()
        {
            if (Request.IsAuthenticated)
                return true;
            else
                return false;
        }

        protected ReservationUserCreationForm initializeUserCreationForm()
        {
            ReservationUserCreationForm userCreationForm;
            userCreationForm = new ReservationUserCreationForm();
            return userCreationForm;
        }

        protected TourTicket initializeTicket(String id)
        {
            TourTicket ticketModel = serv.DeCacheTour(id);

            if (ticketModel == null)
            {
                ticketModel = new TourTicket { Owner = new ResUser() };
            }

            return ticketModel;
        }

        protected TourSlot intializeTourSlot(String id)
        {
            TourSlot slotModel;
            slotModel = slotServ.LoadTour(id);

            if (slotModel == null)
            {
                throw new Exception("Slot does not exist");
            }
            return slotModel;
        }

        private bool createUser(StoryTourTicketFormModel storyTicket)
        {
            //new user, create account
            if (!checkUser())
            {
                try
                {
                    MembershipCreateStatus status;
                    storyTicket.reservationUser.Username = storyTicket.reservationUser.Email;
                    storyTicket.reservationUser = membershipService.CreateUser(storyTicket.reservationUser, storyTicket.userForm.Password, out status).ApplicationUser;

                    if (status != MembershipCreateStatus.Success)
                    {
                        ModelState.AddModelError("ExistingEmail", UserMembershipService.ErrorCodeToString(status));
                        return false;
                    }
                    membershipService.Authenticate(storyTicket.reservationUser);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ExistingEmail", ex);
                    return false;
                }
            }
            return true;
        }

        private ResUser updateUser(ResUser user)
        {
            ResUser newUser = membershipService.GetUserWithAddress(user.Email);
            newUser.Name.First = user.Name.First;
            newUser.Name.Last = user.Name.Last;
            newUser.Address.Zip.Code = user.Address.Zip.Code;
            membershipService.Save(newUser);
            return (newUser);
        }

        #endregion utils

        #region cookies
        public void setCookie(String name, String value, String defaultValue)
        {
            if (Request.Cookies[name] != null)
            {
                HttpCookie oldCookie = new HttpCookie(name);
                oldCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(oldCookie);
            }

            HttpCookie cookie = new HttpCookie(name);

            if (!String.IsNullOrEmpty(value))
            {
                cookie.Value = value;
            }
            else
            {
                cookie.Value = defaultValue;
            }
            cookie.Expires = DateTime.Now.AddMinutes(1);
            Response.Cookies.Add(cookie);
        }

        public String getCookie(String name)
        {

            HttpCookieCollection cookies = Request.Cookies;

            HttpCookie cookie = cookies[name];

            if (cookie != null)
            {
                return (cookie.Value);
            }
            else
            {
                return ("1");
            }
        }
        #endregion cookies

    }
}
