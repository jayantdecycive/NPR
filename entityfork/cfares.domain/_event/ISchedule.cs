using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.shared.domain._base;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfacore.domain._base;

namespace cfares.domain._event
{
    public interface ISchedule:IDomainObject
    {
        

        [Column]
        [DataType("jqui/_DatePicker")]
        DateTimeOffset Start { get; set; }

        [Column]
        [DataType("jqui/_DatePicker")]
        DateTimeOffset End { get; set; }

        [Column]
        int Capacity { get; set; }

        [Column]
        string Name { get; set; }

        [Column]
        string UrlName { get; set; }
        
    }
}
