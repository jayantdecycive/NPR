using System.Web.Http.Controllers;
using cfacore.shared.domain.store;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using cfares.repository.schedule;
using cfares.repository.store;
using cfacore.shared.domain.media;
using cfacore.shared.modules.repository;
using cfaresv2.Api._base;

namespace cfaresv2.Api
{


    public class ScheduleController : RepositoryEntitySetController<ScheduleRepository, Schedule, int>
    {

        protected override ScheduleRepository GetRepository(IResContext context, HttpControllerContext httpContext)
        {
            return new ScheduleRepository(context);
        }

        
    }
}
