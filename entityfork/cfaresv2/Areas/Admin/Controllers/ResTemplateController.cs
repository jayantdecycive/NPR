
#region Imports

using cfacore.domain.store;
using cfacore.shared.domain.store;
using cfares.domain._event;
using cfares.domain.store;
using cfares.repository._event;
using cfares.repository.store;
using cfaresv2.Areas.Admin.Controllers._base;
using System.Web.Mvc;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    public class ResTemplateController : CrudController<ResTemplateRepository, ResTemplate, string>
    {

        public override ResTemplateRepository GetRepository(cfares.entity.dbcontext.res_event.IResContext context)
        {
            return new ResTemplateRepository(context);
        }
    }
}
