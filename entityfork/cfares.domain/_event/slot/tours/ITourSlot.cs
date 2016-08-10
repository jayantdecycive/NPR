using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain.user;
using core.synchronization.Automation;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;
using cfares.domain._event.slot.tours;

namespace cfares.domain._event.slot
{
    
    public interface ITourSlot : ISlot
    {
        [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "GuideId", OtherKey = "UserId")]
        [DataType("AutoComplete/_Admin")]
        ResAdmin Guide { get; set; }

        ICollection<Cameo> Cameos { get; set; }
        
        [Column]        
        bool KidFriendly { get; set; }
        [Column]
        [ClientDefault("Wheelchair")]
        string SpecialNeeds { get; set; }
    }

    public enum CameoType
    {
        Staff,
        Executive,
        Cow,
        Cathy, 
        Guide
    }
    
}
