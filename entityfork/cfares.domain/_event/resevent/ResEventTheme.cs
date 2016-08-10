using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using cfacore.domain._base;
using System.Xml.Serialization;

namespace cfares.domain._event.resevent
{
    /// <summary>
    /// Res Event Themes are used for reservation systems that span multiple events. Events should be grouped by type.
    /// This class is used to identify an appropriate template for events that follow this pattern.
    /// </summary>
    [DataContract]
    public class ResEventTheme : DomainObject,IResEventTheme
    {
        public ResEventTheme(string pid):base() {
            this._Id = pid;
        }
        public ResEventTheme()
            : base()
        {
            
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        private ResTemplate _Template = null;
        [Column]
        [DataType("Picker/_ResTemplate")]
        [IgnoreDataMember]
        [XmlIgnore()]
        public ResTemplate Template
        {
            get { return this._Template; }

            set { this._Template = value; }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        private string _TemplateName = null;
        [DataMember]
        public string TemplateName
        {
            get
            {
                if (this.Template != null)
                    return this._Template.Name;
                return _TemplateName;
            }

            set
            {
                if (this.Template != null)
                    this._Template.Name = value;
                this._TemplateName = value;
            }
        }

        

        

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ResEventThemeId
        {
            get { return this.Id(); }
            set { this.Id(value); }
        }

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/reseventtheme/";
        }


        public override string ToChecksum()
        {
            return Name + TemplateName;
        }

       

        private string _TemplateId = null;
        public string TemplateId
        {
            get
            {
                if (this.Template != null)
                    return this._Template.ResTemplateId;
                return _TemplateId;
            }

            set
            {
                if (this.Template != null)
                    this._Template.ResTemplateId = value;
                this._TemplateId = value;
            }
        }

        


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        private ReservationType _ReservationType = null;
        [Column]
        [DataType("Picker/_ResType")]
        [IgnoreDataMember]
        [XmlIgnore()]
        public ReservationType ReservationType
        {
            get { return this._ReservationType; }

            set { this._ReservationType = value; }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        private string _ReservationTypeName = null;
        [DataMember]
        public string ReservationTypeName
        {
            get
            {
                if (this.ReservationType != null)
                    return this._ReservationType.Name;
                return _ReservationTypeName;
            }

            set
            {
                if (this.ReservationType != null)
                    this._ReservationType.Name = value;
                this._ReservationTypeName = value;
            }
        }

        private string _ReservationTypeId = null;
        [DataMember]
        public string ReservationTypeId
        {
            get
            {
                if (this.ReservationType != null)
                    return this._ReservationType.ReservationTypeId;
                return _ReservationTypeId;
            }

            set
            {
                if (this.ReservationType != null)
                    this._ReservationType.ReservationTypeId = value;
                this._ReservationTypeId = value;
            }
        }
    }
}
