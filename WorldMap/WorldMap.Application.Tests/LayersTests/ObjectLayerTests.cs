using Moq;
using WorldMap.Domain;

namespace WorldMap.Application.Tests
{
    public class ObjectLayerTests
    {
        private Mock<IObjectRepository<GameObject>> _repositoryMock;
        private ObjectLayer _objectLayer;

        public ObjectLayerTests()
        {
            _repositoryMock = new Mock<IObjectRepository<GameObject>>();
            _objectLayer = new ObjectLayer(_repositoryMock.Object);
        }

        [Fact]
        public async Task AddObjectAsync_WhenCalled_AddsObjectAndNotifiesSubscribers()
        {
            // Arrange
            var gameObject = new GameObject { X = 10, Y = 10 };

            // Act
            await _objectLayer.AddObjectAsync(gameObject);

            // Assert
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<GameObject>()), Times.Once());
        }

        [Fact]
        public async Task UpdateObjectAsync_WhenCalled_RemovesOldAndAddsNewObject()
        {
            // Arrange
            var gameObject = new GameObject { X = 10, Y = 10 };

            // Act
            await _objectLayer.UpdateObjectAsync(gameObject);

            // Assert
            _repositoryMock.Verify(r => r.RemoveAsync(It.IsAny<string>()), Times.Once());
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<GameObject>()), Times.Once());
        }

        [Fact]
        public async Task RemoveObjectAsync_WhenCalled_RemovesObjectFromRepository()
        {
            // Arrange
            var gameObject = new GameObject { Id = "test-id" };
            _repositoryMock.Setup(repo => repo.GetByIdAsync("test-id")).ReturnsAsync(gameObject);

            // Act
            await _objectLayer.RemoveObjectAsync("test-id");

            // Assert
            _repositoryMock.Verify(r => r.RemoveAsync("test-id"), Times.Once());
        }

        [Fact]
        public async Task GetByCoordinatesAsync_ReturnsGameObjectForGivenCoordinates()
        {
            // Arrange
            var expectedObject = new GameObject { X = 10, Y = 10 };
            _repositoryMock.Setup(repo => repo.GetByCoordinatesAsync(expectedObject.X, expectedObject.Y)).ReturnsAsync(expectedObject);

            // Act
            var actualObject = await _objectLayer.GetByCoordinatesAsync(expectedObject.X, expectedObject.Y);

            // Assert
            Assert.Equal(expectedObject, actualObject);
        }

        [Fact]
        public async Task GetByAreaAsync_ReturnsCorrectGameObjects()
        {
            // Arrange
            var gameObjects = new List<GameObject>
            {
                new GameObject { X = 10, Y = 10 },
                new GameObject { X = 20, Y = 20 }
            };
            _repositoryMock.Setup(repo => repo.GetByAreaAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(gameObjects);

            // Act
            var retrievedObjects = await _objectLayer.GetByAreaAsync(0, 0, 30, 30);

            // Assert
            Assert.Equal(gameObjects.Count, retrievedObjects.Count());
        }

        [Fact]
        public async Task Subscribe_ShouldNotifyOnObjectChanges()
        {
            // Arrange
            var handlerMock = new Mock<IObjectChangeHandler>();
            _objectLayer.Subscribe(handlerMock.Object);
            var gameObject = new GameObject { X = 10, Y = 10 };

            // Act
            await _objectLayer.AddObjectAsync(gameObject);

            // Assert
            handlerMock.Verify(h => h.OnObjectAddedAsync(It.IsAny<GameObject>()), Times.Once());
        }

        [Fact]
        public async Task Unsubscribe_ShouldStopNotifications()
        {
            // Arrange
            var handlerMock = new Mock<IObjectChangeHandler>();
            _objectLayer.Subscribe(handlerMock.Object);
            _objectLayer.Unsubscribe(handlerMock.Object);
            var gameObject = new GameObject { X = 10, Y = 10 };

            // Act
            await _objectLayer.AddObjectAsync(gameObject);

            // Assert
            handlerMock.Verify(h => h.OnObjectAddedAsync(It.IsAny<GameObject>()), Times.Never());
        }
    }
}
