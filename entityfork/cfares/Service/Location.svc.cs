using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using cfacore.site.controllers.shared;
using cfacore.domain.store;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using cfacore.shared.domain.store;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Location" in code, svc and config file together.
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Location : ILocation
    {        
        public void DoWork()
        {
        }
                
        public IStore LocationById(int LocationNumber)
        {
            StoreService serv = new StoreService();
            Store str= serv.Load(LocationNumber);
            return str;
        }
    }
}
