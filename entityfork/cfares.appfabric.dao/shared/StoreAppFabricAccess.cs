using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.shared.domain.store;

namespace cfares.appfabric.dao.shared
{
    public class StoreAppFabricAccess : AppFabricAccess<Store>
    {
        public StoreAppFabricAccess(string connectionString)
            : base(connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public override bool Save(Store obj)
        {
            Uri uri = new Uri(obj.UriBase() + obj.Id());
            try
            {
                Cache.Add(uri.ToString(), obj);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }
        public bool Save(Store obj, TimeSpan timespan)
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
