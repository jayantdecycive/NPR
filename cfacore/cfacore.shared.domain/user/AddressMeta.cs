using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Data.Linq.Mapping;
using cfacore.shared.domain.store;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace cfacore.shared.domain.user
{
    public class AddressMeta : DomainObject
    {

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [Column]
        public double Lat
        {
            get { return this._Coordinates.Latitude; }

        }
        protected GeographicCoordinate _Coordinates = new GeographicCoordinate();

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [Column]
        public double Lon
        {
            get { return this._Coordinates.Longitude; }

        }

        

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        private string _City = null;
        [Column]
        public string City
        {
            get { return this._City; }

            set { this._City = value; }
        }

        public override string ToChecksum()
        {
            return (Label + Name + Id()+_Zip+City).Replace(" ","");
        }

        protected Zip _Zip = null;

        [Column]
        public string Label { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        private Name _Name = null;

        public AddressMeta()
        {
            Label = null;
            //Line1 = null;
        }

        [Column]
        [ScaffoldColumn(false)]
        public Name Name
        {
            get { return this._Name; }

            set { this._Name = value; }
        }

        

        public override string UriBase()
        {
            return "http://data.core.chick-fil-a.com/profile/address/";
        }

        [Bindable(false)]
        public bool HasContent()
        {
            return !string.IsNullOrEmpty(ToChecksum());
        }

    }
}
