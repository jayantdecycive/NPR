using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.Models.Paging
{
    public class FilterCriteria
    {
        public string filterName { get; set; }
        public string filterCondition { get; set; }
        public object filterValue { get; set; }
    }
}
