using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Data.Linq.Mapping;
using cfacore.shared.domain.attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using cfares.domain.user;
using cfacore.shared.domain.common;
using cfares.domain._event._ticket;
using cfares.domain._event._ticket.tours;
using cfacore.shared.domain.user;
using System.Collections;

namespace cfares.domain._event.slot.tours
{
    // TODO Add attributes to this class
    [Table(Name = "Slot:Tour")]
    [SyncUrl("/DataService/ResEvent.svc/Slot_Tour_Joined", create = "/Service/Slot.svc/CreateTourSlot", update = "/Service/Slot.svc/SaveTourSlot", delete = "/DataService/Slot.svc/Slots")]    
    public class TourSlot:Slot,ITourSlot,IAdminReference
    {
        public TourSlot():base() { 
        
        }

        public TourSlot(string Id)
            : base(Id)
        {

        }

        public TourSlot(TourSlot parent)
            : base(parent)
        {
            if (parent != null)
            {
                this.Guide = parent.Guide;
                this.Cameos = parent.Cameos;                
            }
        }

        public TourSlot(Slot parent)
            : base(parent)
        {
            
        }

        public List<TourTicket> TicketsList { get; set; }
        [ClientIgnore]        
        public IEnumerable<TourTicket> TourTickets
        {
            get {
                return TicketsList.Cast<TourTicket>();
            }
        }

        
        
        public override int TicketsAvailable
        {
            get
            {
                if (Tickets == null)
                    return Capacity - _TotalTicketCache;
                int GuestSum = 0;
                foreach (TourTicket t in Tickets) {
                    GuestSum += t.GuestCount;
                }
                return Capacity - GuestSum;
            }
            set
            {
                if (Tickets == null)
                    _TotalTicketCache = Capacity - value;
            }
        }
        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Thursday, April 05, 2012
		/// </created>

        [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "GuideId", OtherKey = "UserId")]
        [UIHint("AdminLinkLoadable")]
        [DataType("AutoComplete/_Guide")]
        [IgnoreDataMember]
        public virtual ResAdmin Guide
        {
            get;

            set;
        }

        

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Thursday, April 05, 2012
		/// </created>		
        [IgnoreDataMember]
        public virtual ICollection<Cameo> Cameos { get; set; }

        public virtual void AddCameo(ResUser user, CameoType type){
            Cameos.Add(new Cameo{ResUser=user,TourSlot=this, CameoType=type});
        }

        public virtual CameoSets CameoSets { 
            get{
                return new CameoSets
                {
                    AdditionalGuides=Cameos.Where(x=>x.CameoType==CameoType.Guide).Select(x=>x.ResUser),
                    CathyCameos=Cameos.Where(x=>x.CameoType==CameoType.Cathy).Select(x=>x.ResUser),
                    CowCameos=Cameos.Where(x=>x.CameoType==CameoType.Cow).Select(x=>x.ResUser),
                    ExecutiveCameos=Cameos.Where(x=>x.CameoType==CameoType.Executive).Select(x=>x.ResUser),
                    StaffCameos = Cameos.Where(x => x.CameoType == CameoType.Staff).Select(x => x.ResUser)
                };
            }
        }
        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Thursday, April 05, 2012
		/// </created>		
        [Column]
        public bool KidFriendly
        {
            get;
            set;
        }

        
		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Thursday, April 05, 2012
		/// </created>		
        [Column]
        [ClientDefault("Wheelchair")]
        public string SpecialNeeds
        {
            get;
            set;
        }

        public new string AnchorLabel()
        {
            string name = "View Tour Slot";
            if (IsBound())
                name = this.ToString();
            return name;
        }
        public new string AnchorHref()
        {
            return string.Format("/Admin/Slot/TourSlot-Details/{0}", Id());
        }

        public string ToiVisitorCSV()
        {
            return ToiVisitorCSV("^", true, false);
        }

        public string ToiVisitorCSV(string delimiter, bool includeHeaders)
        {
            return ToiVisitorCSV(delimiter, includeHeaders, false);
        }

        public string ToiVisitorCSV(string delimiter, bool includeHeaders, bool isShort)
        {
            string data = "";

            if (this.Tickets == null) {
                throw new Exception("Tickets must be loaded for this slot in order to create CSV");
            }

            if (this.Occurrence == null)
            {
                throw new Exception("Occurrence must be loaded for this slot in order to create CSV");
            }

            

            List<string> rows = new List<string>();

            List<string> headercols = new List<string>();

            headercols.Add("Visitor First Name");
            headercols.Add("Visitor Last Name");
            headercols.Add("Company Name");
            if (isShort)
            {
                headercols.Add("Purpose");
                headercols.Add("Instructions Upon Arrival");
                headercols.Add("Unique Employee ID");
                headercols.Add("Scheduled Time In");
                headercols.Add("Scheduled Time Out");
            }

            if (includeHeaders)
                rows.Add(string.Join(delimiter, headercols.ToArray()));

            foreach (TourTicket t in this.Tickets)
            {
                foreach (Name guestName in t.AllGuestNames)
                {
                    Hashtable dataTable = new Hashtable();
                    foreach (string headercol in headercols)
                    {
                        dataTable.Add(headercol, "");
                    }

                    dataTable["Visitor First Name"] = (guestName.First);
                    dataTable["Visitor Last Name"] = (guestName.Last);

                    dataTable["Company Name"] = (t.GroupName);
                    if (isShort)
                    {
                        dataTable["Purpose"] = "Home Office Tours";
                        dataTable["Instructions Upon Arrival"] = "";

                        dataTable["Unique Employee ID"] = "6342012";

                        dataTable["Scheduled Time In"] = Start.LocalDateTime.ToString();
                        dataTable["Scheduled Time Out"] = End.LocalDateTime.ToString();
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

            data = string.Join(Environment.NewLine, rows.ToArray());

            return data;
        }





        public int? GuideId { get; set; }
    }

    public class Cameo {
        public CameoType CameoType { get; set; }
        public virtual ResUser ResUser { get; set; }
        public virtual TourSlot TourSlot { get; set; }
        public int? ResUserId { get; set; }
        public int? TourSlotId { get; set; }

        
    }

    public class CameoSets {
        [DataType("AutoComplete/_GuideCollection")]
        public IEnumerable<ResUser> CowCameos { get; set; }
        [DataType("AutoComplete/_GuideCollection")]
        public IEnumerable<ResUser> CathyCameos { get; set; }
        [DataType("AutoComplete/_GuideCollection")]
        public IEnumerable<ResUser> ExecutiveCameos { get; set; }
        [DataType("AutoComplete/_GuideCollection")]
        public IEnumerable<ResUser> StaffCameos { get; set; }
        [DataType("AutoComplete/_GuideCollection")]
        public IEnumerable<ResUser> AdditionalGuides { get; set; }

        public string CameoSetsId { get; set; }

        public CameoSets() {
            CowCameos = new List<ResUser>();
            CathyCameos = new List<ResUser>();
            ExecutiveCameos = new List<ResUser>();
            StaffCameos = new List<ResUser>();
            AdditionalGuides = new List<ResUser>();
        }

       
        public static string DescriptionFromType(CameoType typ){
            switch (typ)
            {
                case CameoType.Cathy:
                    return "CathyCameos";                    
                case CameoType.Cow:
                    return "CowCameos";
                case CameoType.Executive:
                    return "ExecutiveCameos";
                    
                case CameoType.Guide:
                    return "AdditionalGuides";
                case CameoType.Staff:
                default:
                    return "StaffCameos";                   

            }
        }



        public ICollection<Cameo> ToList()
        {
            throw new NotImplementedException();
        }
    }    
}
