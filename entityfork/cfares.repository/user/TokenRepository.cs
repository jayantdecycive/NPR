using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfares.domain.communication;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;

namespace cfares.repository.user
{
    public class TokenRepository : GenericRepository<IResContext, ResToken, Guid,ResToken>
    {
        public override ResToken FindBySlug(string slug)
        {
            var g = new Guid(slug);
            return Find(x => x.TokenUID == g);
        }



        public TokenRepository(IResContext context)
            : base(context)
        {
        }
    }
}
