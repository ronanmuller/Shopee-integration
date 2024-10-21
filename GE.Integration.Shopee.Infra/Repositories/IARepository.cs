using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GE.Integration.Shopee.Infra.Repositories
{
    public interface IARepository<TEntity> where TEntity : class
    {
        DbContext DbContext { get; }

        void Commit();
        void Detach(TEntity entity);
        void ClearChangeTracker();
        TEntity GetById(object id);
        IEnumerable<TEntity> GetAllByFilter(Expression<Func<TEntity, bool>> pFilter = null);
        TEntity Find(Expression<Func<TEntity, bool>> keys = null);
        TEntity Find(params object[] keyValues);
        IQueryable<TEntity> SelectQuery(string query, params object[] parameters);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void InsertGraphRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void Delete(params TEntity[] pObjects);
        IQueryable<TEntity> Queryable();
        Task<List<TEntity>> ToListAsync();
        IQueryable<TEntity> QueryableDetach();
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> pFilter = null);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        IIncludableQueryable<TEntity, object> Include(Expression<Func<TEntity, object>> entity);
    }
}
