using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using cfacore.shared.domain._base;
using cfacore.site.controllers.shared;
using cfares.domain.user;
using cfares.site.modules.mail;
using System.Web;
using System.Threading;
using System.ServiceModel.Channels;
using System.Net;
using System.Web.Security;
using System.Runtime.Serialization.Json;
using System.IO;
using cfares.DataService;


namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Slot" in code, svc and config file together.
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Slot : ISlot
    {
        SlotService serv = new SlotService();

        public void DoWork()
        {
            //Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        public string Hello(string id)
        {
            return "World: " + id;
        }


        public cfares.domain._event.slot.tours.TourSlot CreateTourSlot(string OccurrenceId, string Start, string End, int Status, int Capacity, string Cutoff,
            string GuideId, string SlotId, bool KidFriendly, string SpecialNeeds)
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return SaveTourSlot(null,OccurrenceId, Start, End, Status, Capacity,Cutoff, GuideId, SlotId, KidFriendly, SpecialNeeds);
        }

        public cfares.domain._event.slot.tours.TourSlot GetTourSlot(string SlotId)
        {
            return serv.LoadTour(SlotId);
        }

        public cfares.domain._event.Slot GetSlot(string SlotId)
        {
            return serv.Load(SlotId);
        }

        public cfares.domain._event.Slot[] GetByEventTypeWithDateRange(string EventType, DateTime Start, DateTime End)
        {
            return serv.DeCacheOrLoadByEventTypeWithDateRange(EventType, Start, End).ToArray();
        }

        public cfares.domain._event.slot.tours.TourSlot[] GetTourSlotByEventTypeWithDateRange(string EventType, DateTime Start, DateTime End)
        {
            var slots= serv.DeCacheOrLoadTourByEventTypeWithDateRange(EventType, Start, End).ToArray();

            
            return slots;
        }

        public bool SaveCameosForTourSlot(string id, string[] UserIds, int[] CameoTypes)
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return serv.SaveCameosForTourSlot(id, UserIds, CameoTypes);
        }

        public bool RemoveCameosForTourSlot(string id, string[] UserIds, int[] CameoTypes)
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return serv.RemoveCameosForTourSlot(id, UserIds, CameoTypes);
        }
        

        public cfares.domain._event.slot.tours.TourSlot SaveTourSlot(string id, string OccurrenceId, string Start, string End, int Status, int Capacity, string Cutoff,
            string GuideId, string SlotId, bool KidFriendly, string SpecialNeeds)
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            cfares.domain._event.slot.tours.TourSlot newSlot = new cfares.domain._event.slot.tours.TourSlot();
            
            if (!string.IsNullOrEmpty(SlotId))
            {

                newSlot = serv.LoadTour(SlotId);
                if (newSlot==null || !newSlot.IsBound())
                {
                    newSlot = new cfares.domain._event.slot.tours.TourSlot(serv.Load(SlotId));
                }
            }
            
            if (!newSlot.IsBound())
            {              
                newSlot.Status = (cfares.domain._event.SlotStatus)Status;                
            }
            newSlot.Occurrence = new cfares.domain._event.Occurrence(OccurrenceId);
            newSlot.Availability = new DateRange(Start, End);
            newSlot.Cutoff = DateTime.Parse(Cutoff);
            newSlot.Capacity = Capacity;
            newSlot.Guide = new ResAdmin(GuideId);
            


            newSlot.KidFriendly = KidFriendly;
            newSlot.SpecialNeeds = SpecialNeeds;

            /*
             * Important! This effectively means that, if a user id is passed, the existing user will be upgraded to be a TourSlot
             * */
            if (!String.IsNullOrWhiteSpace(SlotId))
                newSlot.Id ( SlotId);

            serv.Save(newSlot);

            return newSlot;
        }


        public cfares.domain._event.slot.tours.TourSlot TestSlot()
        {
            return serv.LoadTour("2540");
        }

        public bool TestDelete()
        {
            return true;
        }

        public bool TestPut()
        {
            return true;
        }

        public bool TestMerge()
        {
            return true;
        }
    }
}
