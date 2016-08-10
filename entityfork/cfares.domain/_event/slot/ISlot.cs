using System;
using System.Collections.Generic;
using cfacore.domain._base;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;
using cfares.domain._event.slot;

// ReSharper disable CheckNamespace
namespace cfares.domain._event
{
    public interface ISlot : IDomainObject
    {
        [System.Data.Linq.Mapping.Association(Name = "FK_Occurrence", ThisKey = "OccurrenceId", OtherKey = "OccurrenceId")]
        [UIHint("Badge/_Occurrence")]
        [ClientDefault(1,type=typeof(string))]
        Occurrence Occurrence { get; set; }

        [UIHint("Badge/_Schedule")]
        [ClientDefault(1, type = typeof(string))]
        Schedule Schedule { get; set; }

        int? ScheduleId { get; set; }

        [UIHint("Tables/_Tickets")]
        IList<Ticket> Tickets { get; set; }

        int? OccurrenceId { get; set; }

        /*
         * The associated metadata type for type 'cfacore.entity.dao._event.Slot'
         * contains the following unknown properties or fields: TicketsAvailable,
         * Availability. Please make sure that the names of these members match
         * the names of the properties on the main type.
         * 
         * ****TicketsAvailable and Availability are commented out
         * */

        void SetAdditionalData(Dictionary<string,object> data);

        SlotGrouping Grouping { get; set; }

        int TicketsAvailable { get; }

	    SlotVisibility Visibility { get; set; }

        //[DataType("jqui/_DateRange")]
        //IDateRange Availability { get; set; }
        [Column]
        [DataType("jqui/_DatePicker")]
        DateTimeOffset Start { get; set; }
        string StartOffsetString { get; set; }

        [Column]
        [DataType("jqui/_DatePicker")]
        DateTimeOffset End { get; set; }
        string EndOffsetString { get; set; }

        [Column]
        bool IsScheduled { get; set; }

        [Column]
        [DataType("jqui/_DatePicker")]
        DateTimeOffset Cutoff { get; set; }
        
        [Column]
        [DataType("Enum/_SlotStatus")]
        [ClientDefault("0")]
        SlotStatus Status { get; set; }

        [Column]
        [Required]        
        int Capacity { get; set; }
        
        /*
         * Should default to "Now"
         * */
        bool IsAvailable();

        int SlotId { get;set; }

        void SetOccurrence(IOccurrence occurrence);

        string Notes { get; set; }

		int CurrentDailyCapacity { get; }
	    bool IsMinimumDailyCapacityMet { get; }
		int CurrentDailyCapacityExcludingThisSlot { get; }
	    bool IsMinimumDailyCapacityMetExcludingThisSlot { get; }

	    void SetOffset(TimeZoneInfo timeZoneInfo);

        bool AreTicketsAvailable(int? except=0);

        void InvalidateCache();
    }
    public enum SlotStatus
    {        
        Active,
        Hold,
        Hidden,
        Archive
    }
    public enum SlotGrouping { 
        Date,
        Day,
        All
    }
}
// ReSharper restore CheckNamespace
