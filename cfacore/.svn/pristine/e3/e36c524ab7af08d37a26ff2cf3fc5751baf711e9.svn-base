using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace cfacore.shared.domain.attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class ModelIdAttribute : Attribute
    {
        public string param = "";
        public string type = "";
        public ModelIdAttribute(string param)
        {
            this.param = param;
        }
        public ModelIdAttribute(string param,string type)
        {
            this.param = param;
            this.type = type;
        }
    }
}
