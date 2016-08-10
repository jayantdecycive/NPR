using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataServicesJSONP;
using System.Linq.Expressions;

namespace cfares.DataService
{
    [JSONPSupportBehavior]
    public class Ticket : DataService<cfacore.entity.dao._event.ResApplicationEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {


            
            config.SetEntitySetAccessRule("Tickets", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Ticket_Tour", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Ticket_Tour_Joined", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Ticket_Tour_Owner", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Ticket_Owner", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Ticket_Tour_Slot", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Ticket_Tour_Slot_Occurrence_Event", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Ticket_Tour_Owner_Slot_Guide", EntitySetRights.AllRead);
            
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        #region validation
        [QueryInterceptor("Ticket_Tour")]
        public Expression<Func<cfacore.entity.dao._event.Ticket_Tour, bool>> OnTicketTourQuery()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return (cfacore.entity.dao._event.Ticket_Tour p) => true;
            
        }

        [QueryInterceptor("Tickets")]
        public Expression<Func<cfacore.entity.dao._event.Ticket, bool>> OnTicketQuery()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return (cfacore.entity.dao._event.Ticket p) => true;

        }

        [QueryInterceptor("Ticket_Tour_Owner")]
        public Expression<Func<cfacore.entity.dao._event.Ticket_Tour_Owner, bool>> OnTicketTourOwnerQuery()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return (cfacore.entity.dao._event.Ticket_Tour_Owner p) => true;

        }

        [QueryInterceptor("Ticket_Tour_Slot")]
        public Expression<Func<cfacore.entity.dao._event.Ticket_Tour_Slot, bool>> OnTicketTourSlotQuery()
        {
            long userId = Validation.ResUserId();
            if (!Validation.IsFormsAuthorized())
                return (cfacore.entity.dao._event.Ticket_Tour_Slot p) => (p.OwnerId == userId);
            return (cfacore.entity.dao._event.Ticket_Tour_Slot p) => true;
        }

        [QueryInterceptor("Ticket_Tour_Slot_Occurrence_Event")]
        public Expression<Func<cfacore.entity.dao._event.Ticket_Tour_Slot_Occurrence_Event, bool>> OnTicketTourSlotOccurrenceEventQuery()
        {
            long userId = Validation.ResUserId();
            if (!Validation.IsFormsAuthorized())
                return (cfacore.entity.dao._event.Ticket_Tour_Slot_Occurrence_Event p) => (p.OwnerId == userId);
            return (cfacore.entity.dao._event.Ticket_Tour_Slot_Occurrence_Event p) => true;
        }

        [QueryInterceptor("Ticket_Tour_Owner_Slot_Guide")]
        public Expression<Func<cfacore.entity.dao._event.Ticket_Tour_Owner_Slot_Guide, bool>> OnTicketTourOwnerSlotGuideQuery()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return (cfacore.entity.dao._event.Ticket_Tour_Owner_Slot_Guide p) => true;

        }

        [QueryInterceptor("Ticket_Owner")]
        public Expression<Func<cfacore.entity.dao._event.Ticket_Owner, bool>> OnTicketOwnerQuery()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return (cfacore.entity.dao._event.Ticket_Owner p) => true;

        }

        [QueryInterceptor("Ticket_Tour_Joined")]
        public Expression<Func<cfacore.entity.dao._event.Ticket_Tour_Joined, bool>> OnTicketTourJoinedQuery()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return (cfacore.entity.dao._event.Ticket_Tour_Joined p) => true;

        }


        [ChangeInterceptor("Tickets")]
        public void ValidateTicket(cfacore.entity.dao._event.Ticket ticket, UpdateOperations operation)
        {
            

            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Ticket), typeof(cfares.domain._event.ticket.TicketMeta)), typeof(cfacore.entity.dao._event.Ticket));
            // Only validates on inserts and updates


            Validation.FireDataServiceValidation(ticket, operation);
        }

        [ChangeInterceptor("Ticket_Tour")]
        public void ValidateTourTicket(cfacore.entity.dao._event.Ticket_Tour ticket, UpdateOperations operation)
        {
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Ticket_Tour), typeof(cfares.domain._event.slot.ITourTicket)), typeof(cfacore.entity.dao._event.Ticket_Tour));

            Validation.FireDataServiceValidation(ticket, operation);
        }
        
        #endregion


        protected override cfacore.entity.dao._event.ResApplicationEntities CreateDataSource()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ResApplicationEntities"].ConnectionString;
            return new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
        }
    }
}
