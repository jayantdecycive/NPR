using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using cfacore.shared.domain.user;
using cfacore.shared.domain.store;

using cfacore.external.service.ServiceWrapper;


namespace cfacore.console.Data
{
    public class ExactTargetTest
    {
        public void Fire() {
            ExactTargetService service = new ExactTargetService();
            do{
                cfacore.external.service.ExactTargetClientS4.Subscriber subscriber = null;
                string testemail = "matthew@thefoundryagency.com";
                subscriber = service.GetSubscriber(testemail);

                Console.WriteLine("Go again?");
            }while(Console.ReadLine()=="y");
        }
    }
}
