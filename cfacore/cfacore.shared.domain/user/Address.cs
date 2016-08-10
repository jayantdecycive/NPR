using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.domain.user;
using cfacore.shared.domain.store;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using core.synchronization.Automation;
using cfacore.shared.domain.attributes;
using cfacore.shared.domain.common;

namespace cfacore.shared.domain.user
{
    public class CityStateZip
    {
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
    }
    public static class IEnumerableExtentions
    {
        public static IList<CityStateZip> CityStateZips(this IEnumerable<Address> addresses)
        {
            return addresses.ToList().GroupBy(x => x.City).Select(x => new CityStateZip()
            {
                City = x.Key,
                Zip = x.First().ZipString,
                State = x.First().State
            }).ToList();
        }
    }

    [Table(Name = "Address")]
    [SyncUrl("/DataService/Location.svc/Addresses")]
	public class Address : AddressMeta,IAddress,IAdminReference
	{
        public Address()
        {
            State = null;
            County = null;
            Line3 = null;
            Line2 = null;
        }

        public Address(string id)
        {
            State = null;
            County = null;
            Line3 = null;
            Line2 = null;
            this._Id = id;
        }

        public Address(int id)
        {
            State = null;
            County = null;
            Line3 = null;
            Line2 = null;
            this._Id = id.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}{4} {5}", 
                string.IsNullOrEmpty( Name.ToString() ) ? "" : ( Name + "\n" ),
				Line1, 
				string.IsNullOrEmpty( Line2 ) ? "" : ( ", " + Line2 ),
                string.IsNullOrEmpty( City ) ? "\n" : ( "\n" + City + ", " ),
				State, 
				( Zip == null || Zip == 0 ) ? string.Empty : Zip.ToString()

			).Trim( new[] { '\r', '\n', ' ', '\t' } );
        }

        public string ToStringSingleLine()
        {
            return string.Format("{0}{1}{2}{3} {4}", 
				Line1, 
				string.IsNullOrEmpty( Line2 ) ? "" : ( ", " + Line2 ),
                string.IsNullOrEmpty( City ) ? ", " : ( ", " + City + ", " ),
				State, 
				( Zip == null || Zip == 0 ) ? string.Empty : Zip.ToString()

			).Trim( new[] { '\r', '\n', ' ', '\t' } );
        }

        public string ToSummary()
        {
            return ToString();
        }

        [Display(Name = "Name")]
        public string NameString
        {

            get
            {
                if (this.Name == null)
                    return "";
                return this.Name.ToString();
            }
            set
            {
                this.Name = new Name(value);
            }

        }

        [StringLength(100), Column]
        public string Line1 { get; set; }

        [Column]
        public string Line2 { get; set; }

        [Column]
        public string Line3 { get; set; }


        /// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Monday, March 12, 2012
		/// </created>		
        [Column]
        [Required(ErrorMessage = "Zip code is required.")]
        [ScaffoldColumn(false)]
        public Zip Zip
        {
            get{
                if (this._Zip == null)
                    return new Zip(0);
                return this._Zip;
            }

            set{ this._Zip = value;}
        }

        [Required(ErrorMessage = "Zip code is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name="Zip Code")]
        public string ZipString
        {
            get {                 
                return Zip.ToString() == "0" ? string.Empty : Zip.ToString(); 
            }

            set
            {
                this.Zip = new Zip(value);
            }
        }


        [Column]
        public string County { get; set; }


        [Column]
        [DataType("DropDown/_State")]
        public string State { get; set; }

        [ScaffoldColumn(false)]
        public int AddressId
        {
            get { return this.IntId(); }
            set { this.Id(value); }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>

        public GeographicCoordinate Coordinates
        {
            get { return this._Coordinates; }

            set { this._Coordinates = value; }
        }

        public string AnchorLabel()
        {
            string name = "View Address";
            if (IsBound())
                name = Name.ToString();
            return name;
        }
        public string AnchorHref()
        {
            return string.Format("/Admin/Address/Details/{0}", Id());
        }
                
    }
}
