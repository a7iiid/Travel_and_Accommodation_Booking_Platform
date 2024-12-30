

namespace Domain.Interfaces
{
    internal interface CRUDReposetory<T>
    {
        Task Add(T  entity);
        Task<T> GetById(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task<bool>Delete(Guid id);
        Task<bool>Exists(Guid id);
        Task<T>Update(T entity);
       
    }
}
