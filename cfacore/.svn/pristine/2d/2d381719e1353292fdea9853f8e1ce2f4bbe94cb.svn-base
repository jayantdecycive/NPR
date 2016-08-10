using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.dao._base
{
    public interface ICacheAccess<T> : IDataAccessObject<T> where T : IDomainObject, new()
    {
        string ConnectionString { get; set; }
        bool Delete(Uri uri);
        
        bool Save(Uri uri, T obj);
        bool Save(string uri, T obj);

        bool Save<Q>(Uri uri, Q obj) where Q : T, new();

        bool Save<Q>(string uri, Q obj) where Q : T, new();

        bool Save<Q>(Q obj) where Q : T, new();

        bool Save<Q>(Q obj, TimeSpan timeout) where Q : T, new();
        Q Load<Q>(Uri uri) where Q : T;
        Q Load<Q>(string key) where Q : T,new();

    }
}
