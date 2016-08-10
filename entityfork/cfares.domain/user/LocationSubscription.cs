using cfares.domain.store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.domain.user
{
    public class LocationSubscription
    {
        public virtual ResUser User { get; set; }
        public int UserId { get; set; }
        public virtual ResStore Store { get; set; }
        public string StoreId { get; set; }
        public bool ReceiveEmails { get; set; }
        public bool Favorite { get; set; }
        public UserOperationRole Role { get; set; }
    }
}
