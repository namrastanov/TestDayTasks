using System.Runtime.CompilerServices;

namespace WorldMap.Layers
{
    public class SurfaceLayer
    {
        public const int DefaultWidth = 1000;
        public const int DefaultHeight = 1000;

        private readonly Tile[] _tiles;

        public int Width { get; }
        public int Height { get; }

        public SurfaceLayer(Span<Tile> initialTiles, int width = DefaultWidth, int height = DefaultHeight)
        {
            if (initialTiles.IsEmpty)
                throw new ArgumentException("Переданная коллекция тайлов пуста");

            if (width * height != initialTiles.Length)
                throw new ArgumentException($"Количество элементов в переданном массиве ({initialTiles.Length}) должно соответствовать ширине ({width}) и высоте ({height})");

            _tiles = initialTiles.ToArray();
            Width = width;
            Height = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetTileType(int x, int y, out TileType result)
        {
            if (ValidateCoordinates(x, y))
            {
                result = _tiles[ToIndex(x, y)].Type;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetTile(int x, int y, out Tile result)
        {
            if (ValidateCoordinates(x, y))
            {
                result = _tiles[ToIndex(x, y)];
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetTile(int x, int y, Tile tile)
        {
            if (ValidateCoordinates(x, y))
            {
                _tiles[ToIndex(x, y)] = tile;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanPlaceObjectInArea(int startX, int endX, int startY, int endY)
        {
            for (int x = startX; x <= endX && x < Width; x++)
            {
                for (int y = startY; y <= endY && y < Height; y++)
                {
                    if (!CanPlaceObjectAt(x, y))
                        return false;
                }
            }
            return true;
        }

        public void FillArea(int startX, int endX, int startY, int endY, TileType fillType)
        {
            for (int x = startX; x <= endX && x < Width; x++)
            {
                for (int y = startY; y <= endY && y < Height; y++)
                {
                    TrySetTile(x, y, new Tile(fillType));
                }
            }
        }

        private int ToIndex(int x, int y) => y * Width + x;

        private bool ValidateCoordinates(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private bool CanPlaceObjectAt(int x, int y)
        {
            if (TryGetTileType(x, y, out var tileType))
                return tileType == TileType.Plain;
            return false;
        }
    }
}