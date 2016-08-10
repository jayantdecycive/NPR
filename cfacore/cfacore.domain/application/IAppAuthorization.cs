using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.domain.user;

namespace cfacore.domain.application
{
    public interface IAppAuthorization:IDomainObject
    {
        DateTime CreationDate { get; set; }
        DateTime ExpirationDate { get; set; }
        IApp App { get; set; }
        IUser User { get; set; }
        string Scope { get; set; }
    }
}
