using System.ComponentModel.DataAnnotations;

namespace cfaresv2.Areas.MyAccount.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
		[DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?",
			ErrorMessage = "Please enter a valid email address")]
		[Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?",
			ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Re-Enter Email")]
        [System.Web.Mvc.Compare("EmailAddress", ErrorMessage = "The email and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }
    }
}