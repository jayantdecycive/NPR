using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using cfacore.mysql.dao._base;
using MySql.Data.MySqlClient;
using cfacore.domain._base;

namespace cfacore.dao._base
{
    public abstract class MySqlAccessObject<T> : SqlAccessObject<T>, IMySqlAccess<T>
        where T : IDomainObject, new()        
    {
        
        public MySqlAccessObject(string connectionString)
            :base(connectionString){         
        }
             

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public delegate T ObjectBinder(MySqlDataReader reader, string qualifier);

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public delegate S ScalarBinder<S>(MySqlDataReader reader, string qualifier);


        /// <summary>
        /// Binds type from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public delegate Q ObjectBinder<Q>(MySqlDataReader reader, string qualifier) where Q : T, new();


        /// <summary>
        /// Binds type Q from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public delegate MySqlParameter[] ParameterBinder<Q>(Q obj, bool identity) where Q : T, new();

        /// <summary>
        /// Binds type from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public delegate MySqlParameter[] ParameterBinder(T obj, bool identity);



        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public abstract T BindFromReader(MySqlDataReader reader, string qualifier);

       

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public virtual T BindFromReader(MySqlDataReader reader)
        {
            return BindFromReader(reader,null);
        }

        

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public virtual int IndexFromReader(MySqlDataReader reader)
        {
            return Convert.ToInt32(reader["_Id"]);
        }


        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public abstract MySqlParameter[] BindToSqlVariableArray(T obj, bool identity);

        /// <summary>
        /// Parameters the array from object array.
        /// </summary>
        /// <param name="indexedParameters">The indexed parameters.</param>
        /// <returns></returns>
        protected MySqlParameter[] ParameterArrayFromObjectArray(Object[] indexedParameters)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            for (int i = 0; i < indexedParameters.Length; i++)
            {
                Object o = indexedParameters[i];
                parameters.Add(new MySqlParameter(string.Format("@p{0}", i+1), o));
            }
            return parameters.ToArray();
        }

        /// <summary>
        /// Binds the list from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual List<T> BindListFromStoredProcedure(string procedure, MySqlParameter[] indexedParameters) {
            return BindListFromStoredProcedure<T>(procedure, indexedParameters, BindFromReader);
        }

        /// <summary>
        /// Binds the list from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual List<T> BindListFromStoredProcedure(string procedure, MySqlParameter[] indexedParameters, ObjectBinder binder)
        {
            return BindListFromStoredProcedure<T>(procedure, indexedParameters, new ObjectBinder<T>(binder));
        }

        

        /// <summary>
        /// Binds the list from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="indexedParameters">The indexed parameters.</param>
        /// <param name="binder">The binder.</param>
        /// <returns></returns>
        public virtual List<Q> BindListFromStoredProcedure<Q>(string procedure, MySqlParameter[] parameters, ObjectBinder<Q> binder)
            where Q : T, new()            
        {
            List<Q> domainObjects = new List<Q>();

            

            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (MySqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            Q domainObj = binder(reader, null);
                            domainObj.Bind();
                            domainObjects.Add(domainObj);
                        }
                    }
                }
                cmd.Dispose();
            }

            return domainObjects;
        }


        /// <summary>
        /// Binds the list from query.
        /// </summary>
        /// <param name="procedure">The query.</param>
        /// <param name="indexedParameters">The indexed parameters.</param>
        /// <param name="binder">The binder.</param>
        /// <returns></returns>
        public virtual List<Q> BindListFromQuery<Q>(string query, MySqlParameter[] parameters, ObjectBinder<Q> binder)
            where Q : T, new()
        {
            List<Q> domainObjects = new List<Q>();



            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(query);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                if (parameters != null)
                {
                    foreach (MySqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }
                cmd.Connection = conn;

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            Q domainObj = binder(reader, null);
                            domainObj.Bind();
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
        public virtual T BindFromStoredProcedure(string procedure, MySqlParameter[] parameters) {
            return BindFromStoredProcedure<T>(procedure, parameters, BindFromReader);
        }

        /// <summary>
        /// Binds from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual T BindFromStoredProcedure(string procedure, MySqlParameter[] parameters, ObjectBinder<T> binder)
        {
            return BindFromStoredProcedure<T>(procedure, parameters, binder);
        }

        /// <summary>
        /// Binds from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="indexedParameters">The indexed parameters.</param>
        /// <param name="binder">The binder.</param>
        /// <returns></returns>
        public virtual Q BindFromStoredProcedure<Q>(string procedure, MySqlParameter[] parameters, ObjectBinder<Q> binder) where Q : T,new()
        {
            Q domainObject = default(Q);

            

            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (MySqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        if (reader.Read())
                        {
                            domainObject = binder(reader,null);
                            domainObject.Bind();
                        }
                    }
                }
                cmd.Dispose();
            }

            return domainObject;
        }


        /// <summary>
        /// Binds from query.
        /// </summary>
        /// <param name="procedure">The query.</param>
        /// <param name="indexedParameters">The indexed parameters.</param>
        /// <param name="binder">The binder.</param>
        /// <returns></returns>
        public virtual Q BindFromQuery<Q>(string query, MySqlParameter[] parameters, ObjectBinder<Q> binder) where Q : T, new()
        {
            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            Q domainObject = default(Q);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(query);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                if (parameters != null)
                {
                    foreach (MySqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }
                cmd.Connection = conn;

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        if (reader.Read())
                        {
                            domainObject = binder(reader, null);
                            domainObject.Bind();
                        }
                    }
                }
                cmd.Dispose();
            }

            return domainObject;
        }


        /// <summary>
        /// Asserts to stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public virtual int AssertToStoredProcedure(string procedure, T obj, bool identity) { 
            return AssertToStoredProcedure<T>(procedure, obj, identity, new ParameterBinder<T>(BindToSqlVariableArray));
        }

        /// <summary>
        /// Asserts to stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public virtual int AssertToStoredProcedure(string procedure, T obj, bool identity, ParameterBinder binder) {
            return AssertToStoredProcedure<T>(procedure, obj, identity, new ParameterBinder<T>(binder));
        }

        public virtual int AssertToStoredProcedure(string procedure, MySqlParameter[] parameters)
        {
            return AssertToStoredProcedure<T>(procedure,parameters);
        }

        public virtual int AssertToStoredProcedure<Q>(string procedure, Q obj, bool identity, ParameterBinder<Q> binder) where Q : T, new() {
            MySqlParameter[] parameters = null;
            if (obj != null)
                parameters = binder(obj, identity);
            return AssertToStoredProcedure<Q>(procedure, parameters);
        }

        /// <summary>
        /// Asserts to stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public virtual int AssertToStoredProcedure<Q>(string procedure, MySqlParameter[] parameters) where Q : T, new()
        {
            int domainIndex = 0;

            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                
                

                if (parameters != null)
                {
                    foreach (MySqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }

                MySqlDataReader reader=null;
                
                    reader = cmd.ExecuteReader();
                
                if (reader != null)
                {
                    using (reader)
                    {
                        if (reader.Read())
                        {
                            domainIndex = IndexFromReader(reader);                            
                        }
                    }
                }
                cmd.Dispose();
            }

            return domainIndex;
        }



        /// <summary>
        /// Asserts to stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public virtual int AssertToQuery<Q>(string query, MySqlParameter[] parameters) where Q : T, new()
        {
            int domainIndex = 0;

            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(query);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                if (parameters != null)
                {
                    foreach (MySqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }
                cmd.Connection = conn;
                
                
                MySqlDataReader reader = null;

                reader = cmd.ExecuteReader();

                if (reader != null)
                {
                    using (reader)
                    {
                        if (reader.Read())
                        {
                            domainIndex = IndexFromReader(reader);
                        }
                    }
                }
                cmd.Dispose();
            }

            return domainIndex;
        }

        public virtual int AssertToStoredProcedure(string procedure) {
            return AssertToStoredProcedure(procedure, default(T), false);
        }
        

        public override T Load(string ID)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(T obj)
        {
            throw new NotImplementedException();
        }

        public override bool Save(T obj)
        {
            throw new NotImplementedException();
        }

        public override T[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }




        /// <summary>
        /// Scalar from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="indexedParameters">The indexed parameters.</param>
        /// <param name="binder">The binder.</param>
        /// <returns></returns>
        public virtual S ScalarFromStoredProcedure<S>(string procedure, MySqlParameter[] parameters, ScalarBinder<S> binder) 
        {
            S scalar = default(S);
            

            //This using block is meant to manage connection resources.
            //If you have any suspicion that connections are not being closed, LOOK HERE FIRST
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (MySqlParameter parm in parameters)
                        if (parm != null)
                            cmd.Parameters.Add(parm);
                }

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        if (reader.Read())
                        {
                            scalar = binder(reader, null);                            
                        }
                    }
                }
                cmd.Dispose();
            }

            return scalar;
        }

        /// <summary>
        /// Scalars from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public virtual string ScalarStringFromReader(MySqlDataReader reader, string qualifier)
        {
            return Convert.ToString(reader["val"]);
        }

        /// <summary>
        /// Scalars from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public virtual int ScalarIntFromReader(MySqlDataReader reader, string qualifier)
        {
            return Convert.ToInt32(reader["val"]);
        }


        /// <summary>
        /// Scalars from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public virtual bool ScalarBoolFromReader(MySqlDataReader reader, string qualifier)
        {
            return Convert.ToBoolean(reader["val"]);
        }

        /// <summary>
        /// Scalars from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public virtual DateTime ScalarDateTimeFromReader(MySqlDataReader reader, string qualifier)
        {
            return Convert.ToDateTime(reader["val"]);
        }
    }
}
