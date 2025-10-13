using System.Collections.Generic;
using WorldMap.Layers.SurfaceLayer;
using WorldMap.Layers.Types;
using Xunit;

namespace WorldMap.Tests.LayersTests
{

    public class SurfaceLayerTests
    {
        [Fact]
        public void Constructor_DefaultSize_CreatesExpectedArray()
        {
            // Arrange
            // Act
            var surfaceLayer = new SurfaceLayer();

            // Assert
            Assert.Equal(SurfaceLayer.DefaultWidth, surfaceLayer.Width);
            Assert.Equal(SurfaceLayer.DefaultHeight, surfaceLayer.Height);
        }

        [Fact]
        public void Constructor_InitializedWithCollection_CorrectlyPopulatesTiles()
        {
            // Arrange
            var tiles = new List<List<Tile>>
            {
                new List<Tile> {new Tile(TileType.Plain), new Tile(TileType.Mountain)},
                new List<Tile> {new Tile(TileType.Mountain), new Tile(TileType.Plain)}
            };

            // Act
            var surfaceLayer = new SurfaceLayer(tiles);

            // Assert
            Assert.Equal(2, surfaceLayer.Width);
            Assert.Equal(2, surfaceLayer.Height);
            Assert.Equal(TileType.Plain, surfaceLayer.GetTileType(0, 0));
            Assert.Equal(TileType.Mountain, surfaceLayer.GetTileType(0, 1));
        }

        [Fact]
        public void Constructor_InitializedWithArray_CorrectlyPopulatesTiles()
        {
            // Arrange
            var tiles = new Tile[2, 2]
            {
                { new Tile(TileType.Plain), new Tile(TileType.Mountain) },
                { new Tile(TileType.Mountain), new Tile(TileType.Plain) }
            };

            // Act
            var surfaceLayer = new SurfaceLayer(tiles);

            // Assert
            Assert.Equal(2, surfaceLayer.Width);
            Assert.Equal(2, surfaceLayer.Height);
            Assert.Equal(TileType.Plain, surfaceLayer.GetTileType(0, 0));
            Assert.Equal(TileType.Mountain, surfaceLayer.GetTileType(0, 1));
        }

        [Fact]
        public void FillLayerWithType_ChangesAllTilesToGivenType()
        {
            // Arrange
            var surfaceLayer = new SurfaceLayer();

            // Act
            surfaceLayer.FillLayerWithType(TileType.Mountain);

            // Assert
            for (var x = 0; x < surfaceLayer.Width; x++)
            {
                for (var y = 0; y < surfaceLayer.Height; y++)
                {
                    Assert.Equal(TileType.Mountain, surfaceLayer.GetTileType(x, y));
                }
            }
        }

        [Theory]
        [InlineData(0, 0, TileType.Plain)]
        [InlineData(999, 999, TileType.Plain)]
        public void GetTileType_ReturnsCorrectType(int x, int y, TileType expectedType)
        {
            // Arrange
            var surfaceLayer = new SurfaceLayer();

            // Act
            var actualType = surfaceLayer.GetTileType(x, y);

            // Assert
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void SetTile_SetsNewTileAtPosition()
        {
            // Arrange
            var surfaceLayer = new SurfaceLayer();
            var testTile = new Tile(TileType.Mountain);

            // Act
            surfaceLayer.SetTile(500, 500, testTile);

            // Assert
            Assert.Equal(testTile, surfaceLayer.GetTile(500, 500));
        }

        [Fact]
        public void FillArea_FillsSpecifiedAreaWithGivenType()
        {
            // Arrange
            var surfaceLayer = new SurfaceLayer();

            // Act
            surfaceLayer.FillArea(100, 200, 100, 200, TileType.Mountain);

            // Assert
            for (var x = 100; x <= 200; x++)
            {
                for (var y = 100; y <= 200; y++)
                {
                    Assert.Equal(TileType.Mountain, surfaceLayer.GetTileType(x, y));
                }
            }
        }

        [Fact]
        public void CanPlaceObjectInArea_ReturnsFalseForMountainTiles()
        {
            // Arrange
            var surfaceLayer = new SurfaceLayer();
            surfaceLayer.FillArea(100, 200, 100, 200, TileType.Mountain);

            // Act
            var canPlace = surfaceLayer.CanPlaceObjectInArea(100, 200, 100, 200);

            // Assert
            Assert.False(canPlace);
        }

        [Fact]
        public void OutOfBounds_ThrowsIndexOutOfRangeException()
        {
            // Arrange
            var surfaceLayer = new SurfaceLayer();

            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => surfaceLayer.GetTile(-1, 0));
            Assert.Throws<IndexOutOfRangeException>(() => surfaceLayer.GetTile(0, -1));
            Assert.Throws<IndexOutOfRangeException>(() => surfaceLayer.GetTile(surfaceLayer.Width + 1, 0));
            Assert.Throws<IndexOutOfRangeException>(() => surfaceLayer.GetTile(0, surfaceLayer.Height + 1));
        }
    }
}