using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.shared.domain.media;

namespace cfares.appfabric.dao.shared
{
    public class MediaAppFabricAccess : AppFabricAccess<Media>
    {
        public MediaAppFabricAccess(string connectionString)
            : base(connectionString)
        {
            this.ConnectionString = connectionString;
        }

        
        public bool Save(Media obj, TimeSpan timespan)
        {
            Uri uri = new Uri(obj.UriBase() + obj.Id());
            try
            {
                Cache.Add(uri.ToString(), obj, timespan);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }
    }
}
