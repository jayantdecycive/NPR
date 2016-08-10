using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.Areas.Tours.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;
using System.Web.Profile;
using System.IO;
using System.Net.Mail;
using cfacore.shared.modules.com.admin;

namespace cfares.Areas.Tours.Controllers
{

    [UserModeFilter(userMode = "Customer")]
    public class ReservationController : Controller
    {
        public ActionResult BeginRegistration()
        {

            GroupRegistrationForm model = new GroupRegistrationForm();
            ViewBag.WizardModel = getWizradPartial(Session);
            return View("ReservationForm", model);


        }

        [HttpPost]
        public ActionResult BeginRegistration(GroupRegistrationForm model)
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

                return View("ReservationForm", model);
            }
            model.update();
            Session["TeamRegistration"] = model;
            return RedirectToAction("ReviewRegistration");

        }

        public ActionResult ModifyRegistration()
        {
            GroupRegistrationForm model = (GroupRegistrationForm)Session["TeamRegistration"];

            if (model == null)
            {
                return RedirectToAction("BeginRegistration");
            }

            return View("ReservationForm", model);
        }

        [HttpPost]
        public ActionResult ModifyRegistration(GroupRegistrationForm model)
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

                return View("ReservationForm", model);
            }
            model.update();
            Session["TeamRegistration"] = model;
            return RedirectToAction("ReviewRegistration");
        }

        public ActionResult ReviewRegistration(string test)
        {
            GroupRegistrationForm model;
            model = (GroupRegistrationForm)Session["TeamRegistration"];
            if (model == null)
            {
                return RedirectToAction("ReservationForm");
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
            GroupRegistrationForm model = (GroupRegistrationForm)Session["TeamRegistration"];
            if (model == null)
            {
                return RedirectToAction("ReservationForm");
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
            TempData["returnController"] = "Reservation";

            return RedirectToAction("SendOutConfirmationEmails", "Email");

        }

        public ActionResult RegistrationConfirmation()
        {
            GroupRegistrationForm model = (GroupRegistrationForm)Session["TeamRegistration"];
            if (model == null)
            {
                return RedirectToAction("ReservationForm");
            }
            //now clear out the session
            Session["TeamRegistration"] = null;
            FormsAuthentication.SignOut();
            return View("ReservationConfirmation", model);
        }

        public ActionResult NewRegistrationCancellation()
        {

            Session["TeamRegistration"] = null;

            return View("CancellationConfirmation");
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
                    TempData["returnController"] = "Reservation";
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

        static public cfacore.shared.modules.com.admin.Wizard getWizradPartial(HttpSessionStateBase Session)
        {
            string wizardId = "homeofficeidwizard";
            
            cfacore.shared.modules.com.admin.Wizard wiz;

            if(Session[wizardId]!=null){
                wiz=(cfacore.shared.modules.com.admin.Wizard)Session[wizardId];
            }else{
            
                wiz = new cfacore.shared.modules.com.admin.Wizard(wizardId, 5);            
                //wiz.Steps[0] = new cfacore.shared.modules.com.admin.WizardStep{Index=1,Name="My Step",Uri=new Uri("/MyUrl")};
                //TODO - add wizard steps

                wiz.Steps[0] = new cfacore.shared.modules.com.admin.WizardStep();
                //wiz.Steps[0] = new cfacore.shared.modules.com.admin.WizardStep{Name="Choose A Tour",Uri=new Uri("http://www.chick-fil-a.com")};
                //wiz.Steps[1] = new cfacore.shared.modules.com.admin.WizardStep{Index=1,Name="Choose A Date",Uri=new Uri("http://www.chick-fil-a.com")};
                //wiz.Steps[2] = new cfacore.shared.modules.com.admin.WizardStep{Index=2,Name="Registration",Uri=new Uri("http://www.chick-fil-a.com")};
                //wiz.Steps[3] = new cfacore.shared.modules.com.admin.WizardStep{Index=3,Name="Overview",Uri=new Uri("http://www.chick-fil-a.com")};
                //wiz.Steps[4] = new cfacore.shared.modules.com.admin.WizardStep{Index=4,Name="Conformation",Uri=new Uri("http://www.chick-fil-a.com")};

                //uncomment this when the above is complete
                Session[wizardId]=wiz;
            }

            return (wiz);

            

        }
    }
}

