using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace cfares.domain._event.resevent
{
    public interface IResSiteUrl
    {
        [Column]
        [Required]
        [Display(Name = "Reservation Website URL")]
        string Url { get; set; }

        [IgnoreDataMember]
        [XmlIgnore()]
        Uri Uri { get; }

        ResEvent ResEvent { get; set; }
        ReservationType ReservationType { get; set; }
        int? ResEventId { get; set; }
        string ResEventTypeId { get; set; }
        int SiteUrlId { get; set; }
        //string OriginalUrl { get; set; }
    }
}