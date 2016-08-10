using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfacore.shared.domain.common;

namespace cfacore.mysql.dao.shared.CommunicationMySqlAccess
{
    public class CommunicationMySqlAccess : MySqlAccessObject<Communication>
    {
        public CommunicationMySqlAccess(string connectionString)
            : base(connectionString)
        {
        }

        public override Communication BindFromReader(MySql.Data.MySqlClient.MySqlDataReader reader, string qualifier)
        {
            Communication communication = new Communication();

            communication.Id ( Convert.ToString(reader["CommunicationId"]));
            communication.Email = Convert.ToString(reader["Email"]);
            communication.EmailUri = new Uri(Convert.ToString(reader["Uri"]));
            communication.CreationDate = Convert.ToDateTime(reader["CreationDate"]);
            return communication;
        }

        public override MySql.Data.MySqlClient.MySqlParameter[] BindToSqlVariableArray(Communication obj, bool identity)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if (identity)
                parameters.Add(new MySqlParameter("_CommunicationId", obj.Id()));
            parameters.Add(new MySqlParameter("_Email", obj.Email));
            parameters.Add(new MySqlParameter("_Uri", obj.EmailUri));
            parameters.Add(new MySqlParameter("_CreationDate", obj.CreationDate));

            return (parameters.ToArray());
        }

        public override Communication Load(string ID)
        {
            return BindFromStoredProcedure("Communication_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public override bool Save(Communication obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Communication_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Communication_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public bool CheckExists(Communication c)
        {
            return ScalarFromStoredProcedure<bool>("Communication_Exists", new MySqlParameter[] { new MySqlParameter("_Email", c.Email), 
                new MySqlParameter("_Uri", c.EmailUri) }, new ScalarBinder<bool>(ScalarBoolFromReader));
        }
    }
}
