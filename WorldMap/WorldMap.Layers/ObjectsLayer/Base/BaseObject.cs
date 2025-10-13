namespace WorldMap.Layers.ObjectsLayer.Base
{
    public abstract class BaseObject
    {
        public string Id { get; init; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
