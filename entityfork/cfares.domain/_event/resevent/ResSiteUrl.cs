using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace cfares.domain._event.resevent
{
    public class ResSiteUrl : IResSiteUrl
    {
        private string _url;
        [Column]
        [Required]
        [Display(Name = "Reservation Website URL")]
        [StringLength(200,ErrorMessage = "That Url is too long")]
        public string Url
        {
            get { return _url; }
            set
            {
                if (!value.StartsWith("http")) value = "" + value; // "http://" + value;
                _url = value;
            }
        }

        //[Column]
       // public string OriginalUrl { get; set; }

        [IgnoreDataMember]
        [XmlIgnore()]
        public Uri Uri
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Url)) return null;
                    return new Uri(Url);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string RouteUrl(string action, string controller)
        {
            var u = new UriBuilder(Url) {Path = string.Format("{0}/{1}", controller, action)};
            return u.Uri.ToString();
        }
        public string RouteUrl(string action, string controller, string id)
        {
            var u = new UriBuilder(Url) { Path = string.Format("{0}/{1}/{2}", controller, action,id) };
            return u.Uri.ToString();
        }
        public string RouteUrl(string action, string controller,string area, string id)
        {
            var u = new UriBuilder(Url) { Path = string.Format("{2}/{0}/{1}/{3}", controller, action,area, id) };
            return u.Uri.ToString();
        }

        public virtual ResEvent ResEvent
        {
            get; set;
        }

        public virtual ReservationType ReservationType
        {
            get;
            set;
        }

        public int? ResEventId { get; set; }
        public string ResEventTypeId { get; set; }
       

        [Key]
        public int SiteUrlId { get; set; }

        public static string GetRouteName(_event.ResEvent e, IResSiteUrl url)
        {
            return string.Format("EventDirect-{0}-{1}", e.ResEventId, url.SiteUrlId);
        }

        public static string GetRouteName(_event.ReservationType t, IResSiteUrl url)
        {
            return string.Format("EventTypeFallback-{0}-{1}-{2}", t.ReservationTypeId, t.Name, url.SiteUrlId);
        }

        public string GetRouteName()
        {
            if (this.ResEventId != null)
            {
                return ResSiteUrl.GetRouteName(this.ResEvent, this);
            }
            else
            {
                return ResSiteUrl.GetRouteName(this.ReservationType, this);
            }
        }
    }
}
