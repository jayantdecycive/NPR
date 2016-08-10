using Ninject;
using cfares.domain.user;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using cfares.entity.dbcontext.res_event;

namespace cfares.repository._base
{
    public abstract class GenericRepository<TC, T, TK> :
        GenericRepository<TC,T,TK,T>
        where T : class
        where TC : IResContext
    {
	    protected GenericRepository()
            : base()
        {
        }

	    protected GenericRepository(TC context):base(context)
        {
        }
    }

    public abstract class GenericRepository<TC, T, TK, TIt> :
    IGenericRepository<TC,TIt,TK>
        where T : class,TIt 
        where TC : IResContext
    {
	    protected TC _entities;

	    protected GenericRepository() {}
        protected GenericRepository(IKernel kernel) : this(kernel.Get<TC>()) { }
	    protected GenericRepository(TC context)
		{
            _entities = context;
            ContextSavedChanges += context_SavedChanges;
            SavedEvent += OnSave;
            DeletedEvent += OnDelete;
            ContextLoadedEntity += context_LoadedEntity;
            LoadedEvent += OnLoad;

			// Required for change tracking in OnSaved events
			((IObjectContextAdapter)_entities).ObjectContext.DetectChanges();
        }

        public void Detatch(TIt existing)
        {
            _entities.Entry((T)existing).State = EntityState.Detached;
        }

        public TC Context
        {

            get { return _entities; }
            set { _entities = value; }
        }
        
        public virtual TIt Find(TK key)
        {
            return _entities.Set<T>().Find(key);
        }

        public virtual IQueryable<TIt> UserQuerySet(ResUser user)
        {
            return GetAll();
        }

        public virtual IQueryable<TIt> PublicQuerySet(){
            return GetAll();
        }

        public virtual TIt Find(Expression<Func<TIt, bool>> keySelector, string include)
        {
            return Find(keySelector, new[] { include });
        }

        public virtual string GenSlug(string name)
        {
            string slug = Slugify(name);
            string finalslug;
            int i = 0;
            do{

				if (i > 0)
                    finalslug = slug + i;
                else
                    finalslug = slug;

				i++;
			
			// ReSharper disable CompareNonConstrainedGenericWithNull
			} while( FindBySlug( finalslug ) != null );
			// ReSharper restore CompareNonConstrainedGenericWithNull

			return finalslug;
        }

        public virtual string Slugify(string name) {
            string slug = name.ToLower();
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            
            slug = Regex.Replace(slug, @"^[^0-9a-z]", "");
            
            slug = Regex.Replace(slug, @"[^0-9a-z]$", "");
            
            slug = Regex.Replace(slug, @"[^0-9a-z_\-\s]", "").Replace("_", "-");
            slug = Regex.Replace(slug, @"\s", "-");
            return slug;
        }

        public abstract TIt FindBySlug(string slug);

        public virtual TIt Find(Expression<Func<TIt, bool>> keySelector, string[] include)
        {
            DbQuery<T> q = (DbSet<T>) GetAll();
            foreach (string i in include)
                q = q.Include(i);

            
            return q.FirstOrDefault(TurnKeySelector(keySelector));
        }

        private static Expression<Func<T, bool>> TurnKeySelector(Expression<Func<TIt, bool>> keySelector)
        {
            return DelegateConversionVisitor.Convert<TIt, T, bool>(keySelector);
        }

        public virtual IQueryable<TIt> GetAll()
        {
            IQueryable<T> query = _entities.Set<T>();
            return query;
        }

        public virtual IQueryable<TIt> GetAll(string with)
        {
            return GetAll(new[]{with});
        }
        public virtual IQueryable<TIt> GetAll(string[] with) {
            DbQuery<T> q = (DbSet<T>) GetAll();
            foreach (string i in with)
                q = q.Include(i);
            return (q);
        }

        public virtual IQueryable<TIt> Get(Expression<Func<TIt, bool>> predicate)
        {
            IQueryable<T> query = _entities.Set<T>().Where(TurnKeySelector(predicate));
            return query;
        }

        public virtual IQueryable<TIt> Get(Expression<Func<TIt, bool>> predicate,string with)
        {
            return Get(predicate, new[] { with });
        }
        public virtual IQueryable<TIt> Get(Expression<Func<TIt, bool>> predicate, string[] with)
        {
            DbQuery<T> q = (DbSet<T>) GetAll();
            foreach (string i in with)
                q = q.Include(i);
            return q.Where(TurnKeySelector(predicate));
        }

        public TIt Find(Expression<Func<TIt, bool>> predicate)
        {
            T query = _entities.Set<T>().FirstOrDefault(TurnKeySelector(predicate));
            return query;
        }

        public virtual void Add(TIt entity)
        {
            _entities.Set<T>().Add((T)entity);
        }

        public virtual void Delete(TIt entity)
        {
            _entities.Set<T>().Remove((T)entity);           
        }

        public virtual bool Save(TIt entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Edit(TIt entity)
        {
            _entities.Entry((T)entity).State = EntityState.Modified;
        }

        public virtual void OnSave(object sender, SavedEventArgs<T> args)
        {

        }

        public virtual void OnDelete(object sender, DeletedEventArgs<T> args)
        {

        }


        public virtual IEnumerable<DbEntityValidationResult> Commit() {
            return Commit(false);
        }
        public virtual IEnumerable<DbEntityValidationResult> Commit(bool robust)
        {
            if (robust)
            {
                try
                {
                    _entities.SaveChanges();
                    return null;
                }
                catch (DbEntityValidationException ex)
                {
                    return ex.EntityValidationErrors;
                }
            }
			

			_entities.SaveChanges();
	        return null;
        }

		public virtual void Cache(TIt entity)
		{
			throw new NotImplementedException();
		}

        public virtual void Forget(TIt entity)
        {
            throw new NotImplementedException();
        }

        /*Saving events*/
        public event EventHandler ContextSavedChanges
        {
            add
            {
                ((IObjectContextAdapter)_entities).ObjectContext.SavingChanges += value;
            }
            remove
            {
                ((IObjectContextAdapter)_entities).ObjectContext.SavingChanges -= value;
            }
        }

       

        public delegate void SavedEventHandler(object sender, SavedEventArgs<T> e);

        public delegate void DeletedEventHandler(object sender, DeletedEventArgs<T> e);

        public event SavedEventHandler SavedEvent;

        public event DeletedEventHandler DeletedEvent;
        

        protected virtual void context_SavedChanges(object sender, EventArgs e)
        {
            ObjectContext context = sender as ObjectContext;
            if (context != null)
            {                
                foreach (ObjectStateEntry entry in
                    context.ObjectStateManager.GetObjectStateEntries(
                    EntityState.Added | EntityState.Modified))
                {
                    if (entry.Entity == null)
                        continue;

					// Get true type ( in the case of proxy objects )
					// SH - entitytType fix from [ Type entityType = entry.Entity.GetType(); ]
					Type entityType = ObjectContext.GetObjectType( entry.Entity.GetType() );

                    if (!entry.IsRelationship && (entityType == typeof(T)))
                    {
                        T t = entry.Entity as T;
                        bool created = entry.State == EntityState.Added;
                        SavedEvent(sender, new SavedEventArgs<T> { Instance = t, Created = created, ObjectState = entry });
                    }
                }

                foreach (ObjectStateEntry entry in
                    context.ObjectStateManager.GetObjectStateEntries(
                    EntityState.Deleted))
                {
                    if (entry.Entity == null)
                        continue;

                    // Get true type ( in the case of proxy objects )
                    // SH - entitytType fix from [ Type entityType = entry.Entity.GetType(); ]
                    Type entityType = ObjectContext.GetObjectType(entry.Entity.GetType());

                    if (!entry.IsRelationship && (entityType == typeof(T)))
                    {
                        T t = entry.Entity as T;
                        DeletedEvent(sender, new DeletedEventArgs<T> { Instance = t, ObjectState = entry });
                    }
                }
            }
        }        
        /*End Saving events*/

        /*Loading Events*/
        public event ObjectMaterializedEventHandler ContextLoadedEntity
        {
            add
            {
                ((IObjectContextAdapter)_entities).ObjectContext.ObjectMaterialized += value;
            }
            remove
            {
                ((IObjectContextAdapter)_entities).ObjectContext.ObjectMaterialized -= value;
            }
        }

        public delegate void LoadedEventHandler(object sender, LoadedEventArgs<T> e);

        public event LoadedEventHandler LoadedEvent;


        protected virtual void context_LoadedEntity(object sender, ObjectMaterializedEventArgs e)
        {
            if(e.Entity is T)
            LoadedEvent(sender, new LoadedEventArgs<T> { Instance = e.Entity as T });
        }

        public virtual void OnLoad(object sender, LoadedEventArgs<T> args)
        {

        }
        /*End Loading Events*/
    }

    public class SavedEventArgs<T> : EventArgs
    {
        public bool Created;
        public T Instance;
	    public ObjectStateEntry ObjectState;
        
    }

    public class DeletedEventArgs<T> : EventArgs
    {
        public T Instance;
        public ObjectStateEntry ObjectState;

    }

    public class LoadedEventArgs<T> : EventArgs
    {        
        public T Instance;
    }

    public sealed class DelegateConversionVisitor : ExpressionVisitor
    {
	    readonly IDictionary<ParameterExpression, ParameterExpression> _parametersMap;

        public static Expression<Func<T2, TResult>> Convert<T1, T2, TResult>(Expression<Func<T1, TResult>> expr)
        {
            var parametersMap = expr.Parameters
                .Where(pe => pe.Type == typeof(T1))
                .ToDictionary(pe => pe, pe => Expression.Parameter(typeof(T2)));

            var visitor = new DelegateConversionVisitor(parametersMap);
            var newBody = visitor.Visit(expr.Body);

            var parameters = expr.Parameters.Select(visitor.MapParameter);

            return Expression.Lambda<Func<T2, TResult>>(newBody, parameters);
        }

        public DelegateConversionVisitor(IDictionary<ParameterExpression, ParameterExpression> parametersMap)
        {
            _parametersMap = parametersMap;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(MapParameter(node));
        }

        private ParameterExpression MapParameter(ParameterExpression source)
        {
            ParameterExpression target;
            _parametersMap.TryGetValue(source, out target);

            return target;
        }
    }
}