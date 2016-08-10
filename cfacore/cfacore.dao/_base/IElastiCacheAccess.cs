using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao;
using cfacore.domain._base;
using cfacore.dao._base;

namespace cfacore.elasticachefabric.dao._base
{
    public interface IElastiCacheAccess<T> : ICacheAccess<T> where T : IDomainObject, new()
    {
        
    }
}
