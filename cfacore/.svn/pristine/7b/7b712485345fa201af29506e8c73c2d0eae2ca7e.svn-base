using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using cfacore.shared.domain.user;
using cfacore.shared.domain.store;



namespace cfacore.console.Data
{
    public class AddressTest
    {
        public void Fire() {
            
            do{
                string conn = "server=idcfamysql.cuwhyqxxofly.us-east-1.rds.amazonaws.com;uid=idSqlAdmin;pwd=drake7;database=idApplication;";
//                AddressService serv = new AddressService(conn);
                //serv.Load(1);
                Address addr = new Address();
                addr.City = "Peachtree City";
                addr.Coordinates = new GeographicCoordinate(0.00,0.00);
                addr.Label = "Matt's old house";
                addr.Line1 = "122 Calloway Crossing";
                addr.Name = new Name("Matthew Ryan Drake");
                addr.State = "GA";
                addr.Zip = new Zip(30309,4141);

               // serv.Save(addr);
                Console.WriteLine("Go again?");
            }while(Console.ReadLine()!="y");
        }
    }
}
