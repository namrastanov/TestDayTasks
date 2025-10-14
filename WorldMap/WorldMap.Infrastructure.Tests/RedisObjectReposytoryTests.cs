using Moq;
using StackExchange.Redis;
using WorldMap.Domain;

namespace WorldMap.Infrastructure.Tests
{
    public class RedisObjectRepositoryTests
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDb;
        private readonly RedisObjectRepository<GameObject> _repository;

        public RedisObjectRepositoryTests()
        {
            _mockConnection = new Mock<IConnectionMultiplexer>();
            _mockDb = new Mock<IDatabase>();

            _mockConnection.Setup(c => c.GetDatabase(-1, null)).Returns(_mockDb.Object);
            _repository = new RedisObjectRepository<GameObject>(_mockConnection.Object);
        }

        [Fact]
        public async Task Test_AddAsync_Succeeds()
        {
            // Arrange
            var gameObject = new GameObject { X = 10, Y = 20, Width = 30, Height = 40 };

            // Act
            await _repository.AddAsync(gameObject);

            // Assert
            _mockDb.Verify(
                db => db.GeoAddAsync(
                    It.Is<RedisKey>(key => key.ToString().Equals("object_locations")),
                    It.IsAny<GeoEntry[]>(),
                    CommandFlags.None),
                Times.Once());

            _mockDb.Verify(db => db.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), CommandFlags.None));
        }

        [Fact]
        public async Task Test_GetByIdAsync_ReturnsCorrectGameObject()
        {
            // Arrange
            var gameObject = new GameObject { Id = Guid.NewGuid().ToString(), X = 10, Y = 20, Width = 30, Height = 40 };

            _mockDb.Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(true));
            _mockDb.Setup(db => db.HashGetAllAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                  .Returns(Task.FromResult(new HashEntry[]
                  {
                  new("id", gameObject.Id),
                  new("x", gameObject.X.ToString()),
                  new("y", gameObject.Y.ToString()),
                  new("width", gameObject.Width.ToString()),
                  new("height", gameObject.Height.ToString())
                  }));

            // Act
            var retrievedObject = await _repository.GetByIdAsync(gameObject.Id);

            // Assert
            Assert.Equal(gameObject.Id, retrievedObject?.Id);
            Assert.Equal(gameObject.X, retrievedObject?.X);
            Assert.Equal(gameObject.Y, retrievedObject?.Y);
            Assert.Equal(gameObject.Width, retrievedObject?.Width);
            Assert.Equal(gameObject.Height, retrievedObject?.Height);
        }

        [Fact]
        public async Task Test_RemoveAsync_DeletesFromDB()
        {
            // Arrange
            var gameObject = new GameObject { Id = Guid.NewGuid().ToString() };
            _mockDb.Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(Task.FromResult(true));

            // Act
            await _repository.RemoveAsync(gameObject.Id);

            // Assert
            _mockDb.Verify(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None));
            _mockDb.Verify(db => db.GeoRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None));
        }

        [Fact]
        public void Test_CheckIfInsideArea_ReturnsFalseForOutOfBoundsObject()
        {
            // Arrange
            var gameObject = new GameObject { X = 100, Y = 100, Width = 10, Height = 10 };

            // Act
            var result = _repository.CheckIfInsideArea(gameObject, 0, 0, 50, 50);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Test_GetByCoordinatesAsync_ReturnsClosestObject()
        {
            // Arrange
            var gameObject = new GameObject { Id = Guid.NewGuid().ToString(), X = 10, Y = 20, Width = 10, Height = 10 };
            _mockDb.Setup(db => db.GeoRadiusAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<double>(), It.IsAny<GeoUnit>(), It.IsAny<int>(), It.IsAny<Order?>(), It.IsAny<GeoRadiusOptions>(), It.IsAny<CommandFlags>()))
                    .Returns(Task.FromResult(new[] { new GeoRadiusResult(1, 1, 1, new GeoPosition()) }));
            _mockDb.Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(Task.FromResult(true));
            _mockDb.Setup(db => db.HashGetAllAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                    .Returns(Task.FromResult(new HashEntry[]
                    {
                new("id", gameObject.Id),
                new("x", gameObject.X.ToString()),
                new("y", gameObject.Y.ToString()),
                new("width", gameObject.Width.ToString()),
                new("height", gameObject.Height.ToString())
                    }));

            // Act
            var retrievedObject = await _repository.GetByCoordinatesAsync(10, 20);

            // Assert
            Assert.NotNull(retrievedObject);
            Assert.Equal(gameObject.Id, retrievedObject!.Id);
        }
    }
}
