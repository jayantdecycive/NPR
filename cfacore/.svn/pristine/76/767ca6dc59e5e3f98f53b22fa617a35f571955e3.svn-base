using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.shared.domain.media;
using cfacore.domain.user;
using MySql.Data.MySqlClient;

namespace cfacore.mysql.dao.shared.media
{
    public class MediaMySqlAccess : MySqlAccessObject<Media>
    {
        public MediaMySqlAccess(string connectionString)
            :base(connectionString){         
        }

        public override Media BindFromReader(MySql.Data.MySqlClient.MySqlDataReader reader, string qualifier)
        {
            Media media = new Media();
            media.Id(Convert.ToString(reader["MediaId"]));
            media.Uri(new Uri(Convert.ToString(reader["Uri"])));
            media.Owner = new User(Convert.ToString(reader["OwnerId"]));
            media.IsSystem = Convert.ToBoolean(reader["IsSystem"]);
            media.Width = Convert.ToInt32(reader["Width"]);
            media.Height = Convert.ToInt32(reader["Height"]);
            media.Length = Convert.ToInt32(reader["Length"]);
            media.FileSize = Convert.ToInt32(reader["FileSize"]);
            media.Name = Convert.ToString(reader["Name"]);
            media.Description = Convert.ToString(reader["Description"]);
            media.CreationDate = Convert.ToDateTime(reader["CreationDate"]);
            return media;
        }

        public override MySql.Data.MySqlClient.MySqlParameter[] BindToSqlVariableArray(Media obj, bool identity)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if (identity)
                parameters.Add(new MySqlParameter("_MediaId", obj.Id()));
            parameters.Add(new MySqlParameter("_Uri", obj.Uri().ToString()));
            parameters.Add(new MySqlParameter("_OwnerId", obj.Owner.Id()));
            parameters.Add(new MySqlParameter("_IsSystem", obj.IsSystem));
            parameters.Add(new MySqlParameter("_Width", obj.Width));
            parameters.Add(new MySqlParameter("_Height", obj.Height));
            parameters.Add(new MySqlParameter("_Length", obj.Length));
            parameters.Add(new MySqlParameter("_FileSize", obj.FileSize));
            parameters.Add(new MySqlParameter("_Name", obj.FileSize));
            parameters.Add(new MySqlParameter("_Description", obj.Description));
            parameters.Add(new MySqlParameter("_CreationDate", obj.CreationDate));            

            return (parameters.ToArray());
        }

        public override Media Load(string ID)
        {
            return BindFromStoredProcedure("Media_GetById", new MySqlParameter[] { 
                new MySqlParameter("_Id",ID)
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
    }
}
