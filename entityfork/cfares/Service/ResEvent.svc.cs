using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using cfares.domain._event;
using cfacore.site.controllers._event;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ResEvent" in code, svc and config file together.
    public class ResEvent : IResEvent
    {
        public void DoWork()
        {
        }

        public ReservationType[] AllEventTypes()
        {
            ResEventTypeService serv = new ResEventTypeService(System.Web.Hosting.HostingEnvironment.MapPath(
                System.Configuration.ConfigurationManager.AppSettings["res-type-dir"]),
                System.Configuration.ConfigurationManager.ConnectionStrings["cfa-app-fabric"].ConnectionString);
            
            return serv.LoadAll().ToArray();
        }
    }
}
