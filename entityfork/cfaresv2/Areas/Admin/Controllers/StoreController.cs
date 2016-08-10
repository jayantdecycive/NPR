using System.Web.Mvc;
using cfacore.domain.store;
using cfacore.shared.domain.store;
using cfares.domain.store;
using cfares.entity.dbcontext.res_event;
using cfares.repository.store;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin" )]
    public class StoreController : CrudController<LocationRepository, ResStore, string>
    {




        public override LocationRepository GetRepository(IResContext context)
        {
            return new LocationRepository(context);
        }
    }
}
