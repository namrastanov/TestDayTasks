using WorldMap.Layers.ObjectsLayer;
using WorldMap.Layers.ObjectsLayer.Base;
using WorldMap.Layers.RegionsLayer;

namespace WorldMap.Server.Tests.EdgeCases
{
    /// <summary>
    /// Tests for edge cases in spatial queries including boundaries, empty areas, and concurrent updates
    /// </summary>
    public class SpatialQueryEdgeCaseTests
    {
        [Fact]
        public async Task GetObjectsInArea_ReturnsEmpty_WhenAreaIsEmpty()
        {
            // Arrange
            var layer = new ObjectLayer();
            await layer.AddObjectAsync(new GameObject { Id = "obj1", X = 100, Y = 100, Width = 10, Height = 10 });

            // Act - Query an area far from any objects
            var result = await layer.GetObjectsInAreaAsync(0, 0, 10, 10);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetObjectsInArea_ReturnsObject_WhenObjectIsAtExactBoundary()
        {
            // Arrange
            var layer = new ObjectLayer();
            // Object at (10, 10) with size 5x5 covers (10-14, 10-14)
            await layer.AddObjectAsync(new GameObject { Id = "obj1", X = 10, Y = 10, Width = 5, Height = 5 });

            // Act - Query area that exactly matches the object's left boundary
            var result = await layer.GetObjectsInAreaAsync(10, 10, 20, 20);

            // Assert
            Assert.Single(result);
            Assert.Equal("obj1", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInArea_ReturnsObject_WhenObjectOverlapsRightBoundary()
        {
            // Arrange
            var layer = new ObjectLayer();
            // Object at (8, 8) with size 3x3 covers (8-10, 8-10)
            await layer.AddObjectAsync(new GameObject { Id = "obj1", X = 8, Y = 8, Width = 3, Height = 3 });

            // Act - Query area (9, 9) to (20, 20) - object overlaps left/top boundary
            var result = await layer.GetObjectsInAreaAsync(9, 9, 20, 20);

            // Assert
            Assert.Single(result);
            Assert.Equal("obj1", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInArea_DoesNotReturnObject_WhenObjectIsJustOutsideBoundary()
        {
            // Arrange
            var layer = new ObjectLayer();
            // Object at (11, 11) with size 1x1 covers only (11, 11)
            await layer.AddObjectAsync(new GameObject { Id = "obj1", X = 11, Y = 11, Width = 1, Height = 1 });

            // Act - Query area (0, 0) to (10, 10) - object is just outside
            var result = await layer.GetObjectsInAreaAsync(0, 0, 10, 10);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetObjectsInArea_ReturnsObject_WhenObjectSpansEntireArea()
        {
            // Arrange
            var layer = new ObjectLayer();
            // Large object that covers the entire query area
            await layer.AddObjectAsync(new GameObject { Id = "large", X = 0, Y = 0, Width = 100, Height = 100 });

            // Act - Query smaller area inside the large object
            var result = await layer.GetObjectsInAreaAsync(25, 25, 75, 75);

            // Assert
            Assert.Single(result);
            Assert.Equal("large", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInArea_ReturnsObject_WhenAreaSpansEntireObject()
        {
            // Arrange
            var layer = new ObjectLayer();
            // Small object
            await layer.AddObjectAsync(new GameObject { Id = "small", X = 50, Y = 50, Width = 5, Height = 5 });

            // Act - Query large area that encompasses the small object
            var result = await layer.GetObjectsInAreaAsync(0, 0, 200, 200);

            // Assert
            Assert.Single(result);
            Assert.Equal("small", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInArea_HandlesNegativeCoordinates()
        {
            // Arrange
            var layer = new ObjectLayer();
            await layer.AddObjectAsync(new GameObject { Id = "obj1", X = -10, Y = -10, Width = 20, Height = 20 });
            await layer.AddObjectAsync(new GameObject { Id = "obj2", X = 5, Y = 5, Width = 5, Height = 5 });

            // Act - Query with negative coordinates
            var result = await layer.GetObjectsInAreaAsync(-15, -15, 0, 0);

            // Assert
            Assert.Single(result);
            Assert.Equal("obj1", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInArea_HandlesZeroSizeArea()
        {
            // Arrange
            var layer = new ObjectLayer();
            await layer.AddObjectAsync(new GameObject { Id = "obj1", X = 5, Y = 5, Width = 1, Height = 1 });

            // Act - Query with zero-size area (single point)
            var result = await layer.GetObjectsInAreaAsync(5, 5, 5, 5);

            // Assert
            Assert.Single(result);
            Assert.Equal("obj1", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInArea_ReturnsAllOverlappingObjects()
        {
            // Arrange
            var layer = new ObjectLayer();
            // Create multiple overlapping objects
            await layer.AddObjectAsync(new GameObject { Id = "obj1", X = 0, Y = 0, Width = 10, Height = 10 });
            await layer.AddObjectAsync(new GameObject { Id = "obj2", X = 5, Y = 5, Width = 10, Height = 10 });
            await layer.AddObjectAsync(new GameObject { Id = "obj3", X = 8, Y = 8, Width = 10, Height = 10 });
            await layer.AddObjectAsync(new GameObject { Id = "obj4", X = 100, Y = 100, Width = 10, Height = 10 });

            // Act - Query area that overlaps with first three objects
            var result = await layer.GetObjectsInAreaAsync(0, 0, 12, 12);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, obj => obj.Id == "obj1");
            Assert.Contains(result, obj => obj.Id == "obj2");
            Assert.Contains(result, obj => obj.Id == "obj3");
            Assert.DoesNotContain(result, obj => obj.Id == "obj4");
        }

        [Fact]
        public async Task GetObjectsInArea_HandlesConcurrentUpdates()
        {
            // Arrange
            var layer = new ObjectLayer();
            
            // Add initial objects
            for (int i = 0; i < 50; i++)
            {
                await layer.AddObjectAsync(new GameObject
                {
                    Id = $"obj{i}",
                    X = i * 2,
                    Y = i * 2,
                    Width = 1,
                    Height = 1
                });
            }

            // Act - Perform concurrent queries and updates
            var tasks = new List<Task>();
            
            // Query tasks
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var result = await layer.GetObjectsInAreaAsync(0, 0, 100, 100);
                    Assert.NotNull(result);
                }));
            }

            // Update tasks
            for (int i = 0; i < 10; i++)
            {
                var index = i;
                tasks.Add(Task.Run(async () =>
                {
                    var obj = await layer.GetObjectByIdAsync($"obj{index}");
                    if (obj != null)
                    {
                        obj.X += 1;
                        await layer.UpdateObjectAsync(obj);
                    }
                }));
            }

            // Assert - All operations complete without deadlocks or exceptions
            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task GetRegionsInArea_HandlesComplexOverlaps()
        {
            // Arrange
            var layer = new RegionLayer();
            
            // Create regions with various overlap patterns
            await layer.AddRegionAsync(new Region
            {
                Id = "region1",
                Name = "Top-Left",
                X = 0,
                Y = 0,
                Width = 50,
                Height = 50
            });
            
            await layer.AddRegionAsync(new Region
            {
                Id = "region2",
                Name = "Top-Right",
                X = 40,
                Y = 0,
                Width = 50,
                Height = 50
            });
            
            await layer.AddRegionAsync(new Region
            {
                Id = "region3",
                Name = "Bottom-Center",
                X = 20,
                Y = 40,
                Width = 50,
                Height = 50
            });
            
            await layer.AddRegionAsync(new Region
            {
                Id = "region4",
                Name = "Far-Away",
                X = 200,
                Y = 200,
                Width = 50,
                Height = 50
            });

            // Act - Query central overlapping area
            var result = await layer.GetRegionsInAreaAsync(30, 30, 60, 60);

            // Assert - Should get the three overlapping regions, not the far one
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.Id == "region1");
            Assert.Contains(result, r => r.Id == "region2");
            Assert.Contains(result, r => r.Id == "region3");
            Assert.DoesNotContain(result, r => r.Id == "region4");
        }

        [Fact]
        public async Task GetObjectsInArea_HandlesVeryLargeCoordinates()
        {
            // Arrange
            var layer = new ObjectLayer();
            await layer.AddObjectAsync(new GameObject
            {
                Id = "distant",
                X = int.MaxValue - 100,
                Y = int.MaxValue - 100,
                Width = 10,
                Height = 10
            });

            // Act
            var result = await layer.GetObjectsInAreaAsync(
                int.MaxValue - 150,
                int.MaxValue - 150,
                int.MaxValue - 50,
                int.MaxValue - 50);

            // Assert
            Assert.Single(result);
            Assert.Equal("distant", result.First().Id);
        }

        [Fact]
        public async Task GetObjectsInArea_PerformanceTest_LargeNumberOfObjects()
        {
            // Arrange
            var layer = new ObjectLayer();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            
            // Add 1000 objects
            for (int i = 0; i < 1000; i++)
            {
                await layer.AddObjectAsync(new GameObject
                {
                    Id = $"obj{i}",
                    X = i % 100,
                    Y = i / 100,
                    Width = 1,
                    Height = 1
                });
            }

            // Act - Query that should return a subset
            var result = await layer.GetObjectsInAreaAsync(25, 3, 75, 7);
            sw.Stop();

            // Assert
            Assert.True(result.Count > 0);
            Assert.True(result.Count < 1000);
            
            // Performance assertion - should complete in reasonable time
            // This is a basic check; adjust threshold based on requirements
            Assert.True(sw.ElapsedMilliseconds < 1000, 
                $"Query took {sw.ElapsedMilliseconds}ms, expected < 1000ms");
        }

        [Fact]
        public async Task ObjectBoundaries_SinglePixelPrecision()
        {
            // Arrange
            var layer = new ObjectLayer();
            
            // Objects that touch but don't overlap
            await layer.AddObjectAsync(new GameObject { Id = "left", X = 0, Y = 0, Width = 5, Height = 5 });
            await layer.AddObjectAsync(new GameObject { Id = "right", X = 5, Y = 0, Width = 5, Height = 5 });

            // Act - Query that should only include left object (0-4)
            var leftResult = await layer.GetObjectsInAreaAsync(0, 0, 4, 5);
            
            // Act - Query that should only include right object (5-9)
            var rightResult = await layer.GetObjectsInAreaAsync(5, 0, 9, 5);
            
            // Act - Query that should include both (boundary pixel)
            var bothResult = await layer.GetObjectsInAreaAsync(0, 0, 5, 5);

            // Assert
            Assert.Single(leftResult);
            Assert.Equal("left", leftResult.First().Id);
            
            Assert.Single(rightResult);
            Assert.Equal("right", rightResult.First().Id);
            
            Assert.Equal(2, bothResult.Count);
        }
    }
}

