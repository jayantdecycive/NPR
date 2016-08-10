using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using cfacore.external.service.ExactTargetClientS4;

namespace cfacore.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IExactTargetReverseProxy" in both code and config file together.
    [ServiceContract]
    public interface IExactTargetReverseProxy
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        ExtendedSubscriber GetSubscriber(string email);

        [OperationContract]
        ExtendedSubscriber GetSubscriberFromHash(string memberhash);

        [OperationContract]
        bool SubscriberExists(string email);

        
        [OperationContract]
        cfacore.external.service.ExactTargetClientS4.ListSubscriber[] GetListsBySubscriber(ExtendedSubscriber Subscriber);
        [OperationContract]
        cfacore.external.service.ExactTargetClientS4.ListSubscriber[] GetListsBySubscriberField(string Email);

        
        [OperationContract]
        cfacore.external.service.ExactTargetClientS4.DetailedCreateResult SaveSubscriber(ExtendedSubscriber subscriber);

        [OperationContract]
        cfacore.external.service.ExactTargetClientS4.DetailedCreateResult SaveSubscriberWithList(ExtendedSubscriber subscriber,int list);

        
        [OperationContract]
        cfacore.external.service.ExactTargetClientS4.DetailedCreateResult UnsubSubscriber(ExtendedSubscriber subscriber);
    }
}
