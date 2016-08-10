using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers.shared;
using cfares.domain.user;
using cfacore.shared.domain.user;
using cfacore.shared.modules.user;
using System.Web.Security;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class UserController : Controller
    {
        UserMembershipService serv;
        bool self = false;


        //
        // GET: /Admin/User/

        public ActionResult Index()
        {
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            serv = new UserMembershipService(HttpContext);
            string id = null;
            if (filterContext.ActionParameters.ContainsKey("id"))
            id= filterContext.ActionParameters["id"] as string;
            if (string.IsNullOrEmpty(id) || (string)Session["UserId"] == id)
            {
                self = true;
                ViewBag.Self = true;
            }

            base.OnActionExecuting(filterContext);
        }


        

        //
        // GET: /Admin/User/Details/5

        public ActionResult Details(string id)
        {
            
            ResUser user;
            if (string.IsNullOrEmpty(id))
                user = serv.GetUser();
            else
                user = serv.Load(id);


            return View(user);
        }

        //
        // GET: /Admin/User/Create

        public ActionResult Create()
        {
            return View(new ResUser());
        }

        //
        // POST: /Admin/User/Create

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Console.Write(filterContext.Result);
            base.OnActionExecuted(filterContext);
        }

        [HttpPost]
        public ActionResult Create(ResUser user, FormCollection collection)
        {
            
            try
            {
                // TODO: Add create logic here
                System.Web.Security.MembershipCreateStatus createStatus;
                string pass;

                user.Name = new Name(collection["Name"]);
                user.HomePhone = new Phone(collection["HomePhone"]);
                user.MobilePhone = new Phone(collection["MobilePhone"]);

                serv.CreateUser(user, out createStatus ,out pass);

                string message = "";

                if (createStatus != System.Web.Security.MembershipCreateStatus.Success)
                {
                    message = UserMembershipService.ErrorCodeToString(createStatus);
                    ModelState.AddModelError("",message); 
                    return View(user);
                }
                if(string.IsNullOrEmpty(pass))
                    message = ("Account existed in a membership authority. A new user object was created for this user.");
                else
                    message = string.Format("Account created! The password for this account is: \"{0}\".",pass);

                return RedirectToAction("Details", new {id=user.Id(),message=message });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("",ex); 
                return View(user);
            }
        }

        //
        // GET: /Admin/User/Edit/5

        public ActionResult Edit(string id)
        {            
            ResUser user;
            if (string.IsNullOrEmpty(id))            
                user = serv.GetUser();
            else
                user = serv.Load(id);
            ViewBag.Membership = serv.GetAccount(user.Username, false);
            return View(user);
        }

        //
        // POST: /Admin/User/Edit/5

        [HttpPost]
        public ActionResult Edit(string id, ResUser user, FormCollection collection)
        {
            user.MobilePhone = new Phone(collection["MobilePhone"]);
            user.HomePhone = new Phone(collection["HomePhone"]);
            user.Name = new Name(collection["Name"]);

            
            string addressId = collection["Address.AddressId"];

            

            if(!string.IsNullOrEmpty(addressId))
                user.Address = new Address(addressId);

            

            try
            {
                // TODO: Add update logic here
                serv.Save(user);
            
                
                string message = "User Updated";
                return RedirectToAction("Details", new { id = user.Id(), message = message });
            }
            catch
            {
                
                if (string.IsNullOrEmpty(id))
                    user = serv.GetUser();
                else
                    user = serv.Load(id);
                return View(user);
            }
        }

        //
        // GET: /Admin/User/Delete/5

        public ActionResult Delete(string id)
        {
            
            ResUser user = serv.Load(id);

            return View(user);
        }

        //
        // POST: /Admin/User/Delete/5

        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            
            ResUser user = serv.Load(id.ToString());

            try
            {
                // TODO: Add delete logic here
                serv.DeleteUserAndAccount(user);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(user);
            }
        }
    }
}
