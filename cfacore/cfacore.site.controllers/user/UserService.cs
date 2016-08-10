using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.site.controllers._base;
using cfacore.domain.user;

namespace cfacore.site.controllers.user
{
    public class UserService : DomainCacheService<CoreUser>
    {

        public override bool Cache(CoreUser user)
        {
            throw new NotImplementedException();
        }

        public override bool CacheAndSave(CoreUser user)
        {
            throw new NotImplementedException();
        }

        public override CoreUser DeCacheOrLoad(Uri uri)
        {
            throw new NotImplementedException();
        }

        public override CoreUser DeCache(Uri uri)
        {
            throw new NotImplementedException();
        }

        public override bool Save(CoreUser obj)
        {
            throw new NotImplementedException();
        }

        public override CoreUser Load(string ID)
        {
            throw new NotImplementedException();
        }

        public override CoreUser Load(Uri uri)
        {
            throw new NotImplementedException();
        }





        public override CoreUser DeCacheOrLoad(string ID)
        {
            throw new NotImplementedException();
        }

        public override CoreUser DeCache(string ID)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(CoreUser obj)
        {
            throw new NotImplementedException();
        }

        public override bool Forget(CoreUser obj)
        {
            throw new NotImplementedException();
        }

        public override CoreUser[] Search(KeyValuePair<string, string>[] criteria)
        {
            throw new NotImplementedException();
        }

        public override List<CoreUser> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}