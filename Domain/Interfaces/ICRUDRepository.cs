

namespace Domain.Interfaces
{
    public interface ICRUDRepository<T>
    {
        Task Add(T  entity);
        Task<T> GetById(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task<bool>Delete(Guid id);
        Task<bool>Exists(Guid id);
        Task Update(T entity);
        public Task SaveChangesAsync();


    }
}
