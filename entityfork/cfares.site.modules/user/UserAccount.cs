using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;
using System.Web.Security;

namespace cfacore.shared.domain.user
{
    public class UserAccount<T> where T : User
    {
        public MembershipUser Membership;
        public T ApplicationUser;
    }
}
