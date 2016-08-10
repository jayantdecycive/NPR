using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.dao._base
{
    public interface IXmlAccessObject<T>
        where T : IDomainObject, new()        
    {
        Uri RootUri { get; set; }
    }
}
