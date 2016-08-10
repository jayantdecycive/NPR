using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao;
using cfacore.domain.user;
using cfacore.ldap.dao._base;

namespace cfacore.ldap.user
{
    public interface IUserAccess : ILDAPAccess<CoreUser>
    {
    }
}
