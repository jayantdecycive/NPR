using System.Web.Http.Controllers;
using cfacore.shared.domain.store;
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
using cfaresv2.Api._base;

namespace cfaresv2.Api
{


    public class MediaController : RepositoryEntitySetController<WebMediaRepository,Media, int,IMedia>
    {

        protected override WebMediaRepository GetRepository(IResContext context, HttpControllerContext httpContext)
        {
            return new WebMediaRepository(context);
        }

        
    }
}
