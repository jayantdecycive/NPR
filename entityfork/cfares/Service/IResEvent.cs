using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using cfares.domain._event;
using HttpMethodOverrideOperationSelection;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IResEvent" in both code and config file together.
    [ServiceContract]
    public interface IResEvent
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ReservationType[] AllEventTypes();
    }
}
