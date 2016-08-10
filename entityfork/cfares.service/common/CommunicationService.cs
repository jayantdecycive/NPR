using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.shared.domain.common;
using cfacore.shared.domain._base;
using cfacore.mysql.dao.shared.CommunicationMySqlAccess;

namespace cfares.service.common
{

    public class CommunicationException : Exception {
        public CommunicationException(string message) : base(message) { }
    }

    public class CommunicationService:DomainService<Communication>
    {
        public CommunicationMySqlAccess SqlDao = null;

        public CommunicationService(string connectionString)
        {
            SqlDao = new CommunicationMySqlAccess(connectionString);
            //Saved += new SaveEventHandler(ApplySlotCapacity);
        }

        public CommunicationService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new CommunicationMySqlAccess(connectionString);
            connectionString = cfares.service.ConfigManager.ElastiCacheConnectionString;
            //Saved += new SaveEventHandler(ApplySlotCapacity);
        }

        public override bool Save(Communication obj)
        {
            bool isNew = false;
            if (!obj.IsBound())
            {                
                isNew = true;
            }
            
            this.OnBeforeSave(new DomainServiceEventArgs { target = obj, isNew = isNew });
            bool success = SqlDao.Save(obj);
            this.OnSave(new DomainServiceEventArgs { target = obj, isNew = isNew, success = success });
            return success;
            
        }

        
      

        public override Communication Load(string ID)
        {
            Communication obj = SqlDao.Load(ID);
            this.OnLoad(new DomainServiceEventArgs{target=obj});
            return obj;
        }

        public override Communication Load(Uri uri)
        {
            Communication obj = SqlDao.Load(uri);
            this.OnLoad(new DomainServiceEventArgs { target = obj });
            return obj;
        }




        public override bool Delete(Communication obj)
        {
            
            bool success = SqlDao.Delete(obj);
            this.OnDelete(new DomainServiceEventArgs { target = obj,success=success });
            return success;
        }



        public override List<Communication> GetAll()
        {

            throw new NotImplementedException();
        }

        public override Communication[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }


        public bool MailAllowed(string Email, Uri uri) {
            Communication c = new Communication();
            c.Email = Email;
            c.EmailUri = uri;
            return !SqlDao.CheckExists(c);
        }

        public bool MailSent(string Email, Uri uri) {
            Communication c = new Communication();
            c.Email = Email;
            c.EmailUri = uri;
            c.CreationDate = DateTime.Now;
            return Save(c);
        }


    }
}
