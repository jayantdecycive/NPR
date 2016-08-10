
#region Imports

using cfacore.domain.store;
using cfacore.shared.domain.store;
using cfares.domain.store;
using cfares.repository.store;
using cfaresv2.Areas.Admin.Controllers._base;
using System.Web.Mvc;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    public class LocationCategoryController : CrudController<LocationCategoryRepository, LocationCategory, int>
    {

        public override LocationCategoryRepository GetRepository(cfares.entity.dbcontext.res_event.IResContext context)
        {
            return new LocationCategoryRepository(context);
        }
    }
}
