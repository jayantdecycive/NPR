
#region Imports

using System.ComponentModel.DataAnnotations;
using cfacore.domain.user;
using cfacore.shared.domain.user;
using cfares.domain.user;

#endregion

namespace cfaresv2.Areas.MyAccount.Models
{
    public class UpdateAccountViewModel : ChangePasswordViewModel
    {
        

		public UserAccount<ResUser> User { get; set; }

		
    }
}