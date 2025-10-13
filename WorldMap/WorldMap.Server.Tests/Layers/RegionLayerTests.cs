using WorldMap.Layers.RegionsLayer;

namespace WorldMap.Server.Tests.Layers
{
    public class RegionLayerTests
    {
        private readonly RegionLayer _regionLayer;

        public RegionLayerTests()
        {
            _regionLayer = new RegionLayer();
        }

        [Fact]
        public async Task AddRegionAsync_AddsRegionSuccessfully()
        {
            // Arrange
            var region = new Region
            {
                Id = "region1",
                Name = "Forest",
                X = 10,
                Y = 10,
                Width = 50,
                Height = 50,
                Metadata = "trees"
            };

            // Act
            await _regionLayer.AddRegionAsync(region);
            var result = await _regionLayer.GetRegionByIdAsync("region1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("region1", result.Id);
            Assert.Equal("Forest", result.Name);
        }

        [Fact]
        public async Task AddRegionAsync_ThrowsException_WhenRegionIdIsEmpty()
        {
            // Arrange
            var region = new Region { Id = "", Name = "Test", X = 10, Y = 10, Width = 50, Height = 50 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _regionLayer.AddRegionAsync(region));
        }

        [Fact]
        public async Task AddRegionAsync_ThrowsException_WhenRegionAlreadyExists()
        {
            // Arrange
            var region1 = new Region { Id = "region1", Name = "Test1", X = 10, Y = 10, Width = 50, Height = 50 };
            var region2 = new Region { Id = "region1", Name = "Test2", X = 20, Y = 20, Width = 30, Height = 30 };

            // Act
            await _regionLayer.AddRegionAsync(region1);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _regionLayer.AddRegionAsync(region2));
        }

        [Fact]
        public async Task GetRegionsInAreaAsync_ReturnsEmptyList_WhenNoRegionsInArea()
        {
            // Act
            var result = await _regionLayer.GetRegionsInAreaAsync(0, 0, 10, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRegionsInAreaAsync_ReturnsRegionsInArea()
        {
            // Arrange
            await _regionLayer.AddRegionAsync(new Region
            {
                Id = "region1",
                Name = "Forest",
                X = 10,
                Y = 10,
                Width = 50,
                Height = 50
            });
            await _regionLayer.AddRegionAsync(new Region
            {
                Id = "region2",
                Name = "Desert",
                X = 100,
                Y = 100,
                Width = 30,
                Height = 30
            });
            await _regionLayer.AddRegionAsync(new Region
            {
                Id = "region3",
                Name = "Mountain",
                X = 30,
                Y = 30,
                Width = 20,
                Height = 20
            });

            // Act
            var result = await _regionLayer.GetRegionsInAreaAsync(0, 0, 60, 60);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Id == "region1");
            Assert.Contains(result, r => r.Id == "region3");
        }

        [Fact]
        public async Task GetRegionsInAreaAsync_HandlesOverlappingRegions()
        {
            // Arrange - Region at (10,10) with width=50, height=50 (covers 10-59, 10-59)
            await _regionLayer.AddRegionAsync(new Region
            {
                Id = "region1",
                Name = "Forest",
                X = 10,
                Y = 10,
                Width = 50,
                Height = 50
            });

            // Act - Query area (40,40) to (100,100) which overlaps with region1
            var result = await _regionLayer.GetRegionsInAreaAsync(40, 40, 100, 100);

            // Assert
            Assert.Single(result);
            Assert.Equal("region1", result.First().Id);
        }

        [Fact]
        public async Task GetRegionsInAreaAsync_HandlesReversedCoordinates()
        {
            // Arrange
            await _regionLayer.AddRegionAsync(new Region
            {
                Id = "region1",
                Name = "Forest",
                X = 10,
                Y = 10,
                Width = 50,
                Height = 50
            });

            // Act - Query with reversed coordinates
            var result = await _regionLayer.GetRegionsInAreaAsync(60, 60, 0, 0);

            // Assert
            Assert.Single(result);
            Assert.Equal("region1", result.First().Id);
        }

        [Fact]
        public async Task GetRegionByIdAsync_ReturnsNull_WhenRegionDoesNotExist()
        {
            // Act
            var result = await _regionLayer.GetRegionByIdAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ConcurrentOperations_HandleThreadSafety()
        {
            // Arrange
            var tasks = new List<Task>();

            // Act - Add 100 regions concurrently
            for (int i = 0; i < 100; i++)
            {
                var index = i;
                tasks.Add(Task.Run(async () =>
                {
                    await _regionLayer.AddRegionAsync(new Region
                    {
                        Id = $"region{index}",
                        Name = $"Region {index}",
                        X = index * 10,
                        Y = index * 10,
                        Width = 10,
                        Height = 10
                    });
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            var allRegions = await _regionLayer.GetRegionsInAreaAsync(0, 0, 1000, 1000);
            Assert.Equal(100, allRegions.Count);
        }
    }
}

