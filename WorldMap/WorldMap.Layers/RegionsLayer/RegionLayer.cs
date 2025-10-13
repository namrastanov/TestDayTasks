using System.Collections.Concurrent;

namespace WorldMap.Layers.RegionsLayer
{
    public class RegionLayer : IRegionLayer
    {
        private readonly ConcurrentDictionary<string, Region> _regions = new();

        public Task<IReadOnlyCollection<Region>> GetRegionsInAreaAsync(int x1, int y1, int x2, int y2)
        {
            // Ensure coordinates are properly ordered
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1, x2);
            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1, y2);

            var regionsInArea = _regions.Values
                .Where(region => IsRegionInArea(region, minX, minY, maxX, maxY))
                .ToList();

            return Task.FromResult<IReadOnlyCollection<Region>>(regionsInArea);
        }

        public Task AddRegionAsync(Region region)
        {
            if (string.IsNullOrEmpty(region.Id))
            {
                throw new ArgumentException("Region ID cannot be null or empty", nameof(region));
            }

            if (!_regions.TryAdd(region.Id, region))
            {
                throw new InvalidOperationException($"Region with ID {region.Id} already exists");
            }

            return Task.CompletedTask;
        }

        public Task<Region?> GetRegionByIdAsync(string id)
        {
            _regions.TryGetValue(id, out var region);
            return Task.FromResult(region);
        }

        private bool IsRegionInArea(Region region, int minX, int minY, int maxX, int maxY)
        {
            // Check if region overlaps with the area
            var regionMaxX = region.X + region.Width - 1;
            var regionMaxY = region.Y + region.Height - 1;

            return !(region.X > maxX || regionMaxX < minX || region.Y > maxY || regionMaxY < minY);
        }
    }
}

