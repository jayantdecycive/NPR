using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacore.shared.domain.common;
using cfares.domain.user;

namespace cfares.domain.communication
{
    public interface IResToken
    {
        Guid TokenUID { get; set; }
        int? UserId { get; set; }
        ResUser User { get; set; }
        string Action { get; set; }
        DateTime Expiration { get; set; }
    }

    public class ResToken:Token<ResUser>, IResToken
    {
        
    }
}
