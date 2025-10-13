using MemoryPack;

namespace WorldMap.Shared.OnionContracts
{
    [MemoryPackable]
    public partial class ObjectDeletedEvent
    {
        public string Id { get; set; } = string.Empty;
    }
}

