using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using cfacom.entity.dao.Designer;
using DataServicesJSONP;
 
namespace cfacore.DataServices
{
    [JSONPSupportBehavior]    
    public class Stories : DataService<CfaComStoriesEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            
            config.UseVerboseErrors = true;
            config.SetEntitySetAccessRule("V2StoryImageAuthorWithTagRating", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("V2StoryWithImageAuthor", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("V2StoryWithImageAuthorQuality", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("V2StoryRatingSummary", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("V2StoryWithImageAuthorQualityTags", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("V2StoryTagSummary", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("StoryLocations", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("StoryLocation_Location", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Location_Story", EntitySetRights.AllRead);
            
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);            
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }

        protected override CfaComStoriesEntities CreateDataSource()
        {
            string connectionString;


            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CfaComStoriesEntities"].ConnectionString;
            return new CfaComStoriesEntities(connectionString);
        }
    }
}
