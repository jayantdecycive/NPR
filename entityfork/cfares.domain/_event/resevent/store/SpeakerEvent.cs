using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using cfacore.shared.domain.media;
using cfacore.shared.domain.user;
using cfares.domain._event.menu;
using cfares.domain._event.occ;

namespace cfares.domain._event.resevent.store
{
    

    public class SpeakerEvent:ResEvent, ISpeakerEvent,ISpeaker
    {

        public SpeakerEvent()
        {
            //if(ScheduleId==null&&Schedule==null)
             //   Schedule=new Schedule();
        }

        public override int TotalReservationsRemaining
        {
            get
            {
                if (Occurrences == null) return base.TotalReservationsRemaining;
                return Occurrences
                    .SelectMany(o => o.SlotsList)
                    .Sum(s => this.Schedule.Capacity - s.TotalTickets);
            }
        }


        public ISlot GetSlot()
        {
            if (Occurrences == null || !Occurrences.Any())// || Occurrences.Count()>1)
                return null;
            return Occurrences.First().SlotsList.FirstOrDefault();
        }

        private static DateTimeOffset CombineDateAndTime(DateTime date, DateTimeOffset dto)
        {
            return new DateTimeOffset(date.Year, date.Month, date.Day, dto.Hour, dto.Minute, dto.Second, dto.Offset);
        }

      

        public DateTimeOffset StartDateTime()
        {
            return CombineDateAndTime(RegistrationStart,Schedule.Start);
        }

        public DateTimeOffset EndDateTime()
        {
            return CombineDateAndTime(RegistrationEnd, Schedule.End);
        }

    
        public TimeZoneInfo GetFirstTimeZone()
        {
            if (this.Occurrences == null || !this.Occurrences.Any())
            {
                return Occurrence.DefaultTimeZone;
            }
            return this.Occurrences.First(x=>x.StoreId!="00000").Store.GMTOffsetTimeZoneInfo;
        }

        public int? OffSiteAddressId { get; set; }
        public virtual Address OffSiteAddress { get; set; }

        [System.Data.Linq.Mapping.Column]
        [DataType(DataType.MultilineText)]
        public string OffSiteDescription { get; set; }

        [System.Data.Linq.Mapping.Column]
        [DataType(DataType.MultilineText)]
        public string OffSiteParkingDescription { get; set; }

        [System.Data.Linq.Mapping.Column]
        public string SpeakerName { get; set; }

        public virtual Media SpeakerMedia { get; set; }

        [System.Data.Linq.Mapping.Column]
        [DataType("Picker/_MediaId")]
        [Display(Name = "Speaker Image")]
        public int? SpeakerMediaId { get; set; }

        [System.Data.Linq.Mapping.Column]
        public bool HasChildCare { get; set; }

        public virtual Schedule Schedule { get; set; }
        public int? ScheduleId { get; set; }

        

        public DateTimeOffset DefaultCutoff()
        {
            return new DateTimeOffset(StartDateTime().Date,StartDateTime().Offset);
        }

        public Slot CreateSlot()
        {
            var slot = new Slot();
            this.SyncSlot(slot);
            return slot;
            
        }

        public void SyncSlot(ISlot x)
        {
            x.Start = this.StartDateTime();
            x.End = this.EndDateTime();
            x.Schedule = this.Schedule;
            x.IsScheduled = true;
            x.Cutoff = this.DefaultCutoff();
            //x.Capacity = this.Schedule.Capacity;
        }


        public string SpeachName()
        {
            return this.Name;
        }
    }
}
