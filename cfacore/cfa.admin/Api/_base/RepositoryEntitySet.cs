using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.OData;
using cfa.admin.context.cfa_com;
using cfares.repository._base;

namespace cfa.admin.Api._base
{
    public abstract class RepositoryEntitySetController<TRepository, TEntity, TKey> : RepositoryEntitySetController<TRepository, TEntity, TKey, TEntity>
        where TEntity : class
        where TRepository : GenericRepository<CfaComContext, TEntity, TKey, TEntity>
    {
    }

    public abstract class RepositoryEntitySetController<TRepository, TEntity, TKey, ITEntity> : EntitySetController<TEntity, TKey>
        where TEntity : class,ITEntity
        where TRepository : GenericRepository<CfaComContext, TEntity, TKey, ITEntity>
    {
        protected TRepository repo;
        //protected UserMembershipRepository membershipService;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            var context = new CfaComContext();
            repo = GetRepository(context);
            //membershipService = new UserMembershipRepository(User, context);
            base.Initialize(controllerContext);
        }
        protected abstract TRepository GetRepository(CfaComContext context);

        /*protected virtual System.Linq.IQueryable<ITEntity> GetAllQueryable(UserOperationRole role, ResUser owner)
        {
            return GetAllQueryable(role);
        }*/

        /*protected virtual System.Linq.IQueryable<ITEntity> GetAllQueryable(UserOperationRole role)
        {
            return repo.GetAll();
        }*/

        protected virtual System.Linq.IQueryable<ITEntity> GetPublicQueryable()
        {
            return repo.PublicQuerySet();
        }

        // GET api/store
        //[Queryable(MaxNodeCount = 300000,AllowedQueryOptions=AllowedQueryOptions.All)]
        [System.Web.Http.Queryable(MaxNodeCount = 300000)]
        public override System.Linq.IQueryable<TEntity> Get()
        {

            System.Linq.IQueryable<ITEntity> result;

            /*if (membershipService.IsAuthenticated && (int)membershipService.Current.OperationRole > 1)
                result = GetAllQueryable(membershipService.Current.OperationRole, membershipService.Current);
            else*/
            
            result = GetPublicQueryable();

            return result as System.Linq.IQueryable<TEntity>;
        }

        protected override TEntity GetEntityByKey(TKey id)
        {
            ITEntity result = repo.Find(id);
            return result as TEntity;
        }

        protected override TEntity CreateEntity(TEntity entity)
        {
            repo.Add(entity);
            repo.Commit();
            return entity as TEntity;
        }

        protected override TEntity UpdateEntity(TKey key, TEntity update)
        {
            repo.Edit(update);
            repo.Commit();
            return update as TEntity;
        }

    }
}