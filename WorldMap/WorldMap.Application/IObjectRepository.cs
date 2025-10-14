using WorldMap.Core.Shared;

namespace WorldMap.Application
{
    public interface IObjectRepository<T> where T : BaseObject, new()
    {
        Task AddAsync(T obj);
        bool CheckIfInsideArea(T obj, int topLeftX, int topLeftY, int width, int height);
        Task<IEnumerable<T>> GetByAreaAsync(int topLeftX, int topLeftY, int width, int height);
        Task<T?> GetByCoordinatesAsync(int x, int y);
        Task<T?> GetByIdAsync(string id);
        Task RemoveAsync(string id);
    }
}