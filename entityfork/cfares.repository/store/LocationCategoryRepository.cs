
#region Imports

using System;
using System.Linq;
using cfacore.domain.store;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.domain.store;

#endregion

namespace cfares.repository.store
{
    public class LocationCategoryRepository : GenericRepository<IResContext, LocationCategory, int>
    {
        public LocationCategoryRepository(IResContext context) : base(context) { }

        

        public override LocationCategory FindBySlug(string slug)
        {
            return GetAll().FirstOrDefault(x => x.Name==slug);
        }

      
    }
}
