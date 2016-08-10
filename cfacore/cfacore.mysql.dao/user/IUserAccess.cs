using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using cfacore.domain.user;
using cfacore.mysql.dao._base;

namespace cfacore.mysql.dao.user
{
    public interface IUserAccess:IMySqlAccess<CoreUser>
    {
    }
}
