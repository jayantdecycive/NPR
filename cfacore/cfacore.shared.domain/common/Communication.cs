using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.shared.domain.common
{
    public class Communication : DomainObject, ICommunication
    {
        public string Email { get; set; }
        

        public Uri EmailUri { get; set; }

        public string EmailUriString
        {
            get { return this.EmailUri.ToString(); }

            set { this.EmailUri = new Uri(value); }
        }

        public DateTime CreationDate { get; set; }

        public int CommunicationId { get { return this.IntId(); } set { this.Id(value); } }

        public override string UriBase()
        {
            return "http://core.chick-fil-a.com/communication";
        }

        public override string ToChecksum()
        {
            return this.Email + this.EmailUri.ToString();
        }
    }
}
