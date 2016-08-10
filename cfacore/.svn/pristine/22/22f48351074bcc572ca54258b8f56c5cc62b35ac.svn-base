using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao;
using System.Data.SqlClient;
using cfacore.domain._base;

namespace cfacore.mssql.dao._base
{
    public interface IMsSqlAccess<T> : IDataAccessObject<T> where T : IDomainObject, new()
    {
        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        T BindFromReader(SqlDataReader reader, string qualifier);
        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        SqlParameter[] BindToSqlVariableArray(T obj, bool index);
        /// <summary>
        /// Binds the list from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        List<T> BindListFromStoredProcedure(string procedure, SqlParameter[] parameters);
        /// <summary>
        /// Binds from stored procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        T BindFromStoredProcedure(string procedure, SqlParameter[] parameters);
    }
}
