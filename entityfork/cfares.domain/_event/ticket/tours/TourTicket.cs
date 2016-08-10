using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event.slot;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.common;
using cfares.domain.user;
using cfacore.shared.domain.attributes;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using cfares.domain._event.slot.tours;
using cfacore.shared.domain.user;

namespace cfares.domain._event._ticket.tours
{
    [SyncUrl("/DataService/ResEvent.svc/TourTickets")]
    [ModelId("TicketId")]
    public class TourTicket:Ticket,ITourTicket,IAdminReference
    {
        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Thursday, April 05, 2012
		/// </created>
		private int _GuestCount = 0;
        [Display(Name = "Guest Count")]
        public int GuestCount
        {
            get { return this._GuestCount; }
            set { this._GuestCount = value; }

        }

        


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        [System.Data.Linq.Mapping.Association(Name = "FK_Slot", ThisKey = "SlotId", OtherKey = "SlotId")]
        [DataType("Picker/_Slot")]
        [UIHint("AdminLink")]
        [ClientDefault(1, type = typeof(string))]        
        public TourSlot TourSlot
        {
            get {                 
                return (TourSlot)this.Slot;                
            }            
        }
        
        


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Thursday, April 05, 2012
        /// </created>
        
        private ResUserCollection _Guests = null;

        public ResUserCollection Guests
        {
            get { return this._Guests; }

            set { this._Guests = value; }
        }


        //Lunch 

        private const decimal priceOfAdultLunch = 7.0M;
        private const decimal priceOfChildLunch = 6.0M;

        public static string ADULT_LUNCH_COST { get { return priceOfAdultLunch.ToString("C2"); } }
        public static string CHILD_LUNCH_COST { get { return priceOfChildLunch.ToString("C2"); } }

        private decimal _totalCostOfLunches = 0;
        [Display(Name = "Total Cost of Lunches")]
        public decimal TotalCostOfLunches { get { return _totalCostOfLunches; } set { _totalCostOfLunches = value; } }
        private void recalculateLunchSum()
        {
            _totalCostOfLunches = (NumberOfAdultLunches * priceOfAdultLunch) +
                (NumberOfAdultLunches * priceOfChildLunch) +
                (NumberOfAdultLunches * priceOfAdultLunch);

        }

        [Required]
        [Display(Name = "Opt In For Lunch")]
        public bool OptInForLunch { get; set; }
                
        [Range(0, 120)]
        [Display(Name = "Number Of Adult Lunches")]
        public int NumberOfAdultLunches { get; set; }
                
        [Range(0, 120)]
        [Display(Name = "Number Of Kid Lunches")]
        public int NumberOfKidLunches { get; set; }

        [Range(0, 120)]
        [Display(Name = "Number Of Special Need Lunches")]
        public int NumberOfSpecialNeedLunches { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length of 250 characters.")]
        [Display(Name = "Special Diet Needs Description")]
        public string SpecialDietNeedsDescription { get; set; }

        //Visitation Requests

        [Display(Name = "Marketing & Innovation")]
        public bool VisitMarketing { get; set; }

        [Display(Name = "Tech Center")]
        public bool VisitTech { get; set; }

        [Display(Name = "Innovation Center")]
        public bool VisitInnovation { get; set; }

        [Display(Name = "Training Simulator")]
        public bool VisitTraining { get; set; }

        [Display(Name = "Wellness Center")]
        public bool VisitWellness { get; set; }

        [Display(Name = "Warehouse")]
        public bool VisitWareHouse { get; set; }

        [Display(Name = "IT Building")]
        public bool VisitIT { get; set; }

        [Display(Name = "Other")]
        public bool VisitOther { get; set; }

        [Display(Name = "Visit Other Description")]
        public string VisitOtherDescription { get; set; }


        //Special Needs
        [Required]
        [Display(Name = "Has Special Needs")]
        public bool HasSpecialNeeds { get; set; }

        [Display(Name = "Visually Impaired")]
        public bool IsVisuallyImpaired { get; set; }

        [Display(Name = "Other")]
        public bool OtherNeeds { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length of 250 characters.")]
        [Display(Name = "Other Needs Description")]
        public string OtherNeedsDescription { get; set; }

        [Display(Name = "Hearing Impaired")]
        public bool IsHearingImpaired { get; set; }

        [Display(Name = "Wheel Chair Needed")]
        public bool NeedsWheelChair { get; set; }


        //Group Description

        [Display(Name = "Family with Kids")]
        public bool IsFamilyWithKids { get; set; }

        [Display(Name = "School Group")]
        public bool IsSchoolGroup { get; set; }

        [Display(Name = "Family without Kids")]
        public bool IsFamilyWithoutKids { get; set; }

        [Display(Name = "Adult Group")]
        public bool IsAdultGroup { get; set; }

        [Display(Name = "Religious Group")]
        public bool IsReligiousGroup { get; set; }

        [Display(Name = "Senior Group")]
        public bool IsSeniorGroup { get; set; }

        [Display(Name = "Business Group")]
        public bool IsBusinessGroup { get; set; }

        [Display(Name = "Operator Raving Fans")]
        public bool IsRavingFans { get; set; }

        [Display(Name = "Team Member")]
        public bool IsTeamMemberGroup { get; set; }

        [Display(Name = "Other")]
        public bool IsOtherTypeOfGroup { get; set; }

        [StringLength(250, ErrorMessage = "Maximum length of 250 characters.")]
        [Display(Name = "Other Type Description")]
        public string OtherTypeDescription { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Guests")]
        public string GuestList
        {
            get {
                if (GuestNames == null)
                    return null;
                List<string> strOut = new List<string>();
                foreach(Name name in GuestNames){
                    if(name!=null)
                        strOut.Add(name.ToString());
                }
                return string.Join(", ", strOut);
            }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    GuestNames = null;
                    return;
                }
                string[] strIn = value.Split(',');
                List<Name> nameIn = new List<Name>();
                foreach(string strName in strIn){
                    nameIn.Add(new Name(strName));
                }
                GuestNames = nameIn.ToArray();
            }
        }
        
        public Name[] GuestNames
        {
            get;
            set;
        }



        public Name[] AllGuestNames { 
            get{
                List<Name> guests;
                if (GuestNames != null)
                    guests = GuestNames.ToList();
                else
                    guests = new List<Name>();
                guests.Add(Owner.Name);
                return guests.ToArray();
            }
        }
    }
}
