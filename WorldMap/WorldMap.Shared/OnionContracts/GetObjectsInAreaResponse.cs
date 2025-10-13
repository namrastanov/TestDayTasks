using MemoryPack;
using WorldMap.Shared.Models;

namespace WorldMap.Shared.OnionContracts
{
    [MemoryPackable]
    public partial class GetObjectsInAreaResponse
    {
        public List<MapObjectDto> Objects { get; set; } = new();
    }
}

