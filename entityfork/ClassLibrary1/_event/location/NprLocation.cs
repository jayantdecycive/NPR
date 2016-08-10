using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npr.domain._event.location
{
    public class NPRLocation : INprLocation
    {
        public NPRLocation()
        {
        }

        public int? NPRLocationId { get; set; }

        [Display(Name = "Name")]
        public string Name
        {
            get;
            set;
        }


        public virtual cfacore.shared.domain.user.Address Address
        {
            get;
            set;
        }

        [Display(Name = "NprNprLocation Url")]
        public Uri Site
        {
            get;
            set;
        }


        [Display(Name = "NprNprLocation Url Name")]
        public string SiteName
        {
            get;
            set;
        }

        [Display(Name = "Space Type")]
        public EventSpaceType EventSpaceType
        {
            get;
            set;
        }

        [Display(Name = "On-Site Parking")]
        public bool Parking
        {
            get;
            set;
        }

        [Display(Name = "On NPR Property")]
        public bool AtNPR
        {
            get;
            set;
        }

        [Display(Name = "Capacity")]
        public int Capacity
        {
            get;
            set;
        }

        [Display(Name = "Contact Name")]
        public string ContactName
        {
            get;
            set;
        }

        [Display(Name = "Phone")]
        public cfacore.shared.domain.user.Phone Phone
        {
            get;
            set;
        }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Please enter a valid email address")]
        public string Email
        {
            get;
            set;
        }

        [Display(Name = "Comments")]
        public string Comments
        {
            get;
            set;
        }

        public int? AddressId { get; set; }
    }
}
