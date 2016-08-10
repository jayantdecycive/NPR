
#region Imports

using System.Web.Mvc;
using cfacore.shared.modules.com.admin;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.com.application;
using cfares.site.modules.user;
using cfaresv2.Areas.MyAccount.Controllers._base;
using cfaresv2.Areas.MyAccount.Models;
using cfacore.shared.domain.user;

#endregion

namespace cfaresv2.Areas.MyAccount.Controllers
{
	[AreaAuthorize( Area = "MyAccount", Roles = "Customer,Guide,Admin,Operator", Controller = "LogOn", Action = "Index" )]
    public class ProfileController : MyAccountController
    {
        // GET: /Profile/Update/
        public ActionResult Update( string returnUrl )
        {
            var user = new UserMembershipRepository( _context ).GetUserAndAccount();
            if(user.ApplicationUser.Address == null)
                user.ApplicationUser.Address = new Address();

			return View( new UpdateAccountViewModel
			{
				User = user
			});
        }

        // POST: /Profile/Update/
		[HttpPost]
        public ActionResult Update( FormCollection collection )
        {
			// TODO - Save data

            if (!string.IsNullOrEmpty(Request.QueryString["RedirectUrl"]))
                return Redirect(Request.QueryString["RedirectUrl"]);

		    if (collection["submit"] == "profile")
		    {
                ModelState.Remove("PasswordCurrent");
                ModelState.Remove("PasswordNew");
                ModelState.Remove("PasswordNewConfirm");

                var repo = new UserMembershipRepository(_context);
                var user = repo.Find(int.Parse(collection["User.ApplicationUser.UserId"]));
                user.FirstName = collection["User.ApplicationUser.FirstName"];
                user.LastName = collection["User.ApplicationUser.FirstName"];
                user.BirthDate = int.Parse(collection["BirthDate"]);
                user.BirthMonth = collection["BirthMonth"];
                user.Address.ZipString = collection["User.ApplicationUser.Address.ZipString"];
                repo.Commit();
		    }
            else if (collection["submit"] == "account")
		    {
                ModelState.Remove("User.ApplicationUser.FirstName");
                ModelState.Remove("User.ApplicationUser.LastName");
                ModelState.Remove("BirthMonth");
                ModelState.Remove("BirthDay");
                ModelState.Remove("User.ApplicationUser.Address.ZipString");
		    }

		    // TODO - Success message
			return RedirectToAction("Confirm");
        }

        // GET: /Profile/Confirm/
        public ActionResult Confirm(int? id)
        {
            ResUser user = null;
            if (id == null)
                user = AppContext.User;
            else
            {
                var userRepo = new UserMembershipRepository(_context);
                user = userRepo.Find(id.Value);
            }
			return View(user);
        }

		// SH - Not implementing at this time per MD
		//// GET: /Profile/Create/
		//public ActionResult Create()
		//{
		//	return View();
		//}

        // GET: /Profile/Password/
        public ActionResult Password()
        {
			return View();
        }

        // POST: /Profile/Password/
		[HttpPost]
        public ActionResult Password( ChangePasswordViewModel model )
        {
			if( ModelState.IsValid )
			{
                IResContext serv = ReservationConfig.GetContext();
                UserMembershipRepository membershipService = new UserMembershipRepository(HttpContext, serv);
				if( membershipService.ValidatePassword(AppContext.User.Username, model.PasswordCurrent) )
				{
					membershipService.ChangePassword( model.PasswordCurrent, model.PasswordNew );

					// TODO - Change password success page
					return ControllerContext.RedirectToArea();
				}
				
				ModelState.AddModelError( "PasswordCurrent", "Invalid username / password" );
			}

			return View();
        }

        
    }
}
