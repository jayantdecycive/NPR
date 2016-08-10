﻿using System.Collections.ObjectModel;
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


    public class TicketController : RepositoryEntitySetController<TicketRepository, Ticket, int,ITicket>
    {

        protected override TicketRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new TicketRepository(context);
        }

        protected ODataQueryValidator GetQueryValidator()
        {
            return null;
        }

        protected override IQueryable<ITicket> GetAllQueryable(UserOperationRole role,ResUser owner)
        {
            var query = base.GetAllQueryable(role).Include("Owner").Include("Owner.Address").Include("Slot");

            if (role == UserOperationRole.Admin)
            {
                return query;
            }else if (role == UserOperationRole.Operator)
            {
                return query.Include("Slot.Occurrence.Store").Where(x => x.Slot.Occurrence.Store.OperatorId == owner.UserId||
                    x.OwnerId==owner.UserId);
            }
            else
            {
                return query.Where(x=>x.OwnerId==owner.UserId);
            }
        }

        protected override IQueryable<ITicket> GetPublicQueryable()
        {
            return new Collection<Ticket>().AsQueryable();
        }

        
    }
}
