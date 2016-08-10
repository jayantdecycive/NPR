using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using cfacore.domain.user;

namespace cfacore.domain.card
{
    public interface ICard
    {
        int UserID { get; set; }
        string Number { get; set; }
        string SecurityNumber { get; set; }

        //[Association(Name = "FK_User", ThisKey = "OwnerId", OtherKey = "UserId")]
        IUser Owner { get; set; }
        IBonusPlanStandingCollection Bonuses { get; set; }
    }
}
