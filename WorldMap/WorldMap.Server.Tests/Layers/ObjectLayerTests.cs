using WorldMap.Layers.ObjectsLayer;
using WorldMap.Layers.ObjectsLayer.Base;

namespace WorldMap.Server.Tests.Layers
{
    public class ObjectLayerTests
    {
        private readonly ObjectLayer _objectLayer;

        public ObjectLayerTests()
        {
            _objectLayer = new ObjectLayer();
        }

        [Fact]
        public async Task AddObjectAsync_AddsObjectSuccessfully()
        {
            // Arrange
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };

            // Act
            await _objectLayer.AddObjectAsync(gameObject);
            var result = await _objectLayer.GetObjectByIdAsync("obj1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("obj1", result.Id);
            Assert.Equal(5, result.X);
        }

        [Fact]
        public async Task AddObjectAsync_ThrowsException_WhenObjectIdIsEmpty()
        {
            // Arrange
            var gameObject = new GameObject { Id = "", X = 5, Y = 5, Width = 2, Height = 2 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _objectLayer.AddObjectAsync(gameObject));
        }

        [Fact]
        public async Task AddObjectAsync_ThrowsException_WhenObjectAlreadyExists()
        {
            // Arrange
            var gameObject1 = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            var gameObject2 = new GameObject { Id = "obj1", X = 10, Y = 10, Width = 1, Height = 1 };

            // Act
            await _objectLayer.AddObjectAsync(gameObject1);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _objectLayer.AddObjectAsync(gameObject2));
        }

        [Fact]
        public async Task UpdateObjectAsync_UpdatesObjectSuccessfully()
        {
            // Arrange
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await _objectLayer.AddObjectAsync(gameObject);

            // Act
            gameObject.X = 10;
            gameObject.Y = 10;
            await _objectLayer.UpdateObjectAsync(gameObject);
            var result = await _objectLayer.GetObjectByIdAsync("obj1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.X);
            Assert.Equal(10, result.Y);
        }

        [Fact]
        public async Task UpdateObjectAsync_ThrowsException_WhenObjectDoesNotExist()
        {
            // Arrange
            var gameObject = new GameObject { Id = "nonexistent", X = 5, Y = 5, Width = 2, Height = 2 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _objectLayer.UpdateObjectAsync(gameObject));
        }

        [Fact]
        public async Task RemoveObjectAsync_RemovesObjectSuccessfully()
        {
            // Arrange
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await _objectLayer.AddObjectAsync(gameObject);

            // Act
            await _objectLayer.RemoveObjectAsync("obj1");
            var result = await _objectLayer.GetObjectByIdAsync("obj1");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveObjectAsync_ThrowsException_WhenObjectDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _objectLayer.RemoveObjectAsync("nonexistent"));
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_ReturnsEmptyList_WhenNoObjectsInArea()
        {
            // Act
            var result = await _objectLayer.GetObjectsInAreaAsync(0, 0, 10, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_ReturnsObjectsInArea()
        {
            // Arrange
            await _objectLayer.AddObjectAsync(new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 });
            await _objectLayer.AddObjectAsync(new GameObject { Id = "obj2", X = 20, Y = 20, Width = 1, Height = 1 });
            await _objectLayer.AddObjectAsync(new GameObject { Id = "obj3", X = 7, Y = 7, Width = 1, Height = 1 });

            // Act
            var result = await _objectLayer.GetObjectsInAreaAsync(0, 0, 10, 10);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, obj => obj.Id == "obj1");
            Assert.Contains(result, obj => obj.Id == "obj3");
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_HandlesOverlappingObjects()
        {
            // Arrange - Object at (5,5) with width=4, height=4 (covers 5-8, 5-8)
            await _objectLayer.AddObjectAsync(new GameObject { Id = "obj1", X = 5, Y = 5, Width = 4, Height = 4 });

            // Act - Query area (7,7) to (15,15) which overlaps with obj1
            var result = await _objectLayer.GetObjectsInAreaAsync(7, 7, 15, 15);

            // Assert
            Assert.Single(result);
            Assert.Equal("obj1", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInAreaAsync_HandlesReversedCoordinates()
        {
            // Arrange
            await _objectLayer.AddObjectAsync(new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 });

            // Act - Query with reversed coordinates
            var result = await _objectLayer.GetObjectsInAreaAsync(10, 10, 0, 0);

            // Assert
            Assert.Single(result);
            Assert.Equal("obj1", result.First().Id);
        }

        [Fact]
        public async Task Subscribe_NotifiesHandler_OnObjectAdded()
        {
            // Arrange
            var handler = new TestObjectChangeHandler();
            _objectLayer.Subscribe(handler);
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };

            // Act
            await _objectLayer.AddObjectAsync(gameObject);

            // Assert
            Assert.Single(handler.AddedObjects);
            Assert.Equal("obj1", handler.AddedObjects[0].Id);
        }

        [Fact]
        public async Task Subscribe_NotifiesHandler_OnObjectUpdated()
        {
            // Arrange
            var handler = new TestObjectChangeHandler();
            _objectLayer.Subscribe(handler);
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await _objectLayer.AddObjectAsync(gameObject);
            handler.AddedObjects.Clear();

            // Act
            gameObject.X = 10;
            await _objectLayer.UpdateObjectAsync(gameObject);

            // Assert
            Assert.Single(handler.UpdatedObjects);
            Assert.Equal("obj1", handler.UpdatedObjects[0].Id);
            Assert.Equal(10, handler.UpdatedObjects[0].X);
        }

        [Fact]
        public async Task Subscribe_NotifiesHandler_OnObjectRemoved()
        {
            // Arrange
            var handler = new TestObjectChangeHandler();
            _objectLayer.Subscribe(handler);
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await _objectLayer.AddObjectAsync(gameObject);

            // Act
            await _objectLayer.RemoveObjectAsync("obj1");

            // Assert
            Assert.Single(handler.RemovedObjectIds);
            Assert.Equal("obj1", handler.RemovedObjectIds[0]);
        }

        [Fact]
        public async Task Unsubscribe_StopsNotifications()
        {
            // Arrange
            var handler = new TestObjectChangeHandler();
            _objectLayer.Subscribe(handler);
            _objectLayer.Unsubscribe(handler);

            // Act
            await _objectLayer.AddObjectAsync(new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 });

            // Assert
            Assert.Empty(handler.AddedObjects);
        }

        [Fact]
        public async Task ConcurrentOperations_HandleThreadSafety()
        {
            // Arrange
            var tasks = new List<Task>();

            // Act - Add 100 objects concurrently
            for (int i = 0; i < 100; i++)
            {
                var index = i;
                tasks.Add(Task.Run(async () =>
                {
                    await _objectLayer.AddObjectAsync(new GameObject
                    {
                        Id = $"obj{index}",
                        X = index,
                        Y = index,
                        Width = 1,
                        Height = 1
                    });
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            var allObjects = await _objectLayer.GetObjectsInAreaAsync(0, 0, 100, 100);
            Assert.Equal(100, allObjects.Count);
        }

        private class TestObjectChangeHandler : IObjectChangeHandler
        {
            public List<GameObject> AddedObjects { get; } = new();
            public List<GameObject> UpdatedObjects { get; } = new();
            public List<string> RemovedObjectIds { get; } = new();

            public Task OnObjectAddedAsync(GameObject gameObject)
            {
                AddedObjects.Add(gameObject);
                return Task.CompletedTask;
            }

            public Task OnObjectUpdatedAsync(GameObject gameObject)
            {
                UpdatedObjects.Add(gameObject);
                return Task.CompletedTask;
            }

            public Task OnObjectRemovedAsync(string id)
            {
                RemovedObjectIds.Add(id);
                return Task.CompletedTask;
            }
        }
    }
}

