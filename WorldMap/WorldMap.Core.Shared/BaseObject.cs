namespace WorldMap.Core.Shared
{ 
    public abstract class BaseObject<T>
    {
        public BaseObject()
        {
        }

        public T Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
