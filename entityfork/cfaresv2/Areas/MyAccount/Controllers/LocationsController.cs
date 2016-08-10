
#region Imports

using System.Web.Mvc;
using cfacore.shared.modules.com.admin;
using System.Linq;
using cfaresv2.Areas.MyAccount.Controllers._base;
using System.Collections.Generic;
using cfares.domain.user;
using cfares.repository.store;
using cfares.domain._event;
using System.Data.Entity;
using System.Web;
using System;
#endregion

namespace cfaresv2.Areas.MyAccount.Controllers
{
	[AreaAuthorize( Area = "MyAccount", Roles = "Customer", Controller = "LogOn", Action = "Index" )]
    public class LocationsController : MyAccountController
    {
        // GET: /Locations/Favorite/
        public ActionResult Favorite()
        {
			return RedirectToAction("PreferredStores");
        }

        // GET: /Locations/PreferredStores/
        public ActionResult PreferredStores()
        {
            IList<LocationSubscription> subscriptions = AppContext.User.UserPreferredLocationSubscriptions.Where(x=>x.Favorite).AsQueryable().Include("Store").ToList();

			return View(subscriptions);
        }

        [HttpPost]
        public RedirectResult Remove(FormCollection collection) {
            var user = AppContext.User;
            LocationRepository repo = new LocationRepository(_context);
            var storeId = collection["StoreId"];
            var userId = AppContext.User.UserId;
            var sub = repo.Context.LocationSubscriptions.First(x => x.StoreId == storeId&&x.UserId==userId);
            repo.Context.LocationSubscriptions.Remove(sub);
            repo.Commit();
            return Redirect("/Account/Locations/PreferredStores?message="+HttpUtility.UrlEncode("That store has been removed."));

        }

        // GET: /Locations/StoreEvents/
        public ActionResult StoreEvents(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Redirect("/");
            var locationRepo = new LocationRepository(_context);
            var location = locationRepo.Find(id);
            ViewBag.Location = location;
            IList<Occurrence> occurrences = location.Occurrences.AsQueryable().Include("ResEvent.Template.Preview").Include("ResEvent.SiteUrls").Where(x=>x.ResEvent.RegistrationEnd>DateTime.Now).ToList();
			return View(occurrences);
        }
    }
}
