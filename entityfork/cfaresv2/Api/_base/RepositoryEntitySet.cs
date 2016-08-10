using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Query.Validators;
using Ninject;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfares.site.modules.user;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using cfares.site.modules.com.application;

namespace cfaresv2.Api._base
{
    
    public abstract class RepositoryEntitySetController<TRepository, TEntity, TKey> : RepositoryEntitySetController<TRepository, TEntity, TKey, TEntity>
        where TEntity : class
        where TRepository : GenericRepository<IResContext, TEntity, TKey, TEntity>
    {
    }

    public abstract class RepositoryEntitySetController<TRepository, TEntity, TKey, ITEntity> : EntitySetController<TEntity, TKey> 
        where TEntity : class,ITEntity
        where TRepository : GenericRepository<IResContext, TEntity, TKey, ITEntity>
    {
        protected TRepository repo;
        protected UserMembershipRepository membershipService;


        protected virtual Dictionary<string, Func<ITEntity, dynamic>> OrderByAlt()
        {
            return new Dictionary<string, Func<ITEntity, dynamic>>();
        }

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {

            IResContext context = ReservationConfig.GetContext();
            repo = GetRepository(context,controllerContext);
            membershipService = new UserMembershipRepository(User, context);
            base.Initialize(controllerContext);
        }
        protected abstract TRepository GetRepository(IResContext context,HttpControllerContext httpController=null);

        protected virtual System.Linq.IQueryable<ITEntity> GetAllQueryable(UserOperationRole role,ResUser owner) {
            return GetAllQueryable(role);
        }

        protected virtual System.Linq.IQueryable<ITEntity> GetAllQueryable(UserOperationRole role)
        {
            return repo.GetAll();
        }

        protected virtual System.Linq.IQueryable<ITEntity> GetPublicQueryable()
        {
            return repo.PublicQuerySet();
        }

        protected virtual ODataValidationSettings GetValidationSettings()
        {
            return new ODataValidationSettings()
            {
                MaxNodeCount = 300000,
                AllowedArithmeticOperators = AllowedArithmeticOperators.All,
                AllowedQueryOptions = AllowedQueryOptions.All,// | AllowedQueryOptions.Expand,
                AllowedFunctions = AllowedFunctions.All,
                AllowedLogicalOperators = AllowedLogicalOperators.All,
                MaxAnyAllExpressionDepth = 1
            };
        }

        private IQueryable<TEntity> ValidateQuery(ODataQueryOptions<TEntity> options, IQueryable<TEntity> query)
        {
            var settings = GetValidationSettings();

            options.Validate(settings);

            return options.ApplyTo(query) as IQueryable<TEntity>;
        }

        // GET api/store
        //[Queryable(MaxNodeCount = 300000,AllowedQueryOptions=AllowedQueryOptions.All)]
        [Queryable(MaxNodeCount = 300000, AllowedQueryOptions = AllowedQueryOptions.All)]
        public override System.Linq.IQueryable<TEntity> Get()
        {
            
            System.Linq.IQueryable<ITEntity> result;

            if (membershipService.IsAuthenticated && (int)membershipService.Current.OperationRole>1)
                result = GetAllQueryable(membershipService.Current.OperationRole, membershipService.Current);
            else
                result = GetPublicQueryable();

            
            /*var orderByAlt = Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key.ToLower() == "orderby");
            if (orderByAlt.Value != null)
            {
                var altDict = OrderByAlt();
                string[][] orders = orderByAlt.Value.Split(',').Select(x => x.Trim().Split(' ')).ToArray();
                foreach (var order in orders)
                {
                    var param = order[0];
                    bool desc = order[1].ToLower() == "desc";
                    if (desc)
                    {
                        return result.OrderByDescending(altDict[param]) as System.Linq.IQueryable<TEntity>;
                    }
                    else
                    {
                        return result.OrderBy(altDict[param]) as System.Linq.IQueryable<TEntity>;
                    }
                }
            }*/

            //return ValidateQuery(QueryOptions,result as System.Linq.IQueryable<TEntity>);

            //return (result as System.Linq.IQueryable<TEntity>);
	        return result.ToList().AsQueryable().Cast<TEntity>();
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