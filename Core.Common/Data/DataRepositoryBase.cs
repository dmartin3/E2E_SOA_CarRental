using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Core.Common.Contracts;
using Core.Common.Utils;

namespace Core.Common.Data {
  public abstract class DataRepositoryBase<T, U> : IDataRepository<T>
    where T : class, IIdentifiableEntity, new()
    where U : DbContext, new() {

    protected abstract T AddEntity(U entityContext, T entity);
    protected abstract T UpdateEntity(U entityContext, T entity);
    protected abstract IEnumerable<T> GetEntities(U entityContext);
    protected abstract T GetEntity(U entityContext, int id);

    public T Add(T entity) {
      using (var entityContext = new U()) {
        var addedEntity = AddEntity(entityContext, entity);
        entityContext.SaveChanges();
        return addedEntity;
      }
    }

    public void Remove(T entity) {
      using (var entityContext = new U()) {
        entityContext.Entry<T>(entity).State = EntityState.Deleted;
        entityContext.SaveChanges();
      }
    }

    public void Remove(int id) {
      using (var entityContext = new U()) {
        var entity = GetEntity(entityContext, id);
        entityContext.Entry<T>(entity).State = EntityState.Deleted;
        entityContext.SaveChanges();
      }
    }

    public T Update(T entity) {
      using (var entityContext = new U()) {
        var existingEntity = UpdateEntity(entityContext, entity);

        SimpleMapper.PropertyMap(entity, existingEntity);

        entityContext.SaveChanges();
        return existingEntity;
      }
    }

    public IEnumerable<T> Get() {
      using (var entityContext = new U())
        return (GetEntities(entityContext)).ToArray().ToList();
    }

    public T Get(int id) {
      using (var entityContext = new U())
        return GetEntity(entityContext, id);
    }
  }
}
