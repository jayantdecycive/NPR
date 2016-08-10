using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.shared.domain.store;

namespace sync.dataaccess
{
    public class LocationParserAccess : MySqlAccessObject<Store>
    {
        
        public LocationParserAccess(string connectionstring):base(connectionstring)
        {
            this.ConnectionString = connectionstring;
        }




        public override Store BindFromReader(MySql.Data.MySqlClient.MySqlDataReader reader, string qualifier)
        {
            throw new NotImplementedException();
        }

        public override MySql.Data.MySqlClient.MySqlParameter[] BindToSqlVariableArray(Store obj, bool identity)
        {
            throw new NotImplementedException();
        }
    }
}
