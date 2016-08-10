using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfacore.shared.domain._base
{
    public class DomainServiceEventArgs:EventArgs,IDomainServiceEventArgs
    {
        public IDomainObject target;
        
        public bool isNew = false;
        public bool isBatch = false;
        public bool success = false;
    }
}
