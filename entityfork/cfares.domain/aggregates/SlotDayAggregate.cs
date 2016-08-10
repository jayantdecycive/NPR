using System.Web.Script.Serialization;
using cfares.domain._event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.domain.aggregates
{
    public class SlotDayAggregateEntry {
        public SlotDayAggregateDx[] Aggregates { get; set; }
        public SlotGrouping Grouping { get; set; }
        
        public DateTime BaseDate { get; set; }
        


        /*public void ApplyBaseDate(SlotDayAggregate slotDayAggregate)
        {
            slotDayAggregate.StartDay = BaseDate.Day;
            slotDayAggregate.EndDay = BaseDate.Day;
            slotDayAggregate.EndYear = BaseDate.Year;
            slotDayAggregate.StartYear = BaseDate.Year;
        }*/
    }

    public class SlotDayAggregateDx {
        public SlotDayAggregate OldValue { get; set; }
        public SlotDayAggregate NewValue { get; set; }

        public bool Unchanged()
        {
            return OldValue.Capacity == NewValue.Capacity
                   && OldValue.EndHour == NewValue.EndHour
                   && OldValue.EndMinute == NewValue.EndMinute
                   && OldValue.StartHour == NewValue.StartHour
                   && OldValue.StartMinute == NewValue.StartMinute
                   && OldValue.EndDay == NewValue.EndDay
                   && OldValue.StartDay == NewValue.StartDay
                   && OldValue.EndMonth == NewValue.EndMonth
                   && OldValue.EndYear == NewValue.EndYear
                   && OldValue.StartMonth == NewValue.StartMonth
                   && OldValue.StartYear == NewValue.StartYear
                   && OldValue.AdditionalDataJson() == NewValue.AdditionalDataJson();
        }
    }

    public class SlotDayAggregate
    {
        public int StartDay
        {
            get
            {
                if (_startDay == 0)
                    return 1;
                return _startDay;
            }
            set { _startDay = value; }
        }
        public List<DateTime> TemplateDates { get; set; }
        private int _startYear;
        private int _endYear;
        private int _startDay;
        private int _endDay;

        public Dictionary<string, object> AdditionalData { get; set; }

        public string AdditionalDataJson()
        {
            if (AdditionalData == null)
                return null;
            return new JavaScriptSerializer().Serialize(AdditionalData);
        }

        public int StartYear
        {
            get
            {
                if (_startYear == 0)
                    return 1989;
                return _startYear;
            }
            set { _startYear = value; }
        }

        public int StartMonth{ get; set; }        
        public int StartMinute { get; set; }
        public int StartHour { get; set; }

        public int EndDay
        {
            get
            {
                if (_endDay == 0)
                    return 1;
                return _endDay;
            }
            set { _endDay = value; }
        }

        public int EndYear
        {
            get
            {
                if (_endYear == 0)
                    return 1989;
                return _endYear;
            }
            set { _endYear = value; }
        }

        public int EndMonth { get; set; }
        public int EndMinute { get; set; }
        public int EndHour { get; set; }


        public int Capacity { get; set; }

        public DateTimeOffset GetStartTime()
        {
            
            return new DateTime(StartYear,StartMonth,StartDay,StartHour,StartMinute,0);
        }

        public DateTimeOffset GetStartTime(DateTimeOffset dateBase)
        {
            return new DateTimeOffset(dateBase.Year, dateBase.Month, dateBase.Day, StartHour, StartMinute, 0,dateBase.Offset);
        }

        public DateTimeOffset GetEndTime()
        {
            return new DateTime(EndYear, EndMonth, EndDay, EndHour, EndMinute, 0);
        }

        public DateTimeOffset GetEndTime(DateTimeOffset dateBase)
        {
            return new DateTimeOffset(dateBase.Year, dateBase.Month, dateBase.Day, EndHour, EndMinute, 0, dateBase.Offset);
        }

    }
    
}
