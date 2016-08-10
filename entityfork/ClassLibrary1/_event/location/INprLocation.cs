using cfacore.shared.domain.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npr.domain._event.location
{
    public interface INprLocation
    {
        string Name { get; set; }

        Address Address { get; set; }

        Uri Site { get; set; }

        string SiteName { get; set; }

        EventSpaceType EventSpaceType { get; set; }

        bool Parking { get; set; }

        bool AtNPR { get; set; }

        int Capacity { get; set; }

        string ContactName { get; set; }

        Phone Phone { get; set; }

        string Email { get; set; }

        string Comments { get; set; }
    }

    public enum EventSpaceType
    {
        Ballroom, Studio, Theater, Conference, Misc
    }
}
