using cfacore.domain._base;
using core.synchronization.Automation;

namespace cfacore.domain.store
{
    [ITable]    
    public interface ILocationCategory : IDomainObject
    {
        string LocationCategoryId { get; set; }

		string Name { get; set; }
        string Description { get; set; }
    }
}
