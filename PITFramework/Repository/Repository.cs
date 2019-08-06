using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Data;
using System.Data.Objects.DataClasses;
using PITFramework.Auditing;

namespace PITFramework.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private ObjectContext _context;
        private readonly IObjectSet<T> _objectSet;
        private bool _auditRepository = true;

        public bool AuditRepository 
        { 
            get { return _auditRepository; }
            set { this._auditRepository = value; }
        }

        public Repository(ObjectContext context)
        {
            _context = context;
            _objectSet = _context.CreateObjectSet<T>();
        }

        public Repository(ObjectContext context, bool auditRepository)
        {
            _context = context;
            _objectSet = _context.CreateObjectSet<T>();

            _auditRepository = auditRepository;
        }

        public Repository(ObjectContext context, bool auditRepository, bool lazyLoading)
        {
            _context = context;
            _objectSet = _context.CreateObjectSet<T>();

            _auditRepository = auditRepository;
            _context.ContextOptions.LazyLoadingEnabled = lazyLoading;
        }

        public virtual void SaveChanges()
        {
            SaveChanges(Audit.GenerateNewSessionToken());
        }

        public virtual void SaveChanges(string sessionToken)
        {
            Audit auditing = new Audit(_context);
            List<int> entityPKDetail = new List<int>();
            List<int> entityPKMaster = new List<int>();
            string auditingOperation = AuditingOperation.INSERT.ToString();

            IEnumerable<ObjectStateEntry> changes = _context.ObjectStateManager
                                                            .GetObjectStateEntries(
                                                                EntityState.Added |
                                                                EntityState.Modified
                                                             );   

            if (_auditRepository)
            {
                auditing.AuditRecord(sessionToken, out entityPKMaster, out entityPKDetail, out auditingOperation);
            }

            _context.SaveChanges();

            var unchanged = changes.Where(c => c.State == EntityState.Unchanged).ToList();
           
            if (_auditRepository && entityPKDetail != null && entityPKDetail.Count != 0) auditing.UpdateInsertedEntityPKDetail(entityPKDetail, unchanged, auditingOperation);
        }

        public virtual void SaveChanges(SaveOptions options)
        {
            _context.SaveChanges(options);
        }

        public virtual void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity is null");
            }

            _objectSet.AddObject(entity);
        }

        public virtual void AddAll(IEnumerable<T> entities)
        {
            if (entities == null || entities.Count() <= 0)
            {
                throw new ArgumentNullException("Entities is null");
            }

            foreach (var entity in entities)
            {
                _objectSet.AddObject(entity);
            }
        }

        public virtual void Edit(T entity)
        {
            _objectSet.Attach(entity);
            _context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity is null");
            }

            _objectSet.DeleteObject(entity);
        }

        public virtual void DeleteAll(List<T> entities)
        {
            if (entities == null || entities.Count() <= 0)
            {
                throw new ArgumentNullException("Entities is null");
            }

            foreach (var entity in entities)
            {
                _objectSet.DeleteObject(entity);
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var records = Find(predicate);

            foreach (var record in records)
            {
                Delete(record);
            }
        }

        public virtual void DeleteRelatedEntities(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity is null");
            }

            var releatedEntities =
                ((IEntityWithRelationships)entity).RelationshipManager.GetAllRelatedEnds().SelectMany(
                    e => e.CreateSourceQuery().OfType<T>()).ToList();
            foreach (var releatedEntity in releatedEntities)
            {
                _objectSet.DeleteObject(releatedEntity);
            }
            _objectSet.DeleteObject(entity);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _objectSet.AsQueryable();
        }

        public virtual IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).AsQueryable();
        }

        public virtual T Single(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Single(predicate);
        }

        public virtual T First(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.First(predicate);
        }

        public virtual int Count()
        {
            return _objectSet.Count();
        }

        public virtual int Count(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_context == null) return;
            _context.Dispose();
            _context = null;
        }
        #endregion
    }
}
