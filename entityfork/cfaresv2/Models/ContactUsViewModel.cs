using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cfares.domain.store;
using cfares.domain.user;

namespace cfaresv2.Models
{
    public class ContactUsViewModel
    {
        public string UserAgent { get; set; }
        public string UserIp { get; set; }
        public int? UserId { get; set; }
        public ResUser User { get; set; }
        public DateTime CreatedDate { get; set; }
        public ResStore Location { get; set; }
        public string LocationNumber { get; set; }
        public string Title { get; set; }
        public string AgeRange { get; set; }
        public bool AgeCheck { get; set; }
        public string PrefeddedContact { get; set; }
        public string DiningFrequency { get; set; }
        public string Comments { get; set; }
    }
}