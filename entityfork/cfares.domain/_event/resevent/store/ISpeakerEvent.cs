using cfacore.shared.domain.media;
using cfacore.shared.domain.user;

namespace cfares.domain._event.resevent.store
{
    public interface ISpeakerEvent:IResEvent
    {
        int? OffSiteAddressId { get; set; }
        Address OffSiteAddress { get; set; }
        string OffSiteDescription { get; set; }
        string OffSiteParkingDescription { get; set; }
        string SpeakerName { get; set; }
        Media SpeakerMedia { get; set; }
        int? SpeakerMediaId { get; set; }
        bool HasChildCare { get; set; }
        
    }
}