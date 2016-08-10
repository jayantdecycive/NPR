using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;
using cfares.domain._event;
using cfacore.shared.domain.attributes;
using cfares.domain._event._ticket;

namespace cfares.domain.user
{
    
    public interface IResUser : IUser
    {
        TicketCollection Tickets { get; set; }
        UserOperationRole OperationRole { get; set; }
        
    }

    public enum UserOperationRole
    {
        None,
        Customer,
        Guide,
        Operator,
        Admin
    }
    public class IMembershipRoles
    {
        public static string Admin = "Admin";
        public static string Customer = "Customer";
        public static string Guide = "Guide";
        public static string None = "None";
        public static string Operator = "Operator";
    }
    
}
