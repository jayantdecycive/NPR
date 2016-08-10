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
using System.Web.Security;
using System.Collections;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace cfares.DataService
{
    [JSONPSupportBehavior]
    public class User : DataService<cfacore.entity.dao._event.ResApplicationEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            // config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            bool isAuthenticated = true;
            if (isAuthenticated)
            {
                config.UseVerboseErrors = true;
                //config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead);
                config.SetEntitySetAccessRule("Addresses", EntitySetRights.All);
                config.SetEntitySetAccessRule("Users", EntitySetRights.All);
                config.SetEntitySetAccessRule("User_Guide", EntitySetRights.All);
            }
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }


        #region validation

        [ChangeInterceptor("Users")]
        public void ValidateSlot(cfacore.entity.dao._event.User user, UpdateOperations operation)
        {
            if (operation != UpdateOperations.Delete)
            {
                user.Creation = user.Creation.ToUniversalTime();
                user.LastActivity = user.LastActivity.ToUniversalTime();
            }

            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.User), typeof(cfacore.domain.user.IUser)), typeof(cfacore.entity.dao._event.User));
            // Only validates on inserts and updates
            
            Validation.FireDataServiceValidation(user, operation);
        }

        [QueryInterceptor("Users")]        
        public Expression<Func<cfacore.entity.dao._event.User, bool>> OnQueryOrganisationUsers()
        {
            if (!Validation.IsFormsAuthorized())            
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return o => true;
        }
        [QueryInterceptor("User_Guide")]
        public Expression<Func<cfacore.entity.dao._event.User_Guide, bool>> OnQueryUser_Guide()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return o => true;
        }
        [QueryInterceptor("Addresses")]
        public Expression<Func<cfacore.entity.dao._event.Address, bool>> OnQueryAddress()
        {
            if (!Validation.IsFormsAuthorized())
                throw new System.Security.Authentication.AuthenticationException("Access Denied");
            return o => true;
        }

        /*
         * @gtg092x
         * DAISNAID: NOT DRY
         * */
        [ChangeInterceptor("Addresses")]
        public void ValidateSlot(cfacore.entity.dao._event.Address address, UpdateOperations operation)
        {
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Address), typeof(cfacore.domain.user.IAddress)), typeof(cfacore.entity.dao._event.Address));

            Validation.FireDataServiceValidation(address, operation);
        }
        

        #endregion

        protected override cfacore.entity.dao._event.ResApplicationEntities CreateDataSource()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ResApplicationEntities"].ConnectionString;
            return new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
        }
    }
}
