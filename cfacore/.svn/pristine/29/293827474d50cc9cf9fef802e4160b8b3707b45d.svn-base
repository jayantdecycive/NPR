using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace cfacore.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUser" in both code and config file together.
    [ServiceContract]
    public interface IUser
    {
        [OperationContract]
        void CurrentUser();

        [OperationContract]
        void UsersByEmail();

        [OperationContract]
        void UsersByStore();

        [OperationContract]
        void UserByCard();

        [OperationContract]
        void UserNotification();

    }
}
