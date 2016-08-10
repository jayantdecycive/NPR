using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using cfacore.domain.store;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILocation" in both code and config file together.
    [ServiceContract]
    public interface ILocation
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        void DoWork();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "ById?ln={LocationNumber}")]
        IStore LocationById(int LocationNumber);
    }
}
