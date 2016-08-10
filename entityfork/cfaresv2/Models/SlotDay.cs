using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using cfares.domain._event;

namespace cfaresv2.Models
{
    public class SlotDay
    {
        public DayOfWeek Day
        {
            get { return Date.DayOfWeek; }
        }

        public DateTime Date { get; set; }

        public List<ISlot> Slots { get; set; }

        public static List<SlotDay> GetSet(ICollection<ISlot> slots)
        {
            return slots.GroupBy(x => x.Start.Date).Select(slotDay => new SlotDay()
                {
                    Date = slotDay.Key, Slots = slotDay.ToList()
                }).ToList();
        }
    }

    public class SlotDayCollection : Collection<SlotDay>
    {
        public SlotDayCollection() : base()
        {
        }

        public SlotDayCollection(IEnumerable<SlotDay> slotDays)
            : base()
        {
            foreach (var slotDay in slotDays)
            {
                this.Add(slotDay);
            }
        }

        public static List<SlotDayCollection> GetSets(ICollection<ISlot> slots,int setSize)
        {
            var sets = new List<SlotDayCollection>();

            var slotDays = SlotDay.GetSet(slots);
            for (var i = 0; i < slotDays.Count;i+=setSize )
            {
                var slice = slotDays.Skip(i).Take(setSize);
                sets.Add(new SlotDayCollection(slice));
            }
            return sets;
        }
    }
}