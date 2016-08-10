using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfares.entity.dbcontext.res_event;
using System.Data.Entity;

namespace cfatours.entity.dbcontext
{
    public class CfaToursContext : CfaResContext
    {
        public CfaToursContext() : base(0)
        {
            Database.SetInitializer<CfaToursContext>(null);
			//Database.SetInitializer(new SeedInitializer());
        }
        public CfaToursContext(string conn)
            : base(0, conn)
        {
            Database.SetInitializer<CfaToursContext>(null);
			//Database.SetInitializer(new SeedInitializer());
        }
    }
}
