using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sync._base;
using cfacore.shared.domain.media;
using cfacore.site.controllers.shared.media;
using cfacore.mysql.dao.shared;
using System.Xml;
using System.Net;
using System.Drawing;
using System.IO;

namespace sync.media
{
    public class MediaParser:XmlParser<Media>,IMediaParser
    {
        protected MediaService mediaServ = new MediaService();
        protected MediaMySqlAccess access = null;

        public MediaParser() {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cfa-res-mysql"].ConnectionString;
            access = new MediaMySqlAccess(connectionString);
        }

        public override Media ParseElement(System.Xml.XmlNode node)
        {
            string description = null;
            string name = null;
            if (node.SelectSingleNode("./name") != null)
                name = node.SelectSingleNode("./name").InnerText;
            if (node.SelectSingleNode("./description") != null)
                description = node.SelectSingleNode("./description").InnerText;

            return mediaServ.LoadFromUri(new Uri(node.SelectSingleNode("./uri").InnerText),name,description);
        }

        
        public bool SaveMedia(Media media)
        {
            Media existingMedia = mediaServ.Load(media.MediaUri);
            if (existingMedia != null && existingMedia.IsBound()) {
                media.MediaId = existingMedia.MediaId;
                media.CreatedDate = existingMedia.CreatedDate;
            }
            return mediaServ.Save(media);
        }

        public bool[] SaveMedia(Media[] media)
        {
            List<bool> successes = new List<bool>();
            foreach (Media med in media) 
                successes.Add(SaveMedia(med));
            
            return successes.ToArray();
        }

        public Media[] ParseDirectory(System.Xml.XmlNode node)
        {
            XmlNode uriNode = node.SelectSingleNode("./uri");
            string uriStr = uriNode.InnerText;            
            string webBase = node.Attributes["webbase"].Value;
            string fileBase = node.Attributes["filebase"].Value;

            Uri[] uris = QueryLocalDirectory(new Uri(fileBase+uriStr));
            return QueryAndParseUris(uris,fileBase, webBase);
        }

        public override int Iterate(System.Xml.XmlNode node, int index)
        {
            string uriStr = node.SelectSingleNode("./uri").InnerText;
            switch (node.Name) { 
                case "directory":
                    Media[] medias = ParseDirectory(node);
                    bool[] successes = SaveMedia(medias);
                    if (medias == null)
                    {
                        Console.WriteLine(string.Format("Uri empty or unauthorized at: {0}",uriStr));
                        return index;
                    }
                    Console.WriteLine(string.Format("Parsed {0} resources at {1}",medias.Length, uriStr));
                    return medias.Length;
                case "media":
                default:
                    Media media = ParseElement(node);
                    bool success = SaveMedia(media);
                    if (!success)
                    {
                        Console.WriteLine(string.Format("Uri empty or unauthorized at: {0}", uriStr));
                        return index;
                    }
                    Console.WriteLine(string.Format("Parsed resources at {0}", uriStr));
                    return int.Parse(media.Id());                    
            }
            
        }

        public override bool Main(System.Xml.XmlDocument doc)
        {
            
            XmlNodeList singleMediaNodes = doc.DocumentElement.SelectNodes("./media|directory");
            int total = singleMediaNodes.Count;
            for (int i = 0; i < total; i++)
            {
                Iterate(singleMediaNodes[i], i);
                double percentFinished = Math.Ceiling((double)i / (double)total * 100.00);
                int Unfinished = (int)Math.Ceiling((100 - (int)percentFinished) / 4.0);
                int Finished = (int)Math.Floor(((int)percentFinished) / 4.0);
                Console.WriteLine("[" + "".PadLeft(Finished, '=') + "".PadRight(Unfinished, ' ') + "]  " + string.Format("{0}% complete", percentFinished));
            }
            return true;
        }

        public override bool Init()
        {
            string default_src = System.IO.Path.GetFullPath("../../media/data/testmedia.xml");
            return Init(default_src);
        }

        public override bool ClearTables()
        {
            throw new NotImplementedException();
        }
                

        

        public Uri[] QueryLocalDirectory(Uri uri)
        {
            string[] files = getFilesFromBaseDir(uri.ToString());
            List<Uri> uris = new List<Uri>();
            foreach (string file in files)
                uris.Add(new Uri(file));
            return uris.ToArray();
        }

        public Media[] QueryAndParseUris(Uri[] uris,string filebase, string webBase)
        {
            List<Media> medias = new List<Media>();
            foreach (Uri uri in uris)
            {
                Media loadedMedia = mediaServ.LoadFromUri(uri);
                Uri weburi = new Uri(uri.ToString().Replace(filebase, webBase));
                loadedMedia.MediaUri = weburi;
                medias.Add(loadedMedia);
            }
            return medias.ToArray();
        }

        private static string[] getFilesFromBaseDir(string baseDir)
        {
            List<string> fileResults = new List<string>();
            List<string> directories = new List<string>();

            // Add inital Directory to our ArrayList
            directories.Add(baseDir);

            // Loop while there is something in our ArrayList
            while (directories.Count > 0)
            {
                // Get directory from ArrayList
                string currentDir = directories[0].ToString();

                // Remove element from ArrayList
                directories.RemoveAt(0);

                // Add all files in this directory
                foreach (string fileName in Directory.GetFiles(currentDir.Replace(@"file:///",""), "*.*"))
                {
                    fileResults.Add(fileName);
                }

                // Add all directories in currentDir
                foreach (string dirName in Directory.GetDirectories(currentDir.Replace(@"file:///", "")))
                {
                    directories.Add(dirName);
                }
            }
            return fileResults.ToArray();
        }
    }
}
