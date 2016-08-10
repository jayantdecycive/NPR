
#region Imports

using System.ComponentModel.DataAnnotations;
using Omu.ValueInjecter;
using cfacore.domain.store;
using cfacore.shared.domain.store;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using cfares.domain._event;
using cfares.domain.user;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using cfacore.shared.domain.attributes;

#endregion

namespace cfares.domain.store
{
    [DataContract]
    [SyncUrl("/api/Store")]    
    public class ResStore : Store
    {
		#region Constructors

		public ResStore() {}
        public ResStore(string id)
        { 
            _Id = id;
        }
		public ResStore( Store store )
		{
			// RE: http://stackoverflow.com/questions/1469155/why-is-implicit-conversion-allowed-from-superclass-to-subclass
            new ValueInjecter().Inject( this, store );
		}

		#endregion
        
		#region Implicit conversion operators

	    public static ResStore FromStore( Store store )
        {
		    return new ResStore( store );
        }

        #endregion

        #region Properties

        [DataMember]
        public string OperatorName
        {
            get
            {
                if (Operator == null)
                    return string.Empty;
                return Operator.NameString;
            }
            set
            {
                if(Operator!=null)
                Operator.NameString = value;
            }
        }

        public LocationCategory Category { get; set; }

        [DataType("Picker/_LocationCategoryId")]
        public string CategoryId { get; set; }


        [DataMember]
        public string City
        {
            get
            {
                if (StreetAddress == null)
                    return string.Empty;
                return StreetAddress.City;
            }
            set
            {
                if(StreetAddress!=null)
                StreetAddress.City = value;
            }
        }

        [ScriptIgnore]
        [XmlIgnore]
        public virtual IEnumerable<_event.ResEvent> EventParticipationList
        {
            get {
                if(Occurrences==null||Occurrences.Count==0)
                    return new List<ResEvent>();
                return Occurrences.Select(x => x.ResEvent);
            }            
        }

        [ScriptIgnore]
        [XmlIgnore]
        public virtual IList<_event.Occurrence> Occurrences
        {
            get;
            set;
        }

        public virtual IEnumerable<ResUser> PreferredUsers
        {
            get
            {
                if (PreferredUserSubscriptions == null || PreferredUserSubscriptions.Count==0)
                    return new List<ResUser>();
                return PreferredUserSubscriptions.Select(x => x.User);
            }
        }

        public virtual ICollection<LocationSubscription> PreferredUserSubscriptions { get; set; }

		#endregion
    }
}
