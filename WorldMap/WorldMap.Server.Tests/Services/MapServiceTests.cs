using Microsoft.Extensions.Logging;
using Moq;
using WorldMap.Layers.ObjectsLayer;
using WorldMap.Layers.ObjectsLayer.Base;
using WorldMap.Layers.RegionsLayer;
using WorldMap.Server.Services;
using WorldMap.Shared.OnionContracts;

namespace WorldMap.Server.Tests.Services
{
    public class MapServiceTests
    {
        private readonly Mock<IObjectLayer> _mockObjectLayer;
        private readonly Mock<IRegionLayer> _mockRegionLayer;
        private readonly Mock<ILogger<MapService>> _mockLogger;
        private readonly MapService _service;

        public MapServiceTests()
        {
            _mockObjectLayer = new Mock<IObjectLayer>();
            _mockRegionLayer = new Mock<IRegionLayer>();
            _mockLogger = new Mock<ILogger<MapService>>();
            _service = new MapService(_mockObjectLayer.Object, _mockRegionLayer.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_ReturnsEmptyList_WhenNoObjectsInArea()
        {
            // Arrange
            var request = new GetObjectsInAreaRequest { X1 = 0, Y1 = 0, X2 = 10, Y2 = 10 };
            _mockObjectLayer.Setup(x => x.GetObjectsInAreaAsync(0, 0, 10, 10))
                .ReturnsAsync(new List<GameObject>());

            // Act
            var response = await _service.GetObjectsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Objects);
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_ReturnsObjects_WhenObjectsExistInArea()
        {
            // Arrange
            var request = new GetObjectsInAreaRequest { X1 = 0, Y1 = 0, X2 = 10, Y2 = 10 };
            var objects = new List<GameObject>
            {
                new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 },
                new GameObject { Id = "obj2", X = 7, Y = 7, Width = 1, Height = 1 }
            };
            _mockObjectLayer.Setup(x => x.GetObjectsInAreaAsync(0, 0, 10, 10))
                .ReturnsAsync(objects);

            // Act
            var response = await _service.GetObjectsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.Objects.Count);
            Assert.Equal("obj1", response.Objects[0].Id);
            Assert.Equal(5, response.Objects[0].X);
            Assert.Equal("obj2", response.Objects[1].Id);
        }

        [Fact]
        public async Task GetRegionsInAreaAsync_ReturnsEmptyList_WhenNoRegionsInArea()
        {
            // Arrange
            var request = new GetRegionsInAreaRequest { X1 = 0, Y1 = 0, X2 = 10, Y2 = 10 };
            _mockRegionLayer.Setup(x => x.GetRegionsInAreaAsync(0, 0, 10, 10))
                .ReturnsAsync(new List<Region>());

            // Act
            var response = await _service.GetRegionsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Regions);
        }

        [Fact]
        public async Task GetRegionsInAreaAsync_ReturnsRegions_WhenRegionsExistInArea()
        {
            // Arrange
            var request = new GetRegionsInAreaRequest { X1 = 0, Y1 = 0, X2 = 100, Y2 = 100 };
            var regions = new List<Region>
            {
                new Region { Id = "region1", Name = "Forest", X = 10, Y = 10, Width = 50, Height = 50, Metadata = "trees" },
                new Region { Id = "region2", Name = "Desert", X = 60, Y = 60, Width = 30, Height = 30, Metadata = "sand" }
            };
            _mockRegionLayer.Setup(x => x.GetRegionsInAreaAsync(0, 0, 100, 100))
                .ReturnsAsync(regions);

            // Act
            var response = await _service.GetRegionsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.Regions.Count);
            Assert.Equal("region1", response.Regions[0].Id);
            Assert.Equal("Forest", response.Regions[0].Name);
            Assert.Equal("region2", response.Regions[1].Id);
            Assert.Equal("Desert", response.Regions[1].Name);
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_HandlesNegativeCoordinates()
        {
            // Arrange
            var request = new GetObjectsInAreaRequest { X1 = -10, Y1 = -10, X2 = 10, Y2 = 10 };
            _mockObjectLayer.Setup(x => x.GetObjectsInAreaAsync(-10, -10, 10, 10))
                .ReturnsAsync(new List<GameObject>());

            // Act
            var response = await _service.GetObjectsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Objects);
        }
    }
}

