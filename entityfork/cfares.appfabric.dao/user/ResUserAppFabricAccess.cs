using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfacore.shared.domain.media;
using cfacore.domain.user;
using cfares.domain.user;

namespace cfares.appfabric.dao.user
{
    public class ResUserAppFabricAccess : AppFabricAccess<ResUser>
    {
        public ResUserAppFabricAccess(string connectionString)
            : base(connectionString)
        {
            this.ConnectionString = connectionString;
        }


    }
}
