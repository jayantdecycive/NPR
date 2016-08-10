using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.Areas.Tours.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace cfares.Areas.Tours.Controllers
{
    [UserModeFilter(userMode = "Operator")]
    public class OperatorController : Controller
    {
        public ActionResult BeginRegistration()
        {

            TeamRegistrationForm model = new TeamRegistrationForm();
            return View("TeamReservationForm", model);

        }

        [HttpPost]
        public ActionResult BeginRegistration(TeamRegistrationForm model)
        {
            if (!ModelState.IsValid)
            {
                if (!model.firstPassValidation)
                {
                    var validationResults = model.Validate(new ValidationContext(model, null, null));
                    foreach (var error in validationResults)
                        foreach (var memberName in error.MemberNames)
                            ModelState.AddModelError(memberName, error.ErrorMessage);


                }

                return View("TeamReservationForm", model);
            }
            Session["TeamRegistration"] = model;
            return RedirectToAction("ReviewRegistration");

        }

        public ActionResult ModifyRegistration()
        {
            TeamRegistrationForm model = (TeamRegistrationForm)Session["TeamRegistration"];

            if (model == null)
            {
                return RedirectToAction("BeginRegistration");
            }

            return View("TeamReservationForm", model);
        }

        [HttpPost]
        public ActionResult ModifyRegistration(TeamRegistrationForm model)
        {
            if (!ModelState.IsValid)
            {
                if (!model.firstPassValidation)
                {
                    var validationResults = model.Validate(new ValidationContext(model, null, null));
                    foreach (var error in validationResults)
                        foreach (var memberName in error.MemberNames)
                            ModelState.AddModelError(memberName, error.ErrorMessage);


                }

                return View("TeamReservationForm", model);
            }
            Session["TeamRegistration"] = model;
            return RedirectToAction("ReviewRegistration");
        }

        public ActionResult ReviewRegistration(string test)
        {
            TeamRegistrationForm model = (TeamRegistrationForm)Session["TeamRegistration"];

            if (model == null)
            {
                return RedirectToAction("BeginRegistration");
            }
            string[] dateRequests = new string[3];

            int i = 0;
            foreach (TimeRequestForm request in model.tourTimeRequests)
            {
                dateRequests[i] = DateTime.Parse(request.day).ToLongDateString() + " at " + request.requestedTimeOfDay;
                i++;
            }
            ViewBag.dateRequests = dateRequests;

            return View("ReservationOverview", model);
        }

        public ActionResult ProcessRegistration()
        {
            TeamRegistrationForm model = (TeamRegistrationForm)Session["TeamRegistration"];
            if (model == null)
            {
                return RedirectToAction("BeginRegistration");
            }
            MembershipCreateStatus createStatus;
            Membership.CreateUser(model.userForm.Email, model.userForm.Password, model.userForm.Email, null, null, true, null, out createStatus);

            if (createStatus != MembershipCreateStatus.Success)
            {
                ViewBag.Error = createStatus.ToString();
                return View("ReservationOverview", model);
            }
            FormsAuthentication.SetAuthCookie(model.userForm.Email, false);
            TempData["returnAction"] = "RegistrationConfirmation";
            TempData["returnController"] = "Operator";


            return RedirectToAction("SendOutConfirmationEmails", "Email");

        }

        public ActionResult RegistrationConfirmation()
        {
            TeamRegistrationForm model = (TeamRegistrationForm)Session["TeamRegistration"];
            if (model == null)
            {
                return RedirectToAction("BeginRegistration");
            }
            //now clear out the session
            FormsAuthentication.SignOut();
            Session["TeamRegistration"] = null;
            return View("ReservationConfirmation", model);
        }

        public ActionResult NewRegistrationCancellation()
        {
            Session["TeamRegistration"] = null;

            return View("CancellationConfirmation");
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult OperatorPlan()
        {
            return View();
        }

        public ActionResult CancelExistingReservation()
        {
            return View("ModifyReservation");
        }

        [HttpPost]
        public ActionResult CancelExistingReservation(LogOnModel model)
        {
            if (ModelState.IsValid)
            {


                if (Membership.ValidateUser(model.email, model.password))
                {
                    FormsAuthentication.SetAuthCookie(model.email, false);
                    //Kick off emails
                    TempData["returnAction"] = "ProcessReservationCancellation";
                    TempData["returnController"] = "Operator";
                    return RedirectToAction("SendOutCancellationEmails", "Email");
                }
                else
                {
                    ViewBag.Result = "Email/Password is incorrect.";

                }
            }
            return View("ModifyReservation");

        }

        [Authorize]
        public ActionResult ProcessReservationCancellation()
        {
            FormsAuthentication.SignOut();
            Membership.DeleteUser(User.Identity.Name);
            return View("CancellationConfirmation");
        }
    }
}
