using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfares.domain._event;

namespace cfares.appfabric.dao._event
{
    public class ResEventAppFabricAccess : AppFabricAccess<ResEvent>
    {
        public ResEventAppFabricAccess(string connectionString)
            : base(connectionString)
        {
            this.ConnectionString = connectionString;
        }



        public bool SaveByUrlName(ResEvent m)
        {
            try
            {

                Cache.Put(m.UriBase().ToString() + m.Urls, m, DEFAULT_TIMEOUT);
                return true;
            }
            catch (Exception Ex)
            {
                Console.Write(Ex.Message);
                return false;
            }
        }

        public ResEvent LoadByUrlName(string subdomain)
        {
            ResEvent shell = new ResEvent();
            return (ResEvent)Cache.Get(shell.UriBase().ToString()+subdomain);
        }
    }
}
