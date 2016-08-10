using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using System.Xml.Serialization;
using System.IO;

namespace cfacore.dao._base
{
    public abstract class XmlAccessObject<T> : DataAccessObject<T>, IXmlAccessObject<T> where T : class,IDomainObject, new()
    {
        public XmlAccessObject(string Uri)
        {
            this._RootUri = new Uri(Uri);
        }

        public XmlAccessObject(Uri Uri)
        {
            this._RootUri = (Uri);
        }

        
        private Uri _RootUri = null;

        public Uri RootUri
        {
            get { return _RootUri; }
            set { _RootUri = value; }
        }

        public List<T> LoadAll()
        {
            return LoadAll(RootUri.AbsolutePath);
        }

        public List<T> LoadAll(string dirpath)
        {
            List<T> domainObjects = new List<T>();
            string[] Paths = Directory.GetFiles(dirpath);
            foreach (string path in Paths)
            {
                T t = Load(new Uri(path));
                if (t != null)
                    domainObjects.Add(t);
            }
            return domainObjects;
        }

        public override T Load(Uri Uri)
        {
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(Uri.AbsolutePath);

                XmlSerializer serializer = new XmlSerializer(typeof(T));


                return serializer.Deserialize(file) as T;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        public override bool Save(T obj)
        {

            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(
                    RootUri.AbsolutePath + "/" + obj.Id() + ".xml");

                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                xmlSerializer.Serialize(file, obj);

                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }


        public override T Load(string ID)
        {
            return Load(new Uri(RootUri.AbsolutePath + "/" + ID + ".xml"));
        }

        public override bool Delete(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
