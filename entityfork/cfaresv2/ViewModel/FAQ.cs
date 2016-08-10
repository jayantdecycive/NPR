using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;


namespace cfaresv2.ViewModel
{
    public class FAQ
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        private string UniqueId { get; set; }

        public FAQ()
        {
            UniqueId = Guid.NewGuid().ToString().Substring(0,8).ToLower().Replace("-","");
        }

        public string HtmlId()
        {
            return Regex.Replace((Question+UniqueId).ToLower(), @"[^a-z0-9]", "-").Trim('-');
        }
    }

    public class FAQGroup
    {
        public ICollection<FAQ> FAQs { get; set; }
        public string Title { get; set; }

        public string HtmlId()
        {
            return Regex.Replace(Title.ToLower(),@"[^a-z0-9]","-").Trim('-');
        }
    }
}