using System.Data.Entity.Infrastructure;
using cfacore.shared.domain.store;
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
using cfares.repository._event;
using cfares.domain._event;
using cfares.domain.user;

namespace cfaresv2.Api
{


    public class EventController : RepositoryEntitySetController<ResEventRepository,ResEvent, int,IResEvent>
    {
        protected override IQueryable<IResEvent> GetAllQueryable(UserOperationRole role)
        {
            if (Request.GetQueryNameValuePairs().Any(x => x.Key == "stores"))
            {
                string[] stores = Request.GetQueryNameValuePairs().First(x => x.Key == "stores").Value.Split(',');
                return
                    (repo.GetAll(ResEventRepository.DefaultEntityIncludes) as DbQuery<ResEvent>).Include("Occurrences")
                                                        .Where(x => x.Occurrences.Any(o => stores.Contains(o.StoreId)));
            }

            return repo.GetAll(ResEventRepository.DefaultEntityIncludes);
        }

        protected override IQueryable<IResEvent> GetPublicQueryable()
        {
            return repo.GetAll(ResEventRepository.DefaultEntityIncludes);
        }


        protected override ResEventRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new ResEventRepository(context);
        }
    }
}
