using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace cfacore.shared.domain.store
{
    [Table]
    public class Distributor:DomainObject,IDistributor
    {

        public Distributor()
        {
            ShortName = null;
            DistributionCenter = null;
            Name = null;
        }

        public Distributor(string id)
        {
            ShortName = null;
            DistributionCenter = null;
            Name = null;
            this._Id = id;
        }

        public override string UriBase()
        {
            throw new NotImplementedException();
        }


        [Column, Required]
        public string DistributionCenter { get; set; }


        [Column, Required]
        public string Name { get; set; }


        [Column, Required, StringLength(10)]
        public string ShortName { get; set; }

        public override string ToChecksum()
        {
            return Id() + ShortName; 
        }

        //public virtual List<Store> Stores { get; set; }

        public string DistributorId
        {
            get { return this.ShortName.ToLower(); }
            set { this.ShortName = value.ToUpper(); }
        }
    }
}
