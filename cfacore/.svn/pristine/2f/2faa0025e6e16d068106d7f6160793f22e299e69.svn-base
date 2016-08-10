using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.mssql.dao._base;
using System.Data.SqlClient;
using System.Data;
using cfacore.domain._base;

namespace cfacore.dao._base
{
    public abstract class MsSqlAccessObject<T> : SqlAccessObject<T>, IMsSqlAccess<T> where T : IDomainObject, new()
    {
        public MsSqlAccessObject(string connectionString)
            :base(connectionString){         
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public abstract T BindFromReader(System.Data.SqlClient.SqlDataReader reader, string qualifier);

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public virtual T BindFromReader(System.Data.SqlClient.SqlDataReader reader)
        {
            return BindFromReader(reader,null);
        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public abstract SqlParameter[] BindToSqlVariableArray(T obj, bool index);


        /// <summary>
        /// Binds the list from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual List<T> BindListFromStoredProcedure(string procedure, SqlParameter[] parameters)
        {
            List<T> domainObjects = new List<T>();

            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (SqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            T domainObj = BindFromReader(reader);
                            
                            domainObjects.Add(domainObj);
                        }
                    }
                }
                cmd.Dispose();
            }

            return domainObjects;
        }

        /// <summary>
        /// Binds from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual T BindFromStoredProcedure(string procedure, SqlParameter[] parameters)
        {
            T domainObject = default(T);

            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (SqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        if (reader.Read())
                        {
                            domainObject = BindFromReader(reader);                            
                        }
                    }
                }
                cmd.Dispose();
            }

            return domainObject;
        }

        public override T Load(string ID)
        {
            throw new NotImplementedException();
        }

        public override bool Save(T obj)
        {
            throw new NotImplementedException();
        }

     
    }
}
