using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyErp.Core.Models;

namespace MyErp.Core.Interfaces
{
    public interface ICmd<T> where T : Common
    {
        Task<T> Add(T entity);
        Task<List<T>> Add(List<T> entities);
        Task<T> Update(T entity);
        Task<List<T>> Update(List<T> entities);
        Task<T> Delete(T entity);
        Task<List<T>> Delete(List<T> entity);
        Task<T> DeletePhysical(T entity);
        Task<List<T>> DeletePhysical(List<T> entities);
        Task<List<T>> DeletePhysical(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression, params string[] includes);
        Task<T> GetFirst();
        Task<T> GetFirst(Expression<Func<T, bool>> expression);
        Task<T> GetFirst(Expression<Func<T, bool>> expression, params string[] includes);
        Task<T> GetFirst(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
        Task<T> GetLast();
        Task<T> GetLast(Expression<Func<T, bool>> expression, params string[] includes);
        Task<T> GetLast(Expression<Func<T, bool>> expression, Expression<Func<T, int>> OrderByExpression, params string[] includes);
        Task<T> GetLast(Expression<Func<T, bool>> expression);
        Task<T> GetLast(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> where, params string[] includes);
        Task<T> GetById(int id);
        Task<T> GetById(Expression<Func<T, bool>> expression, params string[] includes);
        IEnumerable<object> Where(Func<object, bool> value);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        object Include(Func<object, object> value);
    }
}