using System.Collections.ObjectModel;
using System.Web.Http.OData.Query.Validators;
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
using cfacore.shared.domain.media;
using cfacore.shared.modules.repository;
using cfares.domain._event;
using cfares.repository.slot;
using cfares.repository.ticket;
using cfares.site.modules.user;
using cfaresv2.Api._base;
using System.Data.Entity;

namespace cfaresv2.Api
{


    public class TicketTransactionController : RepositoryEntitySetController<TicketTransactionRepository, TicketTransaction, string>
    {

        protected override TicketTransactionRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new TicketTransactionRepository(context);
        }

        protected ODataQueryValidator GetQueryValidator()
        {
            return null;
        }

        protected override IQueryable<TicketTransaction> GetAllQueryable(UserOperationRole role, ResUser owner)
        {
            var query = base.GetAllQueryable(role).Include("Ticket.Owner.Address").Include("Ticket.Slot");

            if (role == UserOperationRole.Admin)
            {
                return query;
            }else if (role == UserOperationRole.Operator)
            {
                return query.Include("Ticket.Slot.Occurrence.Store").Where(x => x.Ticket.Slot.Occurrence.Store.OperatorId == owner.UserId||
                    x.Ticket.OwnerId==owner.UserId);
            }
            else
            {
                return query.Where(x=>x.Ticket.OwnerId==owner.UserId);
            }
        }

        protected override IQueryable<TicketTransaction> GetPublicQueryable()
        {
            return new Collection<TicketTransaction>().AsQueryable();
        }

        
    }
}
