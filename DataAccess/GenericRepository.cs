using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess
{
    /// <summary>
    /// Repositorio Genérico para acceder a nuestra Capa de Acceso a Datos
    /// <para>
    /// Arquitectura N-Capas
    /// </para>
    /// <example> 
    /// <code>
    /// var repository = new GenericRepository();
    /// </code>
    /// </example> 
    /// <remarks>
    /// Autor: Juan David Nicholls Cardona
    /// Fecha: 2014/03/01
    /// </remarks>
    /// </summary>
    public class GenericRepository : IGenericRepository
    {
        private DataContext _context;
        ///<summary>
        /// Acceder a la Base de Datos mediante nuestro Contexto, garantizando una única instancia
        ///</summary>
        private DataContext Context
        {
            get { return _context ?? (_context = new DataContext()); }
        }

        #region [ CRUD Table ]
        ///<summary>
        /// Crear un nuevo registro
        ///</summary>
        /// <example> 
        /// <code>
        /// var user = repository.Create&lt;User&gt;(user);
        /// </code>
        /// </example> 
        /// <param name="entity">Nuevo registro con los datos requeridos</param>
        public TEntity Create<TEntity>(TEntity entity)
            where TEntity : class
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
            return entity;
        }

        ///<summary>
        /// Consultar un registro mediante una condición
        ///</summary>
        /// <example> 
        /// <code>
        /// var user = repository.Read&lt;User&gt;(x=> x.FirstName == "Juan David");
        /// </code>
        /// </example> 
        /// <param name="criteria">Condición de la Consulta</param>
        public TEntity Read<TEntity>(Expression<Func<TEntity, bool>> criteria)
            where TEntity : class
        {
            return Context.Set<TEntity>().FirstOrDefault(criteria);
        }

        ///<summary>
        /// Consultar un registro mediante la clave primaria
        ///</summary>
        /// <example> 
        /// <code>
        /// var user = repository.ReadById&lt;User&gt;(123);
        /// </code>
        /// </example> 
        /// <param name="id">Clave Primaria</param>
        public TEntity ReadById<TEntity>(object id)
            where TEntity : class
        {
            return Context.Set<TEntity>().Find(id);
        }

        ///<summary>
        /// Consultar registros mediante una condición, obtener la cantidad total, paginar e incluir la consulta de entidades relacionadas
        ///</summary>
        /// <example> 
        /// <code>
        /// var users = repository.ReadById&lt;User&gt;(x=> x.FirstName == "Juan David", out total, 0, 100, x => x.UserProducts);
        /// </code>
        /// </example> 
        /// <param name="criteria">Condición de la Consulta</param>
        /// <param name="total">Cantidad Total de registros</param>
        /// <param name="index">Posición Inicial</param>
        /// <param name="size">Cantidad de registros a obtener</param>
        /// <param name="includes">Entidades relacionadas a incluir en la Consulta</param>
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
        /// Actualizar un registro
        ///</summary>
        /// <example> 
        /// <code>
        /// bool updated = repository.Update&lt;User&gt;(user);
        /// </code>
        /// </example> 
        /// <param name="entity">Un registro a actualizar</param>
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
        /// Borrar un registro
        ///</summary>
        /// <example> 
        /// <code>
        /// bool deleted = repository.Delete&lt;User&gt;(user);
        /// </code>
        /// </example> 
        /// <param name="entity">Un registro a eliminar</param>
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
        /// Consultar datos mediante un Procedimiento Almacenado
        ///</summary>
        /// <example> 
        /// <code>
        /// var users = repository.SqlQuery&lt;User&gt;("spEjemplo {0}, {1}", 1, "hola");
        /// </code>
        /// </example> 
        /// <param name="query">Consulta</param>
        /// <param name="parameters">Datos de la Consulta</param>
        public List<TEntity> SqlQuery<TEntity>(string query, params object[] parameters)
            where TEntity : class
        {
            return Context.Database.SqlQuery<TEntity>(query, parameters).ToList();
        }

        ///<summary>
        /// Ejecutar un Procedimiento Almacenado y obtener la cantidad de filas afectadas
        ///</summary>
        /// <example>
        /// <code>
        /// repository.ExecuteSqlCommand("EXEC spEjemplo {0}, {1}", 1, "hola");
        /// </code>
        /// </example>
        /// <param name="query">Consulta</param>
        /// <param name="parameters">Datos de la Consulta</param>
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
