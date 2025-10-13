using WorldMap.Layers.ObjectsLayer.Base;
using WorldMap.Layers.RegionsLayer;

namespace WorldMap.Server.Tests.Helpers
{
    /// <summary>
    /// Helper class for creating test data
    /// </summary>
    public static class TestDataHelper
    {
        public static GameObject CreateGameObject(
            string id,
            int x,
            int y,
            int width = 1,
            int height = 1)
        {
            return new GameObject
            {
                Id = id,
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
        }

        public static Region CreateRegion(
            string id,
            string name,
            int x,
            int y,
            int width,
            int height,
            string metadata = "")
        {
            return new Region
            {
                Id = id,
                Name = name,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                Metadata = metadata
            };
        }

        public static List<GameObject> CreateObjectGrid(
            int startX,
            int startY,
            int cols,
            int rows,
            int spacing = 10,
            string idPrefix = "obj")
        {
            var objects = new List<GameObject>();
            int objectId = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    objects.Add(CreateGameObject(
                        $"{idPrefix}{objectId++}",
                        startX + col * spacing,
                        startY + row * spacing,
                        spacing - 1,
                        spacing - 1));
                }
            }

            return objects;
        }

        public static List<Region> CreateRegionGrid(
            int startX,
            int startY,
            int cols,
            int rows,
            int regionWidth,
            int regionHeight,
            string namePrefix = "Region")
        {
            var regions = new List<Region>();
            int regionId = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    regions.Add(CreateRegion(
                        $"region{regionId}",
                        $"{namePrefix}_{regionId}",
                        startX + col * regionWidth,
                        startY + row * regionHeight,
                        regionWidth,
                        regionHeight,
                        $"Region at ({col}, {row})"));
                    regionId++;
                }
            }

            return regions;
        }

        public static List<GameObject> CreateRandomObjects(
            int count,
            int minX,
            int maxX,
            int minY,
            int maxY,
            int minSize = 1,
            int maxSize = 10,
            string idPrefix = "random")
        {
            var random = new Random(42); // Fixed seed for reproducible tests
            var objects = new List<GameObject>();

            for (int i = 0; i < count; i++)
            {
                objects.Add(CreateGameObject(
                    $"{idPrefix}{i}",
                    random.Next(minX, maxX),
                    random.Next(minY, maxY),
                    random.Next(minSize, maxSize),
                    random.Next(minSize, maxSize)));
            }

            return objects;
        }

        public static List<GameObject> CreateOverlappingObjects(
            int x,
            int y,
            int count,
            string idPrefix = "overlap")
        {
            var objects = new List<GameObject>();

            for (int i = 0; i < count; i++)
            {
                objects.Add(CreateGameObject(
                    $"{idPrefix}{i}",
                    x + i,
                    y + i,
                    10,
                    10));
            }

            return objects;
        }

        public static GameObject CreateLargeObject(
            string id,
            int x,
            int y,
            int width,
            int height)
        {
            return CreateGameObject(id, x, y, width, height);
        }

        public static Region CreateLargeRegion(
            string id,
            string name,
            int x,
            int y,
            int width,
            int height,
            string metadata = "")
        {
            return CreateRegion(id, name, x, y, width, height, metadata);
        }

        /// <summary>
        /// Creates objects at boundary positions for testing edge cases
        /// </summary>
        public static List<GameObject> CreateBoundaryTestObjects()
        {
            return new List<GameObject>
            {
                // Origin
                CreateGameObject("origin", 0, 0, 1, 1),
                
                // Positive boundaries
                CreateGameObject("top_boundary", 50, 0, 5, 5),
                CreateGameObject("right_boundary", 100, 50, 5, 5),
                CreateGameObject("bottom_boundary", 50, 100, 5, 5),
                CreateGameObject("left_boundary", 0, 50, 5, 5),
                
                // Negative coordinates
                CreateGameObject("negative", -10, -10, 5, 5),
                
                // Large coordinates
                CreateGameObject("large", 10000, 10000, 10, 10)
            };
        }

        /// <summary>
        /// Creates a set of objects for stress testing
        /// </summary>
        public static List<GameObject> CreateStressTestObjects(int count)
        {
            var objects = new List<GameObject>(count);
            
            for (int i = 0; i < count; i++)
            {
                objects.Add(CreateGameObject(
                    $"stress{i}",
                    i % 1000,
                    i / 1000,
                    1,
                    1));
            }

            return objects;
        }

        /// <summary>
        /// Verifies if an object is within a specified area
        /// </summary>
        public static bool IsObjectInArea(
            GameObject obj,
            int x1,
            int y1,
            int x2,
            int y2)
        {
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1, x2);
            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1, y2);

            var objMaxX = obj.X + obj.Width - 1;
            var objMaxY = obj.Y + obj.Height - 1;

            return !(obj.X > maxX || objMaxX < minX || obj.Y > maxY || objMaxY < minY);
        }

        /// <summary>
        /// Verifies if a region is within a specified area
        /// </summary>
        public static bool IsRegionInArea(
            Region region,
            int x1,
            int y1,
            int x2,
            int y2)
        {
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1, x2);
            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1, y2);

            var regionMaxX = region.X + region.Width - 1;
            var regionMaxY = region.Y + region.Height - 1;

            return !(region.X > maxX || regionMaxX < minX || region.Y > maxY || regionMaxY < minY);
        }
    }
}

