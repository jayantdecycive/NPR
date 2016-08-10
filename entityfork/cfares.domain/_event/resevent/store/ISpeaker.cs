using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.shared.domain.media;

namespace cfares.domain._event.resevent.store
{
    public interface ISpeaker
    {
        string SpeakerName { get; set; }
        Media SpeakerMedia { get; set; }
        int? SpeakerMediaId { get; set; }
        string SpeachName();
    }
}
