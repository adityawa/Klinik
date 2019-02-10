using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Web.DataAccess
{
    public interface IGenericRepository<TEntity>
    {
        List<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includes);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

        TEntity GetById(object id);

        TEntity GetFirstOrDefault(
        Expression<Func<TEntity, bool>> filter = null,
        params Expression<Func<TEntity, object>>[] includes);

        TEntity FindBy(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(object id);
    }
}
