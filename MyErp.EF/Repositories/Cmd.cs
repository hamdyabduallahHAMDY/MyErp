using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.EF.DataAccess;
using Logger;

namespace MyErp.EF.Repositories
{
    public class Cmd<T> : ICmd<T> where T : Common
    {
        protected readonly ApplicationDbContext _context;

        public Cmd(ApplicationDbContext context)
        {
            _context = context;
        }

        private Expression<Func<T, bool>> CheckActive()
        {
            return x => x.RowStatus == RowStatus.Active;
        }

        public async Task<T> Add(T entity)
        {
            try
            {
                entity.RowStatus = RowStatus.Active;

                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                return null;
            }
        }

        public async Task<List<T>> Add(List<T> entities)
        {
            try
            {
                if (entities == null)
                    return null;

                foreach (var entity in entities)
                {
                    entity.RowStatus = RowStatus.Active;
                }

                _context.ChangeTracker.AutoDetectChangesEnabled = false;

                await _context.Set<T>().AddRangeAsync(entities);
                await _context.SaveChangesAsync();

                return entities;
            }
            catch (Exception ex)
            {
                Logs.Log("In Add List DBContext ,", ex);
                return null;
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public async Task<T> Update(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<List<T>> Update(List<T> entities)
        {
            try
            {
                if (entities == null || !entities.Any())
                    return entities;

                _context.Set<T>().UpdateRange(entities);
                await _context.SaveChangesAsync();

                return entities;
            }
            catch (Exception ex)
            {
                Logs.Log("In Update List DBContext ,", ex);
                return null;
            }
        }

        public async Task<T> Delete(T entity)
        {
            try
            {
                entity.RowStatus = RowStatus.Delete;
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }
        public async Task<List<T>> Delete(Expression<Func<T, bool>> expression)
        {
            try
            {
                var entities = await _context.Set<T>()
                    .Where(CheckActive())
                    .Where(expression)
                    .ToListAsync();

                if (!entities.Any())
                    return new List<T>();

                foreach (var entity in entities)
                {
                    entity.RowStatus = RowStatus.Delete;
                }

                _context.Set<T>().UpdateRange(entities);
                await _context.SaveChangesAsync();

                return entities;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }
        public async Task<List<T>> Delete(List<T> entities)
        {
            try
            {
                if (entities == null || !entities.Any())
                    return entities;

                foreach (var item in entities)
                {
                    item.RowStatus = RowStatus.Delete;
                }

                _context.Set<T>().UpdateRange(entities);
                await _context.SaveChangesAsync();

                return entities;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<T> DeletePhysical(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<List<T>> DeletePhysical(List<T> entities)
        {
            try
            {
                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<List<T>> DeletePhysical(Expression<Func<T, bool>> expression)
        {
            try
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = false;

                var entities = await _context.Set<T>()
                    .Where(expression)
                    .ToListAsync();

                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync();

                return entities;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            try
            {
                return await _context.Set<T>()
                    .Where(CheckActive())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log("GetAll()", ex);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> GetAllByUsers(List<string> allowedUsers)
        {
            try
            {
                if (allowedUsers == null || !allowedUsers.Any())
                    return Enumerable.Empty<T>();

                return await _context.Set<T>()
                    .Where(CheckActive())
                    .Where(e => allowedUsers.Contains(e.CreatedBy))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log("GetAllByUsers()", ex);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _context.Set<T>()
                    .Where(CheckActive())
                    .Where(expression)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log("GetAll(expression)", ex);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (includes != null)
                {
                    foreach (var include in includes)
                        query = query.Include(include);
                }

                return await query.Where(expression).ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _context.Set<T>()
                    .Where(CheckActive())
                    .Where(expression)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> GetBy(Expression<Func<T, bool>> criteria, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (criteria != null)
                {
                    query = query.Where(criteria);
                }

                foreach (var item in includes)
                {
                    query = query.Include(item);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<T> GetFirst()
        {
            try
            {
                T entity = await _context.Set<T>()
                    .Where(CheckActive())
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    Logs.Log("No entity found in the database.");
                }

                return entity;
            }
            catch (Exception ex)
            {
                Logs.Log($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<T> GetFirst(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _context.Set<T>()
                    .Where(CheckActive())
                    .FirstOrDefaultAsync(expression);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<T> GetFirst(Expression<Func<T, bool>> expression, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (expression != null)
                {
                    query = query.Where(expression);
                }

                foreach (var item in includes)
                {
                    query = query.Include(item);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<T> GetFirst(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (expression != null)
                {
                    query = query.Where(expression);
                }

                foreach (var item in includes)
                {
                    query = query.Include(item);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<T> GetLast()
        {
            try
            {
                T entity = await _context.Set<T>()
                    .Where(CheckActive())
                    .OrderBy(x => x.Id)
                    .LastOrDefaultAsync();

                if (entity == null)
                {
                    Logs.Log("No entity found in the database.");
                }

                return entity;
            }
            catch (Exception ex)
            {
                Logs.Log($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<T> GetLast(Expression<Func<T, bool>> expression, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (expression != null)
                {
                    query = query.Where(expression);
                }

                foreach (var item in includes)
                {
                    query = query.Include(item);
                }

                return await query.OrderBy(x => x.Id).LastOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<T> GetLast(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _context.Set<T>()
                    .Where(CheckActive())
                    .Where(expression)
                    .OrderBy(x => x.Id)
                    .LastOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<T> GetLast(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (expression != null)
                {
                    query = query.Where(expression);
                }

                foreach (var item in includes)
                {
                    query = query.Include(item);
                }

                return await query.OrderBy(x => x.Id).LastOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<T> GetById(int id)
        {
            try
            {
                T entity = await _context.Set<T>()
                    .Where(CheckActive())
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Logs.Log("No entity found in the database.");
                }

                return entity;
            }
            catch (Exception ex)
            {
                Logs.Log($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<T> GetById(Expression<Func<T, bool>> expression, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (expression != null)
                {
                    query = query.Where(expression);
                }

                foreach (var item in includes)
                {
                    query = query.Include(item);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.Log($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<T> GetLast(Expression<Func<T, bool>> expression, Expression<Func<T, int>> orderByExpression, params string[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (expression != null)
                {
                    query = query.Where(expression);
                }

                foreach (var item in includes)
                {
                    query = query.Include(item);
                }

                return await query
                    .OrderByDescending(orderByExpression)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public IEnumerable<object> Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(CheckActive());

                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                return await query.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return Enumerable.Empty<T>();
            }
        }

        public object Include(Func<object, object> value)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetQueryable()
        {
            try
            {
                return _context.Set<T>()
                    .Where(CheckActive())
                    .AsQueryable();
            }
            catch (Exception ex)
            {
                Logs.Log("GetQueryable()", ex);
                return Enumerable.Empty<T>().AsQueryable();
            }
        }
    }
}