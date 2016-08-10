using System.ComponentModel.DataAnnotations;
using cfacore.shared.modules.com.datatypes;
using cfares.domain._event;
using cfares.domain._event._tickets;
using cfares.domain.user;

namespace cfaresv2.Models
{
    public class TicketAccountModel<TTicket>: ITicketAccountModel where TTicket : class,ITicket,new()
    {
        public TTicket Ticket { get; set; }

        public ResUser User {
            get
            {
                
                return Ticket.Owner;
            }
            set
            {
                if(Ticket==null)
                    Ticket=new TTicket();
                Ticket.Owner = value;
            }
        }
    

        

        public TicketAccountModel()
        {
            if(this.Ticket==null)
            this.Ticket = new TTicket();
            if (this.User == null)
            this.User = new ResUser();
        }

        public TicketAccountModel(TTicket ticket)
        {
            this.Ticket = ticket;
            if(this.Ticket.Owner==null)
				this.User = new ResUser();
        }



        [Required]
		[RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
		[RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
		[DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?",
			ErrorMessage = "Please enter a valid email address")]
		[Display(Name = "Email Address")]
        public string EmailAddress {
            get
            {
                if (this.User == null)
                    return null;
                return this.User.Email;
            }
            set
            {
                if (this.User != null)
                this.User.Email = value;
            }
        }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?",
			ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Re-Enter Email")]
        [System.Web.Mvc.Compare("EmailAddress", ErrorMessage = "The email and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }

        public bool StoreOptIn { get; set; }
        //public bool ThirteenOrOlder { get; set; }

        public string BirthMonth {
            get
            {
                if (User == null)
                    return null;
                return User.BirthMonth;
            }
            set
            {
                if(User!=null)
                User.BirthMonth = value;
            }
        }

        public int BirthDate
        {
            get
            {
                if (User == null)
                    return 0;
                return User.BirthDate;
            }
            set
            {
                if (User != null)
                User.BirthDate=value;
            }
        }

        [Required]
		// [Range(2, int.MaxValue, ErrorMessage = "You must be of age to attend this event")]
		public YesNo AgeCheck { get; set; }


        public ITicket GetTicket()
        {
            return Ticket;
        }
    }
}