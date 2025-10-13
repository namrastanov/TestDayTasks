using MemoryPack;
using WorldMap.Shared.Models;

namespace WorldMap.Shared.OnionContracts
{
    [MemoryPackable]
    public partial class GetRegionsInAreaResponse
    {
        public List<RegionDto> Regions { get; set; } = new();
    }
}

