using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace cfacore.shared.domain.user
{
    public enum PhoneType { 
        Mobile,Home,Fax
    }

    
    public interface IPhone
    {        
        
        int Number { get; set; }

        int AreaCode { get; set; }

        string Extension { get; set; }

        string Type{get;set;}
        string Carrier { get; set; }
    }
}
