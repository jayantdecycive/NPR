using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace cfacore.shared.domain.attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ClientDefaultAttribute : Attribute
    {
        public object val = "";
        public Type type=null;

        public ClientDefaultAttribute(object val)
        {
            
            this.val = val;
        }

        public ClientDefaultAttribute(object val,Type type)
        {

            this.val = val;
            this.type = type;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ClientIgnoreAttribute : Attribute
    {
        
        public ClientIgnoreAttribute()
        {

            
        }

        

    }
}
