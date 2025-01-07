

namespace Domain.Interfaces
{
    public interface ICRUDRepository<T> where T : class
    {
        Task InsertAsync(T  entity);
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<bool>DeleteAsync(Guid id);
        Task<bool>IsExistsAsync(Guid id);
        Task UpdateAsync(T entity);
        public Task SaveChangesAsync();


    }
}
