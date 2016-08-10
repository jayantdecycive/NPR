using System.Data.Entity.Infrastructure;
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

using cfacore.shared.modules.repository;
using cfares.domain._event;
using cfares.repository._event;
using cfares.site.modules.user;
using cfaresv2.Api._base;
using System.Data.Entity;

namespace cfaresv2.Api
{


    public class OccurrenceController : RepositoryEntitySetController<OccurrenceRepository,Occurrence, int,IOccurrence>
    {
        protected override IQueryable<IOccurrence> GetAllQueryable(UserOperationRole role,ResUser owner)
        {
            var occurrences = repo.GetAll(new string[] { "Store","Store.StreetAddress","ResEvent" });

            return occurrences;
        }

        protected override IQueryable<IOccurrence> GetPublicQueryable()
        {
            return repo.GetAll(new string[] { "Store", "Store.StreetAddress", "ResEvent" });
        }

        protected override OccurrenceRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new OccurrenceRepository(context);
        }
    }
}
