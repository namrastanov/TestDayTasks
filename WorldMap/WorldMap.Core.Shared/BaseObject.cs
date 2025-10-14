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

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public bool Contains(int x, int y)
        {
            return x >= Left && x < Right && y >= Top && y < Bottom;
        }
    }
}
