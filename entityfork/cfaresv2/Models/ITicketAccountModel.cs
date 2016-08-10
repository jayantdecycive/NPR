using System.ComponentModel.DataAnnotations;
using cfacore.shared.modules.com.datatypes;
using cfares.domain._event;
using cfares.domain.user;

namespace cfaresv2.Models
{
    public interface ITicketAccountModel
    {
        ResUser User { get; set; }

        [Required]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        string Password { get; set; }

        [Required]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Re-Enter Email")]
        [System.Web.Mvc.Compare("EmailAddress", ErrorMessage = "The email and confirmation email do not match.")]
        string ConfirmEmail { get; set; }

        bool StoreOptIn { get; set; }
        string BirthMonth { get; set; }
        int BirthDate { get; set; }

        [Required]
// [Range(2, int.MaxValue, ErrorMessage = "You must be of age to attend this event")]
        YesNo AgeCheck { get; set; }

        ITicket GetTicket();
    }
}