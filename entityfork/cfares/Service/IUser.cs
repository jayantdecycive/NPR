using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Mvc;
using System.ServiceModel.Web;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUser" in both code and config file together.
    [ServiceContract]
    public interface IUser
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void EmailResetPassword(string UserId);
    }
}
