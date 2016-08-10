using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;

namespace cfacore.mysql.dao.shared.$safeitemname$
{
    public class $safeitemname$MySqlAccess : MySqlAccessObject<$safeitemname$>
    {
        public $safeitemname$MySqlAccess(string connectionString)
            : base(connectionString)
        {
        }

        public override $safeitemname$ BindFromReader(MySql.Data.MySqlClient.MySqlDataReader reader, string qualifier)
        {
            throw new NotImplementedException();
        }

        public override MySql.Data.MySqlClient.MySqlParameter[] BindToSqlVariableArray($safeitemname$ obj, bool identity)
        {
            throw new NotImplementedException();
        }

        public override $safeitemname$ Load(string ID)
        {
            return BindFromStoredProcedure("$safeitemname$_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public override bool Save($safeitemname$ obj)
        {
            if (obj.Bound)
                return AssertToStoredProcedure("$safeitemname$_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("$safeitemname$_Insert", obj, false);
                if (id > 0)
                    obj.Id = id.ToString();
                return id > 0;
            }
        }
    }
}
