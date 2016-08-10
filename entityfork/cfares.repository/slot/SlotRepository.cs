
#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfacore.shared.domain.user;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.domain._event._ticket;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.domain.aggregates;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Data.Entity;
using cfares.domain._event.slot.tours;
using cfares.domain._event.slot;
using cfares.repository._event;

#endregion

namespace cfares.repository.slot
{
	#region Extensions and ISlotRepository

	public static class SlotQueryExtensions
    {
        public static IQueryable<ISlot> WhereDayOfWeek(this IQueryable<ISlot> slots, DayOfWeek dayOfWeek)
        {
            throw new NotImplementedException("This does not behave as expected");
			//var d = (int)dayOfWeek;
			//return slots.Where(x => SqlFunctions.DatePart("dw", x.Start) == d);
        }
    }

	public class GiveawaySlotRepository : SlotRepository<GiveawaySlot>
    {
        public GiveawaySlotRepository(IResContext context) : base(context) { }
    }

    public class TeamTourSlotRepository : SlotRepository<TeamTourSlot>
    {
        public TeamTourSlotRepository(IResContext context) : base(context) { }
    }

    public class TourSlotRepository : SlotRepository<TourSlot>
    {
        public TourSlotRepository(IResContext context) : base(context) { }
    }

    public class SlotRepository : SlotRepository<Slot>
    {
        public SlotRepository(IResContext context) : base(context) { }

        public ISlot FindOrRemember(int slotId)
        {
            return Find(slotId);
        }
    }

    public interface ISlotRepository
    {
        ISlot FindBySlug(string slug);
        
        ISlot Find(Int32 key);
        ISlot Find(Expression<Func<ISlot, bool>> keySelector, string include);
        ISlot Find(Expression<Func<ISlot, bool>> keySelector, string[] include);
        IQueryable<ISlot> GetAll();
        IQueryable<ISlot> GetAll(string with);
        IQueryable<ISlot> GetAll(string[] with);
        IQueryable<ISlot> Get(Expression<Func<ISlot, bool>> predicate);
        IQueryable<ISlot> Get(Expression<Func<ISlot, bool>> predicate,string with);
        IQueryable<ISlot> Get(Expression<Func<ISlot, bool>> predicate, string[] with);
        ISlot Find(Expression<Func<ISlot, bool>> predicate);
        void Add(ISlot entity);
        void Delete(ISlot entity);
        bool Save(ISlot entity);
        void Edit(ISlot entity);
        void Cache(ISlot entity);
        void Forget(ISlot entity);
    }

	#endregion

	public class SlotRepository<TSlot> : GenericRepository<IResContext, TSlot, int,ISlot>, ISlotRepository where TSlot:class,ISlot
    {
	    public SlotRepository() {}
        public SlotRepository(IResContext context) : base(context) {}

        public override ISlot FindBySlug(string slug)
        {            
            throw new NotImplementedException();
        }

        public IQueryable<TSlot> GetByAggregate(SlotDayAggregate first)
        {
            throw new NotImplementedException();
        }

        public override void OnDelete(object sender, DeletedEventArgs<TSlot> args)
        {
            //carson: invalidate slot ticket cache when saving the slot
            if (args.Instance.Occurrence != null)
                args.Instance.Occurrence.InvalidateCache();
            base.OnDelete(sender, args);
        }

        public override void Add(ISlot entity)
        {
            if (entity.Occurrence != null)
                entity.Occurrence.InvalidateCache();
            base.Add(entity);
        }

        public override void OnSave(object sender, SavedEventArgs<TSlot> args)
        {
            if(!args.Created){
                if (args.Instance.Tickets != null && 
					args.Instance.TicketsAvailable > 0 && 
					args.Instance.Capacity < args.Instance.TicketsAvailable)
                {
                    throw new Exception(string.Format(
						"This slot currently has tickets! You cannot save any capacity lower than {0}.",
                        (args.Instance.Capacity-args.Instance.TicketsAvailable)));
                }
            }
            if (args.Instance.IsScheduled && args.Instance.ScheduleId == null)
            {
                /*var scheduleRepo = new ScheduleRepository(this.Context);
                var intersectsWithDate = scheduleRepo.SpansDate(args.Instance.Start);
                if (intersectsWithDate == null)
                {
                    
                }*/
            }

            //carson: invalidate slot ticket cache when saving the slot
            if (args.Instance.Occurrence != null)
                args.Instance.Occurrence.InvalidateCache();

            base.OnSave(sender, args);
        }

		//public override void OnLoad(object sender, LoadedEventArgs<TSlot> args)
		//{
		//	/*
		//	 * MDRAKE: By default, the slots will load in with Date being their grouping
		//	 * If you want a different grouping, you must set that before you save them
		//	 * */
		//	//if(args.Instance!=null)
		//	//args.Instance.Grouping = SlotGrouping.Date;
		//	base.OnLoad(sender, args);
		//}

        public void AddAggregate(IResEvent resEvent, SlotGrouping mode, SlotDayAggregate value, DateTime cutoff, TimeSpan cutoffDistance)
        {
            //get time range for slots       
            var repo = new OccurrenceRepository(Context);
            IList<IOccurrence> occurrences = repo.Get(x => x.ResEventId == resEvent.ResEventId).Include("Store").ToList();
            foreach(var occurrence in occurrences){
                AddAggregate(occurrence,mode,value,cutoff,cutoffDistance);
            }
            
            
        }

        public void AddAggregate(IOccurrence occurrence, SlotGrouping mode, SlotDayAggregate value, DateTime cutoff, TimeSpan cutoffDistance)
        {            
            var start = occurrence.ResEvent.RegistrationStart;
            var end = occurrence.ResEvent.RegistrationEnd;
            var timeSpan = end - start;
            var iterator = timeSpan.TotalDays;

            if (mode == SlotGrouping.Date)
            {
                AddSlotFromAggregate(occurrence, value.GetStartTime().Date, value, cutoff, cutoffDistance, mode);
            }
            else
            {

                for (int i = 0; i < iterator; i++)
                {
                    //this is just the new date
                    var newDate = start.AddDays(i);

                    //continue if assumptions are not met
                    if (mode == SlotGrouping.Day)
                    {
                        //if day of week doesn't match day agg
                        if (newDate.DayOfWeek != (DayOfWeek)(value.StartDay+1))
                            continue;
                    }
                    else if (mode == SlotGrouping.All)
                    {
	                    //if day of week is weekend for all agg (MD: They may expect Saturday)
                        if (value.TemplateDates != null && value.TemplateDates.Count > 0 && value.TemplateDates.All(x =>
                            x.Day != newDate.Day || x.Month != newDate.Month || x.Year != newDate.Year))
                        {
                            continue;
                        }
	                    if (newDate.DayOfWeek == DayOfWeek.Saturday || newDate.DayOfWeek == DayOfWeek.Sunday)
		                    continue;
                    }
                    else
                    {
                        //if date doesn't match date to do (you might as well make a new slot without the agg here)
                        if (newDate.Day != value.StartDay || newDate.Month != value.StartMonth || newDate.Year != value.StartYear)
                            continue;
                    }

                    
                    //Apply timezone
                    AddSlotFromAggregate(occurrence, newDate, value, cutoff, cutoffDistance, mode);
                    //End Apply Timezone

                }
            }
            Commit();

        }

        protected ISlot AddSlotFromAggregate(IOccurrence occurrence, DateTime newDate, SlotDayAggregate value, DateTime cutoff, TimeSpan cutoffDistance, SlotGrouping mode)
        {
            ISlot newSlot = occurrence.CreateSlot();

            newSlot.SetAdditionalData(value.AdditionalData);
            
            var Start = new DateTime(newDate.Year, newDate.Month, newDate.Day, value.StartHour, value.StartMinute, 0);
            var End = new DateTime(newDate.Year, newDate.Month, newDate.Day, value.EndHour, value.EndMinute, 0);
            newSlot.Start = new DateTimeOffset(Start, occurrence.Store.GMTOffsetTimeZoneInfo.GetUtcOffset(Start));
            newSlot.End = new DateTimeOffset(End, occurrence.Store.GMTOffsetTimeZoneInfo.GetUtcOffset(End));
            newSlot.Capacity = value.Capacity;
            var computedCutoff = ComputeCutoff(newSlot.Start, cutoff, cutoffDistance);


            newSlot.Cutoff = new DateTimeOffset(computedCutoff, occurrence.Store.GMTOffsetTimeZoneInfo.GetUtcOffset(computedCutoff));
            newSlot.Grouping = mode;
            Add(newSlot as TSlot);
            return newSlot;
        }
        
        public Expression<Func<TSlot, SlotDayAggregate>> GetDayAggregateExpression() {
            DateTime firstSunday = new DateTime(1989, 1, 1);
            
            return( x => new SlotDayAggregate
            {
                StartDay = ((int)(EntityFunctions.DiffDays(firstSunday, x.Start) % 7)),
                StartYear = 1989,
                StartMonth = 1,
                StartHour = x.Start.Hour,
                StartMinute = x.Start.Minute,
                EndDay = (int)(EntityFunctions.DiffDays(firstSunday, x.End) % 7),
                EndYear = 1989,
                EndMonth = 1,
                EndHour = x.End.Hour,
                EndMinute = x.End.Minute,
                Capacity = x.Capacity,
            });
        }

        public Expression<Func<TSlot, SlotDayAggregate>> GetDateAggregateExpression()
        {
            return (x => new SlotDayAggregate
            {
                StartDay = x.Start.Day,
                StartYear = x.Start.Year,
                StartMonth = x.Start.Month,
                StartHour = x.Start.Hour,
                StartMinute = x.Start.Minute,
                EndDay = x.End.Day,
                EndYear = x.End.Year,
                EndMonth = x.End.Month,
                EndHour = x.End.Hour,
                EndMinute = x.End.Minute,
                Capacity = x.Capacity,
            });
        }

        public Expression<Func<TSlot, SlotDayAggregate>> GetAllAggregateExpression()
        {
            return (x => new SlotDayAggregate
            {
                StartDay = 0,
                StartYear = 0,
                StartMonth = 1,
                StartHour = x.Start.Hour,
                StartMinute = x.Start.Minute,
                EndDay = 0,
                EndYear = 0,
                EndMonth = 1,
                EndHour = x.End.Hour,
                EndMinute = x.End.Minute,
                Capacity = x.Capacity,
            });
        }

        public Expression<Func<TSlot, SlotDayAggregate>> GetAggregateExpression(SlotGrouping mode) { 
            switch(mode){
                case SlotGrouping.All:
                    return GetAllAggregateExpression();
                    
                case SlotGrouping.Day:
                    return GetDayAggregateExpression();
                    
                //case SlotGrouping.Date:
                default:
                    return GetDateAggregateExpression();
                    
            }        
        }

        public IQueryable<IGrouping<SlotDayAggregate, TSlot>> GetAggregates(SlotGrouping mode, IResEvent resEvent)
        {
            return GetAggregates(mode, Get(x => x.Occurrence.ResEventId == resEvent.ResEventId).Cast<TSlot>());
        }

        public IQueryable<IGrouping<SlotDayAggregate, TSlot>> GetAggregates(SlotGrouping mode, IOccurrence occurrence)
        {
            return GetAggregates(mode, Get(x => x.OccurrenceId == occurrence.OccurrenceId).Cast<TSlot>());
        }

        public IQueryable<IGrouping<SlotDayAggregate, TSlot>> GetAggregates(SlotGrouping mode,IQueryable<TSlot> dbQuery,bool nowhere)
        {
            switch( mode )
            {
                case SlotGrouping.Day:
                    return dbQuery.Include("Occurrence.Store")
						.GroupBy( GetDayAggregateExpression() );

                case SlotGrouping.All:
                    return dbQuery.GroupBy( GetAllAggregateExpression() );

                default:
                    return dbQuery.Include("Occurrence.Store")
						.GroupBy( GetDateAggregateExpression() );
            }
        }

        public IQueryable<IGrouping<SlotDayAggregate, TSlot>> GetAggregates(SlotGrouping mode)
        {
            return GetAggregates(mode, GetAll().Cast<TSlot>());
        }

        public IQueryable<IGrouping<SlotDayAggregate, TSlot>> GetAggregates(SlotGrouping mode, IQueryable<TSlot> dbQuery)
        {
            switch (mode)
            {

                case SlotGrouping.Day:
                    var dayGroup = dbQuery.Where(x => x.Grouping == SlotGrouping.Day).Include("Occurrence.Store").GroupBy(GetDayAggregateExpression());
                    return dayGroup;
                case SlotGrouping.All:
                    var allGroup = dbQuery.Where(x => x.Grouping == SlotGrouping.All).GroupBy(GetAllAggregateExpression());
                    return allGroup;
                default:
                    var dateGroup = dbQuery.Include("Occurrence.Store").Where(x => x.Grouping == SlotGrouping.Date).GroupBy(GetDateAggregateExpression());
                    return dateGroup;
            }
        }

        public void UpdateAggregate(IResEvent resEvent, SlotGrouping mode, SlotDayAggregate oldValue,  SlotDayAggregate value, DateTime cutoff,TimeSpan cutoffDistance)
        {
            var Groups = FilterAggregate(GetAggregates(mode, resEvent), oldValue,mode);
            ApplyAggregate(Groups, value, cutoff,cutoffDistance,mode);

        }

        public void UpdateAggregate(IOccurrence occurrence, SlotGrouping mode, SlotDayAggregate oldValue, SlotDayAggregate value, DateTime cutoff,TimeSpan cutoffDistance)
        {
            var Groups = FilterAggregate(GetAggregates(mode, occurrence),oldValue,mode);
            ApplyAggregate(Groups,value,cutoff,cutoffDistance,mode);
        }

        protected IQueryable<IGrouping<SlotDayAggregate, TSlot>> FilterAggregate(IQueryable<IGrouping<SlotDayAggregate, TSlot>> Groups, SlotDayAggregate value, SlotGrouping grouping)
        { 
            return Groups.Where(AggregateExpression(value,grouping));
        }

        protected void ApplyAggregate(IQueryable<IGrouping<SlotDayAggregate, TSlot>> Groups, SlotDayAggregate value, DateTime cutoff,TimeSpan cutoffDistance,SlotGrouping mode)
        {
            foreach (var group in Groups.ToList())
            {
                foreach (var slot in group.ToList())
                {
                    slot.Capacity = value.Capacity;
                    if (mode == SlotGrouping.Date)
                    {
                        slot.Start = value.GetStartTime();
                        slot.End = value.GetEndTime();
                    }
                    else
                    {
                        slot.Start = value.GetStartTime(slot.Start);
                        slot.End = value.GetEndTime(slot.End);
                    }
                    var computedCutoff = ComputeCutoff(slot.Start, cutoff, cutoffDistance);
                    slot.Cutoff = new DateTimeOffset(computedCutoff, slot.Occurrence.Store.GMTOffsetTimeZoneInfo.GetUtcOffset(computedCutoff));
                    slot.Grouping = mode;
                    //Edit(slot);
                }
            }
            Commit();
        }

        private DateTime ComputeCutoff(DateTimeOffset dateTimeOffset, DateTime cutoff, TimeSpan cutoffDistance)
        {
            var computedCutoff = dateTimeOffset + cutoffDistance;
            return new DateTime(computedCutoff.Year, computedCutoff.Month, computedCutoff.Day, cutoff.Hour,cutoff.Minute,0);
        }

        public void AddAggregate(IOccurrence occurrence, SlotDayAggregateEntry[] SaveAggregates, DateTime cutoff,TimeSpan cutoffDistance)
        {
            foreach (var aggregateDate in SaveAggregates)
            {
                foreach (var aggregate in aggregateDate.Aggregates)
                {
                    if (aggregate.OldValue == null)
                    {
                        AddAggregate(occurrence, aggregateDate.Grouping, aggregate.NewValue, cutoff,cutoffDistance);
                    }
                    else if (!aggregate.Unchanged())
                    {
                        UpdateAggregate(occurrence, aggregateDate.Grouping, aggregate.OldValue, aggregate.NewValue, cutoff, cutoffDistance);
                    }
                }
            }
        }

        public void AddAggregate(IResEvent resEvent, SlotDayAggregateEntry[] SaveAggregates, DateTime cutoff,TimeSpan cutoffDistance)
        {
            foreach(var aggregateDate in SaveAggregates){
                foreach(var aggregate in aggregateDate.Aggregates){
                    if (aggregate.OldValue == null)
                    {
                        AddAggregate(resEvent, aggregateDate.Grouping, aggregate.NewValue, cutoff, cutoffDistance);
                    }
                    else if (!aggregate.Unchanged())
                    {
                        UpdateAggregate(resEvent, aggregateDate.Grouping, aggregate.OldValue, aggregate.NewValue, cutoff,cutoffDistance);
                    }
                }
            }
        }

        public void RemoveAggregate(IResEvent resEvent, SlotDayAggregateEntry[] DeleteAggregates)
        {
            foreach (var aggregateDate in DeleteAggregates)
            {
                foreach (var aggregate in aggregateDate.Aggregates)
                {
                    RemoveAggregate(resEvent, aggregateDate.Grouping, aggregate.OldValue);
                }
            }
        }

        public void RemoveAggregate(IOccurrence occurrence, SlotDayAggregateEntry[] DeleteAggregates)
        {
            foreach (var aggregateDate in DeleteAggregates)
            {
                foreach (var aggregate in aggregateDate.Aggregates)
                {

                    RemoveAggregate(occurrence, aggregateDate.Grouping, aggregate.OldValue);

                }
            }
        }

        private Expression<Func<IGrouping<SlotDayAggregate, TSlot>, bool>> AggregateExpression(SlotDayAggregate slotDayAggregate,SlotGrouping grouping)
        {
            switch (grouping)
            {
                case SlotGrouping.All:
                case SlotGrouping.Day:
                    return x => (x.Key.Capacity == slotDayAggregate.Capacity
                         && x.Key.EndHour == slotDayAggregate.EndHour
                         && x.Key.EndMinute == slotDayAggregate.EndMinute
                         && x.Key.StartHour == slotDayAggregate.StartHour
                         && x.Key.StartMinute == slotDayAggregate.StartMinute);
                default:
                //case SlotGrouping.Date:
                    return x => (x.Key.Capacity == slotDayAggregate.Capacity
                         && x.Key.EndHour == slotDayAggregate.EndHour
                         && x.Key.EndMinute == slotDayAggregate.EndMinute 
                         && x.Key.StartHour == slotDayAggregate.StartHour
                         && x.Key.StartMinute == slotDayAggregate.StartMinute
                         && x.Key.EndDay == slotDayAggregate.EndDay
                         && x.Key.StartDay == slotDayAggregate.StartDay 
                         && x.Key.EndMonth == slotDayAggregate.EndMonth
                         && x.Key.EndYear == slotDayAggregate.EndYear
                         && x.Key.StartMonth == slotDayAggregate.StartMonth 
                         && x.Key.StartYear == slotDayAggregate.StartYear);
            }
            
        }

        private void RemoveAggregate(IOccurrence occurrence, SlotGrouping mode, SlotDayAggregate slotDayAggregate)
        {
            var aggs = GetAggregates(mode, occurrence).Where(AggregateExpression(slotDayAggregate, mode)).ToList();
            foreach (var slotGroup in aggs)
            {
                foreach (var slot in slotGroup)
                    Delete(slot);
            }
        }

        private void RemoveAggregate(IResEvent resEvent, SlotGrouping mode, SlotDayAggregate slotDayAggregate)
        {
            var aggs = GetAggregates(mode, resEvent).Where(AggregateExpression(slotDayAggregate,mode)).ToList();
            foreach(var slotGroup in aggs){
                //Console.Write(slotGroup);
                foreach (var slot in slotGroup)
                    Delete(slot);
            }
        }

        public string ToiVisitorCSV(int slotId, string delimiter, bool includeHeaders, bool isShort)
        {
	        TSlot slot = Find(x=>x.SlotId==slotId) as TSlot;

            List<string> rows = new List<string>();
            List<string> headercols = new List<string>();

            headercols.Add("Visitor First Name");
            headercols.Add("Visitor Last Name");
            headercols.Add("Company Name");
            if (!isShort)
            {
                headercols.Add("Purpose");
                headercols.Add("Instructions Upon Arrival");
                headercols.Add("Unique Employee ID");
                headercols.Add("Scheduled Time In");
                headercols.Add("Scheduled Time Out");
                if(slot is IBadgeSlot)
					headercols.Add("Print Badge");
            }

            if (includeHeaders)
                rows.Add(string.Join(delimiter, headercols.ToArray()));

			if( slot != null )
				foreach (IGroupTicket t in slot.Tickets.Where(t => t.Status != TicketStatus.Canceled).Cast<IGroupTicket>())
				{
					foreach (Name guestName in t.AllGuestNames)
					{
						Hashtable dataTable = new Hashtable();
						foreach (string headercol in headercols)
							dataTable.Add(headercol, "");

						dataTable["Visitor First Name"] = (guestName.First);
						dataTable["Visitor Last Name"] = (guestName.Last);

						dataTable["Company Name"] = (t.GroupName);
						if (!isShort)
						{
							dataTable["Purpose"] = "NPR Tours";
							dataTable["Instructions Upon Arrival"] = "";

							dataTable["Unique Employee ID"] = "6497856";

							// Was previously using .LocalDateTime, however when uploaded to iVisitor
							// .. Results where 4 hours off ( so iVisitor appears to want the original
							// .. date/time offset time ) .. Since we're not sure at this time if iVisitor
							// .. accepts offset strings appended to the end of the time, format the dt
							// .. [ dt.ToString("M/dd/yyyy h:mm:ss tt") ]

							dataTable["Scheduled Time In"] = slot.Start.ToDateTimeStringExport();
							dataTable["Scheduled Time Out"] = slot.End.ToDateTimeStringExport();
							if (slot is IBadgeSlot)
								dataTable["Print Badge"] = (slot as IBadgeSlot).PrintBadge ? "1" : "0";
						}
						List<string> values = new List<string>();

						foreach (string headercol in headercols)
						{
							if (dataTable[headercol] != null)
								values.Add(dataTable[headercol].ToString());
							else
								values.Add(string.Empty);
						}

						rows.Add(string.Join(delimiter, values.ToArray()));
					}
				}

            string data = string.Join(Environment.NewLine, rows.ToArray());

            return data;
        }
    }
}
