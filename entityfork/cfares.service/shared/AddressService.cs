using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.mysql.dao._base;
using cfacore.site.controllers._base;
using cfacore.shared.domain.user;
using cfares.mysql.dao.shared;


namespace cfacore.service
{
    public class AddressService : DomainService<Address>
    {
        public AddressMySqlAccess SqlDao = null;

        public AddressService(string connectionString)
        {
            SqlDao = new AddressMySqlAccess(connectionString);
        }

        public AddressService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new AddressMySqlAccess(connectionString);            
        }

        public override bool Save(Address obj)
        {
            return SqlDao.Save(obj);
        }

        public override Address Load(string ID)
        {
            return SqlDao.Load(ID);
        }

        public List<Address> LoadByLabel(string label)
        {
            return SqlDao.GetByLabel(label);
        }
        
        public List<Address> LoadByLine1Line2AndZip(string label,string line2,int zip)
        {
            return SqlDao.GetByLine1Line2AndZip(label, line2, zip);
        }

        

        public override Address Load(Uri uri)
        {
            return SqlDao.Load(uri);
        }

        public override bool Delete(Address obj)
        {
            return SqlDao.Delete(obj);
        }

        public override List<Address> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Address[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }
    }
}
