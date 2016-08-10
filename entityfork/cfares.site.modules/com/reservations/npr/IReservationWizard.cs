
#region Imports

using cfacore.shared.modules.com.admin;
using cfares.domain._event;

#endregion

namespace cfares.site.modules.com.reservations.npr
{
	public interface IReservationWizard : IWizard<ITicket>
	{
        IReservationWizard Prime(ISlot slot);
        IReservationWizard Prime(IOccurrence occurrence);
        IReservationWizard Prime(IResEvent _event);
	}
}
