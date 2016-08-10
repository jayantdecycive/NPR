using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace cfacore.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICard" in both code and config file together.
    [ServiceContract]
    public interface ICard
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        void CardsByUser();
                
        [OperationContract]
        void UpdateBonusPlanForCard();
    }
}
