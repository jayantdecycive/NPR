using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITicket" in both code and config file together.
    [ServiceContract]
    public interface ITicket
    {
        [OperationContract]
        void DoWork();

        //{ CardNumber:0, SlotId:'', OwnerId:''}
        //{GuestCount:0}
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.slot.ITourTicket CreateTourTicket(int CardNumber, string SlotId, string OwnerId,string TicketId, int GuestCount);
    }
}
