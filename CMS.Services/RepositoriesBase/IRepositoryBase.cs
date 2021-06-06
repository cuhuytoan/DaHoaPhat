using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CMS.Services.RepositoriesBase
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Add New Entity
        /// </summary>
        /// <param name="entity"></param>
        void Create(T entity);

        /// <summary>
        /// Update Entity
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        ValueTask<T> FindAsync(object id);

        T GetById(object id);

        T FirstOrDefault(Expression<Func<T, bool>> expression);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

        string FormatURL(string Title);

        Task SendMail(string FromName, string ToEmail, string ToName, string Subject, string Body);
    }
}