using cfares.domain._event._ticket;
using cfares.domain._event._tickets;
using cfares.domain.user;
using System.ComponentModel.DataAnnotations;

namespace cfaresv2.Areas.npr.Models
{
    public class ModifyTicketViewModel
    {

        public ModifyTicketViewModel(){}

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Confirmation Number")]
        public string ConfirmNumber { get; set; }
    }
}