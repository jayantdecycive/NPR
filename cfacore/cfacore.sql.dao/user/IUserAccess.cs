using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using cfacore.domain.user;
using cfacore.mssql.dao._base;

namespace cfacore.sql.dao.user
{
    public interface IUserAccess:IMsSqlAccess<CoreUser>
    {
    }
}
