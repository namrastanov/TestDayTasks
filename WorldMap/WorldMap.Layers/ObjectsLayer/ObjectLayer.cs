using WorldMap.Domain;
using WorldMap.Infrastructure.Repositories;

namespace WorldMap.Layers
{
    public delegate void ObjectEventHandler(GameObject obj);

    public sealed class ObjectLayer
    {
        private readonly IRedisObjectRepository<GameObject> _objectRepository;
        private readonly object _locker = new();
        private event ObjectEventHandler? OnCreated;
        private event ObjectEventHandler? OnUpdated;
        private event ObjectEventHandler? OnDeleted;

        public ObjectLayer(IRedisObjectRepository<GameObject> objectRepository)
        {
            _objectRepository = objectRepository ?? 
                throw new ArgumentNullException(nameof(objectRepository));
        }

        public void SubscribeToCreate(ObjectEventHandler handler)
        {
            _ = handler ??
                throw new ArgumentNullException(nameof(handler));

            lock (_locker)
            {
                OnCreated += handler;
            }
        }
        public void UnsubscribeFromCreate(ObjectEventHandler handler)
        {
            _ = handler ??
                throw new ArgumentNullException(nameof(handler));

            lock (_locker)
            {
                OnCreated -= handler;
            }
        }
        public void SubscribeToUpdate(ObjectEventHandler handler)
        {
            _ = handler ??
                throw new ArgumentNullException(nameof(handler));

            lock (_locker)
            {
                OnUpdated += handler;
            }
        }

        public void UnsubscribeFromUpdate(ObjectEventHandler handler)
        {
            _ = handler ??
                throw new ArgumentNullException(nameof(handler));

            lock (_locker)
            {
                OnUpdated -= handler;
            }
        }

        public void SubscribeToDelete(ObjectEventHandler handler)
        {
            _ = handler ??
                throw new ArgumentNullException(nameof(handler));

            lock (_locker)
            {
                OnDeleted += handler;
            }
        }

        public void UnsubscribeFromDelete(ObjectEventHandler handler)
        {
            _ = handler ??
                throw new ArgumentNullException(nameof(handler));

            lock (_locker)
            {
                OnDeleted -= handler;
            }
        }

        public async Task CreateObjectAsync(GameObject gameObject)
        {
            await _objectRepository.AddAsync(gameObject);
            lock (_locker)
            {
                OnCreated?.Invoke(gameObject);
            }
        }

        public async Task UpdateObjectAsync(GameObject gameObject)
        {
            await _objectRepository.RemoveAsync(gameObject.Id);
            await _objectRepository.AddAsync(gameObject);
            lock (_locker)
            {
                OnUpdated?.Invoke(gameObject);
            }
        }

        public async Task DeleteObjectAsync(string id)
        {
            var gameObject = 
                await _objectRepository.GetByIdAsync(id);
            if (gameObject != null)
            {
                await _objectRepository.RemoveAsync(id);
                lock (_locker)
                {
                    OnDeleted?.Invoke(gameObject);
                }
            }
        }

        public async Task<IEnumerable<GameObject>> GetByAreaAsync(int topLeftX, int topLeftY, int width, int height)
        {
            var gameObjects =
                await _objectRepository.GetByAreaAsync(topLeftX, topLeftY, width, height);
            return gameObjects;
        }

        public async Task<GameObject?> GetByCoordinatesAsync(int x, int y)
        {
            var gameObject = 
                await _objectRepository.GetByCoordinatesAsync(x, y);
            return gameObject;
        }

        public bool CheckIfInsideAreaAsync(GameObject gameObject, int topLeftX, int topLeftY, int width, int height) =>
            _objectRepository.CheckIfInsideAreaAsync(gameObject, topLeftX, topLeftY, width, height);
    }
}