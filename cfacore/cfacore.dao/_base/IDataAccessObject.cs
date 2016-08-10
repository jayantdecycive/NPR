using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.dao
{
    public interface IDataAccessObject<T> where T : IDomainObject, new()
    {
        T Load(string ID);
        T Load(Uri URI);
        bool Save(T obj);
        bool Delete(T obj);
        T[] Search(KeyValuePair<string, string>[] criteria);

        
    }
}
