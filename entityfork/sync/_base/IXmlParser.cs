using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace sync._base
{
    public interface IXmlParser<T>:IDataParser<T>
    {
        T ParseElement(XmlNode node);
        XmlDocument LoadXml(string src);
        int Iterate(XmlNode node, int index);
        bool Main(XmlDocument doc);
        bool Init();
        bool Init(string src);
    }
}
