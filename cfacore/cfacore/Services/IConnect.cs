using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using cfacore.external.service.ExactTargetClientS4;
using System.ServiceModel.Web;

namespace cfacore.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IConnect" in both code and config file together.
    [ServiceContract]
    public interface IConnect
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        DetailedCreateResult CreateSubscriber(string site, string email, string firstName, string lastName, string dob, string state, string zip);

    }
}
