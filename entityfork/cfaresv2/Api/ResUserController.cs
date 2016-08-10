using System.Data.Entity;
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
using cfares.site.modules.user;
using cfares.domain.user;
using System.Collections.ObjectModel;

namespace cfaresv2.Api
{


    public class ResUserController : RepositoryEntitySetController<UserMembershipRepository, ResUser, int>
    {


        protected override UserMembershipRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new UserMembershipRepository(User,context);
        }

        protected override IQueryable<ResUser> GetAllQueryable(UserOperationRole role,ResUser owner)
        {
            if (role == UserOperationRole.Admin)
            {
                return base.GetAllQueryable(role);
            }
            else if (role == UserOperationRole.Operator)
            {
                return base.GetAllQueryable(role).Include("UserPreferredLocationSubscriptions.Store").Where(x => x.UserPreferredLocationSubscriptions.Any(s => s.Store.OperatorId == owner.UserId));
            }
            else if (role == UserOperationRole.Customer)
            {
                return base.GetAllQueryable(role).Where(x => x.UserId == owner.UserId);
            }
            return null;
        }

        protected override IQueryable<ResUser> GetPublicQueryable()
        {
            return new Collection<ResUser>().AsQueryable();
        }
    }
}
