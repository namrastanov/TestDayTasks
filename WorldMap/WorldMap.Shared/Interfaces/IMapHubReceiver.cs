using WorldMap.Shared.OnionContracts;

namespace WorldMap.Shared.Interfaces
{
    public interface IMapHubReceiver
    {
        void OnObjectAdded(ObjectAddedEvent evt);
        void OnObjectUpdated(ObjectUpdatedEvent evt);
        void OnObjectDeleted(ObjectDeletedEvent evt);
    }
}

