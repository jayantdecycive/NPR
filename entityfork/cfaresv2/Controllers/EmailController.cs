
#region Imports

using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using cfares.domain._event._ticket;
using cfares.domain._event._tickets;
using cfares.entity.dbcontext.res_event;
using cfares.repository.ticket;
using cfares.repository.user;
using cfares.site.modules.com.application;
using cfares.site.modules.mail;
using cfares.site.modules.user;

#endregion

namespace cfaresv2.Controllers
{
    public class EmailController : ControllerBase
	{
		#region Routes

		// GET: /Email/NewReservationConfirmation
        public ActionResult NewReservationConfirmation()
        {
            IResContext context = ReservationConfig.GetContext();
            Ticket t = (Ticket)new TicketRepository(context).GetAll()
				.OrderByDescending( o => o.CreatedDate ).FirstOrDefault();
			DateTicket dt = null;
			if( t != null ) dt = t as DateTicket;
			
	        object m = new ResEmailService().ResGetContextModel( dt );
			return View( "Email/NewReservationConfirmation", m );
        }

        // GET: /Email/NewReservationNotification
		[AreaAuthorize( Area = "MyAccount", Roles = "Customer", Controller = "LogOn", Action = "Index" )]
        public ActionResult NewReservationNotification()
        {            
            return View( "Email/NewReservationNotification" );
        }

		// GET: /Email/PasswordReset
		//[AreaAuthorize( Area = "MyAccount", Roles = "Customer", Controller = "LogOn", Action = "Index" )]
        public ActionResult PasswordReset()
        {
	        // MembershipUser u = Membership.GetUser();
            IResContext context = ReservationConfig.GetContext();
            UserMembershipRepository ur = new UserMembershipRepository(context);
	        MembershipUser u = ur.GetAccount( "gabby@thefoundryagency.com" );
			if( u == null ) throw new ApplicationException( "Unable to locate user with provided username" );

	        object m = new ResEmailService().ResGetContextModel( u );
			return View( "Email/PasswordReset", m );
        }

	    #endregion
	}
}
