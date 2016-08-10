using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfares.repository.exceptions
{
    public class ReservationException:Exception
    {
        public ReservationException(string message)
            : base(message)
        {
        }
    }
    public class CustomerDataException:ReservationException
    {
        public CustomerDataException(string message) : base(message)
        {
        }
    }
}
