using System.Runtime.CompilerServices;
using WorldMap.Layers.Types;

namespace WorldMap.Layers.SurfaceLayer
{
    public class SurfaceLayer
    {
        public const int DefaultWidth = 1000;
        public const int DefaultHeight = 1000;

        private readonly Tile[,] tiles;

        public int Width => tiles.GetLength(0);
        public int Height => tiles.GetLength(1);

        public SurfaceLayer(int width = DefaultWidth, int height = DefaultHeight)
        {
            tiles = new Tile[width, height];
            FillLayerWithType(TileType.Plain);
        }

        public SurfaceLayer(Tile[,] initialTiles)
        {
            if (initialTiles == null || initialTiles.Length == 0)
                throw new ArgumentException("Переданный массив тайлов пуст");

            var width = initialTiles.GetLength(0);
            var height = initialTiles.GetLength(1);

            tiles = new Tile[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    tiles[x, y] = initialTiles[x, y];
                }
            }
        }

        public SurfaceLayer(IEnumerable<IEnumerable<Tile>> collection)
        {
            var enumerator = collection.GetEnumerator();
            List<List<Tile>> rows = new List<List<Tile>>();

            while (enumerator.MoveNext())
            {
                var rowEnum = enumerator.Current.GetEnumerator();
                List<Tile> currentRow = new List<Tile>();

                while (rowEnum.MoveNext())
                    currentRow.Add(rowEnum.Current);

                rows.Add(currentRow);
            }

            tiles = new Tile[rows.Count, rows[0].Count];

            for (int x = 0; x < rows.Count; x++)
            {
                for (int y = 0; y < rows[x].Count; y++)
                {
                    tiles[x, y] = rows[x][y];
                }
            }
        }

        public void FillLayerWithType(TileType tileType)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetTile(x, y, new Tile(tileType));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TileType GetTileType(int x, int y)
        {
            ValidateCoordinates(x, y);
            return tiles[x, y].Type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Tile GetTile(int x, int y)
        {
            ValidateCoordinates(x, y);
            return tiles[x, y];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTile(int x, int y, Tile tile)
        {
            ValidateCoordinates(x, y);
            tiles[x, y] = tile;
        }

        public void FillArea(int startX, int endX, int startY, int endY, TileType fillType)
        {
            for (int x = startX; x <= endX && x < Width; x++)
            {
                for (int y = startY; y <= endY && y < Height; y++)
                {
                    SetTile(x, y, new Tile(fillType));
                }
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

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new IndexOutOfRangeException("Координаты выходят за границу карты");
        }

        private bool CanPlaceObjectAt(int x, int y)
        {
            try
            {
                return GetTileType(x, y) == TileType.Plain;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}