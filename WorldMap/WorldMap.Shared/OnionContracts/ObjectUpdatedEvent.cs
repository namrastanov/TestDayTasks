using MemoryPack;
using WorldMap.Shared.Models;

namespace WorldMap.Shared.OnionContracts
{
    [MemoryPackable]
    public partial class ObjectUpdatedEvent
    {
        public MapObjectDto Object { get; set; } = new();
    }
}

