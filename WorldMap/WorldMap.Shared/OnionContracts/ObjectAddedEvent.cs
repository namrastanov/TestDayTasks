using MemoryPack;
using WorldMap.Shared.Models;

namespace WorldMap.Shared.OnionContracts
{
    [MemoryPackable]
    public partial class ObjectAddedEvent
    {
        public MapObjectDto Object { get; set; } = new();
    }
}

