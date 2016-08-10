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

namespace cfares.DataService
{
    [JSONPSupportBehavior]
    public class Common : DataService<cfacore.entity.dao._event.ResApplicationEntities>
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
                config.SetEntitySetAccessRule("Media", EntitySetRights.All);
            }
            else {
                config.SetEntitySetAccessRule("Media", EntitySetRights.AllRead);
            }
            
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        #region validation

        [ChangeInterceptor("Media")]
        public void ValidateMedia(cfacore.entity.dao._event.Medium media, UpdateOperations operation)
        {
            if (operation != UpdateOperations.Delete)
            {
                media.CreationDate = media.CreationDate.ToUniversalTime();
            }
            TypeDescriptor.AddProviderTransparent(
            new AssociatedMetadataTypeTypeDescriptionProvider(typeof(cfacore.entity.dao._event.Medium), typeof(cfacore.shared.domain.media.IMedia)), typeof(cfacore.entity.dao._event.Medium));
            // Only validates on inserts and updates


            Validation.FireDataServiceValidation(media, operation);
        }
        

        #endregion


        protected override cfacore.entity.dao._event.ResApplicationEntities CreateDataSource()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ResApplicationEntities"].ConnectionString;
            return new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
        }
    }
}
