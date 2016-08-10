using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfaresv2.Areas.Admin.Controllers._base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cfaresv2.Areas.Admin.Controllers
{
    public class TemplateController : ExtendableCrudController<ResTemplateRepository, ResTemplate, string, ResTemplate>
    {

        public override ResTemplateRepository GetRepository(IResContext context)
        {
            return new ResTemplateRepository(context);
        }
    }
}
