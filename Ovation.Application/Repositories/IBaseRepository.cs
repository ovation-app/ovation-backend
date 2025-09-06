namespace Ovation.Application.Repositories;

public interface IBaseRepository<T>
{
    Task CreateAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    //Task<T> Get(Guid id, CancellationToken cancellationToken);
    //Task<List<T>> GetAll(CancellationToken cancellationToken);

}