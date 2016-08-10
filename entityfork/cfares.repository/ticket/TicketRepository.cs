
#region Imports

using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq.Expressions;
using cfacore.domain.user;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using System;
using System.Linq;

#endregion

namespace cfares.repository.ticket
{
    public class TicketRepository : TicketRepository<Ticket> {
        public TicketRepository(IResContext context) : base(context) { }


        public override void OnDelete(object sender, DeletedEventArgs<Ticket> args)
        {
            if (args.Instance.Slot != null)
                args.Instance.Slot.InvalidateCache();
            base.OnDelete(sender, args);
        }

        public override void OnSave(object sender, SavedEventArgs<Ticket> args)
        {
            if (args.Instance.Slot != null)
                args.Instance.Slot.InvalidateCache();
            base.OnSave(sender, args);
        }

        public override void Add(ITicket entity)
        {
            if (entity.Slot != null)
                entity.Slot.InvalidateCache();
            base.Add(entity);
        }
    }

    public interface ITicketRepository
    {
        
        ITicket FindBySlug(string slug);
        IQueryable<ITicket> GetByActiveAndUser( IUser user );
        void Detatch(ITicket existing);
        IResContext Context { get; set; }
        ITicket Find(Int32 key);
        ITicket Find(Expression<Func<ITicket, bool>> keySelector, string include);
        ITicket Find(Expression<Func<ITicket, bool>> keySelector, string[] include);
        IQueryable<ITicket> GetAll();
        IQueryable<ITicket> GetAll(string with);
        IQueryable<ITicket> GetAll(string[] with);
        IQueryable<ITicket> Get(Expression<Func<ITicket, bool>> predicate);
        IQueryable<ITicket> Get(Expression<Func<ITicket, bool>> predicate,string with);
        IQueryable<ITicket> Get(Expression<Func<ITicket, bool>> predicate, string[] with);
        ITicket Find(Expression<Func<ITicket, bool>> predicate);
        void Add(ITicket entity);
        void Delete(ITicket entity);
        bool Save(ITicket entity);
        void Edit(ITicket entity);
        IEnumerable<DbEntityValidationResult> Commit();
        IEnumerable<DbEntityValidationResult> Commit(bool robust);
        void Cache(ITicket entity);
        void Forget(ITicket entity);
    }

    public class TicketRepository<TTicket> : GenericRepository<IResContext, TTicket, int,ITicket>, ITicketRepository where TTicket:class,ITicket,new()
    {
        public TicketRepository(IResContext context) : base(context) {}

		public override IQueryable<ITicket> PublicQuerySet()
        {
            return null;
        }
        
        public override ITicket FindBySlug(string slug)
        {
            throw new NotImplementedException();
        }

		// Ticket ( Owner ID ) >> Slot >> Occurence >> Event
		public IQueryable<ITicket> GetByActiveAndUser( IUser user )
        {
			DateTime now = DateTime.Now;
			return GetAll("Slot.Occurrence.ResEvent")
				.Where( o => o.OwnerId == user.UserId && o.Status!=TicketStatus.Partial && o.Status!=TicketStatus.Canceled )
                .Where(o => o.Slot.Occurrence.ResEvent.Status != ResEventStatus.Cancelled );
		}
    }
}
