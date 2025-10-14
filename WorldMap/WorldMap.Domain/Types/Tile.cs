namespace WorldMap.Domain
{
    public struct Tile : IEquatable<Tile>
    {
        public readonly TileType Type;

        public Tile(TileType type)
        {
            this.Type = type;
        }

        public bool Equals(Tile other)
        {
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || GetType() != obj.GetType())
                return false;

            return Equals((Tile)obj);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public static bool operator ==(Tile left, Tile right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            return !(left == right);
        }
    }
}
