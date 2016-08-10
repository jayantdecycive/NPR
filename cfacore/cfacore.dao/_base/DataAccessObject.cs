using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao;
using cfacore.domain._base;
using System.IO;

namespace cfacore.dao._base
{
    public abstract class DataAccessObject<T> : IDataAccessObject<T>
        where T : IDomainObject, new()
        
	{


        public abstract T Load(string ID);

        public virtual T Load(Uri URI)
        {
            /**
             * TODO:
             * CHECK IF URI IS VALID FOR OBJECT BASE
             * */
            return Load(Path.GetFileName(URI.LocalPath));
        }

        public abstract bool Save(T obj);
        public abstract bool Delete(T obj);
        public abstract T[] Search(KeyValuePair<string, string>[] criteria);
        public virtual IQueryable<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
