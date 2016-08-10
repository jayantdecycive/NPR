using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Data.Linq.Mapping;

namespace cfacore.domain.user
{
    public interface IZip:IParseable<IZip>
    {
       
        int? PlusFour { get; set; }
        
        int Code { get; set; }
        
    }
}
