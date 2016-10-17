using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess
{
    /// <summary>
    /// Generic repository to access our data access layer
    /// <para>
    /// N-tier architecture
    /// </para>
    /// <example> 
    /// <code>
    /// var repository = new GenericRepository();
    /// </code>
    /// </example> 
    /// <remarks>
    /// Author: Juan David Nicholls Cardona
    /// Date: 2014/03/01
    /// </remarks>
    /// </summary>
    public class GenericRepository : IGenericRepository
    {
        private DataContext _context;
        ///<summary>
        /// Access the database through our context, ensuring a single instance
        ///</summary>
        private DataContext Context
        {
            get { return _context ?? (_context = new DataContext()); }
        }

        #region [ CRUD Table ]
        ///<summary>
        /// Create a new record
        ///</summary>
        /// <example> 
        /// <code>
        /// var user = repository.Create<User>(user);
        /// </code>
        /// </example> 
        /// <param name="entity">New record with the required data</param>
        public TEntity Create<TEntity>(TEntity entity)
            where TEntity : class
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
            return entity;
        }

        ///<summary>
        /// Get a record by a condition
        ///</summary>
        /// <example> 
        /// <code>
        /// var user = repository.Read<User>(x=> x.FirstName == "Juan David");
        /// </code>
        /// </example> 
        /// <param name="criteria">Condition of the query</param>
        public TEntity Read<TEntity>(Expression<Func<TEntity, bool>> criteria)
            where TEntity : class
        {
            return Context.Set<TEntity>().FirstOrDefault(criteria);
        }

        ///<summary>
        /// Get a record using the primary key
        ///</summary>
        /// <example> 
        /// <code>
        /// var user = repository.ReadById<User>(123);
        /// </code>
        /// </example> 
        /// <param name="id">Primary Key</param>
        public TEntity ReadById<TEntity>(object id)
            where TEntity : class
        {
            return Context.Set<TEntity>().Find(id);
        }

        ///<summary>
        /// Get records by a condition, get the total amount, paginated and include consulting related entities
        ///</summary>
        /// <example> 
        /// <code>
        /// var users = repository.ReadById<User>(x=> x.FirstName == "Juan David", out total, 0, 100, x => x.UserProducts);
        /// </code>
        /// </example> 
        /// <param name="criteria">Condition of the query</param>
        /// <param name="total">Total number of records</param>
        /// <param name="index">Initial position</param>
        /// <param name="size">Quantity of records to obtain</param>
        /// <param name="includes">Related entities to be included in the query/param>
        public IQueryable<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criteria,
                                                   out int total,
                                                   int index = 0,
                                                   int size = 0,
                                                   params Expression<Func<TEntity, object>>[] includes)
             where TEntity : class
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }
            if (criteria != null)
            {
                query = query.Where(criteria);
            }
            total = query.Count();
            if (index != 0)
            {
                query = query.Skip(index);
            }
            if (size != 0)
            {
                query = query.Take(size);
            }
            return query;
        }

        ///<summary>
        /// Update a record
        ///</summary>
        /// <example> 
        /// <code>
        /// bool updated = repository.Update<User>(user);
        /// </code>
        /// </example> 
        /// <param name="entity">A record to update</param>
        public bool Update<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (Context.Entry<TEntity>(entity).State == EntityState.Detached)
            {
                Context.Set<TEntity>().Attach(entity);
            }
            Context.Entry<TEntity>(entity).State = EntityState.Modified;
            return Context.SaveChanges() > 0;
        }

        ///<summary>
        /// Delete a record
        ///</summary>
        /// <example> 
        /// <code>
        /// bool deleted = repository.Delete<User>(user);
        /// </code>
        /// </example> 
        /// <param name="entity">A record to delete</param>
        public bool Delete<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (Context.Entry<TEntity>(entity).State == EntityState.Detached)
            {
                Context.Set<TEntity>().Attach(entity);
            }
            Context.Set<TEntity>().Remove(entity);
            return Context.SaveChanges() > 0;
        }

        #endregion

        #region [ Stored Procedures ]
        
        ///<summary>
        /// Get data using a stored procedure
        ///</summary>
        /// <example> 
        /// <code>
        /// var users = repository.SqlQuery<User>("spEjemplo {0}, {1}", 1, "hola");
        /// </code>
        /// </example> 
        /// <param name="query">Query</param>
        /// <param name="parameters">Data</param>
        public List<TEntity> SqlQuery<TEntity>(string query, params object[] parameters)
            where TEntity : class
        {
            return Context.Database.SqlQuery<TEntity>(query, parameters).ToList();
        }

        ///<summary>
        /// Execute a Stored Procedure and get the number of rows affected
        ///</summary>
        /// <example>
        /// <code>
        /// repository.ExecuteSqlCommand("EXEC spEjemplo {0}, {1}", 1, "hola");
        /// </code>
        /// </example>
        /// <param name="query">Query</param>
        /// <param name="parameters">Data</param>
        public int ExecuteSqlCommand(string query, params object[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(query, parameters);
        }

        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
