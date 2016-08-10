using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace cfares.domain._event.resevent
{
    [Serializable]
    public class ResEventMeta : DomainObject
    {

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>

    

        public override string ToChecksum()
        {
            return Id() + Name;
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Monday, March 12, 2012
        /// </created>
        private string _Name = null;
        [Column]        
        [Display(Name = "Name")]
        [DataType("Html/_HtmlEditor")]
        [AllowHtml]
        public string Name
        {
            get { return this._Name; }

            set { this._Name = value; }
        }

        

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/resevent/";
        }

       

    }
}
