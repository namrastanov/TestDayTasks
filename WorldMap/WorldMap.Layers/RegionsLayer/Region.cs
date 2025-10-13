namespace WorldMap.Layers.RegionsLayer
{
    public class Region
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Metadata { get; set; } = string.Empty;
    }
}

