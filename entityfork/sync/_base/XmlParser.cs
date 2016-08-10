using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace sync._base
{
    public abstract class XmlParser<T>:IXmlParser<T>
    {

        public abstract T ParseElement(System.Xml.XmlNode node);
        public virtual System.Xml.XmlDocument LoadXml(string src)
        {
            XmlDocument doc = new XmlDocument();
            try {
                doc.Load(src);
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
            return doc;
        }

        public abstract int Iterate(System.Xml.XmlNode node, int index);

        public abstract bool Main(System.Xml.XmlDocument doc);

        public abstract bool Init();
        public abstract bool ClearTables();

        public virtual bool Init(string src)
        {
            XmlDocument doc = LoadXml(src);
            return Main(doc);
        }

        protected static Random random = new Random((int)DateTime.Now.Ticks);
        protected string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        protected string Urlify(string str) 
        {
            return Regex.Replace(str.Replace('+', ' '), @"[^A-Za-z0-9 ]", "").Trim().Replace(' ', '-').Replace("--", "-");
        }
        protected string Domify(string str)
        {
            return Regex.Replace(Urlify(str).ToLower(),@"-","_");
        }
    }
}
