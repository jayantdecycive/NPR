using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfares.domain._event;
using System.Xml;
using System.IO;
using cfares.domain._event.resevent;

namespace cfares.file.dao._event
{
    public class ResEventThemeXmlAccess:XmlAccessObject<ResEventTheme>
    {
        public ResEventThemeXmlAccess(string root):base(root){
    
        }

        public ResEventThemeXmlAccess(Uri root)
            : base(root)
        {

        }

        public override ResEventTheme[] Search(KeyValuePair<string, string>[] criteria)
        {

            return null;
        }
        
    }
}
