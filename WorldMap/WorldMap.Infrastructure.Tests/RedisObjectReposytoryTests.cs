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
            _repository = new RedisObjectRepository<GameObject>(new RedisConnection(_mockConnection.Object));
        }

        [Fact]
        public async Task Test_CheckIfInsideArea_ReturnsFalseForOutOfBoundsObject()
        {
            // Arrange
            var gameObject = new GameObject { X = 100, Y = 100, Width = 10, Height = 10 };

            // Act
            var result = _repository.CheckIfInsideArea(gameObject, 0, 0, 50, 50);

            // Assert
            Assert.False(result);
        }
    }
}
