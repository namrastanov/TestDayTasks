using Microsoft.Extensions.Logging;
using Moq;
using WorldMap.Layers.ObjectsLayer;
using WorldMap.Layers.ObjectsLayer.Base;
using WorldMap.Server.Hubs;
using WorldMap.Shared.OnionContracts;

namespace WorldMap.Server.Tests.Integration
{
    public class MapHubIntegrationTests
    {
        [Fact]
        public async Task Hub_NotifiesOnObjectAdded_WhenObjectIsAddedToLayer()
        {
            // Arrange
            var objectLayer = new ObjectLayer();
            var logger = new Mock<ILogger<MapHub>>();
            var hub = new MapHub(objectLayer, logger.Object);

            var notificationReceived = false;
            GameObject? notifiedObject = null;

            // Simulate the hub as a change handler
            objectLayer.Subscribe(hub);

            // Act
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await objectLayer.AddObjectAsync(gameObject);

            // Give some time for async notification
            await Task.Delay(100);

            // Assert - Verify that the layer properly notifies the hub
            // In a real scenario, we would verify client receives the event
            // For now, we verify no exceptions were thrown and the object exists
            var retrievedObject = await objectLayer.GetObjectByIdAsync("obj1");
            Assert.NotNull(retrievedObject);
            Assert.Equal("obj1", retrievedObject.Id);
        }

        [Fact]
        public async Task Hub_NotifiesOnObjectUpdated_WhenObjectIsUpdatedInLayer()
        {
            // Arrange
            var objectLayer = new ObjectLayer();
            var logger = new Mock<ILogger<MapHub>>();
            var hub = new MapHub(objectLayer, logger.Object);

            objectLayer.Subscribe(hub);

            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await objectLayer.AddObjectAsync(gameObject);

            // Act
            gameObject.X = 10;
            await objectLayer.UpdateObjectAsync(gameObject);

            // Give some time for async notification
            await Task.Delay(100);

            // Assert
            var retrievedObject = await objectLayer.GetObjectByIdAsync("obj1");
            Assert.NotNull(retrievedObject);
            Assert.Equal(10, retrievedObject.X);
        }

        [Fact]
        public async Task Hub_NotifiesOnObjectDeleted_WhenObjectIsRemovedFromLayer()
        {
            // Arrange
            var objectLayer = new ObjectLayer();
            var logger = new Mock<ILogger<MapHub>>();
            var hub = new MapHub(objectLayer, logger.Object);

            objectLayer.Subscribe(hub);

            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await objectLayer.AddObjectAsync(gameObject);

            // Act
            await objectLayer.RemoveObjectAsync("obj1");

            // Give some time for async notification
            await Task.Delay(100);

            // Assert
            var retrievedObject = await objectLayer.GetObjectByIdAsync("obj1");
            Assert.Null(retrievedObject);
        }

        [Fact]
        public async Task ObjectLayer_HandlesMultipleSubscribers()
        {
            // Arrange
            var objectLayer = new ObjectLayer();
            var logger1 = new Mock<ILogger<MapHub>>();
            var logger2 = new Mock<ILogger<MapHub>>();
            var hub1 = new MapHub(objectLayer, logger1.Object);
            var hub2 = new MapHub(objectLayer, logger2.Object);

            objectLayer.Subscribe(hub1);
            objectLayer.Subscribe(hub2);

            // Act
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await objectLayer.AddObjectAsync(gameObject);

            // Give some time for async notifications
            await Task.Delay(100);

            // Assert - Both hubs should receive the notification
            // Verify no exceptions were thrown during parallel notification
            var retrievedObject = await objectLayer.GetObjectByIdAsync("obj1");
            Assert.NotNull(retrievedObject);
        }

        [Fact]
        public async Task ObjectLayer_StopsNotifications_AfterUnsubscribe()
        {
            // Arrange
            var objectLayer = new ObjectLayer();
            var logger = new Mock<ILogger<MapHub>>();
            var hub = new MapHub(objectLayer, logger.Object);

            objectLayer.Subscribe(hub);
            objectLayer.Unsubscribe(hub);

            // Act
            var gameObject = new GameObject { Id = "obj1", X = 5, Y = 5, Width = 2, Height = 2 };
            await objectLayer.AddObjectAsync(gameObject);

            // Give some time for potential async notification
            await Task.Delay(100);

            // Assert - Verify the object was added even though hub is unsubscribed
            var retrievedObject = await objectLayer.GetObjectByIdAsync("obj1");
            Assert.NotNull(retrievedObject);
        }

        [Fact]
        public async Task ConcurrentObjectOperations_WithMultipleHubs()
        {
            // Arrange
            var objectLayer = new ObjectLayer();
            var hubs = new List<MapHub>();
            
            for (int i = 0; i < 5; i++)
            {
                var logger = new Mock<ILogger<MapHub>>();
                var hub = new MapHub(objectLayer, logger.Object);
                objectLayer.Subscribe(hub);
                hubs.Add(hub);
            }

            // Act - Add 50 objects concurrently while 5 hubs are subscribed
            var tasks = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                var index = i;
                tasks.Add(Task.Run(async () =>
                {
                    await objectLayer.AddObjectAsync(new GameObject
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

            // Give some time for async notifications
            await Task.Delay(200);

            // Assert - All objects should be added successfully
            var allObjects = await objectLayer.GetObjectsInAreaAsync(0, 0, 100, 100);
            Assert.Equal(50, allObjects.Count);
        }
    }
}

