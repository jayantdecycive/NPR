using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace cfares.Models
{
    public class DataTableResponse
    {

        


        public int sEcho { get; set; }

        public int iTotalRecords { get; set; }

        public int iTotalDisplayRecords { get; set; }

        public string sColumns { get; set;  }

        public List<string[]> aaData { get; set; }
    }
}