namespace WorldMap.Domain
{ 
    public abstract class BaseObject
    {
        public BaseObject()
        {
            
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
