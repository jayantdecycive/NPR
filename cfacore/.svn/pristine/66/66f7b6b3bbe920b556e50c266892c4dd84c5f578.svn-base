using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao;
using System.Data.SqlClient;
using System.Data.Odbc;
using MySql.Data.MySqlClient;
using cfacore.domain._base;

namespace cfacore.mysql.dao._base
{
    public interface IMySqlAccess<T> : IDataAccessObject<T>
        where T : IDomainObject, new()        
    {
        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        T BindFromReader(MySqlDataReader reader, string qualifier);

        /// <summary>
        /// Binds to Odbc variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        MySqlParameter[] BindToSqlVariableArray(T obj, bool identity);

        /// <summary>
        /// Binds the list from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        List<T> BindListFromStoredProcedure(string procedure, MySqlParameter[] parameters);

        /// <summary>
        /// Binds from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        T BindFromStoredProcedure(string procedure, MySqlParameter[] parameters);

        
    }
}
