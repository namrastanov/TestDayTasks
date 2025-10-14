using System.Collections.Concurrent;
using WorldMap.Domain;

namespace WorldMap.Layers.RegionsLayer
{
    public class RegionLayer : IRegionsLayer
    {
        public Task<string?> GetRegionIdAtPositionAsync(int x, int y)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetRegionMetadataByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Region>> GetRegionsIntersectingAreaAsync(int topLeftX, int topLeftY, int width, int height)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTileInRegionAsync(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}

