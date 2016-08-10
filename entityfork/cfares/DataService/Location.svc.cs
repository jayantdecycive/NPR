using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using cfares.site.modules.com.datatables;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataServicesJSONP;
using System.ServiceModel;
using System.Threading;

namespace cfares.DataService
{
    //[DataTableRequestHandler]
    
    public class Location : DataService<cfacore.entity.dao._event.ResApplicationEntities>
    {
        // This method is called only once to initialize service-wide policies.
        
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            //config.SetEntitySetAccessRule("Locations", EntitySetRights.AllRead);
            bool isAuthenticated = true;
            if (isAuthenticated)
            {
                config.UseVerboseErrors = true;
                config.SetEntitySetAccessRule("Locations", EntitySetRights.All);
                config.SetEntitySetAccessRule("Addresses", EntitySetRights.All);
                config.SetEntitySetAccessRule("Users", EntitySetRights.AllRead);
            }else{
                config.SetEntitySetAccessRule("Locations", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("Addresses", EntitySetRights.AllRead);
            }
                
                    
            config.SetEntitySetAccessRule("Location_StreetAddress", EntitySetRights.AllRead);
                //config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead);
                
                
                config.SetEntitySetAccessRule("Distributors", EntitySetRights.AllRead);
            
            
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        

        #region validation

        [ChangeInterceptor("Locations")]
        public void ValidateLocations(cfacore.entity.dao._event.Location store, UpdateOperations operation)
        {
            if (operation != UpdateOperations.Delete)
            {
                store.ProjectedOpenDate = store.ProjectedOpenDate.ToUniversalTime();
                store.OpenDate = store.OpenDate.ToUniversalTime();
            }
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Location), typeof(cfacore.domain.store.IStore)), typeof(cfacore.entity.dao._event.Location));
            // Only validates on inserts and updates


            Validation.FireDataServiceValidation(store, operation);
        }

        [ChangeInterceptor("Addresses")]
        public void ValidateAddresses(cfacore.entity.dao._event.Address address, UpdateOperations operation)
        {
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Address), typeof(cfacore.shared.domain.user.AddressMeta)), typeof(cfacore.entity.dao._event.Address));

            Validation.FireDataServiceValidation(address, operation);
        }

        [ChangeInterceptor("Distributors")]
        public void ValidateDistributors(cfacore.entity.dao._event.Distributor distributor, UpdateOperations operation)
        {
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Distributor), typeof(cfacore.shared.domain.store.IDistributor)), typeof(cfacore.entity.dao._event.Distributor));

            Validation.FireDataServiceValidation(distributor, operation);
        }



        #endregion

        protected override cfacore.entity.dao._event.ResApplicationEntities CreateDataSource()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ResApplicationEntities"].ConnectionString;
            return new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
        }
    }
}
