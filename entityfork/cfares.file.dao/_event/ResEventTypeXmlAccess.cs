using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfares.domain._event;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace cfares.file.dao._event
{
    public class ResEventTypeXmlAccess : XmlAccessObject<ReservationType>
    {
        public ResEventTypeXmlAccess(string root)
            : base(root)
        {

        }

        public ResEventTypeXmlAccess(Uri root)
            : base(root)
        {

        }

        public override ReservationType[] Search(KeyValuePair<string, string>[] criteria)
        {

            return null;
        }
    }
}
