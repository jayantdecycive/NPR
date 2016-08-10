using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao;
using cfacore.domain._base;

namespace cfacore.queue.dao._base
{
    public interface IQueueAccess<T> : IDataAccessObject<T> where T : IDomainObject, new()
    {
        bool Save(T obj,string action);
    }
}
