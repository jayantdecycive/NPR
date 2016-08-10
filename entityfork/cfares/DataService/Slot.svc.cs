using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DataServicesJSONP;

namespace cfares.DataService
{
    [JSONPSupportBehavior]
    public class Slot : DataService<cfacore.entity.dao._event.ResApplicationEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {



            bool isAuthenticated = true;
            if (isAuthenticated)
            {
                config.UseVerboseErrors = true;
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead);
                //config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);

                config.SetEntitySetAccessRule("Slots", EntitySetRights.All);
                config.SetEntitySetAccessRule("Users", EntitySetRights.All);
                config.SetEntitySetAccessRule("Slot_Tour_Cameo", EntitySetRights.All);
                config.SetEntitySetAccessRule("Slot_Tour_Joined", EntitySetRights.All);

                config.SetEntitySetAccessRule("Slot_Tour_Guide_Joined", EntitySetRights.All);
                config.SetEntitySetAccessRule("Slot_Tour", EntitySetRights.All);
            }
            else {
                config.SetEntitySetAccessRule("Slots", EntitySetRights.AllRead);                
                config.SetEntitySetAccessRule("Slot_Tour_Cameo", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("Slot_Tour_Joined", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("SlotMonth_EventType", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("SlotDate_EventType", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("Slot_Tour_Guide_Joined", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("Slot_Tour", EntitySetRights.AllRead);
            }
            config.SetEntitySetAccessRule("Slot_Tour_Capacity_Event", EntitySetRights.AllRead);
            
            
            config.SetEntitySetAccessRule("C_Admin_SlotDash_Past", EntitySetRights.AllRead);
            
            config.SetEntitySetAccessRule("C_Admin_SlotDash", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("C_Admin_Slot_TourDash", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("C_Admin_Slot_TourDash_Future", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("C_Admin_Slot_TourDash_Past", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("C_Admin_SlotDash_Future", EntitySetRights.AllRead);
            

            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        #region validation
        
        [ChangeInterceptor("Slots")]
        public void ValidateSlot(cfacore.entity.dao._event.Slot slot, UpdateOperations operation)
        {
            if (operation != UpdateOperations.Delete)
            {
                slot.Start = slot.Start.ToUniversalTime();
                slot.End = slot.End.ToUniversalTime();
                if (slot.Cutoff != null && slot.Cutoff.Value != null)
                    slot.Cutoff = slot.Cutoff.Value.ToUniversalTime();
            }

            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Slot), typeof(cfares.domain._event.slot.SlotMeta)), typeof(cfacore.entity.dao._event.Slot));
            
            Validation.FireDataServiceValidation(slot, operation);
        }

        [ChangeInterceptor("Slot_Tour")]
        public void ValidateSlotTour(cfacore.entity.dao._event.Slot_Tour slot, UpdateOperations operation)
        {
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Slot_Tour), typeof(cfares.domain._event.slot.ITourSlot)), typeof(cfacore.entity.dao._event.Slot_Tour));
                        
            Validation.FireDataServiceValidation(slot, operation);
        }

        

        #endregion

        protected override cfacore.entity.dao._event.ResApplicationEntities CreateDataSource()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ResApplicationEntities"].ConnectionString;
            return new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
        }
    }
}
