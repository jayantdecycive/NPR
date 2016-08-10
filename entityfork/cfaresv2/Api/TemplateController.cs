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
using cfaresv2.Api._base;
using cfares.domain._event;
using cfares.repository._event;

namespace cfaresv2.Api
{


    public class TemplateController : RepositoryEntitySetController<ResTemplateRepository,ResTemplate, string,ResTemplate>
    {
        protected override ResTemplateRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new ResTemplateRepository(context);
        }
    }
}
