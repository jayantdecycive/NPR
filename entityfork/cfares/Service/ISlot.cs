using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using cfares.domain._event.slot;
using HttpMethodOverrideOperationSelection;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISlot" in both code and config file together.
    [ServiceContract]
    public interface ISlot
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        void DoWork();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string Hello(string id);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.slot.tours.TourSlot TestSlot();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.slot.tours.TourSlot CreateTourSlot(string OccurrenceId, string Start, string End, int Status, int Capacity, string Cutoff,
        string GuideId, string SlotId, bool KidFriendly, string SpecialNeeds);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "MERGE", UriTemplate = "SaveTourSlot({id}L)", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.slot.tours.TourSlot SaveTourSlot(string id, string OccurrenceId, string Start, string End, int Status, int Capacity, string Cutoff,
        string GuideId, string SlotId, bool KidFriendly, string SpecialNeeds);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST", UriTemplate = "SaveCameosForTourSlot({id}L)", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool SaveCameosForTourSlot(string id, string[] UserIds, int[] CameoTypes);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "DELETE", UriTemplate = "RemoveCameosForTourSlot({id}L)", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool RemoveCameosForTourSlot(string id, string[] UserIds, int[] CameoTypes);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "DELETE", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool TestDelete();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "PUT", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool TestPut();

        [OperationContract]        
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "MERGE", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool TestMerge();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.slot.tours.TourSlot GetTourSlot(string SlotId);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.Slot GetSlot(string SlotId);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.Slot[] GetByEventTypeWithDateRange(string EventType, DateTime Start, DateTime End);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        cfares.domain._event.slot.tours.TourSlot[] GetTourSlotByEventTypeWithDateRange(string EventType, DateTime Start, DateTime End);
    }
}
