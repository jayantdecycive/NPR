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
using System.ServiceModel;
using System.Threading;
using System.Linq.Expressions;

namespace cfares.DataService
{
    [JSONPSupportBehavior]
    public class ResEvent : DataService<cfacore.entity.dao._event.ResApplicationEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {

                config.UseVerboseErrors = true;
                //config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("ResEvents", EntitySetRights.All);
                config.SetEntitySetAccessRule("Schedules", EntitySetRights.All);
                config.SetEntitySetAccessRule("Occurrences", EntitySetRights.All);
                config.SetEntitySetAccessRule("Slots", EntitySetRights.All);
                config.SetEntitySetAccessRule("Slot_Tour_Joined", EntitySetRights.All);
                config.SetEntitySetAccessRule("Slot_Tour_Guide_Joined", EntitySetRights.All);
                config.SetEntitySetAccessRule("SlotMonth_EventType", EntitySetRights.All);
                config.SetEntitySetAccessRule("SlotDate_EventType", EntitySetRights.All);
                config.SetEntitySetAccessRule("SlotMonth_EventType_TourCapacity", EntitySetRights.All);
                config.SetEntitySetAccessRule("SlotDate_EventType_TourCapacity", EntitySetRights.All);
                config.SetEntitySetAccessRule("Users", EntitySetRights.All);
                config.SetEntitySetAccessRule("Slot_Tour", EntitySetRights.All);

            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        #region query
        [QueryInterceptor("ResEvents")]
        public Expression<Func<cfacore.entity.dao._event.ResEvent, bool>> OnEventQuery()
        {
            
            return (cfacore.entity.dao._event.ResEvent p) => true;
            
        }


        [QueryInterceptor("Users")]
        public Expression<Func<cfacore.entity.dao._event.User, bool>> OnUserQuery()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return (cfacore.entity.dao._event.User p) => true;

        }
        #endregion

        #region validation

        [ChangeInterceptor("Schedules")]
        public void ValidateResEvent(cfacore.entity.dao._event.Schedule resEvent, UpdateOperations operation) {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
        }


        [ChangeInterceptor("ResEvents")]
        public void ValidateResEvent(cfacore.entity.dao._event.ResEvent resEvent, UpdateOperations operation)
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            if (operation != UpdateOperations.Delete)
            {
                resEvent.SiteEnd = resEvent.SiteEnd.ToUniversalTime();
                resEvent.SiteStart = resEvent.SiteStart.ToUniversalTime();

                resEvent.RegistrationEnd = resEvent.RegistrationEnd.ToUniversalTime();
                resEvent.RegistrationStart = resEvent.RegistrationStart.ToUniversalTime();
            }

            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.ResEvent),
                typeof(cfares.domain._event.IResEvent)), typeof(cfacore.entity.dao._event.ResEvent));
            // Only validates on inserts and updates


            Validation.FireDataServiceValidation(resEvent, operation);
        }

        [ChangeInterceptor("Slots")]
        public void ValidateSlot(cfacore.entity.dao._event.Slot slot, UpdateOperations operation)
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
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
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Slot_Tour), typeof(cfares.domain._event.slot.ITourSlot)), typeof(cfacore.entity.dao._event.Slot_Tour));

            Validation.FireDataServiceValidation(slot, operation);
        }

        [ChangeInterceptor("Occurrences")]
        public void ValidateOccurrences(cfacore.entity.dao._event.Occurrence occurrence, UpdateOperations operation)
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            if (operation != UpdateOperations.Delete)
            {
                if (occurrence.SlotRangeEnd != null && occurrence.SlotRangeEnd.Value != null)
                    occurrence.SlotRangeEnd = occurrence.SlotRangeEnd.Value.ToUniversalTime();
                if (occurrence.SlotRangeStart != null && occurrence.SlotRangeStart.Value != null)
                    occurrence.SlotRangeStart = occurrence.SlotRangeStart.Value.ToUniversalTime();
            }

            
            

            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Occurrence), typeof(cfares.domain._event.IOccurrence)), typeof(cfacore.entity.dao._event.Occurrence));

            Validation.FireDataServiceValidation(occurrence, operation);
        }



        #endregion

        protected override cfacore.entity.dao._event.ResApplicationEntities CreateDataSource()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ResApplicationEntities"].ConnectionString;
            return new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
        }
    }
}
