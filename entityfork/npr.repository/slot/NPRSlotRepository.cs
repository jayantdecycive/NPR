using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository.slot;

namespace npr.repository.slot
{
    public class NPRSlotRepository : SlotRepository<Slot>
    {
        public NPRSlotRepository(IResContext context) : base(context) { }
    }
}
