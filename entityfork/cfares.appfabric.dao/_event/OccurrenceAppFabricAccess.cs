using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfares.domain._event;

namespace cfares.appfabric.dao._event
{
    public class OccurrenceAppFabricAccess : AppFabricAccess<Occurrence>
    {
        public OccurrenceAppFabricAccess(string connectionString)
            : base(connectionString)
        {
            this.ConnectionString = connectionString;
        }

        
    }
}
