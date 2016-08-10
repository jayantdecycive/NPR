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
    public class ResSiteUrlRepository : GenericRepository<IResContext, ResSiteUrl, int,IResSiteUrl>
    {
        public ResSiteUrlRepository():base()
        {
        }

        public ResSiteUrlRepository(IResContext context)
            : base(context)
        {
        }

        public override IResSiteUrl FindBySlug(string slug)
        {
            throw new NotImplementedException();
        }
    }
}
