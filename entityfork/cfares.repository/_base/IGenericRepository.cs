using cfares.entity.dbcontext.res_event;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace cfares.repository._base
{
    public interface IGenericRepository<C,IT,K>
        //where IT : class
        where C : IResContext
    {

        IQueryable<IT> GetAll();
        IQueryable<IT> GetAll(string include);
        IQueryable<IT> GetAll(string[] include);
        IQueryable<IT> Get(Expression<Func<IT, bool>> predicate);
        IQueryable<IT> Get(Expression<Func<IT, bool>> predicate, string with);
        IQueryable<IT> Get(Expression<Func<IT, bool>> predicate, string[] with);
        C Context { get; set; }
        IT Find(K key);
        IT Find(Expression<Func<IT, bool>> predicate);
        IT Find(Expression<Func<IT, bool>> predicate, string[] includes);
        void Add(IT entity);
        void Delete(IT entity);
        void Edit(IT entity);
        bool Save(IT entity);
        IEnumerable<DbEntityValidationResult> Commit();
        IEnumerable<DbEntityValidationResult> Commit(bool robust);
    }
}
