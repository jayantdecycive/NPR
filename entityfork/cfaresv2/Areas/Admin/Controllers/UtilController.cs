
#region Imports

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.site.modules.com.application;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtilController : Controller
    {
        public ActionResult ClearCache()
        {
            return View();
        }

		public ActionResult RemoveLocation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RemoveLocation(FormCollection collection)
        {
            string storeNumber = collection["store_number"];
            string eventNumber = collection["event_number"];

            //ViewBag.Message = "Successfully Removed occurrence and corresponding slots";
            ViewBag.Message = "Not Set Up";
            return View();
        }

        public ActionResult CopyLocation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CopyLocation(FormCollection collection)
        {
            string storeNumber = collection["store_number"];
            string storeNumberCopy = collection["store_number_copy"];
            string eventNumber = collection["event_number"];
            bool giveaway = collection["giveaway"].Contains("on");

            try
            {
                string ins1 = string.Format("INSERT INTO Occurrences ([Start],[End],[SlotRangeStart],[SlotRangeEnd],[BoundToPrototype],[StoreId],[ResEventId],[Discriminator],[Status],[OffSiteAddressId],[OffSiteDescription])"
                    + "Select [Start],[End],[SlotRangeStart],[SlotRangeEnd],[BoundToPrototype],'{0}',[ResEventId],[Discriminator],[Status],[OffSiteAddressId],[OffSiteDescription]"
                    + "From Occurrences Where StoreId='{1}'And ResEventId={2}", storeNumber, storeNumberCopy, eventNumber);

                DbContext c = (DbContext)ReservationConfig.GetContext();
                ObjectContext oc = (c as IObjectContextAdapter).ObjectContext;
                oc.ExecuteStoreCommand(ins1);
                //int id = oc.ExecuteStoreQuery<int>("SELECT SCOPE_IDENTITY()").FirstOrDefault();
                int id = oc.ExecuteStoreQuery<int>("select max( occurrenceid ) from Occurrences").FirstOrDefault();

                //ObjectParameter parameter = new ObjectParameter("OutputParameterName", typeof(int));
                //int results = oc.ExecuteStoreCommand( ins1, parameter);
                //int id = Convert.ToInt32( parameter.Value );

                string ins2 = string.Format("INSERT INTO [Slots] ([IsScheduled],[Capacity],[Start],[Cutoff],[End],[Discriminator],[OccurrenceId],[ScheduleId],[Grouping],[Status],[Visibility],[Notes],[OriginalCapacity]) "
                    + "select slots.[IsScheduled],slots.[Capacity],slots.[Start],slots.[Cutoff],slots.[End],slots.[Discriminator],{0},slots.[ScheduleId],[Grouping],Occurrences.[Status],[Visibility],[Notes],[OriginalCapacity]  "
                    + "from slots Left Join Occurrences on Occurrences.OccurrenceId=Slots.OccurrenceId "
                    + "Where Occurrences.ResEventId={2} and Occurrences.StoreId='{1}'", id, storeNumberCopy, eventNumber);

                oc.ExecuteStoreCommand(ins2);


                if (giveaway)
                {
                    string ins3 = string.Format("INSERT INTO [GiveawayOccurrences] ({0})", id);
                    oc.ExecuteStoreCommand(ins3);
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = "Sorry something went wrong - " + e.Message;
                return View();
            }
            ViewBag.Message = "Successfully Coppied and Added new occurrence and matching slots";
            return View();
        }
    }
}
