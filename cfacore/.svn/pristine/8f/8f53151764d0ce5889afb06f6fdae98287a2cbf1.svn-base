using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using cfacore.external.service.ExactTargetClientS4;

namespace cfacore.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ExactTargetReverseProxy" in code, svc and config file together.
    public class ExactTargetReverseProxy : IExactTargetReverseProxy
    {
        cfacore.external.service.ServiceWrapper.ExactTargetService service = new cfacore.external.service.ServiceWrapper.ExactTargetService();
        public void DoWork()
        {
        }


        public ExtendedSubscriber GetSubscriber(string email)
        {
            var sub = service.GetSubscriber(email);
            ExtendedSubscriber result=null;
            if(sub!=null)
                result = new ExtendedSubscriber(sub);



            return result;
        }

        public ExtendedSubscriber GetSubscriberFromHash(string memberhash)
        {
            //6d61747468657740746865666f756e6472796167656e63792e636f6d
            string email = service.GetEmailFromHex(memberhash);
            ExtendedSubscriber result = new ExtendedSubscriber(service.GetSubscriber(email));
            return result;
        }

        public bool SubscriberExists(string email)
        {
            return service.SubscriberExists(email);            
        }


        public external.service.ExactTargetClientS4.ListSubscriber[] GetListsBySubscriber(ExtendedSubscriber Subscriber)
        {
            return service.GetListsBySubscriber(Subscriber.subscriber);
        }

        public external.service.ExactTargetClientS4.ListSubscriber[] GetListsBySubscriberField(string Email)
        {
            return service.GetListsBySubscriber(Email);
        }


        public external.service.ExactTargetClientS4.DetailedCreateResult SaveSubscriber(ExtendedSubscriber subscriber)
        {
            string RequestID;
            string ResultStr;
                        

            external.service.ExactTargetClientS4.DetailedCreateResult result = new external.service.ExactTargetClientS4.DetailedCreateResult();
            //result.CreateResult= 
                service.SaveSubscriber(subscriber.subscriber, out RequestID, out ResultStr);
            result.RequestID = RequestID;
            result.ResultStr = ResultStr;
            return result;
        }

        public external.service.ExactTargetClientS4.DetailedCreateResult SaveSubscriberWithList(ExtendedSubscriber subscriber, int list)
        {
            string RequestID;
            string ResultStr;


            external.service.ExactTargetClientS4.DetailedCreateResult result = new external.service.ExactTargetClientS4.DetailedCreateResult();
            
            service.SaveSubscriberWithList(subscriber.subscriber, list, out RequestID, out ResultStr);
            result.RequestID = RequestID;
            result.ResultStr = ResultStr;
            return result;
        }

       

        public external.service.ExactTargetClientS4.DetailedCreateResult UnsubSubscriber(ExtendedSubscriber subscriber)
        {
            string RequestID;
            string ResultStr;
            external.service.ExactTargetClientS4.DetailedCreateResult result = new external.service.ExactTargetClientS4.DetailedCreateResult();
            //result.CreateResult = 
            service.UnsubSubscriber(subscriber.subscriber, out RequestID, out ResultStr);
            result.RequestID = RequestID;
            result.ResultStr = ResultStr;
            return result;
        }
    }
}
