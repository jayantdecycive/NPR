using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfares.domain._event;
using cfares.domain._event.resevent;

namespace cfares.appfabric.dao._event
{
    public class ResEventThemeAppFabricAccess : AppFabricAccess<ResEventTheme>
    {
        public ResEventThemeAppFabricAccess(string connectionString)
            : base(connectionString)
        {
            this.ConnectionString = connectionString;
        }

        
    }
}
