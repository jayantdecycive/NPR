using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfares.domain._event;
using System.Xml;
using System.IO;

namespace cfares.file.dao._event
{
    public class ResEventTemplateXmlAccess:XmlAccessObject<ResTemplate>
    {
        public ResEventTemplateXmlAccess(string root):base(root){
    
        }

        public ResEventTemplateXmlAccess(Uri root)
            : base(root)
        {

        }

        public override ResTemplate[] Search(KeyValuePair<string, string>[] criteria) { return null; }
        

        

       
    }
}
