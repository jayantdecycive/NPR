using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using cfacore.site.controllers.shared;
using cfares.domain._event.slot.tours;
using cfacore.domain.user;
using cfares.domain._event._ticket.tours;
using cfares.domain.user;
using System.Text.RegularExpressions;

namespace cfares.Areas.tours.Models
{
    public class TourTicketFormModel : IValidatableObject
    {
        public bool firstPassValidation;

        public TourTicket tourTicket { get; set; }

        public ResUser reservationUser {
            get 
            {
                return tourTicket.Owner;
            }
            set
            {
                tourTicket.Owner = value;
            }
        }

        public TourTicketFormModel() { }

        public TourTicketFormModel(TourTicket existingTicketInfo)
        {
            tourTicket = existingTicketInfo;
            
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            firstPassValidation = true;

            if (tourTicket.GuestCount > tourTicket.Slot.TicketsAvailable){
                yield return new ValidationResult("Guest count exceeds number of available tickets. Please reduced group's size or choose another group", new[] { "tourTicket.GuestCount"});
            }

            if (tourTicket.HasSpecialNeeds && !(tourTicket.IsVisuallyImpaired || tourTicket.IsHearingImpaired || tourTicket.NeedsWheelChair || tourTicket.OtherNeeds)) {
                yield return new ValidationResult("Please select at least one description", new[] { "tourTicket.OtherNeedsDescription" });
            }

            if (tourTicket.OtherNeeds && String.IsNullOrEmpty(tourTicket.OtherNeedsDescription)) {
                yield return new ValidationResult("Please provide an explanation", new[] { "tourTicket.OtherNeedsDescription" });
            }

            if (tourTicket.GuestCount <= 0 || tourTicket.GuestCount > 120) {
                yield return new ValidationResult("Group size must be between 1 and 120 people.", new[] { "GroupSize" });
            }

        }

    }

    public class ReservationUserCreationForm : UserCreationForm
    {
        [Required]
        public bool isOverThirteen { get; set; }


        [Required]
        [Display(Name = "I understand that all guests in my group must be cleared by security 3 days in advance of my tour. If all guest names have not been submitted 72 hours prior to the tour, my tour will be cancelled.")]
        public bool guestNamesConfirm { get; set; }


        [Required]
        public bool joinInsiders { get; set; }


        [Display(Name = "Confirm Email")]
        public string emailConfirmation { get; set; }
    }


    public class StoryTourTicketFormModel : TourTicketFormModel, IValidatableObject
    {

        public ReservationUserCreationForm userForm { get; set; }

        public StoryTourTicketFormModel() {
                //initialUserCreation = true;
        }

        public StoryTourTicketFormModel(TourTicket existingTicketInfo)
            : this(existingTicketInfo, true)
        {
        }

        public StoryTourTicketFormModel(TourTicket existingTicketInfo, bool initialUser)
            : base(existingTicketInfo)
        {
            
            userForm = new ReservationUserCreationForm();
            
            
        }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> parentErrors = new List<ValidationResult>(base.Validate(validationContext));

            if (tourTicket.OptInForLunch)
            {
                if ((tourTicket.NumberOfAdultLunches == 0) && (tourTicket.NumberOfKidLunches == 0) && (tourTicket.NumberOfSpecialNeedLunches == 0))
                {
                    //parentErrors.Add(new ValidationResult("Please specify how many lunches will be needed", new[] { "lunchForm.optInForLunch" }));
                }

            }

            if (!(tourTicket.IsFamilyWithKids || tourTicket.IsFamilyWithoutKids || tourTicket.IsReligiousGroup || tourTicket.IsBusinessGroup ||
            tourTicket.IsSchoolGroup || tourTicket.IsAdultGroup || tourTicket.IsSeniorGroup || tourTicket.IsOtherTypeOfGroup))
            {
                parentErrors.Add(new ValidationResult("Please select a Group Type", new[] { "tourTicket.OtherTypeDescription" }));
            }

            if (tourTicket.IsOtherTypeOfGroup && String.IsNullOrEmpty(tourTicket.OtherTypeDescription))
            {
                parentErrors.Add(new ValidationResult("Please provide an explanation", new[] { "tourTicket.OtherTypeDescription" }));
            }

            if (!userForm.guestNamesConfirm)
            {
                parentErrors.Add(new ValidationResult("You must aggree to these terms", new[] { "userForm.guestNamesConfirm" }));
            }

            if (initialUserCreation)
            {
                if (!userForm.isOverThirteen)
                {
                    parentErrors.Add(new ValidationResult("Must be over thirteen to register.", new[] { "userForm.isOverThirteen" }));
                }



                if (userForm.emailConfirmation != reservationUser.Email)
                {
                    parentErrors.Add(new ValidationResult("Emails do not match.", new[] { "userForm.emailConfirmation" }));
                }
            }

            if (String.IsNullOrEmpty(tourTicket.GroupName))
            {
                parentErrors.Add(new ValidationResult("Must specify a group name.", new[] { "tourTicket.GroupName" }));
            }

            if (reservationUser.MobilePhone == null || reservationUser.MobilePhone.ToString().Length < 7)
            {
                parentErrors.Add( new ValidationResult("Please enter a valid phone number", new[] { "reservationUser.MobilePhone" }));
            }

            return parentErrors;
        }

        public bool initialUserCreation { get; set; }
    }

    public class TeamRegistrationCreationForm : TourTicketFormModel, IValidatableObject
    {

        public TeamRegistrationCreationForm(TourTicket existingTicket)
            : base(existingTicket)
        {

        }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> parentErrors = new List<ValidationResult>(base.Validate(validationContext));

            if (tourTicket.VisitOther && String.IsNullOrEmpty(tourTicket.VisitOtherDescription))
            {
                parentErrors.Add(new ValidationResult("Please provide a description of the location", new[] { "visitOtherDescription" }));
            }


            return parentErrors;
        }
    }
}