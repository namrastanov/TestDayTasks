using Microsoft.Extensions.Logging;
using Moq;
using System.Drawing;
using System.Xml.Linq;
using WorldMap.Application;
using WorldMap.Domain;
using WorldMap.Server.Services;
using WorldMap.Shared.OnionContracts;

namespace WorldMap.Server.Tests.Services
{
    public class MapServiceTests
    {
        private readonly Mock<IObjectLayer> _mockObjectLayer;
        private readonly Mock<IRegionsLayer> _mockRegionLayer;
        private readonly Mock<ILogger<MapService>> _mockLogger;
        private readonly MapService _service;

        public MapServiceTests()
        {
            _mockObjectLayer = new Mock<IObjectLayer>();
            _mockRegionLayer = new Mock<IRegionsLayer>();
            _mockLogger = new Mock<ILogger<MapService>>();
            _service = new MapService(_mockObjectLayer.Object, _mockRegionLayer.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_ReturnsEmptyList_WhenNoObjectsInArea()
        {
            // Arrange
            var request = new GetObjectsInAreaRequest { X = 0, Y = 0, Width = 10, Height = 10 };
            _mockObjectLayer.Setup(x => x.GetByAreaAsync(0, 0, 10, 10))
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
            var request = new GetObjectsInAreaRequest { X = 0, Y = 0, Width = 10, Height = 10 };
            var objects = new List<GameObject>
            {
                new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 },
                new GameObject { Id = "obj2", X = 7, Y = 7, Width = 1, Height = 1 }
            };
            _mockObjectLayer.Setup(x => x.GetByAreaAsync(0, 0, 10, 10))
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
        public async Task GetRegionsInArea_ReturnsEmptyList_WhenNoRegionsInArea()
        {
            // Arrange
            var request = new GetRegionsInAreaRequest { X = 0, Y = 0, Width = 10, Height = 10 };
            _mockRegionLayer.Setup(x => x.GetRegionsIntersectingArea(0, 0, 10, 10))
                .Returns(new List<Region>());

            // Act
            var response = await _service.GetRegionsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Regions);
        }

        [Fact]
        public async Task GetRegionsInArea_ReturnsRegions_WhenRegionsExistInArea()
        {
            // Arrange
            var request = new GetRegionsInAreaRequest { X = 0, Y = 0, Width = 100, Height = 100 };
            var regions = new List<Region>
            {
                new Region(1, "Forest", new Rectangle(10, 10, 50, 50)) { Metadata = "trees" },
                new Region(2, "Desert", new Rectangle(60, 60, 30, 30)) { Metadata = "sand" }
            };
            _mockRegionLayer.Setup(x => x.GetRegionsIntersectingArea(0, 0, 100, 100))
                .Returns(regions);

            // Act
            var response = await _service.GetRegionsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.Regions.Count);
            Assert.Equal(1, response.Regions[0].Id);
            Assert.Equal("Forest", response.Regions[0].Name);
            Assert.Equal(2, response.Regions[1].Id);
            Assert.Equal("Desert", response.Regions[1].Name);
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_HandlesNegativeCoordinates()
        {
            // Arrange
            var request = new GetObjectsInAreaRequest { X = -10, Y = -10, Width = 10, Height = 10 };
            _mockObjectLayer.Setup(x => x.GetByAreaAsync(-10, -10, 10, 10))
                .ReturnsAsync(new List<GameObject>());

            // Act
            var response = await _service.GetObjectsInAreaAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.Objects);
        }
    }
}

