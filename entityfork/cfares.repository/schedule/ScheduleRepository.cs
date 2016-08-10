using System;
using System.Linq;
using cfacore.shared.domain.common;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;

namespace cfares.repository.schedule
{
    public class ScheduleRepository : GenericRepository<IResContext, Schedule, int>
    {
        public override Schedule FindBySlug(string slug)
        {
            return Find(x => x.UrlName == slug);
        }



        public ScheduleRepository(IResContext context)
            : base(context)
        {
        }

        public Schedule SpansDate(DateTimeOffset dateTimeOffset)
        {
            return this.Find(x => x.Start <= dateTimeOffset && x.End >= dateTimeOffset);
        }
    }
}
