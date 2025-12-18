using System.Linq.Expressions;
using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Repositories.Base;

public interface IGenericRepository : ITransientService
{
    #region Item Existence
    bool Exists<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class;
    #endregion
    
    #region Get Items Collection
    IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "")
        where TEntity : class;
    
    IQueryable<TEntity> GetPagedResult<TEntity>(int pageNumber, int pageSize, out int rowsCount,
        Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, object>>? order = null, bool isAscendingOrder = true) where TEntity : class;
    #endregion
    
    #region Get Item
    TEntity? GetById<TEntity>(object id) where TEntity : class;
    
    TEntity? GetFirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    #endregion
    
    #region Entry Counts
    int GetCount<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class;
    #endregion
    
    #region Data Insertion
    Guid Insert<TEntity>(TEntity entity) where TEntity : class;
    
    bool AddMultipleEntity<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class;
    #endregion
    
    #region Data Updation
    void Update<TEntity>(TEntity entityToUpdate) where TEntity : class;
    
    void UpdateMultipleEntity<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class;
    #endregion
    
    #region Data Deletion
    void Delete<TEntity>(object id) where TEntity : class;
    
    void Delete<TEntity>(TEntity entityToDelete) where TEntity : class;
    
    void DeleteMultipleEntity<TEntity>(Expression<Func<TEntity, bool>>? filter) where TEntity : class;
    
    void RemoveMultipleEntity<TEntity>(IEnumerable<TEntity> removeEntityList) where TEntity : class;
    #endregion
}