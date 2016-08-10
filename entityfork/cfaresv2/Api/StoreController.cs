using cfacore.shared.domain.store;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using cfares.repository.store;
using cfaresv2.Api._base;
using cfacore.shared.domain.user;
using cfares.domain.store;

namespace cfaresv2.Api
{


    public class StoreController : RepositoryEntitySetController<LocationRepository,ResStore, string>
    {


        protected override LocationRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new LocationRepository(context);
        }

        public Address GetStreetAddress([FromODataUri] string key)
        {
            return repo.Find(key).StreetAddress;
        }

        public override IQueryable<ResStore> Get()
        {
            var except = Request.GetQueryNameValuePairs().FirstOrDefault(x=>x.Key=="exclude");
            if (except.Value != null) {
                string[] where = except.Value.Split(',');
                return base.Get().Where(x => !where.Contains(x.LocationNumber));
            }
            var include = Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "include");
            if(include.Value!=null){
                string[] where = include.Value.Split(',');
                return base.Get().Where(x=>where.Contains(x.LocationNumber));
            }
            return base.Get();
        }

        public Address GetBillingAddress([FromODataUri] string key) {
            return repo.Find(key).BillingAddress;
        }

      
        protected override IQueryable<ResStore> GetPublicQueryable()
        {
 	
            var stores = repo.GetAll(new string[] { "Operator", "StreetAddress" });

            return stores;
        }

        protected override IQueryable<ResStore> GetAllQueryable(UserOperationRole role, ResUser owner)
        {
            var stores = repo.GetAll(new string[] { "Operator", "StreetAddress" });

            return stores;
        }
    }
}
