using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;
using System.Reflection;
using System.Text;
using cfacore.console.Data;

namespace cfacore.console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write(typeof (ChangeInterceptorAttribute).Assembly.FullName);
            Console.Read();
            return;

            cfacore.external.service.ServiceWrapper.ExactTargetService service = new cfacore.external.service.ServiceWrapper.ExactTargetService();

            cfacore.external.service.ExactTargetClientS4.Subscriber innersubscriber = new external.service.ExactTargetClientS4.Subscriber();
            cfacore.external.service.ExactTargetClientS4.ExtendedSubscriber subscriber = new external.service.ExactTargetClientS4.ExtendedSubscriber(innersubscriber);
            subscriber.Email = "test." + DateTime.Now.Ticks + "@test.com";
            subscriber.FirstName = "Test";
            subscriber.LastName = "Test" + DateTime.Now.Ticks;
            subscriber.Birthday = DateTime.Now.AddYears(-20);
            subscriber.ZipCode = "30000";
            subscriber.AgeVerification = true;


            string RequestID, ResultStr;
            service.SaveSubscriberWithList(subscriber.subscriber, cfacore.external.service.ServiceWrapper.ExactTargetService.CAL_REMINDER_LID, out RequestID,out ResultStr);

            Console.Write(subscriber);
        }
    }
}
