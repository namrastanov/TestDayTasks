using WorldMap.Domain;

namespace WorldMap.Application
{
    public interface IRegionsLayer
    {
        /// <summary>
        /// Gets the ID of the region at the specified tile coordinates
        /// </summary>
        /// <param name="x">X coordinate of the tile</param>
        /// <param name="y">Y coordinate of the tile</param>
        /// <returns>The ID of the region, or null if no region found</returns>
        Task<string?> GetRegionIdAtPositionAsync(int x, int y);

        /// <summary>
        /// Gets the metadata of a region by its ID
        /// </summary>
        /// <param name="id">The ID of the region</param>
        /// <returns>Metadata string, or null if region not found</returns>
        Task<string?> GetRegionMetadataByIdAsync(string id);

        /// <summary>
        /// Checks whether a tile belongs to any region
        /// </summary>
        /// <param name="x">X coordinate of the tile</param>
        /// <param name="y">Y coordinate of the tile</param>
        /// <returns>True if the tile is within a region; otherwise false</returns>
        Task<bool> IsTileInRegionAsync(int x, int y);

        /// <summary>
        /// Gets all regions that intersect with the specified rectangular area
        /// </summary>
        /// <param name="topLeftX">Start X coordinate</param>
        /// <param name="topLeftY">Start Y coordinate</param>
        /// <param name="width">Width of the area</param>
        /// <param name="height">Height of the area</param>
        /// <returns>Collection of regions intersecting with the area</returns>
        Task<IReadOnlyCollection<Region>> GetRegionsIntersectingAreaAsync(int topLeftX, int topLeftY, int width, int height);
    }
}
