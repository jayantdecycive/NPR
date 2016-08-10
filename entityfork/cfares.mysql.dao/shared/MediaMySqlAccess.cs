using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.domain.user;
using MySql.Data.MySqlClient;
using cfacore.shared.domain.media;
using cfares.domain.user;

namespace cfacore.mysql.dao.shared
{
    public class MediaMySqlAccess : MySqlAccessObject<Media>
    {
        public MediaMySqlAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Binds from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <returns></returns>
        public override Media BindFromReader(MySqlDataReader reader, string qualifier)
        {
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            Media media = new Media();

            media.MediaId = Convert.ToInt32(reader["MediaId"]);

            media.MediaUri = new Uri(Convert.ToString(reader["Uri"]));
            media.Owner = new ResUser(Convert.ToString(reader["OwnerId"]));
            media.IsSystem = Convert.ToBoolean(reader["IsSystem"]);
            media.Width = Convert.ToInt32(reader["Width"]);
            media.Height = Convert.ToInt32(reader["Height"]);
            media.Length = Convert.ToInt32(reader["Length"]);
            media.FileSize = Convert.ToInt32(reader["FileSize"]);
            media.Name = Convert.ToString(reader["Name"]);
            media.Description = Convert.ToString(reader["Description"]);
            media.MediaType = (MediaType)Convert.ToInt32(reader["MediaType"]);
            media.CreatedDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime(reader["CreationDate"]),est);


            return media;

        }

        /// <summary>
        /// Binds to SQL variable array.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="identity">if set to <c>true</c> [[identity]].</param>
        /// <returns></returns>
        public override MySqlParameter[] BindToSqlVariableArray(Media obj, bool identity)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if (identity)
                parameters.Add(new MySqlParameter("_MediaId", obj.Id()));

            parameters.Add(new MySqlParameter("_Uri", obj.MediaUri));
            parameters.Add(new MySqlParameter("_OwnerId", obj.Owner.Id()));
            parameters.Add(new MySqlParameter("_IsSystem", obj.IsSystem));
            parameters.Add(new MySqlParameter("_Width", obj.Width));
            parameters.Add(new MySqlParameter("_Height", obj.Height));
            parameters.Add(new MySqlParameter("_Length", obj.Length));
            parameters.Add(new MySqlParameter("_FileSize", obj.FileSize));
            parameters.Add(new MySqlParameter("_Name", obj.Name));
            parameters.Add(new MySqlParameter("_Description", obj.Description));
            parameters.Add(new MySqlParameter("_MediaType", obj.MediaType));
            parameters.Add(new MySqlParameter("_CreationDate", obj.CreatedDate));


            return (parameters.ToArray());

        }

        public override Media Load(string ID)
        {
            return BindFromStoredProcedure("Media_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
            });
        }

        public override Media Load(Uri uri)
        {
            return BindFromStoredProcedure("Media_GetByUri", new MySqlParameter[] { 
                new MySqlParameter("_Uri",uri)
            });
        }

        public override bool Save(Media obj)
        {
            if (obj.IsBound())
                return AssertToStoredProcedure("Media_Update", obj, true) > 0;
            else
            {
                int id = AssertToStoredProcedure("Media_Insert", obj, false);
                if (id > 0)
                    obj.Id(id.ToString());
                return id > 0;
            }
        }

        public override bool Delete(Media obj)
        {
            return AssertToStoredProcedure("Media_Delete", obj, true) > 0;
        }
    }
}
