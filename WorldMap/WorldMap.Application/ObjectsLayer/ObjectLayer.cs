using System.Collections.Concurrent;
using System.Collections.Immutable;
using WorldMap.Domain;

namespace WorldMap.Application
{
    public delegate void ObjectEventHandler(GameObject obj);

    public sealed class ObjectLayer: IObjectLayer
    {
        private readonly IObjectRepository<GameObject> _objectRepository;
        private readonly ConcurrentDictionary<string, IObjectChangeHandler> _handlers = new();

        public ObjectLayer(IObjectRepository<GameObject> objectRepository)
        {
            _objectRepository = objectRepository ?? throw new ArgumentNullException(nameof(objectRepository));
        }

        public void Subscribe(string observerId, IObjectChangeHandler handler)
        {
            _handlers.TryAdd(observerId, handler);
        }

        public void Unsubscribe(string observerId)
        {
            _handlers.TryRemove(observerId, out _);
        }

        public async Task AddObjectAsync(GameObject gameObject)
        {
            await _objectRepository.AddAsync(gameObject);

            await NotifyHandlersAsync(h => h.OnObjectAddedAsync(gameObject));
        }

        public async Task UpdateObjectAsync(GameObject gameObject)
        {
            await _objectRepository.RemoveAsync(gameObject.Id);
            await _objectRepository.AddAsync(gameObject);

            await NotifyHandlersAsync(h => h.OnObjectUpdatedAsync(gameObject));
        }

        public async Task RemoveObjectAsync(string id)
        {
            var gameObject = await _objectRepository.GetByIdAsync(id);

            if (gameObject != null)
            {
                await _objectRepository.RemoveAsync(id);

                await NotifyHandlersAsync(h => h.OnObjectRemovedAsync(id));
            }
        }

        public async Task SendPrivateMessageAsync(string targetId, string message)
        {
            if (_handlers.TryGetValue(targetId, out var handler))
            {
                await handler.OnPrivateMessageAsync(message);
            }
        }

        public async Task<IEnumerable<GameObject>> GetByAreaAsync(int topLeftX, int topLeftY, int width, int height)
        {
            var gameObjects = await _objectRepository.GetByAreaAsync(topLeftX, topLeftY, width, height);

            return gameObjects;
        }

        public async Task<GameObject?> GetByCoordinatesAsync(int x, int y)
        {
            var gameObject = await _objectRepository.GetByCoordinatesAsync(x, y);

            return gameObject;
        }

        public bool CheckIfInsideArea(GameObject gameObject, int topLeftX, int topLeftY, int width, int height) =>
            _objectRepository.CheckIfInsideArea(gameObject, topLeftX, topLeftY, width, height);

        private async Task NotifyHandlersAsync(Func<IObjectChangeHandler, Task> action)
        {
            var handlers = _handlers.Values;
            var tasks = handlers.Select(action);
            await Task.WhenAll(tasks);
        }
    }
}
