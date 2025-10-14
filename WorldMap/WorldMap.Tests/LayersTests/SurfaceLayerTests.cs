using WorldMap.Application;
using WorldMap.Domain;

namespace WorldMap.Tests.LayersTests
{
    public class SurfaceLayerTests
    {
        [Fact]
        public void CreateSurfaceLayer_ValidParameters_SuccessfulCreation()
        {
            // Arrange
            var tiles = Enumerable.Repeat(new Tile(TileType.Plain), SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight).ToArray();

            // Act
            var surfaceLayer = new SurfaceLayer(tiles.AsSpan());

            // Assert
            Assert.Equal(SurfaceLayer.DefaultWidth, surfaceLayer.Width);
            Assert.Equal(SurfaceLayer.DefaultHeight, surfaceLayer.Height);
        }

        [Fact]
        public void CreateSurfaceLayer_EmptyCollection_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                Span<Tile> emptyTiles = Array.Empty<Tile>();
                new SurfaceLayer(initialTiles: emptyTiles);
            });
        }

        [Fact]
        public void CreateSurfaceLayer_IncorrectSize_ThrowsArgumentException()
        {
            // Arrange
            var incorrectTilesCount = SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight - 1;
            var tiles = Enumerable.Repeat(new Tile(TileType.Plain), incorrectTilesCount).ToArray();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new SurfaceLayer(tiles.AsSpan()));
        }

        [Theory]
        [InlineData(0, 0, TileType.Plain)]
        [InlineData(1, 1, TileType.Plain)]
        public void TryGetTile_ValidCoordinates_ReturnsCorrectTile(int x, int y, TileType expectedType)
        {
            // Arrange
            var tiles = Enumerable.Repeat(new Tile(expectedType), SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight).ToArray();
            var surfaceLayer = new SurfaceLayer(tiles.AsSpan());

            // Act
            bool success = surfaceLayer.TryGetTile(x, y, out Tile actualTile);

            // Assert
            Assert.True(success);
            Assert.Equal(expectedType, actualTile.Type);
        }

        [Theory]
        [InlineData(0, 0, TileType.Mountain)]
        [InlineData(1, 1, TileType.Plain)]
        public void TrySetTile_ValidCoordinates_SetsCorrectly(int x, int y, TileType setType)
        {
            // Arrange
            var tiles = Enumerable.Repeat(new Tile(TileType.Plain), SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight).ToArray();
            var surfaceLayer = new SurfaceLayer(tiles.AsSpan());

            // Act
            bool success = surfaceLayer.TrySetTile(x, y, new Tile(setType));

            // Assert
            Assert.True(success);
            Assert.True(surfaceLayer.TryGetTile(x, y, out Tile actualTile));
            Assert.Equal(setType, actualTile.Type);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(SurfaceLayer.DefaultWidth, 0)]
        [InlineData(0, SurfaceLayer.DefaultHeight)]
        public void TryGetTile_OutOfBounds_ReturnsFalse(int x, int y)
        {
            // Arrange
            var tiles = Enumerable.Repeat(new Tile(TileType.Plain), SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight).ToArray();
            var surfaceLayer = new SurfaceLayer(tiles.AsSpan());

            // Act
            bool success = surfaceLayer.TryGetTile(x, y, out _);

            // Assert
            Assert.False(success);
        }

        [Fact]
        public void FillArea_FillsWithSpecifiedType()
        {
            // Arrange
            var tiles = Enumerable.Repeat(new Tile(TileType.Plain), SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight).ToArray();
            var surfaceLayer = new SurfaceLayer(tiles.AsSpan());

            // Act
            surfaceLayer.FillArea(0, 10, 0, 10, TileType.Mountain);

            // Assert
            foreach (var x in Enumerable.Range(0, 11))
            {
                foreach (var y in Enumerable.Range(0, 11))
                {
                    Assert.True(surfaceLayer.TryGetTileType(x, y, out var tileType));
                    Assert.Equal(TileType.Mountain, tileType);
                }
            }
        }

        [Fact]
        public void CanPlaceObjectInArea_PlainTerrain_ReturnsTrue()
        {
            // Arrange
            var tiles = Enumerable.Repeat(new Tile(TileType.Plain), SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight).ToArray();
            var surfaceLayer = new SurfaceLayer(tiles.AsSpan());

            // Act
            bool canPlace = surfaceLayer.CanPlaceObjectInArea(0, 10, 0, 10);

            // Assert
            Assert.True(canPlace);
        }

        [Fact]
        public void CanPlaceObjectInArea_Mountains_ReturnsFalse()
        {
            // Arrange
            var tiles = Enumerable.Repeat(new Tile(TileType.Mountain), SurfaceLayer.DefaultWidth * SurfaceLayer.DefaultHeight).ToArray();
            var surfaceLayer = new SurfaceLayer(tiles.AsSpan());

            // Act
            bool canPlace = surfaceLayer.CanPlaceObjectInArea(0, 10, 0, 10);

            // Assert
            Assert.False(canPlace);
        }
    }
}