using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace cfares.Areas.Tours.Models
{
    public class TourRegistrationForm
    {
        public bool firstPassValidation;

        public UserInfoForm userForm { get; set; }

        public LunchOptIn lunchForm { get; set; }

        public CustomerTypeDescriptionForm groupDescription { get; set; }

        public SpecialNeedsForm specialNeedsForm { get; set; }

        public ContactInfoForm contactForm { get; set; }

        [Required]
        [Display(Name = "Group Size")]
        public int GroupSize { get; set; }
        
        public List<TimeRequestForm> tourTimeRequests { get; set; }

        public TourRegistrationForm()
        {
            tourTimeRequests = new List<TimeRequestForm>();
            for (int i = 0; i < 3; i++)
            {
                TimeRequestForm preference = new TimeRequestForm();
                tourTimeRequests.Add(preference);
            }

        }

        public void update()
        {
            lunchForm.totalCostofLunches = 6.5 * (lunchForm.numberOfSpecialNeeds + lunchForm.numberOfKidLunches + lunchForm.numberOfAdultLunches);
        }
    }

    public class UserInfoForm
    {

        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage="Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"\d+", ErrorMessage = "Please Enter a Valid Zip Code")]
        [StringLength(6, ErrorMessage = "Please Enter a Valid Zip Code")]
        public string zipCode { get; set; }

    }

    public class LunchOptIn
    {
        public bool paidLunch;

        public LunchOptIn(bool argPaidLunch)
        {
            paidLunch = argPaidLunch;
        }

        public double totalCostofLunches { get; set; }

        [Required]
        public bool optInForLunch { get; set; }

        [Range(0, 120)]
        public int numberOfAdultLunches { get; set; }

        [Range(0, 120)]
        public int numberOfKidLunches { get; set; }

        [Range(0, 120)]
        public int numberOfSpecialNeeds { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length of 250 characters.")]
        public string descriptionOfSpecialNeeds { get; set; }

    }

    public class CustomerTypeDescriptionForm 
    {
        [Display(Name = "Family with Kids")]
        public bool isFamilyWithKids { get; set; }

        [Display(Name = "School Group")]
        public bool isSchoolGroup { get; set; }

        [Display(Name = "Family without Kids")]
        public bool isFamilyWithoutKids { get; set; }

        [Display(Name = "Adult Group")]
        public bool isAdultGroup { get; set; }

        [Display(Name = "Religious Group")]
        public bool isChurchGroup { get; set; }

        [Display(Name = "Senior Group")]
        public bool isSeniorGroup { get; set; }

        [Display(Name = "Business Group")]
        public bool isBusinessGroup { get; set; }

        [Display(Name = "Operator Raving Fans")]
        public bool isRavingFans { get; set; }

        [Display(Name = "Team Member")]
        public bool isTeamMemberGroup { get; set; }

        [Display(Name = "Other")]
        public bool isOtherType { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length of 250 characters.")]
        public string otherTypeDescription { get; set; }


        
    }

    public class SpecialNeedsForm 
    {
        [Required]
        public bool hasSpecialNeeds { get; set; }

        [Display(Name = "Visually Impaired")]
        public bool isVisuallyImpaired { get; set; }

        [Display(Name = "Other")]
        public bool Other { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length of 250 characters.")]
        [Display(Name = "Other Needs Description")]
        public string otherDescription { get; set; }

        [Display(Name = "Hearing Impaired")]
        public bool isHearingImpaired { get; set; }

        [Display(Name = "Wheel Chair Needed")]
        public bool needsWheelChair { get; set; }


    }

    public class ContactInfoForm
    {
        public SelectList contactTypes =
           new SelectList(
           new[] {
                new { Value="Phone" },
                new { Value="Email"}
            }, "Value", "Value");

        [Required]
        [StringLength(250, ErrorMessage = "Maximum length of 250 characters.")]
        public string contactInfo { get; set; }

        [Required]
        public string preferContactType { get; set; }

    }

    public class GroupRegistrationForm : TourRegistrationForm, IValidatableObject
    {


        [Required]
        public bool isOverThirteen { get; set; }
     

        [Required]
        public bool joinInsiders { get; set; }

        public GroupRegistrationForm()
        {
            lunchForm = new LunchOptIn(true);
            contactForm = new ContactInfoForm();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            firstPassValidation = true;

            if (!isOverThirteen)
            {
                yield return new  ValidationResult("Must be over thirteen to register.", new[] { "isOverThirteen" });
            }

            if (lunchForm.optInForLunch)
            {
                if ((lunchForm.numberOfAdultLunches == 0) && (lunchForm.numberOfKidLunches == 0) && (lunchForm.numberOfSpecialNeeds == 0))
                {
                    yield return new ValidationResult("Please specify how many lunches will be needed", new[] { "lunchForm.optInForLunch" });
                }


            }

            if (specialNeedsForm.hasSpecialNeeds && !(specialNeedsForm.isVisuallyImpaired || specialNeedsForm.isHearingImpaired || specialNeedsForm.needsWheelChair || specialNeedsForm.Other))
            {
                yield return new ValidationResult("Please select at least one description", new[] { "specialNeedsForm.otherDescription"});
            }
            else if (specialNeedsForm.Other && String.IsNullOrEmpty(specialNeedsForm.otherDescription))
            {
                yield return new ValidationResult("Please provide an explanation", new[] { "specialNeedsForm.otherDescription" });
            }

            if (!(groupDescription.isFamilyWithKids || groupDescription.isFamilyWithoutKids || groupDescription.isChurchGroup || groupDescription.isBusinessGroup ||
                groupDescription.isSchoolGroup || groupDescription.isAdultGroup || groupDescription.isSeniorGroup || groupDescription.isOtherType))
            {
                yield return new ValidationResult("Please select at least one description", new[] { "groupDescription.otherTypeDescription" });
            }
            else if (groupDescription.isOtherType && String.IsNullOrEmpty(groupDescription.otherTypeDescription))
            {
                yield return new ValidationResult("Please provide an explanation", new[] { "groupDescription.otherTypeDescription" });
            }

            if (Membership.FindUsersByEmail(userForm.Email).Count > 0)
            {
                yield return new ValidationResult("A Tour Registration already exists under this email address", new[] { "userForm.Email" });

            }

            if (GroupSize <= 0 || GroupSize > 120)
            {

                yield return new ValidationResult("Group size must be between 1 and 120 people.", new[] { "GroupSize" });
            }

        }
    }

    public class TeamRegistrationForm : TourRegistrationForm, IValidatableObject
    {

        [Required]
        [Display(Name = "Restaurant # ")]
        [StringLength(5, ErrorMessage = "The Restaurant # must be 5 characters long.", MinimumLength = 5)]
        public string storeNumber { get; set; }

        [Display(Name = "Marketing & Innovation")]
        public bool visitMarketing { get; set; }

        [Display(Name = "Tech Center")]
        public bool visitTech { get; set; }

        [Display(Name = "Innovation Center")]
        public bool visitInnovation { get; set; }

        [Display(Name = "Training Simulator")]
        public bool visitTraining { get; set; }

        [Display(Name = "Wellness Center")]
        public bool visitWellness { get; set; }

        [Display(Name = "Warehouse")]
        public bool visitWareHouse { get; set; }

        [Display(Name = "IT Building")]
        public bool visitIT { get; set; }

        [Display(Name = "Other")]
        public bool visitOther { get; set; }

        public string visitOtherDescription { get; set; }

        public TeamRegistrationForm() {
            contactForm = new ContactInfoForm();
            lunchForm = new LunchOptIn(false);

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            firstPassValidation = true;

            if (visitOther && String.IsNullOrEmpty(visitOtherDescription))
            {
                yield return new ValidationResult("Please provide a description of the location", new[] { "visitOtherDescription" });
            }


            if (specialNeedsForm.hasSpecialNeeds && !(specialNeedsForm.isVisuallyImpaired || specialNeedsForm.isHearingImpaired || specialNeedsForm.needsWheelChair || specialNeedsForm.Other))
            {
                yield return new ValidationResult("Please select at least one description", new[] { "specialNeedsForm.otherDescription" });
            }
            else if (specialNeedsForm.Other && String.IsNullOrEmpty(specialNeedsForm.otherDescription))
            {
                yield return new ValidationResult("Please provide an explanation", new[] { "specialNeedsForm.otherDescription" });
            }


            if (GroupSize <= 0 || GroupSize > 120)
            {

                yield return new ValidationResult("Group size must be between 1 and 120 people.", new[] { "GroupSize" });
            }

        }
    }

    public class AvailableTimeDropDownOption
    {
        public string timeOfDay { get; set; }
    }

    public class TimeRequestForm
    {
        public DateTime tourTime;

         [Required(ErrorMessage = "Please choose a tour date")]
        public string day { get; set; }

        public List<AvailableTimeDropDownOption> availableTimes;

        [Required(ErrorMessage="Please choose a tour time")]
        public string requestedTimeOfDay { get; set; }

        public TimeRequestForm()
        {
            availableTimes = new List<AvailableTimeDropDownOption>();
            availableTimes.Add(new AvailableTimeDropDownOption { timeOfDay = "9:30 AM" });
            availableTimes.Add(new AvailableTimeDropDownOption { timeOfDay = "1:00 PM" });
            availableTimes.Add(new AvailableTimeDropDownOption { timeOfDay = "3:00 PM" });
        }


    }


}