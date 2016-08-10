using cfacore.dao._base;
using cfacore.shared.domain.store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;
using MySql.Data.MySqlClient;

namespace cfacore.mysql.dao.shared
{
    public class DistributorMySqlAccess : MySqlAccessObject<Distributor>
    {
        public DistributorMySqlAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public override Distributor BindFromReader(MySqlDataReader reader, string qualifier)
        {

            Distributor distributor = new Distributor();

            distributor.DistributorId = Convert.ToString(reader["DistributorId"]);
            distributor.DistributionCenter = Convert.ToString(reader["DistributionCenter"]);
            distributor.Name = Convert.ToString(reader["Name"]);
            distributor.ShortName = Convert.ToString(reader["ShortName"]);
            return distributor;

        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [identity].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(Distributor obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if(identity)
                parameters.Add(new MySqlParameter("_DistributorId",obj.Id()));            
            parameters.Add(new MySqlParameter("_DistributionCenter",obj.DistributionCenter));
            parameters.Add(new MySqlParameter("_Name",obj.Name));
            parameters.Add(new MySqlParameter("_ShortName",obj.ShortName));

            return (parameters.ToArray());

        }

        public override Distributor Load(string ID)
        {
            return BindFromStoredProcedure("Distributor_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public Distributor LoadByShortName(string ID)
        {
            return BindFromStoredProcedure("Distributor_GetByShortName", new MySqlParameter[] { 
                new MySqlParameter("_ShortName",ID)
            });
        }

        public override bool Save(Distributor obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Distributor_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Distributor_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public override bool Delete(Distributor obj)
        {
            return AssertToStoredProcedure("Distributor_Delete", obj, true) > 0;
        }
    }
}
