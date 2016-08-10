using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;

namespace cfacore.shared.domain.user
{
    public interface IUserCollection:IList<User>
    {
    }
}
