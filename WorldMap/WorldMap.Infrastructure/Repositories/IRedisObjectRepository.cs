using WorldMap.Domain;

namespace WorldMap.Infrastructure.Repositories
{
    public interface IRedisObjectRepository<T> where T : BaseObject, new()
    {
        Task AddAsync(T obj);
        bool CheckIfInsideAreaAsync(T obj, int topLeftX, int topLeftY, int width, int height);
        Task<IEnumerable<T>> GetByAreaAsync(int topLeftX, int topLeftY, int width, int height);
        Task<T?> GetByCoordinatesAsync(int x, int y);
        Task<T?> GetByIdAsync(string id);
        Task RemoveAsync(string id);
    }
}