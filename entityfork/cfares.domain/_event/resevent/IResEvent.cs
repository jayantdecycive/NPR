
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using cfacore.domain._base;
using cfares.domain._event.resevent;
using cfares.domain.store;
using cfacore.shared.domain._base;
using Ninject;

#endregion

#region Pragmas

// ReSharper disable CheckNamespace

#endregion

namespace cfares.domain._event
{
    public interface IResEventBound
    {
        IResEvent GetEvent();

        bool HasEvent();
    }

    // TODO Create meta resevent for data annotations
    public interface IResEvent : IDomainObject
    {
        /*
         * The associated metadata type for type 'cfacore.entity.dao._event.ResEvent'
         * contains the following unknown properties or fields: Occurances, Slots,
         * RegistrationAvailability, SiteAvailability, ProtoOccurrence,
         * ParticipatingStores, UriBase, Checksum, Uri, Bound, Id, StrUri. Please 
         * make sure that the names of these members match the names of the
         * properties on the main type.
         * 
         * */
        bool HasTickets();
        IEnumerable<Ticket> GetTickets();
        int TotalTickets();
        bool HasTickets(Func<Ticket, bool> exp);
        IEnumerable<Ticket> GetTickets(Func<Ticket, bool> exp);
        int TotalTickets(Func<Ticket, bool> exp);
	    bool FullyBooked { get; }

        IOccurrence GetOccurrence(Func<IOccurrence, bool> predicate);
        IList<Occurrence> Occurrences { get; set; }
        bool SlotIsAvailable(DateTime when);
        bool HasStores();
        Slot SlotAt(DateTime when);
        IKernel GetKernel();

        IEnumerable<object> ParticipatingStoresListViewModel { get; }

        DateTime RegistrationStart { get; set; }
        DateTime RegistrationEnd { get; set; }
        
        string RegistrationStartString { get; set; }
        string RegistrationEndString { get; set; }
        
        void SetSiteAvailability(DateRange range);
        void SetRegistrationAvailability(DateRange range);

        DateTime SiteStart { get; set; }
        DateTime SiteEnd { get; set; }
        string SiteStartString { get; set; }
        string SiteEndString { get; set; }

	    bool IsEventEnded { get; }

	    string Name { get; set; }
        string SubHeading { get; set; }
        string Description { get; set; }

        ICollection<ResSiteUrl> SiteUrls { get; set; }
        string Urls { get; set; }
        string Url { get; }
        

        ResEventStatus Status { get; set; }
        ResEventVisibility Visibility { get; set; }

	    IEnumerable<ResStore> ParticipatingStoresList { get;  }
        IEnumerable<ResStore> ParticipatingStoresListLive { get; }
        IQueryable<Occurrence> ActiveOccurrences { get; }

		string TemplateId { get; set; }

		string OrganizationName { get; set; }
	    string OrganizationDisplayName { get; set; }

	    IOccurrence ProtoOccurrence { get; }

        ReservationType ReservationType { get; set; }
        string ReservationTypeId { get; set; }

        bool MustBeOfAgeToAttend { get; set; }
        int LocationCount { get; set; }

        ReservationCategory Category { get; set; }
        int? CategoryId { get; set; }

        ResTemplate Template { get; set; }
       
        int ResEventId { get; set; }

	    IEnumerable<Slot> AvailableSlots { get; }
	    IEnumerable<Slot> AvailableSlotsByCurrentDateTime { get; }

        bool IsTemp();

        IOccurrence CreateOrDefaultOccurrenceByStoreId(string store);

        DateRange GetRegistrationAvailability();

        bool IsEventUpcoming { get; }
        bool IsFeatured { get; set; }
		bool IsPaid { get; set; }
		decimal? TicketAmount { get; set; }

        int? MaximumCapacity { get; set; }
	    int MinimumDailyCapacity { get; set; }
	    int TotalCapacity { get; }
	    ResEvent.DailyCapacityResult LowestDailyCapacity { get; }
		ResEvent.DailyCapacityResult GetLowestDailyCapacityByOccurrenceId( int occurrenceId );
		ResEvent.DailyCapacityResult GetLowestDailyCapacityByOccurrenceId( IList<Slot> slotList  );
	    void ValidateMinimumDailyCapacity();

	    Type TicketType { get; }
	    Type SlotType { get; }
    }
}

#region Pragmas

// ReSharper restore CheckNamespace

#endregion
