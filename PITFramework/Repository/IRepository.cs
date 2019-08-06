using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Linq;

namespace PITFramework.Repository
{
    public interface IRepository<T> : IDisposable where T : class
    {
        /// <summary>
        /// Saves changes on given context against Database
        /// </summary>
        void SaveChanges();
        /// <summary>
        /// Saves changes on given context against Database
        /// </summary>
        /// <param name="option"></param>
        void SaveChanges(string sessionToken);
        /// <summary>
        /// Saves changes on given context against Database
        /// </summary>
        /// <param name="option"></param>
        void SaveChanges(SaveOptions option);
        /// <summary>
        /// Adds entity to the context
        /// </summary>
        /// <param name="entity">Entity</param>
        void Add(T entity);
         /// <summary>
        /// Adds entities to the context
        /// </summary>
        /// <param name="entities">Entities</param>
        void AddAll(IEnumerable<T> entities);
        /// <summary>
        /// Edits entity on the context
        /// </summary>
        /// <param name="entity">Entity</param>
        void Edit(T entity);
        /// <summary>
        /// Deletes entity from the context
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);
        /// <summary>
        /// Deletes entity or entities from the context based on given predicate
        /// </summary>
        /// <param name="predicate">where clause</param>
        void Delete(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Deletes entities from the context
        /// </summary>
        /// <param name="entities">Entities</param>
        void DeleteAll(List<T> entities);
        /// <summary>
        /// Deletes entity and related entities from the context
        /// </summary>
        /// <param name="entity">Entity</param>
        void DeleteRelatedEntities(T entity);
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>IEnumerable of entities</returns>
        IQueryable<T> GetAll();
        /// <summary>
        /// Finds entity based on given predicate
        /// </summary>
        /// <param name="predicate">where clause</param>
        /// <returns>IEnumerable of entities</returns>
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Gets single entity
        /// </summary>
        /// <param name="predicate">where clause</param>
        /// <returns>Only one entity</returns>
        T Single(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Gets first entity
        /// </summary>
        /// <param name="predicate">where clause</param>
        /// <returns>First Entity</returns>
        T First(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Gets count
        /// </summary>
        /// <returns>count of entities</returns>
        int Count();
        /// <summary>
        /// Gets count based on given criteria
        /// </summary>
        /// <param name="criteria">where clause</param>
        /// <returns>count of entities</returns>
        int Count(Expression<Func<T, bool>> criteria);
    }
}
