using MemoryPack;

namespace WorldMap.Shared.Models
{
    [MemoryPackable]
    public partial class RegionDto
    {
        public ushort Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Metadata { get; set; } = string.Empty;
    }
}

