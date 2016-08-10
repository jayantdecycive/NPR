using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;

namespace cfacore.shared.domain.common
{
    internal interface IToken : IToken<User>
    {
    }

    interface IToken<TOwner>
        where TOwner:IUser
    {
        Guid TokenUID
        {
            get;
            set;
        }

        TOwner User
        {
            get;
            set;
        }

        string Action
        {
            get;
            set;
        }

        DateTime Expiration
        {
            get;
            set;
        }

    }
}
