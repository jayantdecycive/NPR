using cfares.domain._event;

namespace npr.domain._event.slot
{
    public interface INPRSlot : ISlot
    {
        int? GuideId { get; set; }
		bool PrintBadge { get; set; }
    }
}
