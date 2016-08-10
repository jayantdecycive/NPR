using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.mysql.dao._base;
using cfacore.site.controllers._base;
using cfacore.shared.domain.store;
using cfacore.mysql.dao.shared;


namespace cfacore.service.shared
{
    public class DistributorService : DomainService<Distributor>
    {
        public DistributorMySqlAccess SqlDao = null;

        public DistributorService(string connectionString)
        {
            SqlDao = new DistributorMySqlAccess(connectionString);
        }

        public DistributorService()
        {
            string connectionString = cfares.service.ConfigManager.MySqlConnectionString;
            SqlDao = new DistributorMySqlAccess(connectionString);
        }


        public override bool Save(Distributor obj)
        {
            return SqlDao.Save(obj);
        }

        public override Distributor Load(string ID)
        {
            return SqlDao.Load(ID);
        }

        public Distributor LoadByShortName(string ID)
        {
            return SqlDao.LoadByShortName(ID);
        }

        public override Distributor Load(Uri uri)
        {
            return SqlDao.Load(uri);
        }

        public override bool Delete(Distributor obj)
        {
            return SqlDao.Delete(obj);
        }

        public override Distributor[] Search(KeyValuePair<string, string>[] criteria)
        {

            throw new NotImplementedException();
        }

        public override List<Distributor> GetAll()
        {
            throw new NotImplementedException();
        }
    }

}
