using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.dao._base
{
    public interface ISqlAccessObject<T>
        where T : IDomainObject, new()        
    {
        string ConnectionString { get; set; }
    }
}
