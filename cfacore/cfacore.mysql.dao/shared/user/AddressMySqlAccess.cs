using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using cfacore.mssql.dao._base;
using System.Data.SqlClient;
using cfacore.mysql.dao.shared.user;

using cfacore.shared.domain.user;
using cfacore.shared.domain.store;
using MySql.Data.MySqlClient;

namespace cfacore.mysql.dao.shared.user
{
    public class AddressMySqlAccess : MySqlAccessObject<Address>,IAddressMySqlAccess
    {
        public AddressMySqlAccess(string connectionString)
            :base(connectionString){ 
        
        }

        /// <summary>
        /// Loads the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public override Address Load(string Id)
        {
            return BindFromStoredProcedure <Address>("Address_GetById", new MySqlParameter[] { 
                new MySqlParameter("_AddressId",Id)
            }, new ObjectBinder<Address>(BindFromReaderIgnoreZipPlusFour));
        }

        /// <summary>
        /// Gets the address by street.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public List<Address> GetAddressByStreet(string name) {
            return BindListFromStoredProcedure("Address_GetAddressesByStreet", new MySqlParameter[] { 
                new MySqlParameter("_Name",name)
            });
        }

        /// <summary>
        /// Saves the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override bool Save(Address obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Address_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Address_Insert", obj, false);
                if(id>0)
                    obj.Id(id.ToString());
                return id>0;
            }
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public override Address BindFromReader(MySqlDataReader reader, string qualifier)
        {
            Address address = new Address();

            address.Id(Convert.ToString(reader["AddressId"]));
            address.Label = Convert.ToString(reader["Label"]);
            address.Line2 = Convert.ToString(reader["Line2"]);
            address.Name = new Name(Convert.ToString(reader["Name"]));
            address.State = Convert.ToString(reader["State"]);
            address.Zip = new Zip(
                Convert.ToInt32(reader["Zip"]),
                (reader["ZipPlusFour"]==Convert.DBNull||string.IsNullOrEmpty(reader["ZipPlusFour"].ToString()))
                ?0:Convert.ToInt32(reader["ZipPlusFour"])
                );
            address.Coordinates = new GeographicCoordinate(Convert.ToDouble(reader["Lat"]),
                Convert.ToDouble(reader["Lng"]));
            return address;
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public Address BindFromReaderIgnoreZipPlusFour(MySqlDataReader reader, string qualifier)
        {
            Address address = new Address();

            address.Id( Convert.ToString(reader["AddressId"]));
            address.Label = Convert.ToString(reader["Label"]);
            address.Line2 = Convert.ToString(reader["Line2"]);
            address.Name = new Name(Convert.ToString(reader["Name"]));
            address.State = Convert.ToString(reader["State"]);
            address.Zip = new Zip(
                Convert.ToInt32(reader["Zip"]),0);
            address.Coordinates = new GeographicCoordinate(Convert.ToDouble(reader["Lat"]),
                Convert.ToDouble(reader["Lng"]));
            return address;
        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [identity].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(Address obj, bool identity)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if(identity)
                parameters.Add(new MySqlParameter("_AddressId",obj.Id()));
            parameters.Add(new MySqlParameter("_Name",obj.Name.ToString()));
            parameters.Add(new MySqlParameter("_Line1",obj.Line1));
            parameters.Add(new MySqlParameter("_Line2",obj.Line2));
            parameters.Add(new MySqlParameter("_Zip",obj.Zip.Code));
            parameters.Add(new MySqlParameter("_ZipPlusFour",obj.Zip.PlusFour));
            parameters.Add(new MySqlParameter("_Label",obj.Label));
            parameters.Add(new MySqlParameter("_State",obj.State));
            parameters.Add(new MySqlParameter("_City",obj.City));
            parameters.Add(new MySqlParameter("_Lat",obj.Coordinates.Latitude));
            parameters.Add(new MySqlParameter("_Lng",obj.Coordinates.Longitude));

            return (parameters.ToArray());
        }
    }
}
