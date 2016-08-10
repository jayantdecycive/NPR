using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.shared.domain.user;
using cfacore.mysql.dao._base;
using cfacore.site.controllers._base;
using cfacore.mysql.dao.shared.user;


namespace cfacore.service.user
{
    public class AddressService : DomainService<Address>
    {
        public IMySqlAccess<Address> SqlDao = null;

        public AddressService(string connectionString)
        {
            SqlDao = (IMySqlAccess<Address>)new AddressMySqlAccess(connectionString);
        }


        public override bool Save(Address obj)
        {
            return SqlDao.Save(obj);
        }

        public override Address Load(string ID)
        {
            return SqlDao.Load(ID);
        }

        public override Address Load(Uri uri)
        {
            return SqlDao.Load(uri);
        }

        public override bool Delete(Address obj)
        {
            throw new NotImplementedException();
        }

        public override Address[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override List<Address> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
