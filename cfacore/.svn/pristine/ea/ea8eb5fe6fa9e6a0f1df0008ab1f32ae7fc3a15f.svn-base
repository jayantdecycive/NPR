//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using cfacom.entity.dao.Designer;

namespace cfacore.DataServices
{
    public class Food : DataService< CfaComStoriesEntities >
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("Foods", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("FoodImages", EntitySetRights.AllRead);
            
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }
      
    }
}
