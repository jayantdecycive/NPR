using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.dao._base
{
    public abstract class SqlAccessObject<T> : DataAccessObject<T>, ISqlAccessObject<T> where T : IDomainObject, new()
    {
        public SqlAccessObject(string connectionString){
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// The Connection String
        /// TODO: Create Base Contructor that Takes in a connectionString
        /// </summary>
        private string _ConnectionString = string.Empty;

        public string ConnectionString{
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
    }
}
