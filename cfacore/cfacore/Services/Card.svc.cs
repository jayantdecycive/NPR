using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace cfacore.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Card" in code, svc and config file together.
    public class Card : ICard
    {
        public void DoWork()
        {
        }


        public void CardsByUser()
        {
            throw new NotImplementedException();
        }

        public void UpdateBonusPlanForCard()
        {
            throw new NotImplementedException();
        }
    }
}
