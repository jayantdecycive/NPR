using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Mvc;
using cfacore.shared.modules.helpers;
using cfares.domain.user;
using System.Web.Security;
using cfares.site.modules.com.application;
using cfares.site.modules.user;
using cfares.entity.dbcontext.res_event;
using cfaresv2.Areas.Admin.Controllers._base;

namespace cfaresv2.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa,Operator")]
    public class UserController : AdminController
    {
        UserMembershipRepository _serv;
        IResContext _context;

        //
        // GET: /Admin/User/

        public ActionResult Index()
        {
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _context = ReservationConfig.GetContext();
            _serv = new UserMembershipRepository(HttpContext,_context);
            int id = 0;
            if (filterContext.ActionParameters.ContainsKey("id"))
            id= int.Parse(filterContext.ActionParameters["id"].ToString());
            if (int.Parse(Session["UserId"].ToString()) == id)
                ViewBag.Self = true;

            base.OnActionExecuting(filterContext);
        }

        //
        // GET: /Admin/User/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            try
            {
                ResUser user = id == null ? _serv.GetUser() : _serv.Find(id.Value);
                return View(user);
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
                return RedirectToAction("Index");
            }
        }

	    //
        // GET: /Admin/User/Create

        public ActionResult Create()
        {
            return View(new ResUser());
        }

        
        //
        // POST: /Admin/User/Create

        [HttpPost]
        public ActionResult Create( ResUser user, FormCollection collection )
        {
			// Ensure email address is provided
			if( ! user.Email.IsValid() )
				return View( user ).AddModelStateError( ModelState, "Email address is required" );

			// Check to see if user exists ( email address as username )
			if( _serv.IsUserInSystem( user.Email ) )
				return View( user ).AddModelStateError( ModelState, "Email address is already in use" );

            MembershipCreateStatus createStatus = MembershipCreateStatus.UserRejected;
            try
            {
                string pass;
                user.Username = user.Email;
                _serv.CreateUser(user, out createStatus ,out pass);
                
                string message;

                if (createStatus != MembershipCreateStatus.Success)
                {
                    message = UserMembershipRepository.ErrorCodeToString(createStatus);
                    ModelState.AddModelError("",message); 
                    return View(user);
                }

                //add user into roles
                switch (user.OperationRole)
                {
                    case UserOperationRole.Admin:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Admin);
                        break;

                    case UserOperationRole.Customer:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Customer);
                        break;

                    case UserOperationRole.Guide:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Guide);
                        break;

                    case UserOperationRole.None:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.None);
                        break;

                    case UserOperationRole.Operator:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Operator);
                        break;
                }

                message = string.IsNullOrEmpty(pass) ? 
					"Account existed in a membership authority. A new user object was created for this user." : 
					string.Format("Account created! The password for this account is: \"{0}\".",pass);

                return RedirectToAction("Details", new {id=user.Id(),message=message });
            }
            catch (DbEntityValidationException ex)
            {
                if (createStatus == MembershipCreateStatus.Success)
                    _serv.DeleteAccount(user.Email);

				List<string> propertyErrors = new List<string>(); 
                foreach (var field in ex.EntityValidationErrors)
                    foreach (var error in field.ValidationErrors) 
					{
						if( propertyErrors.Contains( error.PropertyName ) ) continue;
						if( error.PropertyName != "LastName" ) // Special case for duplicate last name validation message
	                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

						propertyErrors.Add( error.PropertyName );
                    }

				return View(user);
            }
            catch(Exception ex)
            {
                if (createStatus == MembershipCreateStatus.Success)
                    _serv.DeleteAccount(user.Email);

				ModelState.AddModelError("",ex); 
                return View(user);
            }
        }

        //
        // GET: /Admin/User/Edit/5

        public ActionResult Edit(int? id)
        {
	        ResUser user = id==null ? _serv.GetUser() : _serv.Find(id.Value);

            ViewBag.Membership = _serv.GetAccount(user.Username, false);
            return View(user);
        }

        //
        // POST: /Admin/User/Edit/5

        [HttpPost]
        public ActionResult Edit(int? id, ResUser user, FormCollection collection)
        {
            user.Username = user.Email;
            
            try
            {
                // TODO: Add update logic here
                _serv.Edit(user);


                // update roles
                //add user into roles

                //remove all roles first:
                MembershipUser memUser = Membership.GetUser(user.Username);
                string[] roles = Roles.GetRolesForUser(user.Username);  
                if(roles.Length>0)
                    Roles.RemoveUserFromRoles(user.Username, roles);

                //add roles back
                switch (user.OperationRole)
                {
                    case UserOperationRole.Admin:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Admin);
                        break;

                    case UserOperationRole.Customer:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Customer);
                        break;

                    case UserOperationRole.Guide:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Guide);
                        break;

                    case UserOperationRole.None:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.None);
                        break;

                    case UserOperationRole.Operator:
                        Roles.AddUserToRole(user.Username, IMembershipRoles.Operator);
                        break;
                }

                _serv.Commit();
                    
                
                const string message = "User Updated";
                return RedirectToAction("Details", new { id = user.Id(), message = message });
            }
            catch(Exception)
            {
	            user = id==null ? _serv.GetUser() : _serv.Find(id.Value);
	            return View(user);
            }
        }

        //
        // GET: /Admin/User/Delete/5

        public ActionResult Delete(int? id)
        {
	        Debug.Assert(id != null, "id != null");
	        ResUser user = _serv.Find(id.Value);

            return View(user);
        }

        //
        // POST: /Admin/User/Delete/5

        [HttpPost]
        public ActionResult Delete(int? id, FormCollection collection)
        {
	        Debug.Assert(id != null, "id != null");
	        ResUser user = _serv.Find(id.Value);

            try
            {
                // TODO: Add delete logic here
                _serv.DeleteUserAndAccount(user);
                return RedirectToAction("Index");
            }
			catch( DbUpdateException dbEx )
			{
				// Handler for {"The DELETE statement conflicted with the REFERENCE constraint 
				// .. \"FK_dbo.Tickets_dbo.ResUsers_OwnerId\". The conflict occurred in database
				// .. \"npr\", table \"dbo.Tickets\", column 'OwnerId'.\r\nThe statement has been terminated."}
				Exception ex = dbEx;
				while( ex.InnerException != null ) ex = ex.InnerException;
				if( ex.Message.Contains( "FK_dbo.Tickets_dbo.ResUsers_OwnerId" ) )
					return View(user).AddModelStateError( ModelState, 
						"Unable to delete user - One or more tickets exist for this user" );

				return View(user).AddModelStateError( ModelState, 
					"Unable to delete user - One or more related records exists for this user" );
			}
            catch( Exception ex )
            {
				while( ex.InnerException != null ) ex = ex.InnerException;
				return View(user).AddModelStateError( ModelState, "Unable to delete user - " + ex.Message );
            }
        }
    }
}
