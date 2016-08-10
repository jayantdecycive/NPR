using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.mysql.dao._base;
using cfacore.site.controllers._base;


namespace cfacore.service.$safeitemname$
{
    public class $safeitemname$Service : DomainService<$safeitemname$>
    {
        public IMySqlAccess<$safeitemname$> SqlDao = null;

        public $safeitemname$Service(string connectionString)
        {
            SqlDao = (IMySqlAccess<$safeitemname$>)new $safeitemname$MySqlAccess(connectionString);
        }


        public override bool Save($safeitemname$ obj)
        {
            return SqlDao.Save(obj);
        }

        public override $safeitemname$ Load(string ID)
        {
            return SqlDao.Load(ID);
        }

        public override $safeitemname$ Load(Uri uri)
        {
            return SqlDao.Load(uri);
        }
    }
}
