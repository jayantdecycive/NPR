using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfares.domain._event;
using cfares.domain._event.resevent;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;

namespace cfares.repository._event
{
    public class ReservationCategoryRepository : GenericRepository<IResContext, ReservationCategory, int>
    {
        public ReservationCategoryRepository():base()
        {
        }

        public ReservationCategoryRepository(IResContext context)
            : base(context)
        {
        }

        public override ReservationCategory FindBySlug(string slug)
        {
            throw new NotImplementedException();
        }
    }
}
