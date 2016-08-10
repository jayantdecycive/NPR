using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfacore.shared.domain.user;

namespace cfacore.mysql.dao.shared.user
{
    /// <summary>
    /// Interface for AddressSQLAccessObject
    /// </summary>
    public interface IAddressMySqlAccess
    {
        Address BindFromReader(MySqlDataReader reader, string qualifier);
        Address BindFromReaderIgnoreZipPlusFour(MySqlDataReader reader, string qualifier);
        MySqlParameter[] BindToSqlVariableArray(Address obj, bool identity);
        
        List<Address> GetAddressByStreet(string name);
        
        Address Load(string Id);
        bool Save(Address obj);
    }
}
