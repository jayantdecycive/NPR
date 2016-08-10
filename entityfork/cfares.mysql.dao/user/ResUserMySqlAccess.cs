using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfares.domain.user;
using cfacore.shared.domain.user;
using cfares.domain._event.slot;
using System.Data;
using cfares.domain._event.slot.tours;
using cfares.mysql.dao.shared;

namespace cfacore.mysql.dao.res
{
    public class ResUserMySqlAccess : MySqlAccessObject<ResUser>
    {
        public ResUserMySqlAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public override ResUser BindFromReader(MySqlDataReader reader, string qualifier) {
            return BindFromReader<ResUser>(reader,qualifier);
        }
        public ResUser BindFromReaderWithAddress<Q>(MySqlDataReader reader, string qualifier) where Q : ResUser, new()
        {
            ResUser user = BindFromReader<Q>(reader, qualifier);
            AddressMySqlAccess addressAccess = new AddressMySqlAccess(this.ConnectionString);
            user.Address = addressAccess.BindFromReader(reader,qualifier);
            return user;
        }

        public Q BindFromReaderShort<Q>(MySqlDataReader reader, string qualifier) where Q : ResUser, new()
        {

            Q user = new Q();
            user.Id ( Convert.ToString(reader["UserId"]));

            user.Email = Convert.ToString(reader["Email"]);
            user.Username = Convert.ToString(reader["Username"]);
            user.UID = Convert.ToString(reader["UID"]);

            user.Name = new Name(Convert.ToString(reader["Name"]));
            return user;
        }

        public Q BindFromReader<Q>(MySqlDataReader reader, string qualifier) where Q:ResUser,new()
        {
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            Q user = new Q();
            user.Id ( Convert.ToString(reader["UserId"]));

            user.Email = Convert.ToString(reader["Email"]);
            user.Username = Convert.ToString(reader["Username"]);
            user.UID = Convert.ToString(reader["UID"]);
            
            user.Name = new Name(Convert.ToString(reader["Name"]));

            user.CreatedDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["Creation"]),est);
            user.LastActivity = TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["LastActivity"]),est);
            user.AccountStatus = (UserAccountStatus)Convert.ToInt32(reader["AccountStatus"]);
            user.OperationRole = (UserOperationRole)Convert.ToInt32(reader["UserOperationRole"]);
            user.Authority = Convert.ToString(reader["Authority"]);
            user.AuthorityUID = Convert.ToString(reader["AuthorityUID"]);

            if (reader["DN"] != Convert.DBNull)
                user.DN = Convert.ToString(reader["DN"]);
            if (reader["AddressId"] != Convert.DBNull)
                user.Address = new Address(Convert.ToInt32(reader["AddressId"]));
            if (reader["HomePhone"] != Convert.DBNull)
                user.HomePhone = new Phone(Convert.ToString(reader["HomePhone"]));
            if (reader["MobilePhone"] != Convert.DBNull)
                user.MobilePhone = new Phone(Convert.ToString(reader["MobilePhone"]));

            return user;

        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>

        public override MySqlParameter[] BindToSqlVariableArray(ResUser obj, bool identity)

        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if(identity)

                parameters.Add(new MySqlParameter("_UserId",obj.Id()));            

            parameters.Add(new MySqlParameter("_Email",obj.Email));
            parameters.Add(new MySqlParameter("_Username",obj.Username));
            parameters.Add(new MySqlParameter("_UID",obj.UID));
            parameters.Add(new MySqlParameter("_DN",obj.DN));
            parameters.Add(new MySqlParameter("_Name",obj.Name));
            parameters.Add(new MySqlParameter("_AddressId",obj.Address.Id()));
            parameters.Add(new MySqlParameter("_HomePhone",obj.HomePhone));
            parameters.Add(new MySqlParameter("_MobilePhone",obj.MobilePhone));
			parameters.Add(new MySqlParameter("_Creation",obj.CreatedDate));
            parameters.Add(new MySqlParameter("_LastActivity",obj.LastActivity));
            parameters.Add(new MySqlParameter("_AccountStatus",obj.AccountStatus));
            parameters.Add(new MySqlParameter("_Authority", obj.Authority));
            parameters.Add(new MySqlParameter("_AuthorityId", obj.AuthorityUID));
            parameters.Add(new MySqlParameter("_UserOperationRole", obj.OperationRole));


            return (parameters.ToArray());

        }

        public override ResUser Load(string ID)
        {
            return BindFromStoredProcedure("User_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public ResUser LoadWithAddress(string ID)
        {
            return BindFromStoredProcedure("User$Address_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            },new ObjectBinder<ResUser>(BindFromReaderWithAddress<ResUser>));
        }

        public ResUser LoadByUsernameWithAddress(string username)
        {
            return BindFromStoredProcedure("User$Address_GetByUsername", new MySqlParameter[] { 
                new MySqlParameter("_Username",username)
            }, new ObjectBinder<ResUser>(BindFromReaderWithAddress<ResUser>));
        }

        public ResUser LoadByUsername(string ID)
        {
            return BindFromStoredProcedure("User_GetByUsername", new MySqlParameter[] { 
                new MySqlParameter("_Username",ID)
            });
        }
        public ResUser LoadByEmail(string email)
        {
            return BindFromStoredProcedure("User_GetByEmail", new MySqlParameter[] { 
                new MySqlParameter("_Email",email)
            });
        }
        

        public ResUserCollection LoadByUsernames(string[] Usernames)
        {
            /*
             * This is preferred over doing join operations on the sql server
             * */
            return new ResUserCollection(BindListFromQuery<ResUser>(
                "SELECT * FROM User WHERE FIND_IN_SET (Username,@names)",
                new MySqlParameter[] { 
                    new MySqlParameter("@names",string.Join(",", Usernames))
                },
                new ObjectBinder<ResUser>(BindFromReader)
            ));
        }

        public ResUserCollection GetSlotCameos(string SlotId)
        {
            return new ResUserCollection(BindListFromStoredProcedure(
                "Slot:Tour$Cameos_GetUsersBySlotId",
                new MySqlParameter[] { 
                    new MySqlParameter("_Id",SlotId)
                }
            ));
        }

        public CameoSets GetTypedSlotCameos(string SlotId)
        {
            CameoSets cameos = new CameoSets();

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = "Slot:Tour$Cameos_GetUsersBySlotId";
                cmd.CommandType = CommandType.StoredProcedure;

                
                cmd.Parameters.AddWithValue("_Id",SlotId);
                var slot = new TourSlot(SlotId);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            ResAdmin domainObj = (ResAdmin)BindFromReader<ResAdmin>(reader, null);
                            CameoType typ = (CameoType)Convert.ToInt32(reader["Type"]);
                            domainObj.Bind();
                            slot.AddCameo(domainObj, typ);
                        }
                    }
                }
                cmd.Dispose();
            }
            return cameos;
        }

        public ResUserCollection GetSlotCameos(string SlotId,CameoType type)
        {
            return new ResUserCollection(BindListFromStoredProcedure(
                "Slot:Tour$Cameos_GetUsersBySlotAndType",
                new MySqlParameter[] { 
                    new MySqlParameter("_SlotId",SlotId),
                    new MySqlParameter("_Type",type)
                }
            ));
        }

        public ResUser LoadByDatasource(string datasource, string ID)
        {
            return BindFromStoredProcedure("User_GetByAuthorityAndId", new MySqlParameter[] { 
                new MySqlParameter("_Authority",datasource),
                new MySqlParameter("_AuthorityId",ID)
            });
        }

        public override bool Save(ResUser obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("User_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("User_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public override bool Delete(ResUser obj)
        {
            return AssertToStoredProcedure("User_Delete", new MySqlParameter[] { 
                    new MySqlParameter("_Id",obj.UserId)
                }) > 0;         
        }

        public bool DeleteByUsername(string Username)
        {
            return AssertToStoredProcedure("User_DeleteByUsername", new MySqlParameter[] { 
                    new MySqlParameter("_Username",Username)
                }) > 0;
        }

        public bool SaveCameosForTourSlot(string TourSlotId, string[] UserIds, int[] CameoTypes)
        {

            if(UserIds.Length!=CameoTypes.Length)
                throw new Exception("Cameos length does not match users length");

            string query = "INSERT IGNORE INTO `Slot:Tour$Cameo` (SlotId,UserId,CreationDate,Type) VALUES ";
            string valueTemplate = "(@TourSlotId,@UserId{0},@Now,@Type{0})";

            List<string> valueToBeParamterized = new List<string>();
            List<MySqlParameter> mysqlParams = new List<MySqlParameter>();
            
            mysqlParams.Add(new MySqlParameter("TourSlotId", TourSlotId));
            mysqlParams.Add(new MySqlParameter("Now", DateTime.Now));

            for (int i = 0; i < UserIds.Length;i++)
            {
                valueToBeParamterized.Add(string.Format(valueTemplate,i));
                mysqlParams.Add(new MySqlParameter(string.Format("UserId{0}", i), UserIds[i]));
                mysqlParams.Add(new MySqlParameter(string.Format("Type{0}", i), CameoTypes[i]));
            }

            query = query + string.Join(", ",valueToBeParamterized.ToArray());

            
            return AssertToQuery<ResUser>(query, 
                mysqlParams.ToArray()                
            )>0;
        }

        public bool RemoveCameosForTourSlot(string TourSlotId, string[] UserIds, int[] CameoTypes)
        {

            if (UserIds.Length != CameoTypes.Length)
                throw new Exception("Cameos length does not match users length");

            string query = "DELETE FROM `Slot:Tour$Cameo` WHERE (SlotId,UserId,Type) IN ({0})";
            string valueTemplate = "(@TourSlotId,@UserId{0},@Type{0})";

            List<string> valueToBeParamterized = new List<string>();
            List<MySqlParameter> mysqlParams = new List<MySqlParameter>();

            mysqlParams.Add(new MySqlParameter("TourSlotId", TourSlotId));
            

            for (int i = 0; i < UserIds.Length; i++)
            {
                valueToBeParamterized.Add(string.Format(valueTemplate, i));
                mysqlParams.Add(new MySqlParameter(string.Format("UserId{0}", i), UserIds[i]));
                mysqlParams.Add(new MySqlParameter(string.Format("Type{0}", i), CameoTypes[i]));
            }

            query = string.Format(query ,string.Join(", ", valueToBeParamterized.ToArray()));


            return AssertToQuery<ResUser>(query,
                mysqlParams.ToArray()
            ) > 0;
        }







        
    }
}
