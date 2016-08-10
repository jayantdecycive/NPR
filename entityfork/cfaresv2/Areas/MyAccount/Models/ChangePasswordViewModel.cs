
#region Imports

using System.ComponentModel.DataAnnotations;

#endregion

namespace cfaresv2.Areas.MyAccount.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
		[Display(Name = "Current Password")]
        public string PasswordCurrent { get; set; }

        [Required]
		[Display(Name = "New Password")]
        public string PasswordNew { get; set; }

        [Required]
		[Display(Name = "Re-enter New Password")]
        [System.Web.Mvc.Compare("PasswordNew", ErrorMessage = "New password and re-entered passwords do not match.")]
        public string PasswordNewConfirm { get; set; }
    }
}