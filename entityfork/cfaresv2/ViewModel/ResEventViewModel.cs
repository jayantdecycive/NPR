using cfacore.shared.domain._base;
using cfares.domain._event;
using cfaresv2.ViewModel._base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cfaresv2.ViewModel
{
    public class ResEventViewModel : GenericViewModel<ResEvent>
    {
        public string Name { get; set; }

        public int ResEventId { get; set; }

        public System.DateTime RegistrationStart
        {
            get;
            set;
        }

        public System.DateTime RegistrationEnd
        {
            get;
            set;
        }

        [Required]
        public System.DateTime SiteStart
        {
            get;
            set;
        }

        [Required]
        public System.DateTime SiteEnd
        {
            get;
            set;
        }
    }
}