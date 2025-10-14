using System.Drawing;
using WorldMap.Domain;

namespace WorldMap.Application.Tests
{
    public class RegionLayerTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentExceptionForEmptyInitialRegions()
        {
            Assert.Throws<ArgumentException>(() => new RegionLayer(Array.Empty<Region>(), 10, 10));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        public void GetRegionIdAtPosition_ReturnsValidRegionIdWhenInsideRegion(int x, int y)
        {
            // Arrange
            var regions = CreateTestRegions();
            var layer = new RegionLayer(regions.AsSpan(), 10, 10);

            // Act
            var result = layer.GetRegionIdAtPosition(x, y);

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(11, 1)]
        [InlineData(1, 11)]
        public void GetRegionIdAtPosition_ReturnsNullForInvalidCoordinates(int x, int y)
        {
            // Arrange
            var regions = CreateTestRegions();
            var layer = new RegionLayer(regions.AsSpan(), 10, 10);

            // Act
            var result = layer.GetRegionIdAtPosition(x, y);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        public void IsTileInRegion_ReturnsTrueForTilesWithinRegion(int x, int y)
        {
            // Arrange
            var regions = CreateTestRegions();
            var layer = new RegionLayer(regions.AsSpan(), 10, 10);

            // Act
            var result = layer.IsTileInRegion(x, y);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetRegionMetadataById_ReturnsCorrectMetadata()
        {
            // Arrange
            var regions = CreateTestRegions();
            var layer = new RegionLayer(regions.AsSpan(), 10, 10);
            const string expectedMetadata = "Another Test Region Metadata";

            // Act
            var result = layer.GetRegionMetadataById("2");

            // Assert
            Assert.Equal(expectedMetadata, result);
        }

        [Fact]
        public void GetRegionMetadataById_ReturnsNullForInvalidId()
        {
            // Arrange
            var regions = CreateTestRegions();
            var layer = new RegionLayer(regions.AsSpan(), 10, 10);

            // Act
            var result = layer.GetRegionMetadataById("invalid-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetRegionsIntersectingArea_ReturnsAllIntersections()
        {
            // Arrange
            var regions = CreateTestRegions();
            var layer = new RegionLayer(regions.AsSpan(), 10, 10);

            // Act
            var result = layer.GetRegionsIntersectingArea(0, 0, 10, 10);

            // Assert
            Assert.NotEmpty(result);
        }

        private static Region[] CreateTestRegions()
        {
            return new[]
            {
                new Region(1, "Test Region", new Rectangle(0, 0, 10, 10)) { Metadata = "Test Region Metadata" },
                new Region(2, "Another Test Region", new Rectangle(5, 5, 5, 5)) { Metadata = "Another Test Region Metadata" }
            };
        }
    }
}