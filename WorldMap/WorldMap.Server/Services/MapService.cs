using MagicOnion;
using MagicOnion.Server;
using WorldMap.Application;
using WorldMap.Shared.Interfaces;
using WorldMap.Shared.Models;
using WorldMap.Shared.OnionContracts;

namespace WorldMap.Server.Services
{
    public class MapService : ServiceBase<IMapService>, IMapService
    {
        private readonly IObjectLayer _objectLayer;
        private readonly IRegionsLayer _regionLayer;
        private readonly ILogger<MapService> _logger;

        public MapService(IObjectLayer objectLayer, IRegionsLayer regionLayer, ILogger<MapService> logger)
        {
            _objectLayer = objectLayer ?? throw new ArgumentNullException(nameof(objectLayer));
            _regionLayer = regionLayer ?? throw new ArgumentNullException(nameof(regionLayer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async UnaryResult<GetObjectsInAreaResponse> GetObjectsInAreaAsync(GetObjectsInAreaRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Getting objects in area: ({X}, {Y}) to ({Width}, {Height})",
                    request.X, request.Y, request.Width, request.Height);

                var objects = await _objectLayer.GetByAreaAsync(
                    request.X, request.Y, request.Width, request.Height);

                var response = new GetObjectsInAreaResponse
                {
                    Objects = objects.Select(obj => new MapObjectDto
                    {
                        Id = obj.Id,
                        X = obj.X,
                        Y = obj.Y,
                        Width = obj.Width,
                        Height = obj.Height
                    }).ToList()
                };

                _logger.LogInformation("Found {Count} objects in area", response.Objects.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting objects in area");
                throw;
            }
        }

        public async UnaryResult<GetRegionsInAreaResponse> GetRegionsInAreaAsync(GetRegionsInAreaRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Getting regions in area: ({X}, {Y}) to ({Width}, {Height})",
                    request.X, request.Y, request.Width, request.Height);

                var regions = _regionLayer.GetRegionsIntersectingArea(
                    request.X, request.Y, request.Width, request.Height);

                var response = new GetRegionsInAreaResponse
                {
                    Regions = regions.Select(region => new RegionDto
                    {
                        Id = region.Id,
                        Name = region.Name,
                        X = region.X,
                        Y = region.Y,
                        Width = region.Width,
                        Height = region.Height,
                        Metadata = region.Metadata
                    }).ToList()
                };

                _logger.LogInformation("Found {Count} regions in area", response.Regions.Count);

                await Task.CompletedTask;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting regions in area");
                throw;
            }
        }
    }
}

