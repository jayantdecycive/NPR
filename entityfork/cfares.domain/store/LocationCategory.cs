
#region Imports

using System.Collections.Generic;
using System.Web.Mvc;
using cfacore.domain._base;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;
using cfacore.shared.domain.store;
using cfares.domain.store;

#endregion

namespace cfacore.domain.store
{
    [DataContract]
    [SyncUrl("/api/LocationCategory")]    
    public class LocationCategory : DomainObject, ILocationCategory
	{
		#region Constructors

		public LocationCategory(string id) { _Id = id; }
        public LocationCategory() {}

		#endregion

        [DataMember]
        [Column]
        
        public string LocationCategoryId
        {
            get { return _Id; }
            set { _Id = value; }
        }

		[DataMember, Column, Required]
        public string Name { get; set; }

		[DataMember, Column, Required]
        public string Description { get; set; }

		#region Type Helpers / Class Support

		private System.Type _type;                
        public System.Type Type()
        {
             return _type; 
        }
        public void Type(System.Type value){
            _type = value;
        }
        
        [DataMember]
        [ScaffoldColumn(false)]
        public string StrType {
            get 
			{
	            return _type == null ? string.Empty : _type.ToString();
            }
	        set { _type = System.Type.GetType(value); }
        }

        string TypeName
        {
            get
            {
                return Type().FullName;
            }
            set
            {
                _type = System.Type.GetType(value);
            }
        }

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/store/locationcategory/"; 
        }

        public override string ToChecksum()
        {
            return Name;
		}

		#endregion

        public ICollection<ResStore> Locations { get; set; }
    }
}
