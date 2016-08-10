using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.testconsole._base;
//using cfacore.entity.dao._event;

namespace cfares.testconsole.locations
{
    public class LocationsEntity:IFrameworkTest
    {

        public bool Main()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocationEntityAccess"].ConnectionString;
  //          cfacore.entity.dao._event.ResApplicationEntities lea = new cfacore.entity.dao._event.ResApplicationEntities(connectionString);
    //        foreach(Address addr in from a in lea.Addresses select a){
      //          Console.WriteLine(string.Format("Address: {0}",addr.Label));
        //    }
            Console.ReadLine();
            return false;
        }
    }
}
