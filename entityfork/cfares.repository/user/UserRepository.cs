using System.Linq;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;

namespace cfares.repository.user
{
    public class UserRepository : GenericRepository<IResContext, ResUser, int>
    {
        public UserRepository(IResContext context) : base(context) { }

        public ResUser LoadByDatasource(string authority, string authorityUid)
        {
            return Find(x=>x.Authority==authority&&x.AuthorityUID==authorityUid);
        }

        public override IQueryable<ResUser> PublicQuerySet()
        {
            return base.PublicQuerySet().Where(x=>x.OperationRole!=UserOperationRole.None);
        }

        public override ResUser FindBySlug(string slug)
        {
            return Find(x=>x.Username==slug);
        }

        public ResUser FindByEmail(string email)
        {
            return Find(x=>x.Email==email);
        }
    }
}
