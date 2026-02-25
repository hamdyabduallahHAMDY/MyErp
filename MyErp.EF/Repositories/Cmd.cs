using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.EF.DataAccess;
using Logger;
using Org.BouncyCastle.Utilities;

namespace MyErp.EF.Repositories
{
    public class Cmd<T> : ICmd<T> where T : Common
    {
        protected readonly ApplicationDbContext _context;
        public Cmd(ApplicationDbContext context)
        {
            _context = context;
        }

        //private Expression<Func<T, bool>> CheckActive()
        //{
        //    if (Info.Setting == null)
        //        return x => x.RowStatus == RowStatus.Active;
        //    else
        //    {

        //        return x => x.RowStatus == RowStatus.Active && x.RegistrationTaxFlag == Info.RegistrationTaxFlag;
        //    }
        //}
        public async Task<T> Add(T entity)
        {
            try
            {
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
                if (entities.Count != 0)
                {

                    if (entities[0].Id != 0)
                    {

                        try
                        {
                            _context.ChangeTracker.AutoDetectChangesEnabled = false;
                            await _context.Set<T>().AddRangeAsync(entities);
                            await _context.SaveChangesAsync();
                            _context.ChangeTracker.AutoDetectChangesEnabled = true;
                            return entities;
                        }
                        catch (Exception ex)
                        {

                            Logs.Log("In Add List DBContext ,", ex);
                            return null;

                        }

                    }
                    else
                    {
                        try
                        {
                            _context.ChangeTracker.AutoDetectChangesEnabled = false;
                            await _context.Set<T>().AddRangeAsync(entities);
                            await _context.SaveChangesAsync();
                            _context.ChangeTracker.AutoDetectChangesEnabled = true;
                            return entities;

                        }
                        catch (Exception ex)
                        {
                            Logs.Log("In Add List DBContext ,", ex);
                            return null;

                        }
                    }
                }
                else
                {
                    _context.ChangeTracker.AutoDetectChangesEnabled = false;
                    await _context.Set<T>().AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.AutoDetectChangesEnabled = true;
                    return entities;
                }
            }
            catch (Exception ex)
            {
                Logs.Log("In Add List DBContext ,", ex);
                return null;
            }

        }
        public async Task<T> Update(T entity)
        {
            try
            {
                _context.Entry<T>(entity).State = EntityState.Modified;
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
                var d = await _context.Set<T>().ToListAsync();
                d.Where(x => entities.Contains(x)).ToList().ForEach(x =>
                {
                    x = entities.First(dd => dd == x);
                });
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
               // entity.RowStatus = RowStatus.Delete;
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

        public async Task<List<T>> Delete(List<T> entity)
        {
            try
            {
                foreach (var item in entity)
                {

                 //   item.RowStatus = RowStatus.Delete;

                }
                return await Update(entity);

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
                var entities = _context.Set<T>().Where(expression).ToList();
                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
                return entities.ToList();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            try
            {
                if (typeof(Common).IsAssignableFrom(typeof(T)))
                    //  return await _context.Set<T>().Where(CheckActive()).ToListAsync();
                    return await _context.Set<T>().ToListAsync();
                else
                    return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(" GetAll()", ex);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            try
            {
                if (typeof(Common).IsAssignableFrom(typeof(T)))
                    return await _context.Set<T>().Where(expression).ToListAsync();
                //return await _context.Set<T>().ToListAsync();
                else
                    return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(" GetAll()", ex);
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression, params string[] includes)
        {
            try
            {

                IQueryable<T> query = _context.Set<T>();

                if (includes != null)
                    foreach (var include in includes)
                        query = query.Include(include);

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
            //return await db.Set<T>().Where(CheckActive()).Where(expression).ToListAsync();
            try
            {
                return await _context.Set<T>().Where(expression).ToListAsync();
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
                IQueryable<T> query = _context.Set<T>();
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
                T entity = await _context.Set<T>().FirstOrDefaultAsync();

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
                return await _context.Set<T>().FirstOrDefaultAsync(expression);
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
                IQueryable<T> query = _context.Set<T>();
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
                IQueryable<T> query = _context.Set<T>();
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
                T entity = await _context.Set<T>().LastOrDefaultAsync();

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
                IQueryable<T> query = _context.Set<T>();
                if (expression != null)
                {
                    query = query.Where(expression);
                }
                foreach (var item in includes)
                {
                    query = query.Include(item);

                }

                return await query.LastOrDefaultAsync();
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
                return await _context.Set<T>().LastOrDefaultAsync(expression);
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
                IQueryable<T> query = _context.Set<T>();
                if (expression != null)
                {
                    query = query.Where(expression);
                }
                foreach (var item in includes)
                {
                    query = query.Include(item);

                }

                return await query.LastOrDefaultAsync();
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
                T entity = await _context.Set<T>().FindAsync(id);

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
                IQueryable<T> query = _context.Set<T>();
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

        public async Task<T> GetLast(Expression<Func<T, bool>> expression, Expression<Func<T, int>> OrderByExpression, params string[] includes)
        {
            //try
            //{
            //    IQueryable<T> query = _context.Set<T>();
            //    if (expression != null)
            //    {
            //        query = query.Where(expression).OrderByDescending(OrderByExpression);
            //    }
            //    foreach (var item in includes)
            //    {
            //        query = query.Include(item);

            //    }

            //    return await query.LastOrDefaultAsync();
            //}
            //catch (Exception ex)
            //{
            //    Logs.Log(ex.Message);
            //    return null;
            //}
            try
            {
                var lastOrder = _context.Set<T>()
                                         .Where(expression)
                                         .OrderByDescending(OrderByExpression)
                                         .FirstOrDefault();

                return lastOrder;
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
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.Where(predicate).ToListAsync();
        }

        public object Include(Func<object, object> value)
        {
            throw new NotImplementedException();
        }


    }

}
