using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using cfares.Models.Paging;

namespace cfares.Models
{
    public class PagedItemView<T>
    {
        public IEnumerable<T> items {get; set;}
        public PagingInfo pagingInfo { get; set; }
        public IFilter filter { get; set; }
    }
}