using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;

namespace cfares.domain._event
{
    [DataContract]
    [SyncUrl("/api/Template")]    
    public class ResTemplate:DomainObject,IResTemplate
    {
        public ResTemplate(string pid):base() {
            this._Id = pid;
        }
        public ResTemplate()
            : base()
        {
            
        }
        [DataMember]
        public string ResTemplateId
        {
            get { return this.Id(); }
            set { this.Id(value); }
        }

        public override string ToString()
        {
            return Name;
        }

        public List<ResEvent> ResEvents { get; set; }
        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, May 08, 2012
		/// </created>

        [Column]
        [DataMember]
        public virtual cfacore.shared.domain.media.Media Preview
        {
            get;
            set;
        }

        [DataType("Picker/_MediaId")]
        public virtual int? PreviewId
        {
            get;
            set;
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, May 08, 2012
		/// </created>
		private string _Description = null;
        [DataMember]
        [Column]
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Description")]
        public string Description
        {
            get{ return this._Description;}

            set{ this._Description = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, May 08, 2012
		/// </created>
		private string _Name = null;
        [DataMember]
        [Column]
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Name")]
        public string Name
        {
            get{ return this._Name;}

            set{ this._Name = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, May 08, 2012
		/// </created>
		private string _BrowserMedia = null;
        [DataMember]
        [Column]
        [Required]
        public string BrowserMedia
        {
            get{ return this._BrowserMedia;}

            set{ this._BrowserMedia = value;}
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, May 08, 2012
		/// </created>
		private string _DefaultReservationTypeId = null;
        [System.Data.Linq.Mapping.Association(Name = "FK_ReservationType",
            ThisKey = "DefaultReservationTypeId", OtherKey = "ReservationTypeId")]
        [DataMember]
        public string DefaultReservationTypeId
        {
            get { return this._DefaultReservationTypeId; }

            set { this._DefaultReservationTypeId = value; }
        }

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/reseventtemplate/"; 
        }


        public override string ToChecksum()
        {
            return Name + Description; 
        }
    }
}
