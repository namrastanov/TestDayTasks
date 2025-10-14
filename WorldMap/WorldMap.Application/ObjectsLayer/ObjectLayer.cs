using System.Collections.Immutable;
using WorldMap.Domain;
using WorldMap.Application;

namespace WorldMap.Layers
{
    public delegate void ObjectEventHandler(GameObject obj);

    public sealed class ObjectLayer: IObjectLayer
    {
        private readonly IObjectRepository<GameObject> _objectRepository;
        private ImmutableList<IObjectChangeHandler> _handlers = ImmutableList<IObjectChangeHandler>.Empty;

        public ObjectLayer(IObjectRepository<GameObject> objectRepository)
        {
            _objectRepository = objectRepository ?? throw new ArgumentNullException(nameof(objectRepository));
        }

        public void Subscribe(IObjectChangeHandler handler)
        {
            ImmutableInterlocked.Update(
                ref _handlers,
                list => list.Contains(handler) ? list : list.Add(handler)
            );
        }

        public void Unsubscribe(IObjectChangeHandler handler)
        {
            ImmutableInterlocked.Update(
                ref _handlers,
                list => list.Remove(handler)
            );
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
            var handlers = _handlers;
            var tasks = handlers.Select(action);
            await Task.WhenAll(tasks);
        }
    }
}
