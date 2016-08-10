using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using cfacore.shared.domain._base;
using cfacore.domain._base;
using cfacore.shared.domain.common;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;
using System.Runtime.Serialization;
using cfares.domain._event.slot;

namespace cfares.domain._event
{
    [System.Data.Linq.Mapping.Table]
    [SyncUrl("/api/Schedule")]
    [ModelId("ScheduleId")]
    [DataContract]
    [Serializable]
    public class Schedule : DomainObject, ISchedule, ILoadableAdminReference
    {
        public Schedule():base() { 
        
        }

        public Schedule(string Id):base() {
            this.Id(Id);
        }



        public Schedule(DateTime start, DateTime end,int region)
        {
            // TODO: Complete member initialization
            this.Start = start;
            this.End = end;
            this.Region = region;
            this.Name = this.ImpliedName();
            this.UrlName = this.ImpliedUrlName();
        }

        public string ImpliedUrlName(DateTimeOffset Start, DateTimeOffset End)
        {
            return string.Format("{0}-{1}-{2}", Start.ToString(this.UrlDateString()), End.ToString(this.UrlTimeString()), this.Region).ToLower();
        }

        public string ImpliedUrlName()
        {
            return ImpliedUrlName(Start, End);
        }

        private string UrlDateString()
        {
            return "MMM_d_yyyy_h_mmtt";
        }

        private string UrlTimeString()
        {
            return "h_mmtt";
        }

        public string ImpliedName(DateTimeOffset Start, DateTimeOffset End)
        {
            return string.Format("{0} - {1} @:{2}", Start.ToString(this.LegibleDateString()), End.ToString(this.LegibleTimeString()), this.Region).ToLower();
        }

        public string ImpliedName()
        {
            return ImpliedName(Start,End);
        }

        private string LegibleDateString()
        {
            return "MM/dd/yyyy h:mmtt";
        }

        private string LegibleTimeString()
        {
            return "h:mmtt";
        }
        
		

        public int ScheduleId {
            get { return this.IntId(); }
            set { this.Id(value); }
        }
        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, September 18, 2012
		/// </created>		
        [DataType("jqui/_DateTimePicker")]
        public DateTimeOffset Start
		{
		    get; set; }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, September 18, 2012
		/// </created>		
        [DataType("jqui/_DateTimePicker")]
        public DateTimeOffset End
		{
		    get; set; }


        [System.Data.Linq.Mapping.Column]
        [NotMapped]
        [DataType("jqui/_DateTimePickerStr")]
        public string StartString
        {
            get { return Start.ToString("yyyy-MM-ddTHH:mmzz:00"); }
            set { Start = DateTimeOffset.Parse(value); }
        }

        [System.Data.Linq.Mapping.Column]
        [NotMapped]
        [DataType("jqui/_DateTimePickerStr")]
        public string EndString
        {
            get { return End.ToString("yyyy-MM-ddTHH:mmzz:00"); }
            set { End = DateTimeOffset.Parse(value); }
        }
        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, September 18, 2012
		/// </created>
		private int _Capacity = 40;

        public int Capacity
        {
            get{ return this._Capacity;}
            set { this._Capacity = value; }

        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, September 18, 2012
		/// </created>
		private string _Name = null;

        public string Name
        {
            get{ return this._Name;}

            set{ this._Name = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, September 18, 2012
		/// </created>
		private string _UrlName = null;

        public List<Slot> Slots { get; set; }
        

        public string UrlName
        {
            get{ return this._UrlName;}

            set{ this._UrlName = value;}
        }

        public int Region
        {
            get;

            set;
        }

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/schedule/"; 
        }

        public override string ToChecksum()
        {
            return UrlName;
        }

        public string AnchorLabel()
        {
            string name = "View Schedule";
            if (IsBound() && Name!=null)
                name = Name.ToString();
            return name;
        }
        public string AnchorHref()
        {
            return string.Format("/Admin/Schedule/Details/{0}", ScheduleId);
        }

        public string AnchorAbsoluteHref()
        {
            return "http://tours.chick-fil-a.com" + AnchorHref();
        }

        public string ClientModel()
        {
            return "DomainModel.Schedule";
        }

        public string ClientLabel()
        {
            return "Name";
        }


        
    }
}
