
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;
using cfacore.domain._base;
using cfacore.shared.domain.attributes;
using cfacore.shared.domain.user;
using cfacore.shared.domain.common;
using cfares.domain.store;
using cfares.domain.user;
using cfares.domain._event.ticket;

namespace cfares.domain._event
{
    [System.Data.Linq.Mapping.Table]
    [SyncUrl("/api/Ticket")]
    [Serializable]

    public class Ticket : DomainObject, ITicket, IAdminReference, IResEventBound
    {
        public Ticket()
        {
            //member initializer

            if (AdditionalInformation == null)
                AdditionalInformation = new Dictionary<string, string>();
        }

        public ResStore GetStore()
        {
            if (this.Slot == null || this.Slot.Occurrence == null || this.Slot.Occurrence.Store == null)
                return null;
            return this.Slot.Occurrence.Store;
        }

        public string AdditionalField(string key)
        {
            if (this.AdditionalInformation.ContainsKey(key))
                return AdditionalInformation[key];
            return null;
        }

        public string AdditionalField(string key, string value)
        {
            this.AdditionalInformation[key] = value;
            return value;
        }

        public IResEvent GetEvent()
        {
            return Slot.Occurrence.ResEvent;
        }

        public bool HasEvent()
        {
            return Slot != null && Slot.Occurrence != null && Slot.Occurrence.ResEventId != null;
        }

        public Ticket(string id)
        {
            _Id = id;
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        [System.Data.Linq.Mapping.Association(Name = "FK_Slot", ThisKey = "SlotId", OtherKey = "SlotId")]
        [DataType("Picker/_Slot")]
        [UIHint("AdminLink")]
        [ClientDefault(1, type = typeof(string))]
        public virtual Slot Slot
        {
            get;
            set;
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "OwnerId", OtherKey = "UserId")]
        [DataType("AutoComplete/_User")]
        [UIHint("AdminLinkLoadable")]
        [ClientDefault(1, type = typeof(string))]
        [Display(Name = "Owner")]
        public virtual user.ResUser Owner
        {
            get;
            set;
        }

        [DataType("AutoComplete/_UserId")]
        [Display(Name = "Owner")]
        [Column]
        public int? OwnerId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string Note { get; set; }

        public override string ToString()
        {

            return string.Format("Ticket Id {0} for Slot #{1}", Id(), Slot == null ? "none" : Slot.Id());
        }

        public string AnchorLabel()
        {
            string name = "View Ticket";
            if (Owner.IsBound())
                name = string.Format("Ticket for {0}", Owner.Id());
            return name;
        }

        public string AnchorHref()
        {
            return string.Format("/Admin/Ticket/Details/{0}", Id());
        }

        public virtual bool IsAllocatedCapacityFixed { get { return true; } }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType("jqui/_DatePicker")]
        [Display(Name = "Created On")]
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }

        public DateTime? CanceledDate { get; set; }

        [Column]
        [DataType("Picker/_SlotId")]
        [Display(Name = "Slot Id")]
        [ScaffoldColumn(false)]
        public int? SlotId
        {
            get;
            set;
        }

        [ScaffoldColumn(false)]
        public string OwnerName
        {
            get
            {
                if (Owner == null) return null;
                return Owner.NameString;
            }
            set { if (Owner != null) Owner.NameString = value; }
        }

        [ScaffoldColumn(false)]
        public string OwnerEmail
        {
            get
            {
                if (Owner == null) return null;
                return Owner.Email;
            }
            set { if (Owner != null) Owner.Email = value; }
        }

        [ScaffoldColumn(false)]
        public string OwnerHomePhone
        {
            get
            {
                if (Owner == null) return null;
                if (Owner.HomePhone == null) return null;
                return Owner.HomePhone.ToFormattedString();
            }
            set { if (Owner != null) Owner.HomePhone = new Phone(value); }
        }

        [DataType("Enum/_TicketStatus")]
        [Column]
        public TicketStatus Status { get; set; }

        public virtual int AllocatedCapacity { get { return 1; } set { } }

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/ticket/";
        }

        public override string ToChecksum()
        {
            return Id() + CardNumber;
        }

        [Column]
        public int TicketId
        {
            get { return IntId(); }
            set { Id(value); }
        }

        [System.Data.Linq.Mapping.Column]
        [Display(Name = "Creation Source")]
        [ScaffoldColumn(false)]
        public string CreationSrc
        {
            get { return _creationSrc; }

            set { _creationSrc = value; }
        }
        private string _creationSrc;

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        [System.Data.Linq.Mapping.Column]
        [Display(Name = "Card Number")]
        // TODO: Ask Matt about Range
        [MaxLength(100)]
        public string CardNumber
        {
            get { return _cardNumber; }

            set { _cardNumber = value; }
        }
        private string _cardNumber;

        public string AdditionalInformationString
        {
            get
            {
                if (AdditionalInformation == null)
                    AdditionalInformation = new Dictionary<string, string>();
                return new JavaScriptSerializer().Serialize(AdditionalInformation);
            }
            set
            {
                AdditionalInformation = string.IsNullOrEmpty(value) ? new Dictionary<string, string>() : new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(value);
            }
        }

        public Dictionary<string, string> AdditionalInformation { get; set; }

        public virtual ICollection<TicketTransaction> Transactions
        {
            get;
            set;
        }

        public virtual ICollection<TicketGuests> TicketGuests
        {
            get;
            set;
        }


        public ISlot GetSlot()
        {
            return this.Slot;
        }

        public IOccurrence GetOccurrence()
        {
            if (this.Slot == null)
                return null;
            return this.Slot.Occurrence;
        }
    }

    public class TicketTransaction : DomainObject, ITicketTransaction
    {
        public TicketTransaction()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        
        public virtual Ticket Ticket { get; set; }

        public virtual ResUser Customer { get; set; }

        public virtual TicketGuests TicketGuest { get; set; }
        public int? CustomerId { get; set; }

        public TicketTransactionAction Action { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TicketTransactionId
        {
            get { return this.Id(); }
            set { this.Id(value); }
        }

        public int? TicketId { get; set; }
        public int? TicketGuestsId { get; set; }

        public override string UriBase()
        {
            return "http://data.res.chick-fil-a.com/TicketTransaction/";
        }

        public override string ToChecksum()
        {
            return this.Id();
        }
    }
  
    public class TicketGuests : DomainObject, ITicketGuests
    {
        public TicketGuests()
        {
        }        

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketGuestsId {
            get { return IntId(); }
            set { Id(value); }
        }

        public string GuestName { get; set; }

        public virtual Ticket Ticket { get; set; }

        public int TicketId { get; set; }

        public override string UriBase()
        {
            return "http://data.res.chick-fil-a.com/TicketGuests/";
        }

        public override string ToChecksum()
        {
            return Id();
        }
    }

    public enum TicketTransactionAction
    {
        Redeeem,
        Forfeit,
        Purchase,
        Refund,
    }
}
