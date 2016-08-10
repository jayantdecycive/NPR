using cfacore.domain._base;
using core.synchronization.Automation;

namespace cfares.domain._event
{
    [ITable]    
    public interface IReservationCategory : IDomainObject
    {
        int ReservationCategoryId { get; set; }

		string Name { get; set; }
        string Description { get; set; }
    }
}
