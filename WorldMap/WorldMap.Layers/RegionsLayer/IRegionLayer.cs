namespace WorldMap.Layers.RegionsLayer
{
    public interface IRegionLayer
    {
        /// <summary>
        /// Gets all regions within the specified area
        /// </summary>
        /// <param name="x1">Start X coordinate</param>
        /// <param name="y1">Start Y coordinate</param>
        /// <param name="x2">End X coordinate</param>
        /// <param name="y2">End Y coordinate</param>
        /// <returns>Collection of regions in the area</returns>
        Task<IReadOnlyCollection<Region>> GetRegionsInAreaAsync(int x1, int y1, int x2, int y2);

        /// <summary>
        /// Adds a new region to the layer
        /// </summary>
        /// <param name="region">The region to add</param>
        Task AddRegionAsync(Region region);

        /// <summary>
        /// Gets a region by its ID
        /// </summary>
        /// <param name="id">The ID of the region</param>
        /// <returns>The region or null if not found</returns>
        Task<Region?> GetRegionByIdAsync(string id);
    }
}

