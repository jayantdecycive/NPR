using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.shared.domain.user;
using MySql.Data.MySqlClient;
using cfacore.shared.domain.store;

namespace cfares.mysql.dao.shared
{
    public class AddressMySqlAccess : MySqlAccessObject<Address>
    {
        public AddressMySqlAccess(string connectionString)
            : base(connectionString)
        {

        }

        /// <summary>
        /// Loads the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public override Address Load(string Id)
        {
            return BindFromStoredProcedure("Address_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",Id)
            });
        }

        /// <summary>
        /// Gets the address by label.
        /// </summary>
        /// <param name="name">The label.</param>
        /// <returns></returns>
        public List<Address> GetByLabel(string label)
        {
            return BindListFromStoredProcedure("Address_GetByLabel", new MySqlParameter[] { 
                new MySqlParameter("_Label",label)
            });
        }
        
        /// <summary>
        /// Gets the address by Line1AndZip.
        /// </summary>
        /// <param name="name">The Line1AndZip.</param>
        /// <returns></returns>
        public List<Address> GetByLine1Line2AndZip(string line1,string line2, int zip)
        {
            return BindListFromStoredProcedure("Address_GetByLine1Line2AndZip", new MySqlParameter[] { 
                new MySqlParameter("_Line1",line1),
                new MySqlParameter("_Line2",line2),
                new MySqlParameter("_Zip",zip)
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
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
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

            address.Id ( Convert.ToString(reader["AddressId"]));            
            address.Line1 = Convert.ToString(reader["Line1"]);                       
            address.State = Convert.ToString(reader["State"]);
            address.City = Convert.ToString(reader["City"]);            
            address.Zip = new Zip(Convert.ToString(reader["Zip"]));

            if (reader["County"] != Convert.DBNull)
                address.County = Convert.ToString(reader["County"]);
            if (reader["Line2"] != Convert.DBNull)
                address.Line2 = Convert.ToString(reader["Line2"]);
            if (reader["Line3"] != Convert.DBNull)
                address.Line3 = Convert.ToString(reader["Line3"]);
            if (reader["Label"] != Convert.DBNull)
                address.Label = Convert.ToString(reader["Label"]);
            if (reader["Name"] != Convert.DBNull)
                address.Name = new Name(Convert.ToString(reader["Name"]));
            if (reader["ZipExtension"] != Convert.DBNull)
                address.Zip.PlusFour = Convert.ToInt32(reader["ZipExtension"]);
            if (reader["Lat"] != Convert.DBNull && reader["Lon"] != Convert.DBNull)
            {
                address.Coordinates = new GeographicCoordinate(Convert.ToDouble(reader["Lat"]),
                    Convert.ToDouble(reader["Lon"]));
            }
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
            if (identity)
                parameters.Add(new MySqlParameter("_AddressId", obj.Id()));
            parameters.Add(new MySqlParameter("_Name", obj.Name));
            parameters.Add(new MySqlParameter("_Line1", obj.Line1));
            parameters.Add(new MySqlParameter("_Line2", obj.Line2));
            parameters.Add(new MySqlParameter("_Line3", obj.Line3));
            parameters.Add(new MySqlParameter("_Zip", obj.Zip.Code));            
            parameters.Add(new MySqlParameter("_Label", obj.Label));
            parameters.Add(new MySqlParameter("_State", obj.State));
            parameters.Add(new MySqlParameter("_City", obj.City));            
            parameters.Add(new MySqlParameter("_Lat", obj.Coordinates.Latitude));
            parameters.Add(new MySqlParameter("_Lon", obj.Coordinates.Longitude));
            parameters.Add(new MySqlParameter("_County", obj.County));
            parameters.Add(new MySqlParameter("_ZipExtension", obj.Zip.PlusFour));
            

            return (parameters.ToArray());
        }

        public override bool Delete(Address obj)
        {
            return AssertToStoredProcedure("Address_Delete", obj, true) > 0;
        }
    }
}
