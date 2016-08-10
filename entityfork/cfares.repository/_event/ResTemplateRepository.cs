using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using System.Linq;

namespace cfares.repository._event
{
    public class ResTemplateRepository : GenericRepository<IResContext, ResTemplate, string>
    {
        public ResTemplateRepository(IResContext context) : base(context) { }

        public override IQueryable<ResTemplate> PublicQuerySet()
        {
            return GetAll();
        }

		public IQueryable<ResTemplate> GetByReservationType( string reservationTypeId )
		{
			return GetAll( "Preview" ).Where( o => o.DefaultReservationTypeId == reservationTypeId );
		}

        public override ResTemplate FindBySlug(string slug)
        {
            return Find(x => x.ResTemplateId == slug);
        }
    }
}
