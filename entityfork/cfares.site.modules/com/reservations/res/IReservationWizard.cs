
#region Imports

using System.Web.Routing;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;

#endregion

namespace cfares.site.modules.com.reservations.res
{
	public interface IReservationWizard : IWizard
	{
        int? SlotId { get;  }
        bool? IsFavorite { get; }
        int? OccurrenceId { get; }
        void ApplyContext(RequestContext context);
        IReservationWizard Prime(ISlot slot);
        IReservationWizard Prime(IOccurrence occurrence);
        IReservationWizard Prime(IResEvent _event);
	}

    public interface IReservationWizard<TModel> : IReservationWizard
    {
        IWizard<TModel> Prime(TModel model);        
        /*int? SlotId { get; }
        int? OccurrenceId { get; }
        void ApplyContext(RequestContext context);
        IReservationWizard Prime(ISlot slot);
        IReservationWizard Prime(IOccurrence occurrence);
        IReservationWizard Prime(IResEvent _event);*/
    }
}
