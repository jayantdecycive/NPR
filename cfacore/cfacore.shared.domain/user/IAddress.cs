using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Data.Linq.Mapping;
using core.synchronization.Automation;
using cfacore.shared.domain.store;
using cfacore.shared.domain.attributes;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.user;

namespace cfacore.domain.user
{
    
    public interface IAddress : IParseable<IAddress>,IDomainObject
    {
        
        string Line1
        {
            get;
            set;
        }
       
        string Line2
        {
            get;
            set;
        }

        
        string Line3
        {
            get;
            set;
        }

        
        Name Name
        {
            get;
            set;
        }
        
        Zip Zip
        {
            get;
            set;
        }

        string Label
        {
            get;
            set;
        }
        
        string City
        {
            get;
            set;
        }
        
        string State
        {
            get;
            set;
        }
       
        string County
        {
            get;
            set;
        }
       
        GeographicCoordinate Coordinates
        {
            get;
            set;
        }

        int AddressId
        {
            get;
            set;
        }
    }
}
