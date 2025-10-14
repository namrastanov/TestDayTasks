namespace WorldMap.Core.Shared
{ 
    public abstract class BaseObject
    {
        public BaseObject()
        {
             Id = Guid.NewGuid().ToString();
        }

        public string Id { get; init; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
