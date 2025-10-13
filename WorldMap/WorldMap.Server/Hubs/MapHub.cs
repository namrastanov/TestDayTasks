using MagicOnion.Server.Hubs;
using WorldMap.Domain;
using WorldMap.Layers.ObjectsLayer;
using WorldMap.Shared.Interfaces;
using WorldMap.Shared.Models;
using WorldMap.Shared.OnionContracts;

namespace WorldMap.Server.Hubs
{
    public class MapHub : StreamingHubBase<IMapHub, IMapHubReceiver>, IMapHub, IObjectChangeHandler
    {
        private readonly IObjectLayer _objectLayer;
        private readonly ILogger<MapHub> _logger;
        private IGroup? _group;

        public MapHub(IObjectLayer objectLayer, ILogger<MapHub> logger)
        {
            _objectLayer = objectLayer ?? throw new ArgumentNullException(nameof(objectLayer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task JoinAsync()
        {
            try
            {
                _logger.LogInformation("Client {ConnectionId} joining map hub", ConnectionId);

                _group = await Group.AddAsync("MapRoom");
                _objectLayer.Subscribe(this);

                _logger.LogInformation("Client {ConnectionId} joined successfully", ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining hub for client {ConnectionId}", ConnectionId);
                throw;
            }
        }

        public async Task LeaveAsync()
        {
            try
            {
                _logger.LogInformation("Client {ConnectionId} leaving map hub", ConnectionId);

                _objectLayer.Unsubscribe(this);

                if (_group != null)
                {
                    await _group.RemoveAsync(Context);
                }

                _logger.LogInformation("Client {ConnectionId} left successfully", ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving hub for client {ConnectionId}", ConnectionId);
                throw;
            }
        }

        public async Task OnObjectAddedAsync(GameObject gameObject)
        {
            try
            {
                var evt = new ObjectAddedEvent
                {
                    Object = new MapObjectDto
                    {
                        Id = gameObject.Id,
                        X = gameObject.X,
                        Y = gameObject.Y,
                        Width = gameObject.Width,
                        Height = gameObject.Height
                    }
                };

                await BroadcastToGroupAsync("MapRoom", evt, (receiver, e) => receiver.OnObjectAdded(e));
                _logger.LogInformation("Broadcasted ObjectAdded event for object {ObjectId}", gameObject.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting ObjectAdded event for object {ObjectId}", gameObject.Id);
            }
        }

        public async Task OnObjectUpdatedAsync(GameObject gameObject)
        {
            try
            {
                var evt = new ObjectUpdatedEvent
                {
                    Object = new MapObjectDto
                    {
                        Id = gameObject.Id,
                        X = gameObject.X,
                        Y = gameObject.Y,
                        Width = gameObject.Width,
                        Height = gameObject.Height
                    }
                };

                await BroadcastToGroupAsync("MapRoom", evt, (receiver, e) => receiver.OnObjectUpdated(e));
                _logger.LogInformation("Broadcasted ObjectUpdated event for object {ObjectId}", gameObject.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting ObjectUpdated event for object {ObjectId}", gameObject.Id);
            }
        }

        public async Task OnObjectRemovedAsync(string id)
        {
            try
            {
                var evt = new ObjectDeletedEvent
                {
                    Id = id
                };

                await BroadcastToGroupAsync("MapRoom", evt, (receiver, e) => receiver.OnObjectDeleted(e));
                _logger.LogInformation("Broadcasted ObjectDeleted event for object {ObjectId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting ObjectDeleted event for object {ObjectId}", id);
            }
        }

        protected override ValueTask OnDisconnected()
        {
            _objectLayer.Unsubscribe(this);
            return base.OnDisconnected();
        }

        private async Task BroadcastToGroupAsync<T>(string groupName, T message, Action<IMapHubReceiver, T> action)
        {
            var group = _group ?? await Group.AddAsync(groupName);
            action(Broadcast(group), message);
        }
    }
}

