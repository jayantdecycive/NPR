using System.Collections.ObjectModel;
using System.Web.Http.OData.Query.Validators;
using cfacore.domain.store;
using cfacore.shared.domain.store;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using cfares.repository.store;
using cfacore.shared.domain.media;
using cfacore.shared.modules.repository;
using cfares.domain._event;
using cfares.repository.slot;
using cfares.repository.ticket;
using cfares.site.modules.user;
using cfaresv2.Api._base;
using System.Data.Entity;

namespace cfaresv2.Api
{


    public class LocationCategoryController : RepositoryEntitySetController<LocationCategoryRepository, LocationCategory, int>
    {

        protected override LocationCategoryRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new LocationCategoryRepository(context);
        }

      

        
    }
}
