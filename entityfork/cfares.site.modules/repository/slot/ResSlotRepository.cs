
#region Imports

using System;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.repository.slot;
using cfares.site.modules.com.application;
using cfares.site.modules.user;

#endregion

namespace cfares.site.modules.repository.slot
{
    public class ResSlotRepository<TSlot> : SlotRepository<TSlot> where TSlot:class,ISlot,new()
	{
		#region Constructors

		public ResSlotRepository(IResContext context) : base(context) {}

		#endregion

		//private void ValidateDailyMinimumCapacity( ISlot slot, IResEvent e )
		//{
		//	// Special on-delete logic only present for "ChainwideProduct" at this time
		//	if( e.ReservationTypeId != "ChainwideProduct" ) return; 

		//	IResContext context = ReservationConfig.GetContext();
		//	UserMembershipRepository ur = new UserMembershipRepository( context );
		//	ResUser u = ur.GetUser();
		//	if( u == null ) throw new ApplicationException( "Unable to delete slot - " + 
		//		"User not currently logged in.  Please login and try again" );

		//	if( u.OperationRole != UserOperationRole.Admin && // Admin's can override this check
		//		! slot.IsMinimumDailyCapacityMet ) // Uses slot's start time as the daily range check epoch

		//		throw new ApplicationException( string.Format( "Unable to delete slot - " + 
		//			"The minimum daily capacity of {0} has not been met [ Current capacity for {2} = {1} ]",
		//			e.MinimumDailyCapacity, slot.CurrentDailyCapacity, slot.Start.ToDateStringWithDayOfWeek() ) );
		//}

		//#region OnSave / OnDelete Events

		//public override void OnSave(object sender, SavedEventArgs<TSlot> args)
		//{
		//	ISlot slot = args.Instance;
		//	IResEvent e = slot.Occurrence.ResEvent;
			
		//	ValidateDailyMinimumCapacity( slot, e );

		//	base.OnSave(sender, args);
		//}

		//public override void OnDelete(object sender, DeletedEventArgs<TSlot> args)
		//{
		//	ISlot slot = args.Instance;
		//	IResEvent e = slot.Occurrence.ResEvent;
			
		//	ValidateDailyMinimumCapacity( slot, e );

		//	base.OnDelete(sender, args);
		//}

		//#endregion
    }

	#region Non-Generic Subclass Helper

	public class ResSlotRepository : ResSlotRepository<Slot> {
        public ResSlotRepository(IResContext context) : base(context) { }
    }

	#endregion
}
