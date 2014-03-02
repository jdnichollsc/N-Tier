using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess
{
    interface IGenericRepository : IDisposable
    {
        TEntity Create<TEntity>(TEntity entity) where TEntity : class;
        TEntity Read<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;
        TEntity ReadById<TEntity>(object id) where TEntity : class;
        IQueryable<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criteria,
                                                   out int total,
                                                   int index = 0,
                                                   int size = 0,
                                                   params Expression<Func<TEntity, object>>[] includes) where TEntity : class;
        bool Update<TEntity>(TEntity entity) where TEntity : class;
        bool Delete<TEntity>(TEntity entity) where TEntity : class;
        List<TEntity> SqlQuery<TEntity>(string query, params object[] parameters) where TEntity : class;
        int ExecuteSqlCommand(string query, params object[] parameters);
    }
}
