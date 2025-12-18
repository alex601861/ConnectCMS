using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CMSTrain.Infrastructure.Persistence;
using CMSTrain.Application.Interfaces.Repositories.Base;
  
namespace CMSTrain.Infrastructure.Implementation.Repositories;

public class GenericRepository(ApplicationDbContext dbContext): IGenericRepository
{
    public bool Exists<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class
    {
        return filter != null && dbContext.Set<TEntity>().Any(filter);
    }

    public IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "") where TEntity : class
    {
        var query = dbContext.Set<TEntity>().AsNoTracking();

        if (filter != null) query = query.Where(filter);

        query = includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return orderBy != null ? orderBy(query) : ApplyDefaultOrdering(query);
    }

    public IQueryable<TEntity> GetPagedResult<TEntity>(int pageNumber, int pageSize, out int rowsCount, Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, object>>? order = null,
         bool isAscendingOrder = true) where TEntity : class
    {
        IQueryable<TEntity> query = dbContext.Set<TEntity>();

        rowsCount = filter is not null ? query.Count(filter) : query.Count();

        if (filter is not null)
        {
            if (order is not null)
            {
                query = isAscendingOrder ?
                    query.Where(filter).OrderBy(order).Skip((pageNumber - 1) * pageSize).Take(pageSize) :
                    query.Where(filter).OrderByDescending(order).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
            {
                query = ApplyDefaultOrdering(query).Where(filter).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
        }
        else
        {
            if (order is not null)
            {
                query = isAscendingOrder ?
                    query.OrderBy(order).Skip((pageNumber - 1) * pageSize).Take(pageSize) :
                    query.OrderByDescending(order).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
            {
                query = ApplyDefaultOrdering(query).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
        }

        return query;
    }

    private IOrderedQueryable<TEntity> ApplyDefaultOrdering<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        var entityType = typeof(TEntity);

        var createdAtProperty = entityType.GetProperty("CreatedAt");
        
        if (createdAtProperty != null && createdAtProperty.PropertyType == typeof(DateTime))
        {
            return query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));
        }

        var registeredDateProperty = entityType.GetProperty("RegisteredDate");
        
        if (registeredDateProperty != null && registeredDateProperty.PropertyType == typeof(DateTime))
        {
            return query.OrderByDescending(e => EF.Property<DateTime>(e, "RegisteredDate"));
        }

        var firstProperty = entityType.GetProperties().FirstOrDefault();
        
        if (firstProperty != null)
        {
            return query.OrderByDescending(e => EF.Property<object>(e, firstProperty.Name));
        }

        return query.OrderByDescending(e => e);
    }
    
    public TEntity? GetById<TEntity>(object id) where TEntity : class
    {
        return dbContext.Set<TEntity>().Find(id);
    }

    public TEntity? GetFirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
    {
        return dbContext.Set<TEntity>().FirstOrDefault(filter);
    }

    public int GetCount<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class
    {
        return filter != null 
        ? dbContext.Set<TEntity>().Count(filter)
        : dbContext.Set<TEntity>().Count();
    }
    
    public Guid Insert<TEntity>(TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Set<TEntity>().Add(entity);

        dbContext.SaveChanges();

        var entityType = typeof(TEntity);

        var idProperty = entityType.GetProperty("Id");

        if (idProperty == null || idProperty.PropertyType != typeof(Guid))
        {
            return Guid.Empty;
        }

        return (Guid)idProperty.GetValue(entity)!;
    }

    public bool AddMultipleEntity<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityList);

        dbContext.Set<TEntity>().AddRange(entityList);

        dbContext.SaveChanges();

        const bool flag = true;

        return flag;
    }

    public void Update<TEntity>(TEntity entityToUpdate) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityToUpdate);

        dbContext.Entry(entityToUpdate).State = EntityState.Modified;

        dbContext.SaveChanges();
    }

    public void UpdateMultipleEntity<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityList);

        dbContext.Entry(entityList).State = EntityState.Modified;

        dbContext.SaveChanges();
    }

    public void Delete<TEntity>(object id) where TEntity : class
    {
        var entityToDelete = dbContext.Set<TEntity>().Find(id);

        if (entityToDelete == null) throw new ArgumentNullException(nameof(id));

        dbContext.Set<TEntity>().Remove(entityToDelete);

        dbContext.SaveChanges();
    }

    public void Delete<TEntity>(TEntity entityToDelete) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityToDelete);

        dbContext.Set<TEntity>().Remove(entityToDelete);

        dbContext.SaveChanges();
    }

    public void DeleteMultipleEntity<TEntity>(Expression<Func<TEntity, bool>>? filter) where TEntity : class
    {
        var query = filter != null ? dbContext.Set<TEntity>().Where(filter) : dbContext.Set<TEntity>();

        dbContext.Set<TEntity>().RemoveRange(query);

        dbContext.SaveChanges();
    }

    public void RemoveMultipleEntity<TEntity>(IEnumerable<TEntity> removeEntityList) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(removeEntityList);

        dbContext.Set<TEntity>().RemoveRange(removeEntityList);

        dbContext.SaveChanges();
    }
}