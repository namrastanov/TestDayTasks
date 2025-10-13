using System.Collections.Concurrent;
using WorldMap.Layers.ObjectsLayer.Base;

namespace WorldMap.Layers.ObjectsLayer
{
    public class ObjectLayer : IObjectLayer
    {
        private readonly ConcurrentDictionary<string, GameObject> _objects = new();
        private readonly List<IObjectChangeHandler> _handlers = new();
        private readonly object _handlersLock = new();

        public Task<IReadOnlyCollection<GameObject>> GetObjectsInAreaAsync(int x1, int y1, int x2, int y2)
        {
            // Ensure coordinates are properly ordered
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1, x2);
            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1, y2);

            var objectsInArea = _objects.Values
                .Where(obj => IsObjectInArea(obj, minX, minY, maxX, maxY))
                .ToList();

            return Task.FromResult<IReadOnlyCollection<GameObject>>(objectsInArea);
        }

        public async Task AddObjectAsync(GameObject gameObject)
        {
            if (string.IsNullOrEmpty(gameObject.Id))
            {
                throw new ArgumentException("Object ID cannot be null or empty", nameof(gameObject));
            }

            if (!_objects.TryAdd(gameObject.Id, gameObject))
            {
                throw new InvalidOperationException($"Object with ID {gameObject.Id} already exists");
            }

            await NotifyHandlersAsync(h => h.OnObjectAddedAsync(gameObject));
        }

        public async Task UpdateObjectAsync(GameObject gameObject)
        {
            if (string.IsNullOrEmpty(gameObject.Id))
            {
                throw new ArgumentException("Object ID cannot be null or empty", nameof(gameObject));
            }

            if (!_objects.ContainsKey(gameObject.Id))
            {
                throw new InvalidOperationException($"Object with ID {gameObject.Id} does not exist");
            }

            _objects[gameObject.Id] = gameObject;

            await NotifyHandlersAsync(h => h.OnObjectUpdatedAsync(gameObject));
        }

        public async Task RemoveObjectAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Object ID cannot be null or empty", nameof(id));
            }

            if (!_objects.TryRemove(id, out _))
            {
                throw new InvalidOperationException($"Object with ID {id} does not exist");
            }

            await NotifyHandlersAsync(h => h.OnObjectRemovedAsync(id));
        }

        public Task<GameObject?> GetObjectByIdAsync(string id)
        {
            _objects.TryGetValue(id, out var gameObject);
            return Task.FromResult(gameObject);
        }

        public void Subscribe(IObjectChangeHandler handler)
        {
            lock (_handlersLock)
            {
                if (!_handlers.Contains(handler))
                {
                    _handlers.Add(handler);
                }
            }
        }

        public void Unsubscribe(IObjectChangeHandler handler)
        {
            lock (_handlersLock)
            {
                _handlers.Remove(handler);
            }
        }

        private bool IsObjectInArea(GameObject obj, int minX, int minY, int maxX, int maxY)
        {
            // Check if object overlaps with the area
            var objMaxX = obj.X + obj.Width - 1;
            var objMaxY = obj.Y + obj.Height - 1;

            return !(obj.X > maxX || objMaxX < minX || obj.Y > maxY || objMaxY < minY);
        }

        private async Task NotifyHandlersAsync(Func<IObjectChangeHandler, Task> action)
        {
            List<IObjectChangeHandler> handlersCopy;
            lock (_handlersLock)
            {
                handlersCopy = new List<IObjectChangeHandler>(_handlers);
            }

            var tasks = handlersCopy.Select(action);
            await Task.WhenAll(tasks);
        }
    }
}

